using Microsoft.VisualBasic.Devices;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.UserDataTasks;

namespace hiro
{
    /// <summary>
    /// Hiro_Croper.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Cropper : Window
    {
        internal Point original = new Point(0, 0);
        internal int cropFlag = 0;
        internal Thickness originalThickness = new Thickness(0);
        internal Point originalSize = new Point(0, 0);
        internal string filePath = "C:\\Users\\liuyulong02\\Downloads\\wallpaper\\Cascade\\contents\\images\\3840x2160.png";
        internal Point Picture = new Point(0, 0);
        public Hiro_Cropper()
        {
            InitializeComponent();
            var a = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            Original.Source = a;
            Picture = new Point(a.Width, a.Height);
            // 切割图片



        }

        private void CropBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            original = System.Windows.Input.Mouse.GetPosition(this);
            originalThickness = CropBorder.Margin;
            originalSize = new(CropBorder.Width, CropBorder.Height);
            cropFlag = 1;
            if (original.Y > CropBorder.Margin.Top - 8 && original.Y < CropBorder.Margin.Top + 8)//up
            {
                cropFlag += 2;
            }
            if (original.X > CropBorder.Margin.Left - 8 && original.X < CropBorder.Margin.Left + 8)//left
            {
                cropFlag += 4;
            }
            if (original.X < CropBorder.Margin.Left + CropBorder.Width + 8 && original.X > CropBorder.Margin.Left + CropBorder.Width - 8)//right
            {
                cropFlag += 8;
            }
            if (original.Y < CropBorder.Margin.Top + CropBorder.Height + 8 && original.Y > CropBorder.Margin.Top + CropBorder.Height - 8)//down
            {
                cropFlag += 16;
            }
        }


        private void CropBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            original = new Point(0, 0);
            originalThickness = new(0);
            cropFlag = 0;
        }

        private void CropBorder_MouseMove(object sender, MouseEventArgs e)
        {
            var diff = System.Windows.Input.Mouse.GetPosition(this);
            var w = originalSize.X;
            var h = originalSize.Y;
            var m = originalThickness;
            var ew = original.X - diff.X;
            var eh = ew * h / w;
            var dh = original.Y - diff.Y;
            switch (cropFlag)
            {
                case 1:
                    {
                        var dl = m.Left - ew;
                        var dt = m.Top - dh;
                        if (dl + CropBorder.Width > Original.ActualWidth)
                            dl = Original.ActualWidth - CropBorder.Width;
                        if (dl < 0)
                            dl = 0;
                        if (dt + CropBorder.Height > Original.ActualHeight)
                            dt = Original.ActualHeight - CropBorder.Height;
                        if (dt < 0)
                            dt = 0;
                        CropBorder.Margin = new Thickness(dl, dt, 0, 0);
                        break;
                    }
                case 7://left up
                    {
                        var dl = m.Left - ew;
                        var dt = m.Top - eh;
                        if (dl >= 0 && dt >= 0)
                        {
                            CropBorder.Width = w + ew;
                            CropBorder.Height = h + eh;
                            CropBorder.Margin = new Thickness(m.Left - ew, m.Top - eh, 0, 0);
                        }
                        break;
                    }
                case 5:
                case 21://left down
                    {
                        var ddh = h + eh;
                        var dl = m.Left - ew;
                        if (dl >= 0 && m.Top + ddh <= Original.ActualHeight)
                        {
                            CropBorder.Width = w + ew;
                            CropBorder.Height = h + eh;
                            CropBorder.Margin = new Thickness(m.Left - ew, m.Top, 0, 0);
                        }

                        break;
                    }
                case 3:
                case 11://right up
                    {
                        var dt = m.Top + eh;
                        var ddw = w - ew;
                        if (dt >= 0 && m.Left + ddw <= Original.ActualWidth)
                        {
                            CropBorder.Width = ddw;
                            CropBorder.Height = h - eh;
                            CropBorder.Margin = new Thickness(m.Left, dt, 0, 0);
                        }
                        break;
                    }
                case 9:
                case 17:
                case 25:
                    {
                        var ddw = w - ew;
                        var ddh = h - eh;
                        if (m.Left + ddw <= Original.ActualWidth && m.Top + ddh <= Original.ActualHeight)
                        {
                            CropBorder.Width = w - ew;
                            CropBorder.Height = h - eh;
                        }
                        break;
                    }
                default:
                    {
                        var mFlag = 1;
                        if (diff.Y > CropBorder.Margin.Top - 8 && diff.Y < CropBorder.Margin.Top + 8)//up
                        {
                            mFlag += 2;
                        }
                        if (diff.X > CropBorder.Margin.Left - 8 && diff.X < CropBorder.Margin.Left + 8)//left
                        {
                            mFlag += 4;
                        }
                        if (diff.X < CropBorder.Margin.Left + CropBorder.Width + 8 && diff.X > CropBorder.Margin.Left + CropBorder.Width - 8)//right
                        {
                            mFlag += 8;
                        }
                        if (diff.Y < CropBorder.Margin.Top + CropBorder.Height + 8 && diff.Y > CropBorder.Margin.Top + CropBorder.Height - 8)//down
                        {
                            mFlag += 16;
                        }
                        Mask.Cursor = mFlag switch
                        {
                            3 or 17 => Cursors.SizeNS,
                            5 or 9 => Cursors.SizeWE,
                            7 or 25 => Cursors.SizeNWSE,
                            11 or 21 => Cursors.SizeNESW,
                            _ => Cursors.Arrow
                        };
                        break;
                    }
            }
        }

        private void SaveCroppedImage(string savePath)
        {
            ImageSource imageSource = Original.Source;
            System.Drawing.Bitmap bitmap = ImageSourceToBitmap(imageSource);
            BitmapSource bitmapSource = BitmapToBitmapImage(bitmap);
            var resize = Math.Max(Original.ActualHeight / Picture.Y, Original.ActualWidth / Picture.X);
            BitmapSource newBitmapSource = CutImage(bitmapSource, new Int32Rect(Convert.ToInt32(CropBorder.Margin.Left / resize), Convert.ToInt32(CropBorder.Margin.Top / resize), Convert.ToInt32(CropBorder.Width / resize), Convert.ToInt32(CropBorder.Height / resize)));
            PngBitmapEncoder PBE = new PngBitmapEncoder();
            PBE.Frames.Add(BitmapFrame.Create(newBitmapSource));
            using (Stream stream = File.Create(Hiro_Utils.Path_Prepare(savePath)))
            {
                PBE.Save(stream);
            }
        }
        public static BitmapSource CutImage(BitmapSource bitmapSource, Int32Rect cut)
        {
            //计算Stride
            var stride = bitmapSource.Format.BitsPerPixel * cut.Width / 8;
            //声明字节数组
            byte[] data = new byte[cut.Height * stride];
            //调用CopyPixels
            bitmapSource.CopyPixels(cut, data, stride, 0);

            return BitmapSource.Create(cut.Width, cut.Height, 0, 0, PixelFormats.Bgr32, null, data, stride);
        }

        // ImageSource --> Bitmap
        public static System.Drawing.Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            BitmapSource m = (BitmapSource)imageSource;

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            BitmapData data = bmp.LockBits(
            new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bmp.UnlockBits(data);

            return bmp;
        }

        // Bitmap --> BitmapImage
        public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }

        private void Mask_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
