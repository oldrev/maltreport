using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public static class DocumentAsyncExtensions
    {
        public static async Task LoadAsync(this IDocument self, string filePath)
        {
            using var fs = File.OpenRead(filePath);
            await self.LoadAsync(fs);
        }

        public static async Task LoadAsync(this IDocument self, byte[] buffer)
        {
            using var fs = new MemoryStream(buffer);
            await self.LoadAsync(fs);
        }

        public static async Task SaveAsync(this IDocument self, string filePath)
        {
            using var fs = File.Create(filePath);
            await self.SaveAsync(fs);
        }

        public static async Task<byte[]> ToBufferAsync(this IDocument self)
        {
            using var ms = new MemoryStream();
            await self.SaveAsync(ms);
            return ms.ToArray();
        }


    }
}
