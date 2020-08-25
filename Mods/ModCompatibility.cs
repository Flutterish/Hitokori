using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public static class ModCompatibility {
		public static readonly Dictionary<Type, RulesetModList> mods = new Dictionary<Type, RulesetModList>();

		public static void RegisterMod ( Type ruleset, Type mod ) {
			if ( !mods.ContainsKey( ruleset ) ) mods.Add( ruleset, new RulesetModList( ruleset ) );
			mods[ ruleset ].AddMod( mod );
		}

		public static Type[] IncompatibleWith ( Type mod )
			=> mods[ RulesetOf( mod ) ].GetIncompatible( mod ).ToArray();

		public static Type RulesetOf ( Type mod ) {
			foreach ( var ( ruleset, mods ) in mods ) {
				if ( mods.HasMod( mod ) ) return ruleset;
			}
			throw new InvalidOperationException();
		}

		public static Type[] IncompatibleWith ( Type ruleset, Type mod )
			=> mods[ ruleset ].GetIncompatible( mod ).ToArray();

		public static void ApplyMod ( Mod mod, params object[] targets )
			=> mods[ RulesetOf( mod.GetType() ) ].GetMod( mod.GetType() ).Apply( mod, targets );
	}

	public class RulesetModList {
		public readonly Type Ruleset;
		readonly Dictionary<Type, ModDependencies> Mods = new Dictionary<Type, ModDependencies>();

		public RulesetModList ( Type ruleset ) {
			Ruleset = ruleset;
		}

		public bool HasMod ( Type mod )
			=> Mods.ContainsKey( mod );

		public void AddMod ( Type mod ) {
			if ( !Mods.ContainsKey( mod ) ) {
				var newMod = new ModDependencies( mod );
				
				foreach ( ModDependencies dependencies in Mods.Values ) {
					newMod.GenerateIncompabilitiesWith( dependencies );
				}

				Mods.Add( mod, newMod );
			}
		}

		public IEnumerable<Type> GetIncompatible ( Type mod )
			=> Mods[ mod ].IncompatibleCache;

		public ModDependencies GetMod ( Type mod )
			=> Mods[ mod ];
	}

	public class ModDependencies {
		public readonly Type Mod;
		public readonly List<Type> IncompatibleCache = new List<Type>();
		public readonly Dictionary<Type, List<(PropertyInfo by, PropertyInfo target)>> AffectedProperties = new Dictionary<Type, List<(PropertyInfo by, PropertyInfo target)>>();

		public ModDependencies ( Type mod ) {
			Mod = mod;
			foreach ( var prop in mod.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) {
				foreach ( ModifiesAttribute modifies in prop.GetCustomAttributes<ModifiesAttribute>( true ) ) {
					var targetType = modifies.Type;
					var targetProperty = targetType.GetProperty( modifies.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

					if ( !targetProperty.GetCustomAttributes<ModdableAttribute>().Any() ) throw new InvalidOperationException( $"Cannot modify {targetType.Name}.{targetProperty.Name} as it's not [Moddable]" );

					if ( !AffectedProperties.ContainsKey( targetType ) ) AffectedProperties.Add( targetType, new List<(PropertyInfo by, PropertyInfo target)>() );
					AffectedProperties[ targetType ].Add( ( prop, targetProperty) );
				}
			}
		}

		public void GenerateIncompabilitiesWith ( ModDependencies mod ) {
			if ( IncompatibleCache.Contains( mod.Mod ) ) return;

			foreach ( var sharedType in AffectedProperties.Keys.Where( mod.AffectedProperties.Keys.Contains ) ) {
				if ( AffectedProperties[ sharedType ].Any( x => mod.AffectedProperties[ sharedType ].Any( y => y.target == x.target ) ) ) {
					IncompatibleCache.Add( mod.Mod );
					if ( !mod.IncompatibleCache.Contains( Mod ) ) mod.IncompatibleCache.Add( Mod );
					return;
				}
			}
		}

		public void Apply ( Mod mod, params object[] targets ) {
			foreach ( var target in targets ) {
				var targetType = target.GetType();
				foreach ( var typeEffects in AffectedProperties.Where( x => x.Key.IsAssignableFrom( targetType ) ) ) {
					foreach ( var ( by, targetProp ) in typeEffects.Value ) {
						targetProp.SetValue( target, by.GetValue( mod ) );
					}
				}
			}
		}
	}

	public class ModifiesAttribute : Attribute {
		public Type Type;
		public string Name;

		public ModifiesAttribute ( Type type, string name ) {
			Type = type;
			Name = name;
		}
	}

	public class ModdableAttribute : Attribute {
		public ModdableAttribute () { }
	}
}
