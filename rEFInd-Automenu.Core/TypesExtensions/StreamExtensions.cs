using System;
using System.IO;
using System.Threading.Tasks;

namespace rEFInd_Automenu.TypesExtensions
{
    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null)
        {
            long totalBytesRead = 0;
            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                int bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    progress?.Report(totalBytesRead);
                    break;
                }

                await destination.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }
}
