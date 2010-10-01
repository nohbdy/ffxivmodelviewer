using System;

namespace DatDigger.Sound
{
    public interface IPlayableStream : IPlayable, IDisposable
    {
        int StreamLength { get; }
        int GetWaveData(byte[] buffer, int length);
    }

    public interface IPlayable
    {
        SlimDX.Multimedia.WaveFormat WaveFormat { get; }
        byte[] WaveData { get; }
    }
}
