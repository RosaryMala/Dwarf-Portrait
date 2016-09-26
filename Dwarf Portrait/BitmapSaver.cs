using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dwarf_Portrait
{
    class BitmapSaver
    {
        public static void SaveElementToPng(FrameworkElement element, string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                BitmapSaver.SaveAsPng(BitmapSaver.GetImage(element), stream);
                SystemSounds.Beep.Play();
            }
        }

        public static RenderTargetBitmap GetImage(FrameworkElement element)
        {
            Size size = new Size(element.ActualWidth, element.ActualHeight);
            if (size.IsEmpty)
                return null;
            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

            result.Render(element);
            return result;
        }

        public static void SaveAsPng(RenderTargetBitmap src, Stream outputStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(outputStream);
        }
    }
}
