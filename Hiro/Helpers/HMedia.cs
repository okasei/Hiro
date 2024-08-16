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
                g.Clear(System.Drawing.Color.Transparent);
                //设置画布的描绘质量           
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourImage, new System.Drawing.Rectangle((destWidth - width) / 2, (destHeight - height) / 2, width, height), 0, 0, sourImage.Width, sourImage.Height, System.Drawing.GraphicsUnit.Pixel);
                //g.DrawImage(sourImage, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, sourImage.Width, sourImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                //设置压缩质量       
                EncoderParameters encoderParams = new();
                long[] quality = new long[1];
                quality[0] = 100;
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
    }
}
