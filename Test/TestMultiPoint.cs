using System.Collections.Generic;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class TestMultiPoint : TestBase
    {
        
        [Test]
        public void TestModify()
        {
            var p1 = ReadWkt<MultiPoint>("MULTIPOINT((2 2), (3 3))");
            var p2 = ReadWkt<MultiPoint>("MULTIPOINT((1 2), (1.5 2.5))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestInsert()
        {
            MultiPoint p1 = null;
            var p2 = ReadWkt<MultiPoint>("MULTIPOINT((1 2), (1.5 2.5))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.IsNull(unpatched);
        }

        [Test]
        public void TestDelete()
        {
            var p1 = ReadWkt<MultiPoint>("MULTIPOINT((2 2), (3 3))");
            MultiPoint p2 = null;

            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.IsNull(patched);


            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }


        [Test]
        public void TestCreateModifyDiff()
        {
            var p = ReadWkt<MultiPoint>("MULTIPOINT((2 2), (3 3))");
            var p2 = ReadWkt<MultiPoint>("MULTIPOINT((1 2), (1.5 2.5))");
            var n = (MultiPointDiff)Differ.CreateDiff(p, p2);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Modify, n.Operation);
            Assert.AreEqual(2, n.Value.Count);

        }

        [Test]
        public void TestCreateDeleteDiff()
        {
            var p = ReadWkt<MultiPoint>("MULTIPOINT((2 2), (3 3))");

            var n = (MultiPointDiff)Differ.CreateDiff(p, null);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Delete, n.Operation);
            Assert.AreEqual(2, n.Value.Count);
        }

        [Test]
        public void TestCreateInsertDiff()
        {
            var p = ReadWkt<MultiPoint>("MULTIPOINT((2 2), (3 3))");

            var n = (MultiPointDiff)Differ.CreateDiff(null, p);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Insert, n.Operation);
            Assert.AreEqual(2, n.Value.Count);
        }


        [Test]
        public void TestApplyModify()
        {

            var p = ReadWkt<MultiPoint>("MULTIPOINT((2 2), (3 3))");
            var diff = new MultiPointDiff()
            {
                Index = 0,
                Operation = Operation.Modify,
                Value = new List<PointDiff>()
            {
                new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y = 2}},
                new PointDiff(){Index = 0, Operation = Operation.Modify, Value = new CoordinateDelta(){X = -0.5, Y = 0.5}},
                new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y = 3}}
            }
            };
            var n = diff.Apply(p);
            var p2 = ReadWkt<MultiPoint>("MULTIPOINT((1 2), (1.5 2.5))");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }

        [Test]
        public void TestApplyCreate()
        {

            MultiPoint p = null;
            var diff = new MultiPointDiff()
            {
                Index = 0,
                Operation = Operation.Insert,
                Value = new List<PointDiff>()
                {
                    new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y = 2}},

                    new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 3, Y = 3}}
                }
            };
            var n = diff.Apply(p);
            var p2 = ReadWkt<MultiPoint>("MULTIPOINT((1 2), (3 3))");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }

        [Test]
        public void TestApplyDelete()
        {
            var p = ReadWkt<MultiPoint>("MULTIPOINT((2 2), (3 3))");
            var diff = new MultiPointDiff()
            {
                Index = 0,
                Operation = Operation.Delete,
                Value = new List<PointDiff>()
                {
                    new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 1, Y = 2}},
                    new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y = 3}}
                }
            };
            var n = diff.Apply(p);
            Assert.IsNull(n);
        }

        [Test]
        public void TestReverseModify()
        {
            var diff = new MultiPointDiff()
            {
                Index = 0,
                Operation = Operation.Modify,
                Value = new List<PointDiff>()
                {
                    new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y = 2}},
                    new PointDiff(){Index = 0, Operation = Operation.Modify, Value = new CoordinateDelta(){X = -0.5, Y = 0.5}},
                    new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y = 3}}
                }
            };
            var reversed = (MultiPointDiff)diff.Reverse();
            Assert.AreEqual(0, reversed.Index);
            Assert.AreEqual(Operation.Modify, reversed.Operation);
            Assert.AreEqual(3, reversed.Value.Count);

            var d1 = reversed.Value[0];
            Assert.AreEqual(Operation.Delete, d1.Operation);
            Assert.AreEqual(0, d1.Index);
            Assert.AreEqual(1, d1.Value.X);
            Assert.AreEqual(2, d1.Value.Y);

            var d2 = reversed.Value[1];
            Assert.AreEqual(Operation.Modify, d2.Operation);
            Assert.AreEqual(1, d2.Index);
            Assert.AreEqual(0.5, d2.Value.X);
            Assert.AreEqual(-0.5, d2.Value.Y);

            var d3 = reversed.Value[2];
            Assert.AreEqual(Operation.Insert, d3.Operation);
            Assert.AreEqual(2, d3.Index);
            Assert.AreEqual(3, d3.Value.X);
            Assert.AreEqual(3, d3.Value.Y);

        }

        [Test]
        public void TestReverseInsert()
        {
            var diff = new MultiPointDiff()
            {
                Index = 0,
                Operation = Operation.Insert,
                Value = new List<PointDiff>()
                {
                    new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y = 2}},

                    new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 3, Y = 3}}
                }
            };
            var reversed = (MultiPointDiff)diff.Reverse();
            Assert.AreEqual(0, reversed.Index);
            Assert.AreEqual(Operation.Delete, reversed.Operation);

        }

        [Test]
        public void TestReverseDelete()
        {
            var diff = new MultiPointDiff()
            {
                Index = 0,
                Operation = Operation.Delete,
                Value = new List<PointDiff>()
                {
                    new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 1, Y = 2}},
                    new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y = 3}}
                }
            };
            var reversed = (MultiPointDiff)diff.Reverse();
            Assert.AreEqual(0, reversed.Index);
            Assert.AreEqual(Operation.Insert, reversed.Operation);

        }

    }

}
