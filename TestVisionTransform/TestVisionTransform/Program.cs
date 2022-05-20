using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;

namespace TestVisionTransform
{
    class Program
    {
        static void Main(string[] args)
        {
            double outX, outY;
            CogTransform2DLinear cogTransform2D = new CogTransform2DLinear();
            // 这里面的TranslationX和TranslationY其实就是吸嘴的旋转中心
            // 平移的offset也是这两个，在Halcon相当于下面的代码
            // hom_mat2d_identity (HomMat2DIdentity)
            // hom_mat2d_translate (HomMat2DIdentity, 30, 20, HomMat2DTranslate)
            // hom_mat2d_rotate (HomMat2DTranslate, rad(90), 30, 20, HomMat2DRotate)

            cogTransform2D.TranslationX = 30;
            cogTransform2D.TranslationY = 20;
            cogTransform2D.Rotation = CogMisc.DegToRad(90);

            // 这里仿射的点，只是基于刚刚的旋转中心，先旋转再平移的点
            // 所以在实际运用中，需要减去目标位置得到偏移量
            cogTransform2D.MapPoint(5, 5, out outX, out outY);
        }
    }
}
