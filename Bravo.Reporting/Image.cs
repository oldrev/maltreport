using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bravo.Reporting
{
    public class Image
    {
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
            this.Data = imageData;
        }

        public Image(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentNullException("imagePath");
            }

            this.Id = Guid.NewGuid();
            this.ExtensionName = Path.GetExtension(imagePath).Substring(1);
            this.Data = File.ReadAllBytes(imagePath);
        }

        public Guid Id { get; private set; }

        public string ExtensionName { get; private set; }

        public byte[] Data { get; private set; }

        public string DocumentFileName
        {
            get
            {
                return this.Id.ToString("N").ToUpper() + "." + this.ExtensionName;
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
