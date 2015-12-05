using RingElection.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RingElection
{
  public class RingElectionMain
  {
    public static void Main(string[] args)
    {
      do
      {
        Console.Write("Select test" +
                          "\n(1) All the way test" +
                          "\n(2) All the way complexity" +
                          "\n(3) As far as test" +
                          "\n(4) As far as best case" +
                          "\n(5) As far as worst case" +
                          "\n(6) As far as bi-directional test" +
                          "\n(7) Controlled distance test" +
                          "\n(8) Controlled distance complexity" +
                          "\n(9) Stages test" +
                          "\n(10) Stages best case" +
                          "\n(11) Stages complexity" +
                          "\n(12) Alternate steps test" +
                          "\n(13) Alternate steps complexity" +
                          "\n(14) Comparative Evaluation" +
                          "\n(15) Exit: ");
        try
        {
          var option = Convert.ToInt32(Console.ReadLine());
          switch (option)
          {
            case 1:
              Console.WriteLine("All the way test started");
              var allTheWayRing = new Ring(ReadIDs().Select(id => new AllTheWay(id)));
              allTheWayRing.Elect();
              Console.WriteLine("All the way test finished");
              break;

            case 2:
              AllTheWayComplexity.Main(null);
              break;

            case 3:
              Console.WriteLine("As far as test started");
              var asFarAsRing = new Ring(ReadIDs().Select(id => new AsFarAs(id)));
              asFarAsRing.Elect();
              Console.WriteLine("As far as test finished");
              break;

            case 4:
              AsFarAsBestCase.Main(null);
              break;

            case 5:
              AsFarAsWorstCase.Main(null);
              break;

            case 6:
              Console.WriteLine("As far as bi-directional test started");
              var asFarAsBi = new Ring(ReadIDs().Select(id => new AsFarAsBidirectional(id)));
              asFarAsBi.Elect();
              Console.WriteLine("As far as bi-directional test finished");
              break;

            case 7:
              Console.WriteLine("Controlled distance test started");
              var contDistRing = new Ring(ReadIDs().Select(id => new ControlledDistance(id)));
              contDistRing.Elect();
              Console.WriteLine("Controlled distance test finished");
              break;

            case 8:
              ControlledDistanceComplexity.Main(null);
              break;

            case 9:
              Console.WriteLine("Stages test started");
              var stagesRing = new Ring(ReadIDs().Select(id => new Stages(id)));
              stagesRing.Elect();
              Console.WriteLine("Stages test finished");
              break;

            case 10:
              StagesBestCase.Main(null);
              break;

            case 11:
              StagesComplexity.Main(null);
              break;

            case 12:
              Console.WriteLine("Alternate steps test started");
              var altStepsRing = new Ring(ReadIDs().Select(id => new AlternateSteps(id)));
              altStepsRing.Elect();
              Console.WriteLine("Alternate steps test finished");
              break;

            case 13:
              AlternateStepsComplexity.Main(null);
              break;

            case 14:
              ElectionEvaluation.Main(null);
              break;

            case 15:
              return;

            default:
              Console.WriteLine("Invalid option");
              break;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine("Invalid option.");
        }
      } while (true);
    }

    private static IEnumerable<int> ReadIDs()
    {
      var validInput = true;
      List<int> nodeIDs = new List<int>();
      Console.Write("Enter at least 3 integer IDs separate by ',': ");
      do
      {
        var idstr = Console.ReadLine();
        try
        {
          var idsArr = idstr.Split(',');
          foreach (var id in idsArr)
          {
            try
            {
              var idInt = Int32.Parse(id);
              nodeIDs.Add(idInt);
              validInput = true;
            }
            catch
            {
              Console.Write("Invalid input. Enter at least 3 integer IDs separate by ',': ");
              validInput = false;
              nodeIDs = new List<int>();
            }
          }
        }
        catch (Exception)
        {
          Console.Write("Invalid input. Enter at least 3 integer IDs separate by ',': ");
          validInput = false;
          nodeIDs = new List<int>();
        }
        if (nodeIDs.Count < 3)
        {
          Console.Write("Invalid input. Enter at least 3 integer IDs separate by ',': ");
          validInput = false;
          nodeIDs = new List<int>();
        }
      } while (!validInput);

      return nodeIDs;
    }
  }
}

