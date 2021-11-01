using AutoMapper.Internal;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.ConstrainableProperties;
using osu.Game.Rulesets.Hitokori.Edit.Setup;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors {
	public abstract class ConnectorBlueprint : Container {
		public readonly TilePointConnector Connector;

		[Resolved, MaybeNull, NotNull]
		protected HitokoriHitObjectComposer Composer { get; private set; }
		protected HitokoriBeatmap Beatmap => Composer.Beatmap;
		protected EditorBeatmap EditorBeatmap => Composer.EditorBeatmap;
		protected HitokoriPlayfield Playfield => Composer.Playfield;

		protected Vector2 PositionAt ( Vector2 normalized )
			=> ToLocalSpace( Playfield.ScreenSpacePositionOf( normalized ) );
		protected Vector2 PositionAt ( Vector2d normalized )
			=> ToLocalSpace( Playfield.ScreenSpacePositionOf( (Vector2)normalized ) );

		protected ConnectorBlueprint ( TilePointConnector connector ) {
			Connector = connector;
			RelativeSizeAxes = Axes.Both;
		}

		public virtual Drawable? CreateSettingsSection () => null;

		public virtual bool CanResetConstraints => false;
		public virtual void ResetConstraints () { }

		// TODO Settings section for multi-selection

		public event Action? Modified;
		protected void InvokeModified () {
			Modified?.Invoke();
		}

		protected override bool OnMouseDown ( MouseDownEvent e ) { // we do this so the selection doesnt steal our click
			return e.Button == MouseButton.Left
				&& Children.Any( x => x.ReceivePositionalInputAt( e.ScreenSpaceMousePosition ) );
		}
	}

	public class ConnectorBlueprint<T> : ConnectorBlueprint where T : TilePointConnector {
		new public T Connector => (T)base.Connector;

		public ConnectorBlueprint ( T connector ) : base( connector ) { }

		public override Drawable? CreateSettingsSection ()
			=> new ReflectionBasedConnectorBlueprintSettingsSection( Connector );
	}

	public class ReflectionBasedConnectorBlueprintSettingsSection : FillFlowContainer {
		public readonly TilePointConnector Connector;
		public event Action? Modified;
		public ReflectionBasedConnectorBlueprintSettingsSection ( TilePointConnector connector ) {
			Direction = FillDirection.Vertical;
			AutoSizeAxes = Axes.Y;
			RelativeSizeAxes = Axes.X;
			Connector = connector;
		}

		protected class PropertySubsection : SetupSubsection {
			private string title;

			public PropertySubsection ( string title ) {
				this.title = title;
			}

			protected override void LoadComplete () {
				base.LoadComplete();
			}

			public override LocalisableString Title => title;
		}

		/// <summary>
		/// Creates a system which will track a value and update it bi-directionally based on user input.
		/// </summary>
		/// <typeparam name="V">The value type</typeparam>
		/// <typeparam name="F">The format type. This is how the user inputs and receives the value, usually as a string</typeparam>
		/// <typeparam name="D">The type of drawable that displays the value. It needs to be an <see cref="IHasCurrentValue{F}"/></typeparam>
		/// <param name="tracker">The container to which the drawable will be added</param>
		/// <param name="drawable">The drawable which displays the value</param>
		/// <param name="getter">Value getter</param>
		/// <param name="setter">Value setter</param>
		/// <param name="format">A function that formats the value to the display type</param>
		/// <param name="parse">A function that converts the format type to a value, or <see langword="null"/> if the format is invalid</param>
		/// <param name="revert">A function that in case the new value causes an excpetion, reverts the value and performs any cleanup necessary</param>
		/// <returns>An action to be performed when the user commits a value</returns>
		public Action TrackValue<V, F, D> ( Container<Drawable> tracker, D drawable,
			Func<V> getter, Action<V> setter, Func<V, F> format, Func<F, V?> parse, Action<V> revert
		) where D : Drawable, IHasCurrentValue<F> {
			Bindable<V> bindable = new Bindable<V>();
			tracker.Add( drawable );

			FillFlowContainer errorContainer = new FillFlowContainer {
				Direction = FillDirection.Vertical,
				AutoSizeAxes = Axes.Y,
				RelativeSizeAxes = Axes.X
			};
			tracker.Add( errorContainer );

			tracker.OnUpdate += _ => {
				bindable.Value = getter();
			};
			bindable.BindValueChanged( v => {
				Schedule( () => drawable.Current.Value = format( v.NewValue ) ); // we need to schedule because some components are funky when you change the current twice in a single frame
			} );
			return () => {
				var oldValue = getter();

				try {
					var newValue = parse( drawable.Current.Value );
					if ( newValue is V t ) {
						setter( t );

						// invoke this to test if its supported, otherwise we will revert that in the catch block
						Connector.GetEndState();
					}
					else {
						bindable.TriggerChange();
					}
				}
				catch ( Exception e ) {
					revert( oldValue );
					bindable.TriggerChange();
					// TODO error message
				}
				finally {
					Modified?.Invoke();
				}
			};
		}

		protected override void LoadComplete () {
			base.LoadComplete();

			Type? getGenericType ( Type generic, Type? current ) {
				if ( current is null ) return null;

				if ( current.IsGenericType && current.GetGenericTypeDefinition() == generic )
					return current;
				else
					return getGenericType( generic, current.BaseType );
			}

			var fields = Connector.GetType().GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
				.Where( x => x.GetCustomAttribute<InspectableAttribute>() is not null );

			var properties = Connector.GetType().GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
				.Where( x => x.GetCustomAttribute<InspectableAttribute>() is not null );

			var members = ( fields as IEnumerable<MemberInfo> ).Concat( properties );

			var sections = members.GroupBy( x => x.GetCustomAttribute<InspectableAttribute>()!.Section ?? InspectableAttribute.SectionProperties );

			foreach ( var memberSection in sections ) {
				var section = new PropertySubsection( memberSection.Key );
				Add( section );

				foreach ( var member in memberSection ) {
					var info = member.GetCustomAttribute<InspectableAttribute>()!;

					var type = member is FieldInfo f ? f.FieldType : member is PropertyInfo p ? p.PropertyType : typeof( object );
					info.IsReadonly |= member is FieldInfo ? false : member is PropertyInfo pp ? !pp.CanWrite : false;
					var constrainableType = getGenericType( typeof( ConstrainableProperty<> ), type );
					if ( constrainableType is not null )
						type = constrainableType.GetGenericArguments()[ 0 ];

					Func<object, string>? format;
					if ( info.FormatMethod is string formatName ) {
						var method = Connector.GetType().GetMethod( formatName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
						if ( method is not null && method.ReturnType == typeof( string ) )
							format = v => (string)method.Invoke( Connector, new object[] { v } )!;
						else
							format = null;
					}
					else format = null;

					Func<string, object?>? parse;
					if ( info.ParseMethod is string parseName ) {
						var method = Connector.GetType().GetMethod( parseName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
						if ( method is not null && method.GetParameters().Length == 1 && method.GetParameters().Single().ParameterType == typeof( string ) && method.ReturnType != typeof( void ) )
							parse = v => method.Invoke( Connector, new object[] { v } )!;
						else
							parse = null;
					}
					else parse = null;

					section.OnLoadComplete += _ => {
						if ( constrainableType is not null ) {
							var subsection = new PropertySubsection( info.Label ?? member.Name );
							section.Add( subsection );

							subsection.OnLoadComplete += _ => {
								var constrainableProp = member.GetMemberValue( Connector );

								void trackConstraint<V> ( ConstrainableProperty<V> prop ) where V : struct {
									var toggle = new LabelledSwitchButton { FixedLabelWidth = SetupSubsection.LABEL_WIDTH, Label = "Is constrained", Description = "Whether this property is manually set, rather than computed automatically" };
									var commit = TrackValue( subsection!, toggle,
										() => prop.IsConstrained,
										v => {
											if ( v )
												prop.Constrain( prop.Value );
											else
												prop.ReleaseConstraint();
										},
										v => v,
										v => v,
										v => {
											if ( v )
												prop.Constrain( prop.Value );
											else
												prop.ReleaseConstraint();
										}
									);
									toggle.Current.Disabled = info!.IsReadonly;

									toggle.Current.BindValueChanged( v => commit() );
								}

								switch ( constrainableProp ) {
									case ConstrainableProperty<double> doubleProp:
										trackConstraint( doubleProp );

										var textBox = new LabelledTextBox { FixedLabelWidth = SetupSubsection.LABEL_WIDTH, Label = "Value", ReadOnly = info.IsReadonly };
										var commit = TrackValue<double?, string, LabelledTextBox>( subsection, textBox,
											() => doubleProp.Value,
											v => { if ( v is double d ) doubleProp.Constrain( d ); },
											v => format?.Invoke( doubleProp.Value ) ?? doubleProp.StringifyValue(),
											v => parse is null ? doubleProp.ParseValue( v ) : parse( v ) as double?,
											_ => doubleProp.ReleaseConstraint()
										);
										textBox.OnCommit += ( _, newValue ) => {
											if ( newValue ) commit();
										};
										break;
								}
							};
						}
						else {
							format ??= v => Newtonsoft.Json.JsonConvert.SerializeObject( v );
							parse ??= v => Newtonsoft.Json.JsonConvert.DeserializeObject( v, type );

							var value = member.GetMemberValue( Connector );

							if ( value is Vector2d ) {
								var subsection = new PropertySubsection( info.Label ?? member.Name );
								section.Add( subsection );

								subsection.OnLoadComplete += _ => {
									var textBox = new LabelledTextBox { FixedLabelWidth = SetupSubsection.LABEL_WIDTH, Label = "X", ReadOnly = info.IsReadonly };
									var commit = TrackValue<double?, string, LabelledTextBox>( subsection, textBox,
										() => ( (Vector2d)member.GetMemberValue( Connector ) ).X,
										v => { if ( v is double d ) member.SetMemberValue( Connector, ( (Vector2d)member.GetMemberValue( Connector ) ) with { X = d } ); },
										v => format.Invoke( ( (Vector2d)member.GetMemberValue( Connector ) ).X ),
										v => parse( v ) as double?,
										v => member.SetMemberValue( Connector, ( (Vector2d)member.GetMemberValue( Connector ) ) with { X = v!.Value } )
									);
									textBox.OnCommit += ( _, newValue ) => {
										if ( newValue ) commit();
									};
									textBox = new LabelledTextBox { FixedLabelWidth = SetupSubsection.LABEL_WIDTH, Label = "Y", ReadOnly = info.IsReadonly };
									commit = TrackValue<double?, string, LabelledTextBox>( subsection, textBox,
										() => ( (Vector2d)member.GetMemberValue( Connector ) ).Y,
										v => { if ( v is double d ) member.SetMemberValue( Connector, ( (Vector2d)member.GetMemberValue( Connector ) ) with { Y = d } ); },
										v => format.Invoke( ( (Vector2d)member.GetMemberValue( Connector ) ).Y ),
										v => parse( v ) as double?,
										v => member.SetMemberValue( Connector, ( (Vector2d)member.GetMemberValue( Connector ) ) with { Y = v!.Value } )
									);
									textBox.OnCommit += ( _, newValue ) => {
										if ( newValue ) commit();
									};
								};
							}
							else {
								var textBox = new LabelledTextBox { FixedLabelWidth = SetupSubsection.LABEL_WIDTH, Label = info.Label ?? member.Name, ReadOnly = info.IsReadonly };
								var commit = TrackValue( section, textBox,
									() => member.GetMemberValue( Connector ),
									v => member.SetMemberValue( Connector, v ),
									v => format( v ),
									v => parse( v ),
									v => member.SetMemberValue( Connector, v )
								);
								textBox.OnCommit += ( _, newValue ) => {
									if ( newValue ) commit();
								};
							}
						}
					};
				}
			}
		}
	}
}
