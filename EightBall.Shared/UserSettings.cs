using Kadro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EightBall.Shared
{
    public static class UserSettings
    {
        public static void SetSoundEffectVolume(float volume)
        {
            SoundSettings.EffectsVolume = volume;
        }
    }
}
