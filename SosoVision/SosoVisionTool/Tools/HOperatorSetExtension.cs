using HalconDotNet;
using System;
using Prism.Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
