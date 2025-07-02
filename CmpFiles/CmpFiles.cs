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
    public class Compressor
    {
        public static void CompressImage(string SourcePath, string DestPath, int quality)
        {
            var fileName = Path.GetFileNameWithoutExtension(SourcePath);
            string destFilePathWithoutExtension = DestPath + "\\" + fileName;
            string originalExt = Path.GetExtension(SourcePath);
            string jpgExt = ".jpg";

            string originalDest = destFilePathWithoutExtension + originalExt;
            string jpgDest = destFilePathWithoutExtension + jpgExt;

            void justCopy()
            {
                Console.WriteLine($"Не удаётся сжать \"{SourcePath}\", скопировано");
                var ext = Path.GetExtension(SourcePath);
                File.Copy(SourcePath, originalDest, true);
            }

            var tempJpg = Path.GetTempFileName() + jpgExt;
            try
            {
                compressImage(SourcePath, tempJpg, 50);
            }
            catch
            {
                justCopy();
                return;
            }

            compressImage(tempJpg, jpgDest, quality);
            ShrinkImage(tempJpg, jpgDest, quality);
            var sourceInfo = new FileInfo(SourcePath);
            var resultInfo = new FileInfo(jpgDest);
            if (sourceInfo.Length < resultInfo.Length)
            {
                File.Delete(jpgDest);
                justCopy();
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

        public static void ShrinkImage(string SourcePath,string DestPath,int quality)
        {
            var fileName = Path.GetFileNameWithoutExtension(SourcePath);
            string jpgExt = ".jpg";
            string destFilePathWithoutExtension = DestPath + "\\" + fileName;
            string jpgDest = destFilePathWithoutExtension + jpgExt;
            Bitmap bitmap;

            using (Bitmap bmp = new Bitmap(SourcePath)){
            double percentQuality = (double)quality / 100;
            int newWidth = (int)(bmp.Width * percentQuality);
            int newHeight = (int)(bmp.Height * percentQuality);
            bitmap = ResizeBitmap(bmp, newWidth, newHeight);
            }
            bitmap.Save(SourcePath, ImageFormat.Jpeg);
            Console.WriteLine("Разрешение изображения изменено!!");
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(bmp, 0, 0, width, height);
            }
            return bitmap;
        }
    }
}
