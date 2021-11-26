using osu.Framework.Audio.Track;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
namespace osu.Game.Rulesets.Hitokori.UI
{
    public class BeatProvider : BeatSyncedContainer
    {
        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            OnBeat?.Invoke(beatIndex, timingPoint, effectPoint, amplitudes, this);
        }

        public delegate void BeatEventHandler(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes, BeatProvider provider);
        public event BeatEventHandler? OnBeat;
    }
}
