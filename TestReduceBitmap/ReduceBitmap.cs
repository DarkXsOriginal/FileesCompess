using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace TestReduceBitmap
{
    internal class ReduceBitmaps
    {
        public static Bitmap ReduceBitmap(Bitmap originalBitmap, int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentException("Процент должен быть в диапазоне от 0 до 100");

            if (percent == 100)
                return (Bitmap)originalBitmap.Clone();

            // Вычисляем новые размеры
            float ratio = percent / 100f;
            int newWidth = (int)(originalBitmap.Width * ratio);
            int newHeight = (int)(originalBitmap.Height * ratio);

            // Создаём новый битмап с новыми размерами
            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);

            // Используем Graphics для высококачественного масштабирования
            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(
                    originalBitmap,
                    new System.Drawing.Rectangle(0, 0, newWidth, newHeight),
                    new System.Drawing.Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                    GraphicsUnit.Pixel);
            }

            return resizedBitmap;
        }
        private static void compressImage(string SourcePath, string DestPath, int quality)
        {
            using (Bitmap bmp1 = new Bitmap(SourcePath))
            {
                ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder QualityEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(QualityEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp1.Save(DestPath, pngEncoder, myEncoderParameters);
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
