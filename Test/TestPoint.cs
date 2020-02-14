using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class TestPoint : TestBase
    {

        
        [Test]
        public void TestModify()
        {
            var p1 = ReadWkt<Point>("POINT(10.53 60.10)");
            var p2 = ReadWkt<Point>("POINT(10.52 60.10)");
            var diff = (PointDiff)Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestInsert()
        {
            Point p1 = null;
            var p2 = ReadWkt<Point>("POINT(10.52 60.10)");
            var diff = (PointDiff)Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.IsNull(unpatched);
        }

        [Test]
        public void TestDelete()
        {
            var p1 = ReadWkt<Point>("POINT(10.52 60.10)");
            Point p2 = null;

            var diff = (PointDiff)Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.IsNull(patched);


            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }


        [Test]
        public void TestCreateModifyDiff()
        {
            var p = ReadWkt<Point>("POINT(10.53 60.10)");
            var p2 = ReadWkt<Point>("POINT(10.52 60.10)");
            var n = (PointDiff)Differ.CreateDiff(p, p2);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Modify, n.Operation);
            Assert.AreEqual(-0.01, n.Value.X, 0.0000001);
            Assert.AreEqual(0, n.Value.Y);
            Assert.AreEqual(double.NaN, n.Value.Z);
        }

        [Test]
        public void TestCreateDeleteDiff()
        {
            var p = ReadWkt<Point>("POINT(10.53 60.10)");

            var n = (PointDiff)Differ.CreateDiff(p, null);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Delete, n.Operation);
            Assert.AreEqual(10.53, n.Value.X);
            Assert.AreEqual(60.10, n.Value.Y);
            Assert.AreEqual(double.NaN, n.Value.Z);
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
            var p = ReadWkt<Point>("POINT(10.53 60.10)");
            var diff = new PointDiff() { Index = 0, Operation = Operation.Modify, Value = new CoordinateDelta() { X = -0.01 } };
            var n = diff.Apply(p);

            var p2 = ReadWkt<Point>("POINT(10.52 60.10)");

            Assert.AreEqual(p2, n);
        }

        [Test]
        public void TestApplyCreate()
        {
            var diff = new PointDiff() { Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta() { X = 1, Y = 2 } };
            var n = diff.Apply(null);
            var p2 = ReadWkt<Point>("POINT(1 2)");

            Assert.AreEqual(p2, n);
        }

        [Test]
        public void TestApplyDelete()
        {
            var p = ReadWkt<Point>("POINT(10.53 60.10)");
            var diff = new PointDiff() { Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta() { X = 1, Y = 2 } };
            var n = diff.Apply(p);
            Assert.AreEqual(null, n);
        }

        [Test]
        public void TestReverseModify()
        {
            var diff = new PointDiff() { Index = 0, Operation = Operation.Modify, Value = new CoordinateDelta() { X = -0.01 } };
            var reversed = (PointDiff)diff.Reverse();
            Assert.AreEqual(0, reversed.Index);
            Assert.AreEqual(Operation.Modify, reversed.Operation);
            Assert.AreEqual(0.01, reversed.Value.X);
        }

        [Test]
        public void TestReverseInsert()
        {
            var diff = new PointDiff() { Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta() { X = 1, Y = 2 } };
            var reversed = (PointDiff)diff.Reverse();
            Assert.AreEqual(0, reversed.Index);
            Assert.AreEqual(Operation.Delete, reversed.Operation);
            Assert.AreEqual(1, reversed.Value.X);
            Assert.AreEqual(2, reversed.Value.Y);
        }

        [Test]
        public void TestReverseDelete()
        {
            var diff = new PointDiff() { Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta() { X = 1, Y = 2 } };
            var reversed = (PointDiff)diff.Reverse();
            Assert.AreEqual(0, reversed.Index);
            Assert.AreEqual(Operation.Insert, reversed.Operation);
            Assert.AreEqual(1, reversed.Value.X);
            Assert.AreEqual(2, reversed.Value.Y);
        }

    }
}
