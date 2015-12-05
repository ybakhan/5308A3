using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RingElection.Algorithm;
using RingElection.Util;

namespace RingElection
{
    public static class ElectionEvaluation
    {
        private static readonly Random rnd = new Random();

        public static void Main(string[] args)
        {
            Console.Write("*************************Election Algorithms Comparison*************************" +
                              "\nThis program will output message cost and time cost of election algorithms all the way, as far as uni-directional, as far as bi-directional, controlled distance, stages, and alternate steps in two scenarios (1) all nodes initiate election (2) randomly choosen nodes initiate election. " +
                              "\nPlease enter number of trials. " +
                              "\n1 trial will test ring of size 3 only " +
                              "\n2 trials will test rings of size 3 and 4 " +
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

            Console.WriteLine("Started all initiators test");
            var resultFile1 = Compare(maxRingSize, false);

            Console.WriteLine("Started random initiators test");
            var resultFile2 = Compare(maxRingSize, true);

            Console.WriteLine("Finished tests. Results in file {0} and {1}", resultFile1, resultFile2);
        }

        private static string Compare(int maxRingSize, bool randomInitiators)
        {
            var results = new List<ComparisonResult>();
            for (var i = 3; i <= maxRingSize; i++)
            {
                var result = new ComparisonResult { NodeCount = i };
                var ids = RandomUniqueList.GenerateRandom(i, 1, 9999);
                var randomInitaors = ids.Shuffle().Take(rnd.Next(1, i));
                var min = ids.Min();

                var allTheWayRing = new Ring(ids.Select(id => new AllTheWay(id)));

                INode leader;
                var timer = new Stopwatch();
                timer.Start();
                if (randomInitiators)
                    leader = allTheWayRing.Elect(randomInitaors);
                else
                    leader = allTheWayRing.Elect();
                timer.Stop();

                result.AllTheWay_MessageCount = allTheWayRing.Sum(n => n.MessagesSent);
                result.AllTheWay_Seconds = timer.Elapsed.TotalSeconds;

                var success = min == leader.Id;
                if (!success)
                    Console.WriteLine("election failed");

                var asFarAsRing = new Ring(ids.Select(id => new AsFarAs(id)));
                timer = new Stopwatch();
                timer.Start();
                if (randomInitiators)
                    leader = asFarAsRing.Elect(randomInitaors);
                else
                    leader = asFarAsRing.Elect();
                timer.Stop();

                result.AsFarAs_MessageCount = asFarAsRing.Sum(n => n.MessagesSent);
                result.AsFarAs_Seconds = timer.Elapsed.TotalSeconds;

                success = min == leader.Id;
                if (!success)
                    Console.WriteLine("election failed");

                var asFarAsBiRing = new Ring(ids.Select(id => new AsFarAsBidirectional(id)));
                timer = new Stopwatch();
                timer.Start();
                if (randomInitiators)
                    leader = asFarAsBiRing.Elect(randomInitaors);
                else
                    leader = asFarAsBiRing.Elect();
                timer.Stop();

                result.AsFarAsBi_MessageCount = asFarAsBiRing.Sum(n => n.MessagesSent);
                result.AsFarAsBi_Seconds = timer.Elapsed.TotalSeconds;

                success = min == leader.Id;
                if (!success)
                    Console.WriteLine("election failed");

                var controlledDistRing = new Ring(ids.Select(id => new ControlledDistance(id)));
                timer = new Stopwatch();
                timer.Start();
                if (randomInitiators)
                    leader = controlledDistRing.Elect(randomInitaors);
                else
                    leader = controlledDistRing.Elect();
                timer.Stop();

                result.ContDist_MessageCount = controlledDistRing.Sum(n => n.MessagesSent);
                result.ContDist_Seconds = timer.Elapsed.TotalSeconds;

                success = min == leader.Id;
                if (!success)
                    Console.WriteLine("election failed");

                var stagesRing = new Ring(ids.Select(id => new Stages(id)));
                timer = new Stopwatch();
                timer.Start();
                if (randomInitiators)
                    leader = stagesRing.Elect(randomInitaors);
                else
                    leader = stagesRing.Elect();
                timer.Stop();

                result.Stages_MessageCount = stagesRing.Sum(n => n.MessagesSent);
                result.Stages_Seconds = timer.Elapsed.TotalSeconds;

                success = min == leader.Id;
                if (!success)
                    Console.WriteLine("election failed");

                var altStepsRing = new Ring(ids.Select(id => new AlternateSteps(id)));
                timer = new Stopwatch();
                timer.Start();
                if (randomInitiators)
                    leader = altStepsRing.Elect(randomInitaors);
                else
                    leader = altStepsRing.Elect();
                timer.Stop();

                result.AltSteps_MessageCount = altStepsRing.Sum(n => n.MessagesSent);
                result.AltSteps_Seconds = timer.Elapsed.TotalSeconds;

                success = min == leader.Id;
                if (!success)
                    Console.WriteLine("election failed");

                results.Add(result);
            }

            var exporter = new CsvExport<ComparisonResult>(results);
            var timeNow = DateTime.Now;

            var resultsFile = string.Format("ElectionComparison_{5}_{0}{1}{2}{3}{4}.csv",
                timeNow.Minute, timeNow.Hour, timeNow.Day, timeNow.Month, timeNow.Year, randomInitiators
                    ? "RandomInitiators"
                    : "AllInitiators");
            exporter.ExportToFile(resultsFile);

            return resultsFile;
        }
    }
}
