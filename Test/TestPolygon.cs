using System.Collections.Generic;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class TestPolygon : TestBase
    {



        [Test]
        public void TestModify()
        {
            var p1 = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5))");
            var p2 = ReadWkt<Polygon>("POLYGON((0 0, 0.5 0.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5), (7 7, 8 8, 10 10, 7 7))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestInsert()
        {
            Polygon p1 = null;
            var p2 = ReadWkt<Polygon>("POLYGON((0 0, 0.5 0.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5), (7 7, 8 8, 10 10, 7 7))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.IsNull(unpatched);
        }

        [Test]
        public void TestDelete()
        {
            var p1 = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5))");
            Polygon p2 = null;

            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.IsNull(patched);


            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestCreateModifyDiff()
        {
            var p = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5))");
            var p2 = ReadWkt<Polygon>("POLYGON((0 0, 0.5 0.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5), (7 7, 8 8, 10 10, 7 7))");
            var n = (PolygonDiff)Differ.CreateDiff(p, p2);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Modify, n.Operation);
            Assert.AreEqual(3, n.Value.Count);

            var r1 = n.Value[0];
            Assert.AreEqual(Operation.Modify, r1.Operation);
            Assert.AreEqual(0, r1.Index);
            Assert.AreEqual(1, r1.Value.Count);

            var r1P1 = r1.Value[0];
            Assert.AreEqual(Operation.Modify, r1P1.Operation);
            Assert.AreEqual(1, r1P1.Index);
            Assert.AreEqual(-0.5, r1P1.Value.X);
            Assert.AreEqual(-0.5, r1P1.Value.Y);
            Assert.AreEqual(double.NaN, r1P1.Value.Z);



            var r2 = n.Value[1];
            Assert.AreEqual(Operation.Delete, r2.Operation);
            Assert.AreEqual(1, r2.Index);
            Assert.AreEqual(4, r2.Value.Count);


            var r3 = n.Value[2];
            Assert.AreEqual(Operation.Insert, r3.Operation);
            Assert.AreEqual(3, r3.Index);
            Assert.AreEqual(4, r3.Value.Count);

        }

        [Test]
        public void TestCreateDeleteDiff()
        {
            var p = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5))");


            var n = (PolygonDiff)Differ.CreateDiff(p, null);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Delete, n.Operation);
            Assert.AreEqual(3, n.Value.Count);
            Assert.AreEqual(Operation.Delete, n.Value[0].Operation);
            Assert.AreEqual(Operation.Delete, n.Value[0].Value[0].Operation);
        }

        [Test]
        public void TestCreateInsertDiff()
        {
            var p = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5))");

            var n = (PolygonDiff)Differ.CreateDiff(null, p);


            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Insert, n.Operation);
            Assert.AreEqual(3, n.Value.Count);
            Assert.AreEqual(Operation.Insert, n.Value[0].Operation);
            Assert.AreEqual(Operation.Insert, n.Value[0].Value[0].Operation);
        }

        [Test]
        public void TestApplyModify()
        {
            var p = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5))");
            var diff = new PolygonDiff()
            {
                Operation = Operation.Modify,
                Index = 0,
                Value = new List<RingDiff>()
                {
                    new RingDiff() {Index = 0, Operation = Operation.Modify, Value = new List<PointDiff>()
                    {
                        new PointDiff(){Index = 1, Operation = Operation.Modify, Value = new CoordinateDelta() {X = -0.5, Y = -0.5}}
                    }},
                    new RingDiff(){Index = 1, Operation = Operation.Delete, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 3, Y = 3}},
                            new PointDiff(){Index = 1, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 4, Y = 4}},
                            new PointDiff(){Index = 2, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 5, Y = 5}},
                            new PointDiff(){Index = 3, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 3, Y = 3}},
                        }},
                    new RingDiff(){Index = 3, Operation = Operation.Insert, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 7, Y = 7}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 8, Y = 8}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 10, Y = 10}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 7, Y = 7}}
                        }}
                }
            };



            var n = (Polygon)diff.Apply(p);
            Assert.NotNull(n.Shell);
            Assert.AreEqual(2, n.Holes.Length);

            var p2 = ReadWkt<Polygon>("POLYGON((0 0, 0.5 0.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5), (7 7, 8 8, 10 10, 7 7))");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));

        }



        [Test]
        public void TestApplyDelete()
        {
            var p = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3))");
            var diff = new PolygonDiff()
            {
                Operation = Operation.Delete,
                Index = 0,
                Value = new List<RingDiff>()
                {
                    new RingDiff(){Index = 0, Operation = Operation.Noop, Value = new List<PointDiff>()
                    {
                        new PointDiff(){Index = 0, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 0, Y = 0}},
                        new PointDiff(){Index = 1, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 1, Y = 1}},
                        new PointDiff(){Index = 2, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 2, Y = 2}},
                        new PointDiff(){Index = 3, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 0, Y = 0}},
                    }},
                    new RingDiff(){Index = 1, Operation = Operation.Noop, Value = new List<PointDiff>()
                    {
                        new PointDiff(){Index = 0, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 3, Y = 3}},
                        new PointDiff(){Index = 1, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 4, Y = 4}},
                        new PointDiff(){Index = 2, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 5, Y = 5}},
                        new PointDiff(){Index = 3, Operation = Operation.Noop, Value = new CoordinateDelta() {X = 3, Y = 3}},
                    }},
                }
            };
            var n = diff.Apply(p);
            Assert.IsNull(n);
        }

        [Test]
        public void TestApplyCreate()
        {
            Polygon p = null;
            var diff = new PolygonDiff()
            {
                Operation = Operation.Insert,
                Index = 0,
                Value = new List<RingDiff>()
                {
                    new RingDiff(){Index = 0, Operation = Operation.Insert, Value = new List<PointDiff>()
                    {
                        new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 0, Y = 0}},
                        new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 1, Y = 1}},
                        new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 2, Y = 2}},
                        new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 0, Y = 0}},
                    }},
                    new RingDiff(){Index = 1, Operation = Operation.Insert, Value = new List<PointDiff>()
                    {
                        new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 3, Y = 3}},
                        new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 4, Y = 4}},
                        new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 5, Y = 5}},
                        new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta() {X = 3, Y = 3}},
                    }},
                }
            };

            var n = diff.Apply(p);
            var p2 = ReadWkt<Polygon>("POLYGON((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3))");

            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }
    }
}
