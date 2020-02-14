using System.Collections.Generic;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class TestLineString : TestBase
    {

        [Test]
        public void TestModify()
        {
            var p1 = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3, 4 4)");
            var p2 = ReadWkt<LineString>("LINESTRING(0 0, 1 1, 2.5 2.5, 3 3)");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestInsert()
        {
            LineString p1 = null;
            var p2 = ReadWkt<LineString>("LINESTRING(0 0, 1 1, 2.5 2.5, 3 3)");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.IsNull(unpatched);
        }

        [Test]
        public void TestDelete()
        {
            var p1 = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3, 4 4)");
            LineString p2 = null;

            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.IsNull(patched);


            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestCreateModifyDiff()
        {
            var p = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3, 4 4)");
            var p2 = ReadWkt<LineString>("LINESTRING(0 0, 1 1, 2.5 2.5, 3 3)");
            var n = (LineStringDiff)Differ.CreateDiff(p, p2);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Modify, n.Operation);
            Assert.AreEqual(3, n.Value.Count);

            var d1 = n.Value[0];
            Assert.AreEqual(0, d1.Index);
            Assert.AreEqual(Operation.Insert, d1.Operation);
            Assert.AreEqual(0, d1.Value.X);
            Assert.AreEqual(0, d1.Value.Y);
            Assert.AreEqual(double.NaN, d1.Value.Z);

            var d2 = n.Value[1];
            Assert.AreEqual(1, d2.Index);
            Assert.AreEqual(Operation.Modify, d2.Operation);
            Assert.AreEqual(0.5, d2.Value.X);
            Assert.AreEqual(0.5, d2.Value.Y);
            Assert.AreEqual(double.NaN, d2.Value.Z);

            var d3 = n.Value[2];
            Assert.AreEqual(3, d3.Index);
            Assert.AreEqual(Operation.Delete, d3.Operation);
            Assert.AreEqual(4, d3.Value.X);
            Assert.AreEqual(4, d3.Value.Y);
            Assert.AreEqual(double.NaN, d3.Value.Z);

        }


        [Test]
        public void TestCreateDeleteDiff()
        {
            var p = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3, 4 4)");

            var n = (LineStringDiff)Differ.CreateDiff(p, null);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Delete, n.Operation);
            Assert.AreEqual(4, n.Value.Count);

            var d1 = n.Value[0];
            Assert.AreEqual(0, d1.Index);
            Assert.AreEqual(Operation.Delete, d1.Operation);
            Assert.AreEqual(1, d1.Value.X);
            Assert.AreEqual(1, d1.Value.Y);
            Assert.AreEqual(double.NaN, d1.Value.Z);

            var d2 = n.Value[1];
            Assert.AreEqual(1, d2.Index);
            Assert.AreEqual(Operation.Delete, d2.Operation);
            Assert.AreEqual(2, d2.Value.X);
            Assert.AreEqual(2, d2.Value.Y);
            Assert.AreEqual(double.NaN, d2.Value.Z);

            var d3 = n.Value[2];
            Assert.AreEqual(2, d3.Index);
            Assert.AreEqual(Operation.Delete, d3.Operation);
            Assert.AreEqual(3, d3.Value.X);
            Assert.AreEqual(3, d3.Value.Y);
            Assert.AreEqual(double.NaN, d3.Value.Z);

            var d4 = n.Value[3];
            Assert.AreEqual(3, d4.Index);
            Assert.AreEqual(Operation.Delete, d4.Operation);
            Assert.AreEqual(4, d4.Value.X);
            Assert.AreEqual(4, d4.Value.Y);
            Assert.AreEqual(double.NaN, d4.Value.Z);

        }

        [Test]
        public void TestCreateInsertDiff()
        {
            var p2 = ReadWkt<Point>("POINT(10.53 60.10)");

            var n = (PointDiff)Differ.CreateDiff(null, p2);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Insert, n.Operation);
            Assert.AreEqual(10.53, n.Value.X);
            Assert.AreEqual(60.10, n.Value.Y);
            Assert.AreEqual(double.NaN, n.Value.Z);
        }


        [Test]
        public void TestApplyModify()
        {
            var p = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3, 4 4)");
            var diff = new LineStringDiff()
            {
                Operation = Operation.Modify,
                Index = 0,
                Value = new List<PointDiff>()
                {
                    new PointDiff() {Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 0, Y = 0}},
                    new PointDiff(){Index = 1, Operation = Operation.Modify, Value = new CoordinateDelta(){X = 0.5, Y = 0.5}},
                    new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 4, Y = 4}}
                }
            };
            var n = diff.Apply(p);

            var p2 = ReadWkt<LineString>("LINESTRING(0 0, 1 1, 2.5 2.5, 3 3)");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }

        [Test]
        public void TestApplyModify2()
        {
            var p = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3, 4 4)");
            var diff = new LineStringDiff()
            {
                Operation = Operation.Modify,
                Index = 0,
                Value = new List<PointDiff>()
                {
                    new PointDiff() {Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 0, Y = 0}},
                    new PointDiff(){Index = 0, Operation = Operation.Modify, Value = new CoordinateDelta(){X = 0.5, Y = 0.5}},
                    new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 4, Y = 4}}
                }
            };
            var n = diff.Apply(p);
            var p2 = ReadWkt<LineString>("LINESTRING(0 0, 1.5 1.5, 2 2, 3 3)");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));

        }

        [Test]
        public void TestApplyDelete()
        {
            var p = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3, 4 4)");
            var diff = new LineStringDiff()
            {
                Operation = Operation.Delete,
                Index = 0,
                Value = new List<PointDiff>()
                {
                    new PointDiff(){Index = 0, Operation = Operation.Noop, Value = new CoordinateDelta(){X = 1, Y = 1}},
                    new PointDiff(){Index = 1, Operation = Operation.Noop, Value = new CoordinateDelta(){X = 2, Y = 2}},
                    new PointDiff(){Index = 2, Operation = Operation.Noop, Value = new CoordinateDelta(){X = 3, Y = 3}},
                    new PointDiff(){Index = 3, Operation = Operation.Noop, Value = new CoordinateDelta(){X = 4, Y = 4}}
                }
            };
            var n = diff.Apply(p);
            Assert.IsNull(n);
        }

        [Test]
        public void TestApplyCreate()
        {
            LineString p = null;
            var diff = new LineStringDiff()
            {
                Operation = Operation.Insert,
                Index = 0,
                Value = new List<PointDiff>()
                {
                    new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y = 1}},
                    new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 2, Y = 2}},
                    new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 3, Y = 3}}
                }
            };
            var n = diff.Apply(p);

            var p2 = ReadWkt<LineString>("LINESTRING(1 1, 2 2, 3 3)");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }
    }
}
