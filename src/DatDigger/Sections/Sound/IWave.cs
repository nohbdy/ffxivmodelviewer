namespace DatDigger.Sections.Sound
{
    public interface IWave : INavigable
    {
        string DataFormat { get; }
        WaveHeader WaveHeader { get; }
        void LoadData(BinaryReaderEx reader);
    }
}
