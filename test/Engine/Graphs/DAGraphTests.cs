﻿using RecipeCalculator.Engine.Graphs;

namespace RecipeCalculator.Engine.Test.Graphs;

[TestClass]
public class DAGraphTests
{
    [TestMethod]
        public void TestMultiple()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(1, 1, new[] { 2, 3, 4 });
            graph.AddNode(2, 4, new[] { 4 });
            graph.AddNode(3, 9, new[] { 2, 4 });
            graph.AddNode(4, 16, Array.Empty<int>());

            var (layers, detached) = graph.TopologicalSort();

            Assert.AreEqual(4, layers.Count);
            Assert.AreEqual(0, detached.Count);
            Assert.AreEqual(16, graph[layers[0][0]]);
            Assert.AreEqual(4, graph[layers[1][0]]);
            Assert.AreEqual(9, graph[layers[2][0]]);
            Assert.AreEqual(1, graph[layers[3][0]]);
        }

        [TestMethod]
        public void TestBranch()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(7, 1, new[] { 4, 6 });
            graph.AddNode(5, 1, new[] { 1, 4 });
            graph.AddNode(4, 1, new[] { 2, 3 });
            graph.AddNode(3, 1, new[] { 1, 2 });
            graph.AddNode(2, 1, new[] { 1 });
            graph.AddNode(1, 1, Array.Empty<int>());
            graph.AddNode(6, 1, new[] { 5, 4 });

            var incoming = graph.GetIncoming(1);
            var outgoing = graph.GetOutgoing(3);

            Assert.AreEqual(1, graph[1]);
            Assert.AreEqual(3, incoming.Count);
            Assert.AreEqual(3, incoming.Distinct().Count());
            Assert.AreEqual(2, outgoing.Count);
            Assert.AreEqual(2, outgoing.Distinct().Count());
        }

        [TestMethod]
        public void TestFlat()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(1, 1, Array.Empty<int>());
            graph.AddNode(2, 1, Array.Empty<int>());
            graph.AddNode(3, 1, Array.Empty<int>());
            graph.AddNode(4, 1, Array.Empty<int>());

            var (layers, detached) = graph.TopologicalSort();
            var incoming = graph.GetIncoming(1);
            var outgoing = graph.GetOutgoing(0);

            Assert.AreEqual(1, layers.Count);
            Assert.AreEqual(0, detached.Count);
            Assert.AreEqual(4, layers[0].Count);
            Assert.AreEqual(0, incoming.Count);
            Assert.AreEqual(0, outgoing.Count);
        }

        [TestMethod]
        public void TestDuplicate()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(1, 1, new[] { 2 });
            graph.AddNode(2, 1, new[] { 3 });
            graph.AddNode(3, 1, new[] { 4 });
            graph.AddNode(4, 1, new[] { 5 });

            Assert.ThrowsException<ArgumentException>(() => graph.AddNode(1, 1, new[] { 5 }));
        }

        [TestMethod]
        public void TestCycle()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(1, 1, new[] { 2 });
            graph.AddNode(2, 1, new[] { 3 });
            graph.AddNode(3, 1, new[] { 4 });
            graph.AddNode(4, 1, new[] { 5 });

            Assert.ThrowsException<ArgumentException>(() => graph.AddNode(5, 1, new[] { 1 }));
            Assert.ThrowsException<ArgumentException>(() => graph.AddNode(5, 1, new[] { 5 }));
        }

        [TestMethod]
        public void TestDetached()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(1, 1, Array.Empty<int>());
            graph.AddNode(2, 1, new[] { 1 });
            graph.AddNode(3, 1, new[] { 2 });
            graph.AddNode(4, 1, new[] { 10 });
            graph.AddNode(5, 1, new[] { 3, 4 });

            var (layers, detached) = graph.TopologicalSort();

            Assert.AreEqual(3, layers.Count);
            Assert.AreEqual(2, detached.Count);
        }

        [TestMethod]
        public void TestAllDetached()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(1, 1, new[] { 10 });
            graph.AddNode(2, 1, new[] { 11 });
            graph.AddNode(3, 1, new[] { 12 });
            graph.AddNode(4, 1, new[] { 13 });

            var (layers, detached) = graph.TopologicalSort();

            Assert.AreEqual(0, layers.Count);
            Assert.AreEqual(4, detached.Count);
        }

        [TestMethod]
        public void TestTrim()
        {
            var graph = new DAGraph<int, int>();
            graph.AddNode(1, 1, new[] { 2, 3, 4 });
            graph.AddNode(2, 4, new[] { 4 });
            graph.AddNode(3, 9, new[] { 2, 4 });
            graph.AddNode(4, 16, Array.Empty<int>());

            var clone = graph.Clone();
            clone.Trim(new[] { 3 });

            var (originalLayers, _) = graph.TopologicalSort();
            var (layers, detached) = clone.TopologicalSort();

            Assert.AreEqual(4, graph.Count);
            Assert.AreEqual(4, originalLayers.Count);
            Assert.AreEqual(3, clone.Count);
            Assert.AreEqual(3, layers.Count);
            Assert.AreEqual(0, detached.Count);
            Assert.ThrowsException<ArgumentNullException>(() => clone.Trim(null!));
            Assert.ThrowsException<ArgumentException>(() => clone.RemoveNode(100));
        }
}