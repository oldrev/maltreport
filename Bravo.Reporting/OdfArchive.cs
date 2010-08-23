using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

namespace Bravo.Reporting
{
    public class OdfArchive
    {
        public const string ENTRY_MIMETYPE = "mimetype";
        public const string ENTRY_CONTENT = "content.xml";
        public const string ENTRY_MANIFEST = "META-INF/manifest.xml";
        public const string ENTRY_SETTINGS = "settings.xml";

        /// <summary>
        /// 加载到内存的 ODF 的文件内容
        /// </summary>
        protected IDictionary<string, byte[]> odfEntries = new Dictionary<string, byte[]>();

        public OdfArchive()
        {
            this.OdfPath = null;
        }

        public OdfArchive(string odfPath)
        {
            if (string.IsNullOrEmpty(odfPath))
            {
                throw new ArgumentNullException();
            }

            this.OdfPath = odfPath;

            this.LoadContents(odfPath);
        }

        public OdfArchive(Stream odfStream)
        {
            if (odfStream == null)
            {
                throw new ArgumentNullException();
            }

            this.LoadContents(odfStream);
        }

        private void LoadContents(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                this.LoadContents(fs);
            }
        }

        private void LoadContents(Stream stream)
        {
            //把 zip 的内容加载到内存
            using (var zf = new ZipFile(this.OdfPath))
            {
                foreach (ZipEntry ze in zf)
                {
                    var zis = zf.GetInputStream(ze.ZipFileIndex);

                    using (var ws = this.GetContentOutputStream(ze.Name))
                    {
                        CopyStream(zis, ws);
                    }
                }
            }
        }

        public void Save(Stream outStream)
        {
            //ODF 格式约定 mimetype 必须为第一个文件
            if (!this.odfEntries.ContainsKey(ENTRY_MIMETYPE))
            {
                throw new InvalidDataException("Missing entry 'mimetype'");
            }

            using (var zos = new ZipOutputStream(outStream))
            {
                zos.SetLevel(9);
                zos.UseZip64 = UseZip64.Off;

                this.WriteZipEntry(zos, ENTRY_MIMETYPE);

                foreach (var item in this.odfEntries)
                {
                    if (item.Key == ENTRY_MIMETYPE)
                    {
                        continue;
                    }

                    this.WriteZipEntry(zos, item.Key);
                }
            }
        }

        private void WriteZipEntry(ZipOutputStream zipStream, string name)
        {
            var data = this.odfEntries[name];
            var ze = new ZipEntry(name);
            zipStream.PutNextEntry(ze);
            zipStream.Write(data, 0, data.Length);
            zipStream.CloseEntry();
        }

        public string OdfPath { get; set; }

        public IEnumerable<string> OdfEntryNames
        {
            get { return this.odfEntries.Keys; }
        }

        public Stream GetContentInputStream(string name)
        {
            var data = this.odfEntries[name];
            return new MemoryStream(data);
        }

        public StreamReader GetContentReader(string name)
        {
            return new StreamReader(this.GetContentInputStream(name));
        }

        public Stream GetContentOutputStream(string name)
        {
            var ms = new OutputMemoryStream(name, this.odfEntries);
            this.odfEntries[name] = ms.GetBuffer();

            return ms;
        }

        public StreamWriter GetContentWriter(string name)
        {
            return new StreamWriter(this.GetContentOutputStream(name));
        }

        public bool Exists(string name)
        {
            return this.odfEntries.ContainsKey(name);
        }

        public void CopyTo(OdfArchive arc)
        {
            if (arc == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var item in this.odfEntries)
            {
                using (var outStream = arc.GetContentOutputStream(item.Key))
                {
                    outStream.Write(item.Value, 0, item.Value.Length);
                }
            }
        }

        protected static void CopyStream(Stream src, Stream dest)
        {
            var bufSize = 2048;
            var buf = new byte[bufSize];
            int nRead = 0;
            while ((nRead = src.Read(buf, 0, bufSize)) > 0)
            {
                dest.Write(buf, 0, nRead);
            }
        }

    }
}
