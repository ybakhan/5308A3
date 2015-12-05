using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RingElection.Algorithm;

namespace RingElection.Test
{
  [TestClass]
  public class AsFarAsTest
  {
    private static Random rnd = new Random();

    #region worst case

    [TestMethod]
    public void TestElect_WorstCase_3nodes_randomInitiators()
    {
      TestElect_WorstCase_nNodes(3);
    }

    [TestMethod]
    public void TestElect_WorstCase_3nodes_AllInitiate()
    {
      TestElect_WorstCase_nNodes(3, allInitiate: true);
    }

    [TestMethod]
    public void TestElect_WorstCase_10nodes_randomInitiators()
    {
      TestElect_WorstCase_nNodes(10);
    }

    [TestMethod]
    public void TestElect_WorstCase_10nodes_AllInitiate()
    {
      TestElect_WorstCase_nNodes(10, allInitiate: true);
    }

    private void TestElect_WorstCase_nNodes(int n, bool allInitiate = false)
    {
      var nodes = new List<AsFarAs>();
      for (int i = 1; i <= n; i++)
        nodes.Add(new AsFarAs(i));

      var expectedLeader = nodes.ElementAt(0);
      var expectedMsgCount = TestCommon.SumToN(1, n) + n;

      var network = new Ring(nodes);
      AsFarAs actualLeader;
      if (allInitiate)
      {
        actualLeader = network.Elect() as AsFarAs;
      }
      else
      {
          var initiators = nodes.Shuffle().Take(rnd.Next(1, n)).Select(node => node.Id);
        actualLeader = network.Elect(initiators) as AsFarAs;
      }

      var actualMsgCount = network.Sum(node => node.MessagesSent);

      Assert.AreEqual(expectedLeader, actualLeader);
      for (int i = 0; i < n; i++)
        Assert.AreEqual(i + 2, nodes.ElementAt(i).MessagesSent);
      Assert.AreEqual(expectedMsgCount, actualMsgCount);
    }

    #endregion

    #region best case

    [TestMethod]
    public void TestElect_BestCase_3Nodes_randomInitators()
    {
      TestElect_BestCase_nNodes(3);
    }

    [TestMethod]
    public void TestElect_BestCase_10Nodes_randomInitators()
    {
      TestElect_BestCase_nNodes(10);
    }

    private void TestElect_BestCase_nNodes(int n, bool allInitiate = false)
    {
      var nodes = new List<AsFarAs>();
      for (int i = n; i >= 1; i--)
        nodes.Add(new AsFarAs(i));

      var expectedLeader = nodes.ElementAt(n - 1);
      var expectedMsgCount = n + (n - 1) + n;

      var network = new Ring(nodes);
      AsFarAs actualLeader;
      if (allInitiate)
      {
        actualLeader = network.Elect() as AsFarAs;
      }
      else
      {
          var initiators = nodes.Shuffle().Take(rnd.Next(1, n)).Select(node => node.Id);
        actualLeader = network.Elect(initiators) as AsFarAs;
      }

      var actualMsgCount = network.Sum(node => node.MessagesSent);

      Assert.AreEqual(expectedLeader, actualLeader);
      Assert.AreEqual(expectedMsgCount, actualMsgCount);
      for (int i = 0; i < n - 1; i++)
          Assert.AreEqual(3, nodes.ElementAt(i).MessagesSent);
      Assert.AreEqual(2, nodes.ElementAt(n - 1).MessagesSent);
    }

    #endregion
  }
}
