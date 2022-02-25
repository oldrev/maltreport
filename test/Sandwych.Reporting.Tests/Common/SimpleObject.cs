using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Sandwych.Reporting.Tests.Common
{
    public class SimpleObject
    {
        private const string PngImagePath = "Sandwych.Reporting.Tests.Resources.Assets.JpegImage.jpeg";

        public SimpleObject()
        {
            using var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PngImagePath);
            var buf = new byte[resStream.Length];
            resStream.Read(buf, 0, buf.Length);
            this.JpegImage = new Blob("jpg", buf);
        }


        public string StringValue => "Hello World!";

        public int IntegerValue => 1;

        public double DoubleValue => 3.1415926;

        public decimal DecimalValue => 3.14M;

        public Blob JpegImage { get; private set; }
    }

}
