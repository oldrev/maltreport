using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface IDocument
    {
        Task LoadAsync(Stream inStream);

        void Load(Stream inStream);

        Task SaveAsync(Stream outStream);

        void Save(Stream outStream);

        byte[] AsBuffer();
    }


    public static class DocumentExtensions
    {
        public static void Load(this IDocument self, string path) =>
            self.LoadAsync(path).GetAwaiter().GetResult();

        public static async Task LoadAsync(this IDocument self, string path)
        {
            using (var stream = File.OpenRead(path))
            {
                await self.LoadAsync(stream);
            }
        }

        public static void Save(this IDocument self, string path) =>
            self.SaveAsync(path).GetAwaiter().GetResult();


        public static async Task SaveAsync(this IDocument self, string path)
        {
            using (var stream = File.Create(path))
            {
                await self.SaveAsync(stream);
            }
        }

        public static string ToBase64String(this IDocument self)
        {
            return Convert.ToBase64String(self.AsBuffer());
        }


    }

}
