using System;
using System.Collections.Generic;
using DatDigger.Sound;
using SlimDX.XAudio2;

namespace ModelViewer.Sound
{
    public class SoundManager : IDisposable
    {
        #region Singleton Management
        private static SoundManager instance;

        public static SoundManager Instance { get { return instance; } }
        public static void Init()
        {
            if (instance == null)
            {
                instance = new SoundManager();
                instance.InitInstance();
            }
        }

        public static void Shutdown()
        {
            if (instance != null)
            {
                instance.Dispose();
                instance = null;
            }
        }
        #endregion

        private SlimDX.XAudio2.XAudio2 xaudio;
        private SlimDX.XAudio2.MasteringVoice masteringVoice;
        private SlimDX.XAudio2.SourceVoice sourceVoice;
        private SlimDX.XAudio2.AudioBuffer audioBuffer;
        private SlimDX.DataStream dataStream;
        private IPlayable current;
        private IPlayableStream currentStream;
        private int streamLength;
        private int streamBuffered;
        private object mutex = new object();
        private System.Threading.Thread decodeThread;

        private const int NumStreamingBuffers = 8;
        private const int StreamingBufferSize = 4096;
        private List<byte[]> byteBuffers;
        private List<SlimDX.XAudio2.AudioBuffer> streamBuffers;
        private System.Collections.BitArray bufferStatus;
        private EventHandler<ContextEventArgs> bufferEndCallback;

        private bool isPaused;

        private SoundManager() {
        }

        ~SoundManager()
        {
            this.Dispose();
        }

        private void InitInstance()
        {
            lock (mutex)
            {
                bufferEndCallback = new EventHandler<ContextEventArgs>(streaming_BufferEnd);
                streamBuffers = new List<AudioBuffer>(NumStreamingBuffers);
                byteBuffers = new List<byte[]>(NumStreamingBuffers);
                bufferStatus = new System.Collections.BitArray(NumStreamingBuffers, false);
                for (var i = 0; i < NumStreamingBuffers; i++)
                {
                    byte[] byteBuff = new byte[StreamingBufferSize];
                    byteBuffers.Add(byteBuff);
                    AudioBuffer audioBuff = new AudioBuffer();
                    audioBuff.AudioData = new SlimDX.DataStream(byteBuff, true, true);
                    streamBuffers.Add(audioBuff);
                }

                xaudio = new XAudio2();
                masteringVoice = new MasteringVoice(xaudio);
            }
        }

        public void Play(IPlayable wave)
        {
            lock (mutex)
            {
                if (wave == current)
                {
                    if (isPaused)
                    {
                        sourceVoice.Start();
                        isPaused = false;
                        return;
                    }
                }

                DisposeWave();
                xaudio.CommitChanges();
                isPaused = false;
                current = wave;
                currentStream = wave as IPlayableStream;

                if (decodeThread != null)
                {
                    System.Threading.Monitor.Pulse(mutex);
                    decodeThread = null;
                }

                if (currentStream == null)
                {
                    sourceVoice = new SourceVoice(xaudio, wave.WaveFormat);
                    dataStream = new SlimDX.DataStream(wave.WaveData, true, false);

                    audioBuffer = new SlimDX.XAudio2.AudioBuffer();
                    audioBuffer.AudioData = dataStream;
                    audioBuffer.AudioBytes = wave.WaveData.Length;
                    audioBuffer.Flags = BufferFlags.EndOfStream;

                    sourceVoice.SubmitSourceBuffer(audioBuffer);
                    sourceVoice.Start();
                }
                else
                {
                    streamLength = currentStream.StreamLength;
                    streamBuffered = 0;

                    sourceVoice = new SourceVoice(xaudio, wave.WaveFormat);
                    sourceVoice.BufferEnd += bufferEndCallback;

                    // Fill buffers initially
                    bool isDone = false;
                    for (var i = 0; i < NumStreamingBuffers; i++)
                    {
                        isDone = FillBuffer(i, currentStream);
                        if (isDone)
                        {
                            break;
                        }
                    }

                    sourceVoice.Start();
                    if (!isDone)
                    {
                        System.Threading.ParameterizedThreadStart threadProc = DecoderThread;
                        decodeThread = new System.Threading.Thread(threadProc);
                        decodeThread.Name = "Vorbis Decoder Thread";
                        decodeThread.Start(currentStream);
                    }
                }
            }
        }

        void streaming_BufferEnd(object sender, ContextEventArgs e)
        {
            lock (mutex)
            {
                // Unset the bit for the completed buffer
                bufferStatus.Set((int)e.Context, false);

                // Signal to the DecoderThread
                System.Threading.Monitor.Pulse(mutex);
            }
        }

        private void DecoderThread(object wave)
        {
            IPlayableStream stream = wave as IPlayableStream;

            lock (mutex)
            {
                while (true)
                {
                    if (current != wave)
                    {
                        return;
                    }

                    // Fill any unfilled buffers
                    for (var i = 0; i < bufferStatus.Count; i++)
                    {
                        if (bufferStatus.Get(i) == false)
                        {
                            bool isDone = FillBuffer(i, stream);
                            if (isDone)
                            {
                                return;
                            }
                        }
                    }

                    // Wait to be signalled that a buffer is empty
                    System.Threading.Monitor.Wait(mutex);
                }
            }
        }

        private bool FillBuffer(int bufferIdx, IPlayableStream stream)
        {
            AudioBuffer buffer = this.streamBuffers[bufferIdx];
            byte[] byteBuffer = this.byteBuffers[bufferIdx];

            bool isDone;
            var dataRead = stream.GetWaveData(byteBuffer, StreamingBufferSize);
            if (dataRead > 0)
            {
                streamBuffered += dataRead;
                isDone = (streamBuffered >= streamLength);
                buffer.Flags = isDone ? BufferFlags.EndOfStream : BufferFlags.None;
                buffer.AudioBytes = dataRead;
                buffer.Context = (IntPtr)bufferIdx;
                sourceVoice.SubmitSourceBuffer(buffer);
                bufferStatus.Set(bufferIdx, true);
            } else {
                isDone = true;
            }

            return isDone;
        }

        public void Pause()
        {
            lock (mutex)
            {
                if (sourceVoice != null)
                {
                    if (isPaused)
                    {
                        sourceVoice.Start();
                        isPaused = false;
                    }
                    else
                    {
                        sourceVoice.Stop();
                        isPaused = true;
                    }
                }
            }
        }

        public void Stop()
        {
            DisposeWave();
        }

        private void DisposeWave()
        {
            lock (mutex)
            {
                if (dataStream != null)
                {
                    dataStream.Dispose();
                    dataStream = null;
                }

                if (audioBuffer != null)
                {
                    audioBuffer.Dispose();
                    audioBuffer = null;
                }

                if (sourceVoice != null)
                {
                    if (currentStream != null)
                    {
                        sourceVoice.BufferEnd -= bufferEndCallback;
                    }
                    sourceVoice.Stop();
                    sourceVoice.FlushSourceBuffers();
                    sourceVoice.Dispose();
                    sourceVoice = null;
                }

                current = null;
                currentStream = null;
            }
        }

        public void Dispose()
        {
            lock (mutex)
            {
                DisposeWave();

                if (decodeThread != null)
                {
                    decodeThread.Abort();
                    System.Threading.Monitor.Pulse(mutex);
                    decodeThread = null;
                }

                if (streamBuffers != null)
                {
                    foreach (AudioBuffer buffer in streamBuffers)
                    {
                        buffer.AudioData.Dispose();
                        buffer.Dispose();
                    }
                    byteBuffers = null;
                    streamBuffers = null;
                }

                if (masteringVoice != null)
                {
                    masteringVoice.Dispose();
                    masteringVoice = null;
                }

                if (xaudio != null)
                {
                    xaudio.Dispose();
                    xaudio = null;
                }
            }
        }
    }
}
