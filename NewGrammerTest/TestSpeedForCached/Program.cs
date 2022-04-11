using System.Diagnostics;


long l1, l2, l3, l4, l5, l6, l7;
long number;
long b1, b2, b3, b4, b5, b6, b7;

Stopwatch stopwatch = new Stopwatch();

stopwatch.Start();
for (long i = 0; i < 1000000000; i++)
{
    number = i;
}
for (long i = 0; i < 1000000000; i++)
{
    number = i;
}
stopwatch.Stop();

Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);