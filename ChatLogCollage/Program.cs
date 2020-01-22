using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace ChatLogCollage
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = Path.Combine(GetFolderPath(SpecialFolder.Desktop),@"AION - What the nyerk\test");

            var files = new DirectoryInfo(directory)
                .EnumerateFiles("*.jpg")
                .Where(f=>f.Name != "merge.jpg")
                .ToList();

            var images = new List<Bitmap>();
            foreach (var file in files)
            {
                images.Add(ImageEditor.GetBitmap(file.FullName));
            }
            
            var result = ImageEditor.Merge(images);

            using (MemoryStream jpegStream = new MemoryStream())
            {
                result.Save(jpegStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                //You have to rewind the MemoryStream before copying
                jpegStream.Seek(0, SeekOrigin.Begin);

                using (FileStream fs = new FileStream(Path.Combine(directory,"merge.jpg"), FileMode.OpenOrCreate))
                {
                    jpegStream.CopyTo(fs);
                    fs.Flush();
                }
            }
        }
    }
}
