using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RingElection.Algorithm;
using RingElection.Util;

namespace RingElection
{
    public static class StagesBestCase
    {
        public static void Main(string[] args)
        {
            Console.Write("*************************Stages best case test************************" +
                              "\nThis test will output message cost and time cost of stages in best case" +
                              "\nPlease enter number of trials" +
                              "\n1 trial will test ring of size 3 only " +
                              "\n2 trials will test rings of size 3 to 4 " +
                              "\nx trials will test rings of size 3 to x+2: ");

            bool validTrials;
            var trials = 0;
            do
            {
                try
                {
                    trials = Convert.ToInt32(Console.ReadLine());
                    validTrials = true;
                }
                catch (Exception)
                {
                    Console.Write("Invalid number of trials. Enter number >=3: ");
                    validTrials = false;
                }
            } while (!validTrials);
            var maxRingSize = trials + 2;

            Console.WriteLine("Stages best case test started");

            var results = new List<TestResult>();
            for (var i = 3; i <= maxRingSize; i++)
            {
                var result = new TestResult { NodeCount = i };
                var ids = GetBestCaseIDs(i);
                var min = ids.Min();

                var ring = new Ring(ids.Select(id => new Stages(id)));

                var timer = new Stopwatch();
                timer.Start();
                var leader = ring.Elect();
                timer.Stop();

                var success = min == leader.Id;
                if (!success)
                    Console.WriteLine("election failed");

                result.MessageCount = ring.Sum(n => n.MessagesSent);
                result.Seconds = timer.Elapsed.TotalSeconds;

                results.Add(result);
            }

            var exporter = new CsvExport<TestResult>(results);
            var timeNow = DateTime.Now;
            var file = string.Format("StagesBestCase_{0}{1}{2}{3}{4}.csv",
                timeNow.Minute, timeNow.Hour, timeNow.Day, timeNow.Month, timeNow.Year);
            exporter.ExportToFile(file);

            Console.WriteLine("Stages best case test finished. Results saved in {0}", file);
        }

        private static List<int> GetBestCaseIDs(int count)
        {
            var ids = new List<int>();
            for (var i = 1; i <= count; i++)
            {
                ids.Add(i);
            }
            return ids;
        }
    }
}
