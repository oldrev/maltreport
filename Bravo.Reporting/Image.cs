using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Bravo.Reporting
{
    public class Image
    {

        private byte[] data;
        private string documentFileName;

        public Image(string extensionName, byte[] imageData)
        {
            if (string.IsNullOrEmpty(extensionName))
            {
                throw new ArgumentNullException("extensionName");
            }

            if (imageData == null || imageData.Length <= 0)
            {
                throw new ArgumentNullException("imageData");
            }

            this.Id = Guid.NewGuid();
            this.ExtensionName = extensionName;
            this.data = imageData;
            this.SetDocumentFileName();
        }

        public Image(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentNullException("imagePath");
            }

            this.Id = Guid.NewGuid();
            this.ExtensionName = Path.GetExtension(imagePath).Substring(1);
            this.data = File.ReadAllBytes(imagePath);
            this.SetDocumentFileName();
        }

        private void SetDocumentFileName()
        {
            this.documentFileName = 
                this.Id.ToString("N").ToUpperInvariant() + "." + 
                this.ExtensionName.ToLowerInvariant();
        }

        public Guid Id { get; private set; }

        public string ExtensionName { get; private set; }

        public byte[] GetData()
        {
            Debug.Assert(this.data != null);
            return this.data;
        }

        public int DataSize
        {
            get
            {
                Debug.Assert(this.data != null);
                return this.data.Length;
            }
        }

        public string DocumentFileName
        {
            get
            {
                return this.documentFileName;
            }
        }

        public override bool Equals(object obj)
        {
            return this.Id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
