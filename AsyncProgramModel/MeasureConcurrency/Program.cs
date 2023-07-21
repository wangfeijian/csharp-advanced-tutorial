using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureConcurrency
{
    internal class Program
    {
        private static int NUM_INTS = 500000000;

        private static ParallelQuery<int> GenerateInputData()
        {
            return ParallelEnumerable.Range(1, NUM_INTS);
        }

        private static object _syncCalc = new object();

        private static double CalculateX(int intNum)
        {
            double result;

            //lock (_syncCalc)
            //{
                result = Math.Pow(Math.Sqrt(intNum / Math.PI), 3);
            //}
            return result;
        }

        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();

            var inputIntegers = GenerateInputData();

            var parReductionQuery = (from intNum in inputIntegers.AsParallel()
                                     where intNum % 5 == 0 ||
                                     intNum % 7 == 0 || intNum % 9 == 0
                                     select (CalculateX(intNum))).Average();
            Console.WriteLine($"Average {parReductionQuery}");
            Console.WriteLine(sw.Elapsed);
            Console.ReadLine();
        }
    }
}
