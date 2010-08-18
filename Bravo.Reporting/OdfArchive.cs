using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

namespace Bravo.Reporting
{
    public abstract class OdfArchive
    {
        public OdfArchive(string odfPath)
        {
            this.TmpDirectoryPath = Path.GetTempFileName();
            this.ExtractFilesToTmpDirectory(odfPath);
            this.OdfPath = odfPath;
        }

        private void ExtractFilesToTmpDirectory(string odfPath)
        {
            var fs = new FastZip();
            fs.ExtractZip(odfPath, this.TmpDirectoryPath, "*");
        }

        public string OdfPath { get; private set; }

        protected string TmpDirectoryPath { get; private set; }

        public string[] Files
        {
            get
            {
                var files = new List<string>();
                using (var zipStream = new ZipInputStream(File.OpenRead(this.OdfPath)))
                {
                    ZipEntry ze = null;
                    while ((ze = zipStream.GetNextEntry()) != null)
                    {
                        files.Add(ze.Name);
                    }
                }

                return files;
            }
        }

    }
}
