using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmpFiles
{
    public class Compressor
    {
        public static void CompressImage(string SoucePath, string DestPath, int quality)
        {
            var fileName = Path.GetFileNameWithoutExtension(SoucePath);
            string destFilePathWithoutExtension = DestPath + "\\" + fileName;
            string originalExt = Path.GetExtension(SoucePath);
            string jpgExt = ".jpg";

            string originalDest = destFilePathWithoutExtension + originalExt;
            string jpgDest = destFilePathWithoutExtension + jpgExt;

            void justCopy()
            {
                Console.WriteLine($"Не удаётся сжать \"{SoucePath}\", скопировано");
                var ext = Path.GetExtension(SoucePath);
                File.Copy(SoucePath, originalDest, true);
            }

            var tempJpg = Path.GetTempFileName() + jpgExt;
            try
            {
                compressImage(SoucePath, tempJpg, 50);
            }
            catch
            {
                justCopy();
                return;
            }

            compressImage(tempJpg, jpgDest, quality);
            var sourceInfo = new FileInfo(SoucePath);
            var resultInfo = new FileInfo(jpgDest);
            if (sourceInfo.Length < resultInfo.Length)
            {
                File.Delete(jpgDest);
                justCopy();
            }
            File.Delete(tempJpg);


        }
        private static void compressImage(string SoucePath, string DestPath, int quality)
        {
            using (Bitmap bmp1 = new Bitmap(SoucePath))
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
