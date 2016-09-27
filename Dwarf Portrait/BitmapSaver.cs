using System;
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
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    SaveAsPng(GetImage(element), stream);
                    SystemSounds.Beep.Play();
                }
            }
            catch (Exception e)
            {
                SystemSounds.Exclamation.Play();
                File.Delete(path);
                MessageBox.Show(e.Message);
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
