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
        public bool IsNew { get; private set; } = false;
        public abstract byte[] AsBuffer();
        public abstract void Load(Stream inStream);
        public abstract Task LoadAsync(Stream inStream);
        public abstract void Save(Stream outStream);
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

        public static TDocument LoadFrom(Stream inStream)
        {
            var doc = new TDocument();
            doc.Load(inStream);
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

        public static TDocument LoadFrom(string filePath)
        {
            return Task.Run(() => LoadFromAsync(filePath)).Result;
        }

        public static TDocument LoadFrom(byte[] buffer)
        {
            //TODO: optimize
            using (var ms = new MemoryStream(buffer))
            {
                return LoadFrom(ms);
            }
        }

        public void Save(string path)
        {
            using (var stream = File.Create(path))
            {
                this.Save(stream);
            }
        }

        public async Task SaveAsync(string path)
        {
            using (var stream = File.Create(path))
            {
                await this.SaveAsync(stream);
            }
        }

        public string ToBase64String() =>
            Convert.ToBase64String(this.AsBuffer());

    }

}
