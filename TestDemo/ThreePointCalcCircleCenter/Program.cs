using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreePointCalcCircleCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            PointF p1 = new PointF(0.5f, 1.0f);
            PointF p2 = new PointF(0.0f, 0.5f);
            PointF p3 = new PointF(0.5f, 0.0f);
            PointF p4 = new PointF(1.0f, 0.5f);
            PointF point;
            List<PointF> list = new List<PointF>();
            list.Add(p1);
            list.Add(p2);
            list.Add(p3);
            list.Add(p4);
            point = FitCenter(list);
            Console.WriteLine(point.X + " " + point.Y);
            Console.ReadLine();
        }

        private static PointF FitCenter(List<PointF> pts, double epsilon = 0.1)
        {
            double totalX = 0, totalY = 0;
            int setCount = 0;

            for (int i = 0; i < pts.Count; i++)
            {
                for (int j = 1; j < pts.Count; j++)
                {
                    for (int k = 2; k < pts.Count; k++)
                    {
                        double delta = (pts[k].X - pts[j].X) * (pts[j].Y - pts[i].Y) - (pts[j].X - pts[i].X) * (pts[k].Y - pts[j].Y);

                        if (Math.Abs(delta) > epsilon)
                        {
                            double ii = Math.Pow(pts[i].X, 2) + Math.Pow(pts[i].Y, 2);
                            double jj = Math.Pow(pts[j].X, 2) + Math.Pow(pts[j].Y, 2);
                            double kk = Math.Pow(pts[k].X, 2) + Math.Pow(pts[k].Y, 2);

                            double cx = ((pts[k].Y - pts[j].Y) * ii + (pts[i].Y - pts[k].Y) * jj + (pts[j].Y - pts[i].Y) * kk) / (2 * delta);
                            double cy = -((pts[k].X - pts[j].X) * ii + (pts[i].X - pts[k].X) * jj + (pts[j].X - pts[i].X) * kk) / (2 * delta);

                            totalX += cx;
                            totalY += cy;

                            setCount++;
                        }
                    }
                }
            }

            if (setCount == 0)
            {
                //failed
                return PointF.Empty;
            }

            return new PointF((float)totalX / setCount, (float)totalY / setCount);
        }
    }
}
