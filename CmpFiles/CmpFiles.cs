using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CmpFiles
{
    public static class Compressor
    {
        static void justCopy(string SourcePath, string DestFolder, string message)
        {
            Console.WriteLine(message);
            var ext = Path.GetExtension(SourcePath);
            File.Copy(SourcePath, FromPath(SourcePath, DestFolder), true);
        }
        static string GetExtension(string SourcePath)
        {
            return Path.GetExtension(SourcePath);
        }
        static string FromPath(string SourcePath, string DestFolder)
        {
            var fileName = Path.GetFileNameWithoutExtension(SourcePath);
            string destPath = Path.Combine(DestFolder, fileName)+ GetExtension(SourcePath);
            return destPath;

        }
        static bool tryCompressImage(string SourcePath, ref string DestPath)
        {
            var _destPath = Path.GetTempFileName() + ".jpg"; ;
            try
            {
                compressImage(SourcePath, _destPath, 50);
                DestPath = _destPath;
                var sourceInfo = new FileInfo(SourcePath);
                var resultInfo = new FileInfo(DestPath);
                if (sourceInfo.Length < resultInfo.Length)
                {
                    File.Delete(_destPath);
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        static bool checkIfImage(string SourcePath)
        {
            try
            {
                new Bitmap(SourcePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CompressImage(string SourcePath, string DestFolder, int quality)
        {
            bool isImage = checkIfImage(SourcePath);
            if (!isImage)
            {
                justCopy(SourcePath, DestFolder, $"Файл \"{SourcePath}\" не является изображением, скопировано");
                return;
            }
            string compressedImage = SourcePath;
            bool isCompressed = tryCompressImage(SourcePath, ref compressedImage);
            string shrinkedImage = string.Empty;
            ShrinkImage(compressedImage, ref shrinkedImage, quality);
            var fileName = Path.GetFileNameWithoutExtension(SourcePath);
            var destPath = Path.Combine(DestFolder, fileName) + Path.GetExtension(SourcePath);
            File.Copy(shrinkedImage, destPath);
            if (isCompressed)
                File.Delete(compressedImage);
            File.Delete(shrinkedImage);
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
                    new Rectangle(0, 0, newWidth, newHeight),
                    new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                    GraphicsUnit.Pixel);
            }

            return resizedBitmap;
        }

        static void ShrinkImage(string SourcePath, ref string shrinkedImage, int quality)
        {
             shrinkedImage = Path.GetTempFileName() + ".jpg"; 
            var fileName = Path.GetFileNameWithoutExtension(SourcePath);
            Bitmap bitmap;
            using (Bitmap bmp = new Bitmap(SourcePath))
            {
                bitmap = ReduceBitmap(bmp, quality);
            }
            bitmap.Save(shrinkedImage, ImageFormat.Jpeg);
        }
    }
}
