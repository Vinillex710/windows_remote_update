using NAudio.CoreAudioApi;

namespace SystemControlApp.Controllers
{
    public static class VolumeController
    {
        private static readonly MMDevice device =
            new MMDeviceEnumerator()
                .GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        public static void SetVolume(float level)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = level;
        }

        public static float GetVolume()
        {
            return device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
    }
}
