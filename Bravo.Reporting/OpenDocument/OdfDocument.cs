//作者：李维
//创建时间：2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;

using ICSharpCode.SharpZipLib.Zip;

namespace Bravo.Reporting.OpenDocument
{
    public class OdfDocument : DocumentBase
    {
        public const string MimeTypeEntryPath = "mimetype";
        public const string ManifestEntryPath = "META-INF/manifest.xml";
        public const string SettingsEntryPath = "settings.xml";

        /// <summary>
        /// 加载到内存的 ODF 的文件内容
        /// </summary>
        private IDictionary<string, byte[]> odfEntries = new Dictionary<string, byte[]>();

        public OdfDocument()
        {
            this.Path = null;
        }

        public OdfDocument(string odfPath)
        {
            if (string.IsNullOrEmpty(odfPath))
            {
                throw new ArgumentNullException("odfPath");
            }

            this.Path = odfPath;

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
            using (var zf = new ZipFile(this.Path))
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

        public override void Save(Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite)
            {
                throw new ArgumentNullException("outStream");
            }

            //ODF 格式约定 mimetype 必须为第一个文件
            if (!this.odfEntries.ContainsKey(MimeTypeEntryPath))
            {
                throw new InvalidDataException("Can not found entry: 'mimetype'");
            }

            using (var zos = new ZipOutputStream(outStream))
            {
                //zos.SetLevel(0);
                zos.UseZip64 = UseZip64.Off;

                this.WriteZipEntry(zos, MimeTypeEntryPath);

                foreach (var item in this.odfEntries)
                {
                    if (item.Key == MimeTypeEntryPath)
                    {
                        continue;
                    }

                    this.WriteZipEntry(zos, item.Key);
                }
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

        public string Path { get; private set; }

        public override ICollection<string> EntryPaths
        {
            get { return this.odfEntries.Keys; }
        }

        public override Stream GetEntryInputStream(string entryPath)
        {
            var data = this.odfEntries[entryPath];
            return new MemoryStream(data);
        }

        /// <summary>
        /// 千万要记得关闭
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Stream GetEntryOutputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            var oms = new OutputMemoryStream(entryPath, this.odfEntries);
            this.odfEntries[entryPath] = oms.GetBuffer();

            return oms;
        }

        public override bool EntryExists(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }
            return this.odfEntries.ContainsKey(entryPath);
        }

        public override string AddImage(Image img)
        {
            var fullPath = "Pictures/" + img.DocumentFileName;
            using (var outStream = this.GetEntryOutputStream(fullPath))
            {
                outStream.Write(img.GetData(), 0, img.DataSize);
            }

            var manifestDoc = new OdfManifestDocument();
            using (var manifestInStream = this.GetEntryInputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Load(manifestInStream);
            }

            manifestDoc.AppendImageFileEntry(img.ExtensionName, fullPath);
            manifestDoc.CreatePicturesEntryElement();

            using (var manifestOutStream = this.GetEntryOutputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Save(manifestOutStream);
            }

            return fullPath;
        }


        public override string MainContentEntryPath { get { return "content.xml"; } }
    }
}
