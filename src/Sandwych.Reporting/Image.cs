using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Sandwych.Reporting
{
    /// <summary>
    /// 
    /// </summary>
    public class Image : IEquatable<Image>
    {
        private readonly Guid _id;
        private readonly string _extName;
        private readonly byte[] _imgData;
        private readonly string _entryNameInDocument;

        public Image(string extensionName, byte[] imageData)
        {
            if (string.IsNullOrEmpty(extensionName) || extensionName.StartsWith("."))
            {
                throw new ArgumentNullException("extensionName");
            }

            if (imageData == null || imageData.Length <= 0)
            {
                throw new ArgumentNullException("imageData");
            }

            this._id = Guid.NewGuid();
            this._extName = extensionName.ToLowerInvariant();
            this._imgData = imageData;
            this._entryNameInDocument = this.MakeDocumentFileName();
        }

        public Image(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentNullException("imagePath");
            }

            this._id = Guid.NewGuid();
            this._extName = Path.GetExtension(imagePath).Substring(1).ToLowerInvariant();
            this._imgData = File.ReadAllBytes(imagePath);
            this._entryNameInDocument = this.MakeDocumentFileName();
        }

        private string MakeDocumentFileName()
        {
            return "img" + this.Id.ToString("N").ToUpperInvariant() + "." +
                this.ExtensionName.ToLowerInvariant();
        }

        public Guid Id
        {
            get { return this._id; }
        }

        public string ExtensionName
        {
            get { return this._extName; }
        }

        public byte[] GetData()
        {
            Debug.Assert(this._imgData != null);
            return this._imgData;
        }

        public int DataSize
        {
            get
            {
                Debug.Assert(this._imgData != null);
                return this._imgData.Length;
            }
        }

        public string DocumentFileName
        {
            get { return this._entryNameInDocument; }
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
