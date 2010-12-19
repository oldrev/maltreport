using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Xunit;

namespace Bravo.Reporting.Test
{
    public class ImageTest
    {
        const string ImagePath = "resources/go-home.PNG";

        [Fact]
        public void TestConstructor()
        {
            var img1 = new Image(ImagePath);
            Assert.Equal("png", img1.ExtensionName);
            Assert.Equal(File.ReadAllBytes(ImagePath), img1.GetData());
        }

        [Fact(DisplayName = "测试 Image 类的相等性比较")]
        public void TestImageEquals()
        {
            var img1 = new Image(ImagePath);

            var img2 = new Image("png", File.ReadAllBytes(ImagePath));

            Assert.NotSame(img1, img2);
            Assert.NotSame(img1.Id, img2.Id);

            var img3 = img1;

            Assert.Same(img1, img3);

            Assert.False(img1.Equals(img2));
            Assert.True(img1.Equals(img3));

        }
    }
}
