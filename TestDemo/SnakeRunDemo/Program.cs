using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeRunDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] num = { { 1, 2, 3}, { 4, 5, 6}, { 7, 8, 9}};

            // 蛇形循环
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    for (int j = 2; j >= 0; j--)
                    {
                        Console.Write(num[i, j] + "\t");
                    }
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Console.Write(num[i,j] + "\t");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
