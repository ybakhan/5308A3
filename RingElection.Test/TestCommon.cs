using RingElection.Util;
using System.Collections.Generic;

namespace RingElection.Test
{
  public static class TestCommon
  {
    public static IEnumerable<int> GenerateRandomIDs(int n, int minId, int maxId)
    {
      return RandomUniqueList.GenerateRandom(n, 1, 101);
    }

    public static int SumToN(int min, int n)
    {
      var sum = 0;
      for (int i = min; i <= n; i++)
        sum += i;
      return sum;
    }
  }
}
