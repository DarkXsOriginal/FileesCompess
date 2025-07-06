using CmpFiles;
using System.Drawing;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {


            string ImagePath = "C:\\Users\\Leon\\Documents\\Picture\\2bc0784868937b62f92068cb2836d861.png";
            using (Bitmap bmp1 = new Bitmap(ImagePath))
            {
                Bitmap bitmap = Compressor.ReduceBitmap(bmp1, 50);
                Assert.IsTrue(bitmap.Width < bmp1.Width);
            }

        }
    }
}
