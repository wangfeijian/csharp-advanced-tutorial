using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] scores = { 89, 90, 70, 65, 82, 88, 92, 74 };
            var highScores = from score in scores
                             where score > 80
                             orderby score descending
                             select score;

            foreach (var highScore in highScores)
            {
                Console.WriteLine(highScore);
            }

            var strScores = from score in scores
                            where score > 70
                            orderby score descending
                            select $"The score is {score}";

            foreach (var strScore in strScores)
            {
                Console.WriteLine(strScore);
            }

            var count = (from s in scores
                         where s > 80
                         select s).Count();
            Console.WriteLine(count);

            string[] words = { "apples", "blueberries", "oranges", "bananas", "apricots"};

            var wordGroup = from w in words
                group w by w[0]
                into groupLetter
                where groupLetter.Count() >= 2
                select new {firstLetter = groupLetter.Key, letterCount = groupLetter.Count()};

            foreach (var w in wordGroup)
            {
                Console.WriteLine($"{w.firstLetter} has {w.letterCount} elements.");
            }
        }
    }
}
