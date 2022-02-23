using HalconDotNet;
using System;
using Prism.Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SosoVisionCommonTool.Log;
using System.Drawing;
using System.Drawing.Imaging;

namespace SosoVisionTool.Tools
{
    public static class HOperatorSetExtension
    {

        public static HObject ReadImage(string file)
        {
            HObject image;
            try
            {
                HOperatorSet.ReadImage(out image, file);
                ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogInfo($"从文件 {file} 采集图片成功。");
                return image;
            }
            catch (Exception)
            {
                ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"{file}错误！！");
                return null;
            }
        }

        public static HObject BitmapGrayToHObject(Bitmap bmp)
        {
            HObject image;
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);
                return image;

            }
            catch (Exception ex)
            {
                ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"转换错误！！{ex}");
                return null;
            }
        }

    }
}
