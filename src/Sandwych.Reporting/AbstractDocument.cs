using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public abstract class AbstractDocument<TDocument> 
        where TDocument : AbstractDocument<TDocument>, new()
    {
        public abstract byte[] AsBuffer();
        protected abstract void OnLoad(Stream inStream);
        protected abstract Task OnLoadAsync(Stream inStream);
        public abstract void Save(Stream outStream);
        public abstract Task SaveAsync(Stream outStream);

        public static async Task<TDocument> LoadAsync(Stream inStream)
        {
            var doc = new TDocument();
            await doc.OnLoadAsync(inStream);
            return doc;
        }

        public static TDocument Load(Stream inStream)
        {
            var doc = new TDocument();
            doc.OnLoad(inStream);
            return doc;
        }

        public static async Task<TDocument> LoadAsync(string filePath)
        {
            using (var inStream = File.OpenRead(filePath))
            {
                var doc = new TDocument();
                await doc.OnLoadAsync(inStream);
                return doc;
            }
        }

        public static TDocument Load(string filePath)
        {
            return LoadAsync(filePath).GetAwaiter().GetResult();
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
