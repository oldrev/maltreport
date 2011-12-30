using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using NUnit.Framework;

namespace Malt.Reporting
{
	[TestFixture]
    public class ImageTest
    {
        const string ImagePath = "resources/go-home.PNG";

        [Test]
        public void TestConstructor()
        {
            var img1 = new Image(ImagePath);
            Assert.AreEqual("png", img1.ExtensionName);
            Assert.AreEqual(File.ReadAllBytes(ImagePath), img1.GetData());
        }

        [Test]
        public void TestImageEquals()
        {
            var img1 = new Image(ImagePath);

            var img2 = new Image("png", File.ReadAllBytes(ImagePath));

            Assert.AreNotSame(img1, img2);
            Assert.AreNotSame(img1.Id, img2.Id);

            var img3 = img1;

            Assert.AreSame(img1, img3);

            Assert.That(!img1.Equals(img2));
            Assert.That(img1.Equals(img3));

        }
    }
}
