//作者：李维
//创建时间：2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using ICSharpCode.SharpZipLib.Zip;

namespace Bravo.Reporting
{
    public class OdfDocument
    {
        private const string ENTRY_MIMETYPE = "mimetype";
        public const string ENTRY_CONTENT = "content.xml";
        public const string ENTRY_MANIFEST = "META-INF/manifest.xml";
        public const string ENTRY_SETTINGS = "settings.xml";

        /// <summary>
        /// 加载到内存的 ODF 的文件内容
        /// </summary>
        protected IDictionary<string, byte[]> odfEntries = new Dictionary<string, byte[]>();

        public OdfDocument()
        {
            this.OdfPath = null;
        }

        public OdfDocument(string odfPath)
        {
            if (string.IsNullOrEmpty(odfPath))
            {
                throw new ArgumentNullException();
            }

            this.OdfPath = odfPath;

            this.LoadContents(odfPath);
        }

        public OdfDocument(Stream documentStream)
        {
            if (documentStream == null)
            {
                throw new ArgumentNullException("documentStream");
            }

            this.LoadContents(documentStream);
        }

        private void LoadContents(string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));

            using (var fs = File.OpenRead(path))
            {
                this.LoadContents(fs);
            }
        }

        private void LoadContents(Stream stream)
        {
            Debug.Assert(stream != null);

            //把 zip 的内容加载到内存
            using (var zf = new ZipFile(this.OdfPath))
            {
                foreach (ZipEntry ze in zf)
                {
                    var zis = zf.GetInputStream(ze.ZipFileIndex);

                    using (var ws = this.GetEntryOutputStream(ze.Name))
                    {
                        CopyStream(zis, ws);
                    }
                }
            }
        }

        public void Save(Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite)
            {
                throw new ArgumentException("outStream");
            }

            //ODF 格式约定 mimetype 必须为第一个文件
            if (!this.odfEntries.ContainsKey(ENTRY_MIMETYPE))
            {
                throw new InvalidDataException("Missing entry 'mimetype'");
            }

            using (var zos = new ZipOutputStream(outStream))
            {
                //zos.SetLevel(9);
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

        public void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            using (var fs = File.Open(path, FileMode.CreateNew, FileAccess.Write))
            {
                this.Save(fs);
            }
        }

        private void WriteZipEntry(ZipOutputStream zipStream, string name)
        {
            Debug.Assert(zipStream != null);
            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(this.odfEntries.ContainsKey(name));

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

        public Stream GetEntryInputStream(string name)
        {
            var data = this.odfEntries[name];
            return new MemoryStream(data);
        }

        public TextReader GetEntryReader(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return new StreamReader(this.GetEntryInputStream(name));
        }

        /// <summary>
        /// 千万要记得关闭
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Stream GetEntryOutputStream(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            var oms = new OutputMemoryStream(name, this.odfEntries);
            this.odfEntries[name] = oms.GetBuffer();

            return oms;
        }

        public TextWriter GetEntryWriter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return new StreamWriter(this.GetEntryOutputStream(name));
        }

        public bool Exists(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return this.odfEntries.ContainsKey(name);
        }

        public void CopyTo(OdfDocument destDoc)
        {
            if (destDoc == null)
            {
                throw new ArgumentNullException("destDoc");
            }

            foreach (var item in this.odfEntries)
            {
                using (var outStream = destDoc.GetEntryOutputStream(item.Key))
                {
                    outStream.Write(item.Value, 0, item.Value.Length);
                }
            }
        }

        protected static void CopyStream(Stream src, Stream dest)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

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
