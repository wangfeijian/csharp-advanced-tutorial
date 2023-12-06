#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：ec9ad647-5e56-4f49-8727-881a6d5161f9
 * 文件名：MathHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/14/2023 11:31:02 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// 数学常用运算帮助类
    /// </summary>
    /// <remarks>
    /// 主要收集一些常用的数学运算，便于使用
    /// </remarks>
    public static class MathHelp
    {
        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="rad">弧度</param>
        /// <returns>角度</returns>
        public static double RadToDeg(double rad)
        {
            return (rad / Math.PI) * 180;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="deg">角度</param>
        /// <returns>弧度</returns>
        public static double DegToRad(double deg)
        {
            return (deg / 180) * Math.PI;
        }


        /// <summary>
        /// 角度归一化
        /// </summary>
        /// <remarks>
        /// 让角度始终保持在-180度到180之间
        /// </remarks>
        /// <param name="deg">角度</param>
        /// <returns>归一化后的角度</returns>
        public static double DegNormalization(double deg)
        {
            deg = deg % 360;

            if (deg >= -180 && deg <= 180)
            {
                return deg;
            }

            return deg > 180 ? deg - 360 : deg + 360;
        }


        /// <summary>
        /// 弧度归一化
        /// </summary>
        /// <remarks>
        /// 让弧度制永远保持在-PI到PI之间
        /// </remarks>
        /// <param name="rad">弧度</param>
        /// <returns>归一化后的弧度</returns>
        public static double RadNormalization(double rad)
        {
            rad = rad % (2 * Math.PI);

            if (rad >= -Math.PI && rad <= Math.PI)
            {
                return rad;
            }

            return rad > Math.PI ? rad - 2 * Math.PI : rad + 2 * Math.PI;
        }


        /// <summary>
        /// 根据输入的点集，确定一个最佳平面的系数
        /// </summary>
        /// <remarks>
        /// 根据平面方程Ax + By + Cz + D = 0，通过最小二乘法拟合平面得到到四个系统A、B、C、D<br/>
        /// 输入的x、y、z点坐标长度要一致，且长度要大于3个点
        /// </remarks>
        /// <param name="pointX">点的X坐标集合</param>
        /// <param name="pointY">点的Y坐标集合</param>
        /// <param name="pointZ">点的Z坐标集合</param>
        /// <returns>返回四个平面系数</returns>
        /// <exception cref="ArgumentException"/>
        public static double[] FitPlane(double[] pointX, double[] pointY, double[] pointZ)
        {
            double[] result = new double[4];

            if (pointX.Length != pointY.Length || pointY.Length != pointZ.Length)
            {
                throw new ArgumentException("Parameter's length must be equal!!");
            }

            if (pointX.Length < 3)
            {
                throw new ArgumentException("Parameter's length must greater than or equal than 3!!");
            }

            double sumOfX = 0, sumOfY = 0, sumOfZ = 0;
            double sumOfXSquare = 0, sumOfYSquare = 0, sumOfXY = 0, sumOfZX = 0, sumOfZY = 0;

            for (int i = 0; i < pointX.Length; i++)
            {
                sumOfX += pointX[i];
                sumOfY += pointY[i];
                sumOfZ += pointZ[i];
                sumOfXSquare += pointX[i] * pointX[i];
                sumOfYSquare += pointY[i] * pointY[i];
                sumOfXY += pointX[i] * pointY[i];
                sumOfZX += pointX[i] * pointZ[i];
                sumOfZY += pointY[i] * pointZ[i];
            }

            // 矩阵数据         A                          x          b
            // --                           ----        -   -  --        ---
            // |  Sum(X^2)  Sum(X*Y)    Sum(X) |        | A |  |  Sum(X*Z) |
            // |  Sum(X*Y)  Sum(Y*Z)    Sum(Y) |        | B |  |  Sum(Y*Z) |
            // |  Sum(X)    Sum(Y)      pNums  |        | C |  |  Sum(Z)   |
            // __                           ----        -   -  --        ---   
            double[,] aOfArray = { { sumOfXSquare, sumOfXY, sumOfX }, { sumOfXY, sumOfYSquare, sumOfY }, { sumOfX, sumOfY, pointX.Length } };
            double[] bOfArray = { sumOfZX, sumOfZY, sumOfZ };

            // 初始化矩阵
            Matrix<double> A = Matrix<double>.Build.DenseOfArray(aOfArray);
            Vector<double> b = Vector<double>.Build.DenseOfArray(bOfArray);

            // 矩阵求逆x=A^-1 * b
            var x = A.Inverse().Multiply(b);

            result[0] = x.At(0);
            result[1] = x.At(1);
            result[2] = -1;
            result[3] = x.At(2);

            return result;
        }

        /// <summary>
        /// 计算平面度
        /// </summary>
        /// <remarks>
        /// 根据拟合的平面系统，和输入的X、Y、Z坐标，计算这些点的平面度<br/>
        /// 实际就是计算各个点平最佳平面的距离，将最大值减去最小值，就是所得平面的平面度
        /// </remarks>
        /// <param name="plane">平度拟合系数</param>
        /// <param name="pointX">点的X坐标集合</param>
        /// <param name="pointY">点的Y坐标集合</param>
        /// <param name="pointZ">点的Z坐标集合</param>
        /// <returns>平面度</returns>
        /// <exception cref="ArgumentException"/>
        public static double Flatness(double[] plane, double[] pointX, double[] pointY, double[] pointZ)
        {
            if (pointX.Length != pointY.Length || pointY.Length != pointZ.Length)
            {
                throw new ArgumentException("Parameter's length must be equal!!");
            }

            if (pointX.Length < 3)
            {
                throw new ArgumentException("Parameter's length must greater than or equal than 3!!");
            }

            if (plane.Length != 4)
            {
                throw new ArgumentException("Parameter plane's length must equal than 4!!");
            }

            double SumOfSquares = Math.Sqrt(plane[0] * plane[0] + plane[1] * plane[1] + plane[2] * plane[2]);
            List<double> distances = new List<double>();

            for (int i = 0; i < pointX.Length; i++)
            {
                double tempPlane = plane[0] * pointX[i] + plane[1] * pointY[i] + plane[2] * pointZ[i] + plane[3];
                distances.Add(tempPlane / SumOfSquares);
            }

            distances.Sort();
            return distances.Last() - distances.First();
        }

        /// <summary>
        /// 计算点到面的高度
        /// </summary>
        /// <remarks>
        /// 根据平面方程Ax + By + Cz + D = 0，通过最小二乘法拟合平面得到到四个系统A、B、C、D，作为输入参数<br/>
        /// 计算点到这个拟合平面的距离
        /// </remarks>
        /// <param name="plane">平面系数</param>
        /// <param name="pointX">点X坐标</param>
        /// <param name="pointY">点Y坐标</param>
        /// <param name="pointZ">点Z坐标</param>
        /// <returns>返回高度</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double PointToPlane(double[] plane, double pointX, double pointY, double pointZ)
        {
            if (plane.Length != 4)
            {
                throw new ArgumentException("Parameter plane's length must equal than 4!!");
            }

            double SumOfSquares = Math.Sqrt(plane[0] * plane[0] + plane[1] * plane[1] + plane[2] * plane[2]);
            double tempPlane = plane[0] * pointX + plane[1] * pointY + plane[2] * pointZ + plane[3];
            return tempPlane / SumOfSquares;
        }

        /// <summary>
        /// 求两个面的夹角角度
        /// </summary>
        /// <remarks>
        /// 根据两个平面的法向量系数，求两个平面的夹角
        /// </remarks>
        /// <param name="plane1">平面1法向量</param>
        /// <param name="plane2">平面2法向量</param>
        /// <returns>夹角</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double AngleOfTwoPlane(double[] plane1, double[] plane2)
        {
            if (plane1.Length != 4 || plane2.Length != 4)
            {
                throw new ArgumentException("Parameter plane's length must equal than 4!!");
            }

            double SumOfSquares1 = Math.Sqrt(plane1[0] * plane1[0] + plane1[1] * plane1[1] + plane1[2] * plane1[2]);
            double SumOfSquares2 = Math.Sqrt(plane2[0] * plane2[0] + plane2[1] * plane2[1] + plane2[2] * plane2[2]);
            double cos = Math.Abs(plane1[0] * plane2[0] + plane1[1] * plane2[1] + plane1[2] * plane2[2]) / (SumOfSquares1 * SumOfSquares2);
            return RadToDeg(Math.Acos(cos));
        }
    }
}