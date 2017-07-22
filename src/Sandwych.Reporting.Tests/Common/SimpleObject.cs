using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Tests.Common
{
    public class SimpleObject
    {
        private const string PngImagePath = "Sandwych.Reporting.Tests.Assets.JpegImage.jpeg";

        public SimpleObject()
        {
            using (var resStream = DocumentTestHelper.GetResource(PngImagePath))
            {
                this.JpegImage = Blob.FromStream("jpeg", resStream);
            }
        }


        public string StringValue => "Hello World!";

        public int IntegerValue => 1;

        public double DoubleValue => 3.1415926;

        public decimal DecimalValue => 3.14M;

        public Blob JpegImage { get; private set; }
    }

}
