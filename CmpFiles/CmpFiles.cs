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
        static void justCopy(string SourcePath, string originalDest,string message)
            {
                Console.WriteLine(message);
                var ext = Path.GetExtension(SourcePath);
                File.Copy(SourcePath, originalDest, true);
            }

        public static void CompressImage(string SourcePath, string DestPath, int quality)
        {

            var fileName = Path.GetFileNameWithoutExtension(SourcePath);
            string destFilePathWithoutExtension = DestPath + "\\" + fileName;
            string originalExt = Path.GetExtension(SourcePath);
            string jpgExt = ".jpg";

            string originalDest = destFilePathWithoutExtension + originalExt;
            string jpgDest = destFilePathWithoutExtension + jpgExt;


            void justCopyCompress()
            {
                justCopy(SourcePath, originalDest, $"Не удаётся сжать \"{SourcePath}\", скопировано");
            }

            var tempJpg = Path.GetTempFileName() + jpgExt;
            try
            {
                compressImage(SourcePath, tempJpg, 50);
            }
            catch
            {
                justCopyCompress();
                return;
            }

            ShrinkImage(SourcePath, tempJpg, quality);
            compressImage(tempJpg, jpgDest, quality);
            
            var sourceInfo = new FileInfo(SourcePath);
            var resultInfo = new FileInfo(jpgDest);
            if (sourceInfo.Length < resultInfo.Length)
            {
                File.Delete(jpgDest);
                justCopyCompress();
            }
            File.Delete(tempJpg);


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

        public static void ShrinkImage(string SourcePath, string DestPath, int quality)
        {
            var fileName = Path.GetFileNameWithoutExtension(SourcePath);

            string destFilePathWithoutExtension = DestPath + "\\" + fileName;
            string originalExt = Path.GetExtension(SourcePath);
            string jpgExt = ".jpg";

            string originalDest = destFilePathWithoutExtension + originalExt;
            string jpgDest = destFilePathWithoutExtension + jpgExt;


            void justCopyShrink()
            {
                justCopy(SourcePath, originalDest, $"Не удаётся уменьшить \"{SourcePath}\", скопировано");
            }

            try
            {
                Bitmap bitmap;
                using (Bitmap bmp = new Bitmap(SourcePath))
                {
                    bitmap = ReduceBitmap(bmp, quality);
                }
                bitmap.Save(DestPath, ImageFormat.Jpeg);
            }
            catch
            {
                justCopyShrink();
            }
        }
    }
}
