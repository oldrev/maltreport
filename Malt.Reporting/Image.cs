using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Malt.Reporting
{
    /// <summary>
    /// 
    /// </summary>
    public class Image : IEquatable<Image>
    {
        private readonly Guid id;
        private readonly string extName;
        private readonly byte[] data;
        private readonly string documentFileName;

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

            this.id = Guid.NewGuid();
            this.extName = extensionName.ToLowerInvariant();
            this.data = imageData;
            this.documentFileName = this.MakeDocumentFileName();
        }

        public Image(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentNullException("imagePath");
            }

            this.id = Guid.NewGuid();
            this.extName = Path.GetExtension(imagePath).Substring(1).ToLowerInvariant();
            this.data = File.ReadAllBytes(imagePath);
            this.documentFileName = this.MakeDocumentFileName();
        }

        private string MakeDocumentFileName()
        {
            return this.Id.ToString("N").ToUpperInvariant() + "." + 
                this.ExtensionName.ToLowerInvariant();
        }

        public Guid Id
        {
            get { return this.id; }
        }

        public string ExtensionName
        {
            get { return this.extName; }
        }

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
            get { return this.documentFileName; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var rhs = obj as Image;
            if (rhs == null)
            {
                return false;
            }

            if (this.Id == rhs.Id)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #region IEquatable<Image> 成员

        public bool Equals(Image other)
        {
            return other.Id == this.Id;
        }

        #endregion
    }
}
