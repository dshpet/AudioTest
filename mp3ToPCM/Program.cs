using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

using NAudio.Wave;

namespace mp3ToPCM
{
    class Program
    {
        //[DllImport("D:\\Other\\mp3ToPCM\\mp3sharp15\\mp3sharp15\\mp3sharp\\mp3sharp\\bin\\Release\\Mp3Sharp.dll")]

        static private readonly string WavFilePath = "C:\\Users\\shpetnyi\\Downloads\\Audio\\Drum-loop-122-bpm\\Drum-loop-122-bpm.wav";
        static private readonly string mp3FilePath = "C:\\Users\\shpetnyi\\Downloads\\Audio\\test.mp3";

        static void Main(string[] args)
        {
            var A = generateSound(440, 48000, 5);
            var waveFormat = new WaveFormat(8000, 16, 1);
            using(WaveFileWriter writer = new WaveFileWriter("OLOLO.wav", waveFormat))
            {
                writer.WriteData(A, 0, A.Length);
            }


            return;

            NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.WaveFileReader(WavFilePath));
          
            if (pcm.WaveFormat.Channels != 2)
                return;

            int someInterval = pcm.WaveFormat.Channels * pcm.WaveFormat.SampleRate * pcm.WaveFormat.BitsPerSample / 8;

            float time = 10; // 10 secs
            float samplingRate = pcm.WaveFormat.SampleRate;
            int index = (int)(time / samplingRate);

            byte[] buffer = new byte[someInterval];
            int current = 0;
            int ret = 0;
            do
            {
                ret = pcm.Read(buffer, current, someInterval);

                float[] res = new float[buffer.Length / 2];

                for (int i = 0; i < res.Length; i += 2)
                {
                    res[i] = (buffer[i] + buffer[i + 1]) / 2 / 32768.0f;
                }

                
                current += someInterval;
            } while (ret != -1);
             
            PlayPCM(pcm);
        }

        static byte[] generateSound(float frequency, float samplingRate, float duration)
        {
            var pcm = new byte[(int)(samplingRate * duration)];
            float increment = 2 * (float)Math.PI * frequency / samplingRate;
            float angle = 0;

            for (int i = 0; i < pcm.Length; i++)
            {
                pcm[i] = (byte)(Math.Sin(angle) * 100);
                angle += increment;
            }

            return pcm;
        }

        static void PlayPCM(IWaveProvider _pcm)
        {
            var device = new WaveOut();
            device.Init(_pcm);

            device.Play();
            while (device.PlaybackState != PlaybackState.Stopped)
            {
                Thread.Sleep(20);
            }

            device.Stop();
        }
    }
}
