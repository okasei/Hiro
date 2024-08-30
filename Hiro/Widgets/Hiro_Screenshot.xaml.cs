using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using Hiro.Helpers;

namespace Hiro.Widgets;

public partial class Hiro_Screenshot : Window
{
    private System.Windows.Point startPoint;
    private System.Windows.Point endPoint;
    private bool isSelecting = false;
    private BitmapSource fullScreenshot;

    private double dpiFactorX;
    private double dpiFactorY;
    private bool loaded = false;
    private bool oneClick = false;
    private bool _fs = false;
    private bool _as = false;

    public Hiro_Screenshot(bool isFullScreen = false, bool oneClick = false)
    {
        InitializeComponent();
        InitializeDpiFactors();
        CaptureFullScreen();
        Title = HText.Get_Translate("shotTitle").Replace("%t", HText.Get_Translate("scrshot")).Replace("%a", App.appTitle);
        this.oneClick = oneClick;
        if (isFullScreen)
        {
            CaptureSelectedArea(true);
            if (!HSet.Read_DCIni("Ani", "2").Equals("0"))
            {
                _fs = true;
            }
            else
            {
                Close();
            }

        }
        else
        {
            _as = true;
            Loaded += delegate
            {
                TopMask.Width = SelectionCanvas.ActualWidth;
                TopMask.Height = SelectionCanvas.ActualHeight;
                Canvas.SetLeft(TopMask, 0);
                Canvas.SetTop(TopMask, 0);
                loaded = true;
                SelectionRectangle.Stroke = new SolidColorBrush(App.AppAccentColor);
            };
        }
    }
    private void InitializeDpiFactors()
    {
        var _s = HWin.GetDpiScale();
        dpiFactorX = _s.Width;
        dpiFactorY = _s.Height;
    }


    private void CaptureFullScreen()
    {
        int screenWidth = (int)(SystemParameters.PrimaryScreenWidth * dpiFactorX);
        int screenHeight = (int)(SystemParameters.PrimaryScreenHeight * dpiFactorY);

        using (var screenBmp = new Bitmap(screenWidth, screenHeight))
        {
            using (Graphics g = Graphics.FromImage(screenBmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenWidth, screenHeight));
            }

            IntPtr hBitmap = screenBmp.GetHbitmap();
            try
            {
                fullScreenshot = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(screenWidth, screenHeight));
                // 设置 DPI
                fullScreenshot.Freeze(); // 冻结图像以便在多个线程间使用
                ScreenshotImage.Source = fullScreenshot;

                // 确保 Image 控件的尺寸与截图一致
                ScreenshotImage.Width = SystemParameters.PrimaryScreenWidth;
                ScreenshotImage.Height = SystemParameters.PrimaryScreenHeight;
            }
            finally
            {
                DeleteObject(hBitmap); // 释放 GDI 对象
            }
        }
        Visibility = Visibility.Visible;
        if (!HSet.Read_DCIni("Ani", "2").Equals("0"))
        {
            Opacity = 0;
            var sb = HAnimation.AddDoubleAnimaton(1, 250, this, "Opacity", null);
            sb.Completed += (e, args) =>
            {
                Opacity = 1;
                if (_fs)
                {
                    startPoint = new(0, 0);
                    endPoint = new(SelectionCanvas.ActualWidth, SelectionCanvas.ActualHeight);
                    SelectionRectangle.Visibility = Visibility.Visible;
                    UpdateSelectionRectangle();
                    ShowCloseAnimation();
                }
            };
            sb.Begin();
        }
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && _as)
        {
            startPoint = e.GetPosition(this);
            isSelecting = true;
            SelectionRectangle.Visibility = Visibility.Visible;
            UpdateSelectionRectangle();
            e.Handled = true;
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            Close();
            e.Handled = true;
        }
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (isSelecting)
        {
            endPoint = e.GetPosition(this);
            UpdateSelectionRectangle();
            e.Handled = true;
        }
    }

    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (isSelecting && oneClick && _as)
        {
            isSelecting = false;
            _as = false;
            CaptureSelectedArea();
            ShowCloseAnimation();
        }
        e.Handled = true;
    }

    private void ShowCloseAnimation()
    {
        if (!HSet.Read_DCIni("Ani", "2").Equals("0"))
        {
            var sb = HAnimation.AddColorAnimaton(System.Windows.Media.Color.FromArgb(90, 255, 255, 255), 250, SelectionRectangle, "(Rectangle.Fill).(SolidColorBrush.Color)", null);
            sb.Completed += (e, args) =>
            {
                SelectionRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(90, 255, 255, 255));
                var sb = HAnimation.AddDoubleAnimaton(0, 250, this, "Opacity", null);
                sb.Completed += (e, args) =>
                {
                    Close();
                };
                sb.Begin();
            };
            sb.Begin();
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && loaded &&_as)
        {
            _as = false;
            CaptureSelectedArea();
            this.Close();
            e.Handled = true;
        }
        if (e.Key == Key.Escape)
        {
            this.Close();
            e.Handled = true;
        }
    }

    private void UpdateSelectionRectangle()
    {
        double x = Math.Min(startPoint.X, endPoint.X);
        double y = Math.Min(startPoint.Y, endPoint.Y);
        double width = Math.Abs(startPoint.X - endPoint.X);
        double height = Math.Abs(startPoint.Y - endPoint.Y);

        SelectionRectangle.Width = width;
        SelectionRectangle.Height = height;
        Canvas.SetLeft(SelectionRectangle, x);
        Canvas.SetTop(SelectionRectangle, y);

        // 更新遮罩区域
        TopMask.Width = SelectionCanvas.ActualWidth;
        TopMask.Height = y;
        Canvas.SetLeft(TopMask, 0);
        Canvas.SetTop(TopMask, 0);

        BottomMask.Width = SelectionCanvas.ActualWidth;
        BottomMask.Height = SelectionCanvas.ActualHeight - y - height;
        Canvas.SetLeft(BottomMask, 0);
        Canvas.SetTop(BottomMask, y + height);

        LeftMask.Width = x;
        LeftMask.Height = height;
        Canvas.SetLeft(LeftMask, 0);
        Canvas.SetTop(LeftMask, y);

        RightMask.Width = SelectionCanvas.ActualWidth - x - width;
        RightMask.Height = height;
        Canvas.SetLeft(RightMask, x + width);
        Canvas.SetTop(RightMask, y);
    }

    private void CaptureSelectedArea(bool isFullScreen = false)
    {
        string filePath = HText.Path_PPX(@"<ipicture>\Screenshots\Hiro_<date><time>.png");
        string tranKey = "scrshotfull";
        if (isFullScreen)
        {
            double x = 0;
            double y = 0;
            double width = fullScreenshot.PixelWidth;
            double height = fullScreenshot.PixelHeight;
            if (App.dflag)
                HLogger.LogtoFile($"[{x}:{y} - {width}:{height}]");
            // 保存截图
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(fullScreenshot));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
        else
        {
            var pps = PointToScreen(startPoint);
            var ppe = PointToScreen(endPoint);
            double x = Math.Min(pps.X, ppe.X);
            double y = Math.Min(pps.Y, ppe.Y);
            double width = Math.Min(Math.Abs(pps.X - ppe.X) + 1, fullScreenshot.PixelWidth);
            double height = Math.Min(Math.Abs(pps.Y - ppe.Y) + 1, fullScreenshot.PixelHeight);
            if (App.dflag)
                HLogger.LogtoFile($"[{pps.X},{pps.Y}]:[{ppe.X},{ppe.Y}] - [{x}:{y} - {width}:{height}]");

            // 从已捕获的图像中裁剪选中的区域
            var croppedBitmap = new CroppedBitmap(fullScreenshot, new Int32Rect((int)x, (int)y, (int)width, (int)height));

            // 保存截图
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
            tranKey = "scrshotarea";
        }

        DataObject dataObject = new DataObject();
        // 将图片数据放入剪贴板（支持直接粘贴为图片）
        dataObject.SetData(DataFormats.Bitmap, Hiro_Utils.GetBitmapImage(filePath) ?? null);
        // 将图片文件路径放入剪贴板（支持粘贴为文件）
        dataObject.SetData(DataFormats.FileDrop, new string[] { filePath });
        Clipboard.SetDataObject(dataObject, true);

        App.Notify(new(HText.Get_Translate(tranKey), 2, HText.Get_Translate("scrshot"), new(() =>
        {
            Hiro_Utils.RunExe($"\"{filePath}\"");
        })));
    }
}