using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlatness
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,] A = new double[16, 3];
            double[] dist = new double[16];
            double[] flatness = new double[16];
            A[0, 0] = 0;
            A[1, 0] = 1;
            A[2, 0] = 2;
            A[3, 0] = 3;
            A[4, 0] = 0;
            A[5, 0] = 1;
            A[6, 0] = 2;
            A[7, 0] = 3;
            A[8, 0] = 0;
            A[9, 0] = 1;
            A[10, 0] = 2;
            A[11, 0] = 3;
            A[12, 0] = 0;
            A[13, 0] = 1;
            A[14, 0] = 2;
            A[15, 0] = 3;

            A[0, 1] = 0;
            A[1, 1] = 0;
            A[2, 1] = 0;
            A[3, 1] = 0;
            A[4, 1] = 1;
            A[5, 1] = 1;
            A[6, 1] = 1;
            A[7, 1] = 1;
            A[8, 1] = 2;
            A[9, 1] = 2;
            A[10, 1] = 2;
            A[11, 1] = 2;
            A[12, 1] = 3;
            A[13, 1] = 3;
            A[14, 1] = 3;
            A[15, 1] = 3;

            A[0, 2] = 5;
            A[1, 2] = 4;
            A[2, 2] = 1;
            A[3, 2] = 1;
            A[4, 2] = 9;
            A[5, 2] = 7;
            A[6, 2] = 3;
            A[7, 2] = -1;
            A[8, 2] = 9;
            A[9, 2] = 5;
            A[10, 2] = 2;
            A[11, 2] = -3;
            A[12, 2] = 1;
            A[13, 2] = -2;
            A[14, 2] = 0;
            A[15, 2] = 3;


            double[] para = new double[3];
            GetFlatness(A, A.Length / 3, out para);

            for (int i = 0; i < A.Length / 3; i++)
            {
                DidtPiontPlant(A[i, 0], A[i, 1], A[i, 2], para[0], para[1], para[2], out dist[i]);
                CalcFlatness(A[i, 0], A[i, 1], A[i, 2], para[0], para[1], para[2], out flatness[i]);
                dist[i] = Math.Round(dist[i], 3);
                flatness[i] = Math.Round(flatness[i], 3);
            }


            Console.WriteLine("系数");

            for (int i = 0; i < 3; i++)
            {
                para[i] = Math.Round(para[i], 3);
                Console.WriteLine($"{para[i]}");
            }

            Console.WriteLine("平面度");

            for (int i = 0; i < A.Length / 3; i++)
            {
                Console.WriteLine($"{flatness[i]}");
            }

            Console.WriteLine("高度差");

            for (int i = 0; i < A.Length / 3; i++)
            {
                Console.WriteLine($"{dist[i]}");
            }


            Console.ReadLine();
        }


        private static void GetFlatness(double[,] coef, int n, out double[] para)
        {
            para = new double[3];
            double[,] A = new double[3, 4];
            for (int i = 0; i < 3 - 1; i++)
            {
                for (int j = 0; j < 3 - 1; j++)
                {
                    A[i, j] = 0;
                    for (int k = 0; k < n; k++)
                        A[i, j] += coef[k, i] * coef[k, j];
                }
            }

            for (int i = 0; i < 3 - 1; i++)
            {
                A[i, 3 - 1] = A[3 - 1, i] = 0;
                for (int j = 0; j < n; j++)
                    A[i, 3 - 1] += coef[j, i];
                A[3 - 1, i] = A[i, 3 - 1];
            }

            A[3 - 1, 3 - 1] = n;

            double[] b = new double[3];
            for (int i = 0; i < 3 - 1; i++)
            {
                b[i] = 0;
                for (int j = 0; j < n; j++)
                    b[i] += coef[j, 3 - 1] * coef[j, i];
            }

            b[3 - 1] = 0;
            for (int i = 0; i < n; i++)
                b[3 - 1] += coef[i, 3 - 1];

            double[,] invA = new double[3, 4];
            InvMatrix(A, 3, out invA);

            for (int i = 0; i < 3; i++)
            {
                para[i] = 0;
                for (int j = 0; j < 3; j++)
                    para[i] += invA[i, j] * b[j];
            }

            //do domething
        }

        public static void InvMatrix(double[,] M, int n, out double[,] invM)
        {
            invM = new double[3, 4];
            if (n == 1)
                invM[0, 0] = 1 / M[0, 0];
            else
            {
                double cu, cd, normb;
                double[,] b = new double[n, 4];
                double[,] invv = new double[n, 4];
                for (int j = 0; j < n; j++)
                {
                    double[] schu = new double[j];
                    double[] schd = new double[j];
                    if (j > 0)
                    {
                        for (int k = 0; k < j; k++)
                        {
                            schu[k] = schd[k] = 0;
                            for (int i = 0; i < n; i++)
                            {
                                schu[k] += b[i, k] * M[i, j];
                                schd[k] += b[i, k] * b[i, k];
                            }
                        }
                    }

                    normb = 0;
                    for (int i = 0; i < n; i++)
                    {
                        b[i, j] = M[i, j];
                        if (j > 0)
                        {
                            for (int k = 0; k < j; k++)
                                b[i, j] -= b[i, k] * schu[k] / schd[k];
                        }

                        normb += b[i, j] * b[i, j];
                    }

                    normb = Math.Sqrt(normb);
                    for (int i = 0; i < n; i++)
                        invv[j, i] = b[i, j] / normb;
                }

                double[,] c = new double[n, 4];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        cu = cd = 0;
                        if (j < i)
                        {
                            for (int k = 0; k < n; k++)
                            {
                                cu += M[k, i] * b[k, j];
                                cd += b[k, j] * b[k, j];
                            }

                            c[j, i] = cu / Math.Sqrt(cd);
                        }
                        else
                        {
                            for (int k = 0; k < n; k++)
                                cd += b[k, j] * b[k, j];
                            c[j, i] = Math.Sqrt(cd);
                        }
                    }
                }

                double[,] invc = new double[n, 4];
                for (int j = 0; j < n; j++)
                {
                    for (int i = n - 1; i >= 0; i--)
                    {
                        if (i > j)
                            invc[i, j] = 0;
                        else if (i == j)
                            invc[i, j] = 1 / c[i, j];
                        else
                        {
                            invc[i, j] = 0;
                            for (int k = i + 1; k <= j; k++)
                                invc[i, j] -= c[i, k] * invc[k, j];
                            invc[i, j] /= c[i, i];
                        }
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        invM[i, j] = 0;
                        for (int k = 0; k < n; k++)
                            invM[i, j] += invc[i, k] * invv[k, j];
                    }
                }

            }
        }



        private static void DidtPiontPlant(double x, double y, double z, double a, double b, double c, out double
            tempDoubles)
        {
            double A = b;
            double B = c;
            double C = a;
            double fenmu = Math.Sqrt(A * A + B * B + C * C);
            double fenzhi = Math.Abs(A * x + B * y + C * z);
            tempDoubles = fenzhi / fenmu;
        }

        private static void CalcFlatness(double x, double y, double z, double a, double b, double c, out double
            tempDoubles)
        {
            tempDoubles = z - (a * x + b * y + c);
        }
    }
}


