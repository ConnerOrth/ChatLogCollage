using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLogCollage
{
    public class ImageEditor
    {
        private readonly string baseDir;
        private readonly string collageFile;

        public ImageEditor(string baseDir)
        {
            this.baseDir = baseDir;
            this.collageFile = Path.Combine(baseDir, "collage.jpg");
            if (!File.Exists(collageFile))
            {
                File.Create(collageFile);
            }
        }

        public static Bitmap GetBitmap(string filepath)
        {
            // Create a Bitmap object from a file.
            Bitmap myBitmap = new Bitmap(filepath);

            // Clone a portion of the Bitmap object.
            Rectangle cloneRect = new Rectangle(0, 600, 500, 350);
            System.Drawing.Imaging.PixelFormat format = myBitmap.PixelFormat;
            Bitmap cloneBitmap = myBitmap.Clone(cloneRect, format);
            return cloneBitmap;
        }
		
        public void CropImage(string filepath)
        {
            // Create a Bitmap object from a file.
            Bitmap myBitmap = new Bitmap(filepath);

            // Clone a portion of the Bitmap object.
            Rectangle cloneRect = new Rectangle(0, 600, 500, 350);
            System.Drawing.Imaging.PixelFormat format = myBitmap.PixelFormat;
            Bitmap cloneBitmap = myBitmap.Clone(cloneRect, format);

            // Save as JPEG
            using (MemoryStream jpegStream = new MemoryStream())
            {
                cloneBitmap.Save(jpegStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                //You have to rewind the MemoryStream before copying
                jpegStream.Seek(0, SeekOrigin.Begin);

                using (FileStream fs = new FileStream(filepath.Replace("jpg", ".png"), FileMode.OpenOrCreate))
                {
                    jpegStream.CopyTo(fs);
                    fs.Flush();
                }
            }
        }

        public void CollageImages(string filepath1, string filepath2)
        {
            int x = 0;
            int y = 600;
            int width = 500;
            int height = 350;

            // Create a Bitmap object from a file.
            Bitmap tmp = new Bitmap(collageFile);

            Bitmap file1 = new Bitmap(filepath1);
            Bitmap file2 = new Bitmap(filepath1);

            // Clone a portion of the Bitmap object.
            Rectangle cloneRect = new Rectangle(x, y, width, height);
            System.Drawing.Imaging.PixelFormat format = file1.PixelFormat;

            Bitmap cloneBitmap1 = file1.Clone(cloneRect, format);
            Bitmap cloneBitmap2 = file2.Clone(cloneRect, format);

            Bitmap collage = new Bitmap(tmp, new Size(width, tmp.Height+height));
        }

        public static Bitmap Merge(IEnumerable<Bitmap> images, MergeDirection mergeDirection = MergeDirection.Bottom)
        {
            var enumerable = images as IList<Bitmap> ?? images.ToList();

            int width = 0;
            int height = 0;

            // Get max width and height of the image
            foreach (var image in enumerable)
            {
                //width = image.Width > width ? image.Width : width;
                //height = image.Height > height ? image.Height : height;
                if (mergeDirection == MergeDirection.Bottom)
                {
                    width = image.Width > width ? image.Width : width;
                    height += image.Height;
                }
            }

            // merge images
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int currentOffset = 0;

                foreach (var image in enumerable)
                {
                    switch (mergeDirection)
                    {
                        case MergeDirection.Top:
                            break;
                        case MergeDirection.Right:
                            break;
                        case MergeDirection.Unknown:
                        case MergeDirection.Bottom:
                            g.DrawImage(image, 0, currentOffset);
                            currentOffset += image.Height;
                            break;
                        case MergeDirection.Left:
                            break;
                    }
                }
            }

            return bitmap;
        }
    }
    public enum MergeDirection
    {
        Unknown = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
        Left = 4
    }
}
