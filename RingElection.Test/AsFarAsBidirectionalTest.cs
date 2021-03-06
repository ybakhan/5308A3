﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingElection.Algorithm;

namespace RingElection.Test
{
    [TestClass]
    public class AsFarAsBidirectionalTest
    {
        private static readonly Random rnd = new Random();

        [TestMethod]
        public void TestElect_10nodes_randomInititators()
        {
            TestElect_nNodes_randomInitiators(10);
        }

        [TestMethod]
        public void TestElect_3nodes_randomInititators()
        {
            TestElect_nNodes_randomInitiators(3);
        }

        [TestMethod]
        public void TestElect_10nodes_AllInititators()
        {
            TestElect_nNodes_randomInitiators(10, true);
        }

        [TestMethod]
        public void TestElect_3nodes_AllInititators()
        {
            TestElect_nNodes_randomInitiators(3, true);
        }

        private void TestElect_nNodes_randomInitiators(int n, bool allInitiate = false)
        {
            var nodes = new List<AsFarAsBidirectional>();
            for (int i = 1; i <= n; i++)
                nodes.Add(new AsFarAsBidirectional(i));

            var expectedLeader = nodes.ElementAt(0);
            var network = new Ring(nodes);

            AsFarAsBidirectional actualLeader;
            if (allInitiate)
            {
                actualLeader = network.Elect() as AsFarAsBidirectional;
            }
            else
            {
                var initiators = nodes.Shuffle().Take(rnd.Next(1, n)).Select(node => node.Id);
                actualLeader = network.Elect(initiators) as AsFarAsBidirectional;
            }

            Assert.AreEqual(expectedLeader, actualLeader);
            Assert.AreEqual(n - 1, network.Count(node => node.State == NodeState.Follower));
        }
    }
}
