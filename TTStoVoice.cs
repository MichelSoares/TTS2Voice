using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TTS2Voice
{
    public static class TTStoVoice
    {
        public static async Task TTStoVoicePollyAWS(string nameFile, string txtToVoice, string pathDiretoryMidias, string vozTTS)
        {
            //Coloque suas credenciais da API, abaixo
            string accessKey = "****************";
            string secretKey = "*********************";

            Amazon.Polly.Model.SynthesizeSpeechRequest request = null;

            using (var pollyClient = new AmazonPollyClient(accessKey, secretKey, RegionEndpoint.USWest2))
            {
                switch (vozTTS)
                {
                    case "Camila":
                        request = new SynthesizeSpeechRequest
                        {
                            OutputFormat = OutputFormat.Pcm,
                            SampleRate = "8000",
                            Text = txtToVoice,
                            VoiceId = VoiceId.Camila
                        };
                        break;
                    case "Ricardo":
                        request = new SynthesizeSpeechRequest
                        {
                            OutputFormat = OutputFormat.Pcm,
                            SampleRate = "8000",
                            Text = txtToVoice,
                            VoiceId = VoiceId.Ricardo
                        };
                        break;
                    case "Vitoria":
                        request = new SynthesizeSpeechRequest
                        {
                            OutputFormat = OutputFormat.Pcm,
                            SampleRate = "8000",
                            Text = txtToVoice,
                            VoiceId = VoiceId.Vitoria
                        };
                        break;
                    default:
                        break;
                }

                var response = await pollyClient.SynthesizeSpeechAsync(request);

                if (!Directory.Exists(pathDiretoryMidias)) Directory.CreateDirectory(pathDiretoryMidias);

                string pcmFilePath = Path.Combine(pathDiretoryMidias, nameFile + ".pcm");
                string wavFilePath = Path.Combine(pathDiretoryMidias, nameFile + ".wav");

                using (var outputStream = new FileStream(pcmFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[2 * 1024];
                    int readBytes;

                    var inputStream = response.AudioStream;
                    while ((readBytes = inputStream.Read(buffer, 0, 2 * 1024)) > 0)
                    {
                        outputStream.Write(buffer, 0, readBytes);
                    }

                    outputStream.Flush();
                }

                ConvertPcmToWav(pcmFilePath, wavFilePath, 8000, 16, 1);            
                File.Delete(pcmFilePath);
                Console.WriteLine("Arquivo gerado com sucesso {0}", wavFilePath);
            }
        }

        static void ConvertPcmToWav(string pcmFilePath, string wavFilePath, int sampleRate, int bitsPerSample, int channels)
        {
            using (var pcmStream = new FileStream(pcmFilePath, FileMode.Open, FileAccess.Read))
            using (var wavStream = new FileStream(wavFilePath, FileMode.Create, FileAccess.Write))
            {
                var waveFormat = new WaveFormat(sampleRate, bitsPerSample, channels);

                using (var writer = new WaveFileWriter(wavStream, waveFormat))
                {
                    var buffer = new byte[4096];
                    int bytesRead;

                    while ((bytesRead = pcmStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

    }
}
