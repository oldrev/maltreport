using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public static class TypedDocumentExtensions
    {
        public static async Task SaveAsync<TDocument>(this TDocument self, string path, CancellationToken ct = default)
            where TDocument : ITypedDocument<TDocument>, new()
        {
            using var fs = File.OpenWrite(path);
            await self.SaveAsync(fs, ct);
        }

        public static async Task LoadAsync<TDocument>(this TDocument self, string path, CancellationToken ct = default)
            where TDocument : ITypedDocument<TDocument>, new()
        {
            using var fs = File.OpenRead(path);
            await self.LoadAsync(fs, ct);
        }
    }
}
