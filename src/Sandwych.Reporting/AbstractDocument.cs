using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{

    public abstract class AbstractDocument<TDocument> : IDocument
        where TDocument : AbstractDocument<TDocument>, new()
    {
        public bool IsNew { get; protected set; } = true;
        public abstract Task LoadAsync(Stream inStream);
        public abstract Task SaveAsync(Stream outStream);

        protected virtual void OnLoaded()
        {
            this.IsNew = false;
        }

        public static async Task<TDocument> LoadFromAsync(Stream inStream)
        {
            var doc = new TDocument();
            await doc.LoadAsync(inStream);
            return doc;
        }

        public static async Task<TDocument> LoadFromAsync(string filePath)
        {
            using (var inStream = File.OpenRead(filePath))
            {
                var doc = new TDocument();
                await doc.LoadAsync(inStream);
                return doc;
            }
        }

    }

}
