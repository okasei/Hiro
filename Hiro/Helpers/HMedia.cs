using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.Helpers
{
    internal class HMedia
    {
        #region 图像压缩
        /// <summary>
        /// 压缩图片至200 Kb以下
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="format">图片格式</param>
        /// <param name="targetLen">压缩后大小</param>
        /// <param name="srcLen">原始大小</param>
        /// <returns>压缩后的图片</returns>
        public static System.Drawing.Image ZipImage(System.Drawing.Image img, ImageFormat format, long targetLen, long srcLen = 0)
        {
            //设置大小偏差幅度 10kb
            const long nearlyLen = 10240;
            //内存流  如果参数中原图大小没有传递 则使用内存流读取
            var ms = new MemoryStream();
            if (0 == srcLen)
            {
                img.Save(ms, format);
                srcLen = ms.Length;
            }

            //单位 由Kb转为byte 若目标大小高于原图大小，则满足条件退出
            targetLen *= 1024;
            if (targetLen > srcLen)
            {
                ms.SetLength(0);
                ms.Position = 0;
                img.Save(ms, format);
                img = System.Drawing.Image.FromStream(ms);
                return img;
            }

            //获取目标大小最低值
            var exitLen = targetLen - nearlyLen;

            //初始化质量压缩参数 图像 内存流等
            var quality = (long)Math.Floor(100.00 * targetLen / srcLen);
            var parms = new EncoderParameters(1);

            //获取编码器信息
            ImageCodecInfo? formatInfo = null;
            var encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo icf in encoders)
            {
                if (icf.FormatID == format.Guid)
                {
                    formatInfo = icf;
                    break;
                }
            }

            //使用二分法进行查找 最接近的质量参数
            long startQuality = quality;
            long endQuality = 100;
            quality = (startQuality + endQuality) / 2;

            while (true)
            {
                //设置质量
                parms.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                //清空内存流 然后保存图片
                ms.SetLength(0);
                ms.Position = 0;
                if (formatInfo != null)
                    img.Save(ms, formatInfo, parms);

                //若压缩后大小低于目标大小，则满足条件退出
                if (ms.Length >= exitLen && ms.Length <= targetLen)
                {
                    break;
                }
                else if (startQuality >= endQuality) //区间相等无需再次计算
                {
                    break;
                }
                else if (ms.Length < exitLen) //压缩过小,起始质量右移
                {
                    startQuality = quality;
                }
                else //压缩过大 终止质量左移
                {
                    endQuality = quality;
                }

                //重新设置质量参数 如果计算出来的质量没有发生变化，则终止查找。这样是为了避免重复计算情况{start:16,end:18} 和 {start:16,endQuality:17}
                var newQuality = (startQuality + endQuality) / 2;
                if (newQuality == quality)
                {
                    break;
                }
                quality = newQuality;
                //Console.WriteLine("start:{0} end:{1} current:{2}", startQuality, endQuality, quality);
            }
            img = System.Drawing.Image.FromStream(ms);
            return img;
        }

        /// <summary>
        ///获取图片格式
        /// </summary>
        /// <param name="img">图片</param>
        /// <returns>默认返回JPEG</returns>
        public static ImageFormat GetImageFormat(System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(ImageFormat.Jpeg))
            {
                return ImageFormat.Jpeg;
            }
            if (img.RawFormat.Equals(ImageFormat.Gif))
            {
                return ImageFormat.Gif;
            }
            if (img.RawFormat.Equals(ImageFormat.Png))
            {
                return ImageFormat.Png;
            }
            if (img.RawFormat.Equals(ImageFormat.Bmp))
            {
                return ImageFormat.Bmp;
            }
            return ImageFormat.Jpeg;//根据实际情况选择返回指定格式还是null
        }

        /// <summary>
        /// 不管多大的图片都能在指定大小picturebox控件中显示
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="destHeight">picturebox控件高</param>
        /// <param name="destWidth">picturebox控件宽</param>
        /// <returns></returns>
        public static System.Drawing.Image ZoomImage(System.Drawing.Image bitmap, int destHeight, int destWidth)
        {
            try
            {
                System.Drawing.Image sourImage = bitmap;
                int width = 0, height = 0;
                //按比例缩放             
                int sourWidth = sourImage.Width;
                int sourHeight = sourImage.Height;
                if (sourHeight > destHeight || sourWidth > destWidth)
                {
                    if ((sourWidth * destHeight) > (sourHeight * destWidth))
                    {
                        width = destWidth;
                        height = (destWidth * sourHeight) / sourWidth;
                    }
                    else
                    {
                        height = destHeight;
                        width = (sourWidth * destHeight) / sourHeight;
                    }
                }
                else
                {
                    width = sourWidth;
                    height = sourHeight;
                }
                System.Drawing.Bitmap destBitmap = new(destWidth, destHeight);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(destBitmap);
                g.Clear((HSet.Read_DCIni("RefillMode", "0")) switch
                {
                    "1" => System.Drawing.Color.White,
                    "2" => System.Drawing.Color.Black,
                    _ => System.Drawing.Color.Transparent
                });
                //设置画布的描绘质量           
                g.CompositingQuality = (HSet.Read_DCIni("ComposeQuality", "2")) switch
                {
                    "-1" => System.Drawing.Drawing2D.CompositingQuality.Invalid,
                    "1" => System.Drawing.Drawing2D.CompositingQuality.HighSpeed,
                    "2" => System.Drawing.Drawing2D.CompositingQuality.HighQuality,
                    "3" => System.Drawing.Drawing2D.CompositingQuality.GammaCorrected,
                    "4" => System.Drawing.Drawing2D.CompositingQuality.AssumeLinear,
                    _ => System.Drawing.Drawing2D.CompositingQuality.Default
                };
                g.SmoothingMode = (HSet.Read_DCIni("SmoothQuality", "2")) switch
                {
                    "-1" => System.Drawing.Drawing2D.SmoothingMode.Invalid,
                    "1" => System.Drawing.Drawing2D.SmoothingMode.HighSpeed,
                    "2" => System.Drawing.Drawing2D.SmoothingMode.HighQuality,
                    "3" => System.Drawing.Drawing2D.SmoothingMode.None,
                    "4" => System.Drawing.Drawing2D.SmoothingMode.AntiAlias,
                    _ => System.Drawing.Drawing2D.SmoothingMode.Default
                };
                g.InterpolationMode = (HSet.Read_DCIni("InterpolateQuality", "7")) switch
                {
                    "-1" => System.Drawing.Drawing2D.InterpolationMode.Invalid,
                    "1" => System.Drawing.Drawing2D.InterpolationMode.Low,
                    "2" => System.Drawing.Drawing2D.InterpolationMode.High,
                    "3" => System.Drawing.Drawing2D.InterpolationMode.Bilinear,
                    "4" => System.Drawing.Drawing2D.InterpolationMode.Bicubic,
                    "5" => System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor,
                    "6" => System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear,
                    "7" => System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic,
                    _ => System.Drawing.Drawing2D.InterpolationMode.Default
                };
                g.DrawImage(sourImage, new System.Drawing.Rectangle((destWidth - width) / 2, (destHeight - height) / 2, width, height), 0, 0, sourImage.Width, sourImage.Height, System.Drawing.GraphicsUnit.Pixel);
                //g.DrawImage(sourImage, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, sourImage.Width, sourImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                //设置压缩质量       
                EncoderParameters encoderParams = new();
                long[] quality = new long[1];
                if (long.TryParse(HSet.Read_DCIni("ZipQuality", "100"), out quality[0]))
                    quality[0] = 100;
                quality[0] = Math.Clamp(quality[0], 1, 100);
                EncoderParameter encoderParam = new(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                sourImage.Dispose();
                return destBitmap;
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Utils.ZoomImage");
                return bitmap;
            }
        }


        #endregion


        internal static void ToggleMediaKey(string path, List<string> parameter)
        {
            System.Windows.Input.Key situation = path.ToLower() switch
            {
                "media(0)" or "media(off)" or "media(down)" or "media(↓)" or "media(stop)" or "media(end)" => System.Windows.Input.Key.MediaStop,
                "media(1)" or "media(on)" or "media(up)" or "media(↑)" or "media(start)" or "media(begin)" or "media(invoke)" or "media(play)" or "media(pause)" => System.Windows.Input.Key.MediaPlayPause,
                "media(next)" or "media(2)" or "media(right)" or "media(→)" => System.Windows.Input.Key.MediaNextTrack,
                "media(previous)" or "media(last)" or "media(3)" or "media(left)" or "media(←)" => System.Windows.Input.Key.MediaPreviousTrack,
                _ => System.Windows.Input.Key.MediaStop,
            };

            var _res = false;
            switch (situation)
            {
                case System.Windows.Input.Key.MediaStop:
                    {
                        _res = HMediaInfoManager.TryToggleStop().GetAwaiter().GetResult();
                        break;
                    }
                case System.Windows.Input.Key.MediaPlayPause:
                    {
                        _res = HMediaInfoManager.TryTogglePlay().GetAwaiter().GetResult();
                        break;
                    }
                case System.Windows.Input.Key.MediaNextTrack:
                    {
                        _res = HMediaInfoManager.TryToggleNext().GetAwaiter().GetResult();
                        break;
                    }
                case System.Windows.Input.Key.MediaPreviousTrack:
                    {
                        _res = HMediaInfoManager.TryTogglePrevious().GetAwaiter().GetResult();
                        break;
                    }
                default:
                    break;
            }
            if (_res)
                return;
            var keyinfo = situation switch
            {
                System.Windows.Input.Key.MediaStop => "End",
                System.Windows.Input.Key.MediaPlayPause => "Start",
                System.Windows.Input.Key.MediaNextTrack => "Next",
                System.Windows.Input.Key.MediaPreviousTrack => "Previous",
                _ => "Next"
            };
            var keyi = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey(situation);
            if (App.dflag)
                HLogger.LogtoFile("[MEDIA]Media Control : " + keyinfo);
            Hiro_Utils.keybd_event(keyi, Hiro_Utils.MapVirtualKey(keyi, 0), 0x0001, 0);
            Hiro_Utils.keybd_event(keyi, Hiro_Utils.MapVirtualKey(keyi, 0), 0x0001 | 0x0002, 0);
        }
    }
}
