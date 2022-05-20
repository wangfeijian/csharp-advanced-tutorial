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
            double rotateX = 130, rotateY = 20, inX = 15, inY = 25, deg = 14.3;
            CogTransform2DLinear cogTransform2D = new CogTransform2DLinear();
            // 这里面的TranslationX和TranslationY其实就是将坐标原点移动到旋转中心
            // 移动之后，再通过旋转的角度，将点位旋转到指定的位置
            // hom_mat2d_identity (HomMat2DIdentity)
            // hom_mat2d_rotate (HomMat2DIdentity, rad(deg), rotateX, rotateY, HomMat2DRotate)

            cogTransform2D.TranslationX = rotateX;
            cogTransform2D.TranslationY = rotateY;
            cogTransform2D.Rotation = CogMisc.DegToRad(deg);

            // 这里仿射出来的值都是经过平移后得到的，所以在映射之前要将输入点减去之前的平移量，才能得到旋转后正确的点位
            // 所以在实际运用中，需要用仿射的实际点位，减去上面平移的偏移量
            cogTransform2D.MapPoint(inX - rotateX, inY - rotateY, out outX, out outY);
            Console.WriteLine($"outX: {outX:f3} __  outY: {outY:f3}");
            Console.ReadKey();
        }
    }
}
