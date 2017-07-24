using System;
using System.IO;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface IDocument
    {
        Task SaveAsync(Stream outStream);

        void Save(Stream outStream);

        byte[] AsBuffer();
    }

    public static class DocumentExtensions
    {
        public static void Save(this IDocument self, string path) =>
            self.SaveAsync(path).GetAwaiter().GetResult();

        public static async Task SaveAsync(this IDocument self, string path)
        {
            using (var stream = File.Create(path))
            {
                await self.SaveAsync(stream);
            }
        }

        public static string ToBase64String(this IDocument self) =>
            Convert.ToBase64String(self.AsBuffer());
    }
}