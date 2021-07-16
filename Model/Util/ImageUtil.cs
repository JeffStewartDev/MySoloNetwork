using System.Drawing;

namespace MSN.Util
{
    public static class ImageUtlil
    {
        public static bool AdjustImage(string WWWRootFolder, string ImageAbsoluteFilePath, bool RotateImage = false, bool FlipImage = false)
        {
            bool result = false;
            if (RotateImage || FlipImage)
                try
                {
                    string path = $"{WWWRootFolder}{ImageAbsoluteFilePath}";
                    using Bitmap bitmap = new Bitmap(path);
                    if (RotateImage)
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    if (FlipImage)
                        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap.Dispose();
                    result = true;
                }
                catch (System.Exception)
                {
                    result = false;
                }
            return result;
        }
    }
}