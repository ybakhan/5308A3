using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RingElection.Algorithm;

namespace RingElection.Test
{
  [TestClass]
  public class AllTheWayTest
  {
    private static Random rnd = new Random();

    [TestMethod]
    public void TestElect_3nodes_randomInitiators()
    {
      TestElect_nNodes(3);
    }

    [TestMethod]
    public void TestElect_3nodes_AllInitiate()
    {
      TestElect_nNodes(3, allInitiate: true);
    }

    [TestMethod]
    public void TestElect_10nodes_randomInitiators()
    {
      TestElect_nNodes(10);
    }

    [TestMethod]
    public void TestElect_10nodes_AllInitiate()
    {
      TestElect_nNodes(10, allInitiate: true);
    }

    private void TestElect_nNodes(int n, bool allInitiate = false)
    {
      var nodes = new List<AllTheWay>();
      for (var i = 1; i <= n; i++)
        nodes.Add(new AllTheWay(i));

      var expectedLeader = nodes.ElementAt(0);
      var expectedMsgCount = n * n;

      var network = new Ring(nodes);
      AllTheWay actualLeader;
      if (allInitiate)
      {
        actualLeader = network.Elect() as AllTheWay;
      }
      else
      {
        var initiators = nodes.Shuffle().Take(rnd.Next(1, n)).Select(node => node.Id);
        actualLeader = network.Elect(initiators) as AllTheWay;
      }

      var actualMsgCount = network.Sum(node => node.MessagesSent);

      Assert.AreEqual(expectedLeader, actualLeader);
      for (int i = 0; i < n; i++)
        Assert.AreEqual(n, nodes.ElementAt(i).MessagesSent);
      Assert.AreEqual(expectedMsgCount, actualMsgCount);
    }
  }
}
