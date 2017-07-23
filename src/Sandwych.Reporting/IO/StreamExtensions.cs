using System.IO;
using System.Threading.Tasks;

namespace Sandwych.Reporting.IO
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ReadAllBytesAsync(this Stream self)
        {
            using (var outStream = new MemoryStream())
            {
                var buf = new byte[1024 * 4];
                var nread = 0;
                while ((nread = await self.ReadAsync(buf, 0, buf.Length)) > 0)
                {
                    await outStream.WriteAsync(buf, 0, nread);
                }

                return outStream.ToArray();
            }
        }

        public static byte[] ReadAllBytes(this Stream self)
        {
            using (var outStream = new MemoryStream())
            {
                var buf = new byte[1024 * 4];
                var nread = 0;
                while ((nread = self.Read(buf, 0, buf.Length)) > 0)
                {
                    outStream.Write(buf, 0, nread);
                }

                return outStream.ToArray();
            }
        }
    }
}