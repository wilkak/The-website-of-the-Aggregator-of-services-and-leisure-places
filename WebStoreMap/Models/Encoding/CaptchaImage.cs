using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WebStoreMap.Models.Encoding
{
    public class CaptchaImage
    {
        private readonly string Text; // текст капчи
        private readonly int Width; // ширина картинки
        private readonly int Height; // высота картинки
        public Bitmap Image { get; set; } // изображение капчи

        public CaptchaImage(string Text, int Width, int Height)
        {
            this.Text = Text;
            this.Width = Width;
            this.Height = Height;
            GenerateImage();
        }

        // создаем изображение
        private void GenerateImage()
        {
            Bitmap Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            Graphics Graphics = Graphics.FromImage(Bitmap);
            Graphics.Clear(Color.White);
            Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // отрисовка строки
            Graphics.DrawString(Text, new Font("Tahoma", Height / 2, FontStyle.Italic),
                                Brushes.GreenYellow, new RectangleF(0, 0, Width, Height));
            Pen Pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            Pen Pen1 = new Pen(Color.Gray);
            Pen Pen2 = new Pen(Color.Indigo);
            Pen Pen3 = new Pen(Color.Indigo);
          
            Graphics.DrawLine(Pen, 20, 10, 110, 30);
            Graphics.DrawLine(Pen1, 50, 10, 50, 40);
            Graphics.DrawLine(Pen2, 110, 10, 10, 30);
            Graphics.DrawLine(Pen3, 170, 20, 10, 20);
           
            Graphics.Flush();
            Graphics.Dispose();

            Image = Bitmap;
        }

        ~CaptchaImage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                Image.Dispose();
            }
        }
    }
}