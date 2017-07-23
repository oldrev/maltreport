using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
    public class ImageBlob : Blob
    {
        public ImageBlob(string extensionName, byte[] buffer) : base(extensionName, buffer)
        {
        }
    }
}
