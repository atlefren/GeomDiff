using System.Collections.Generic;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class TestMultiLineString : TestBase
    {

        [Test]
        public void TestModify()
        {
            var p1 = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");
            var p2 = ReadWkt<MultiLineString>("MULTILINESTRING((0 0, 1.5 1.5, 2 2, 3 3), (4 5, 6 7))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestInsert()
        {
            MultiLineString p1 = null;
            var p2 = ReadWkt<MultiLineString>("MULTILINESTRING((0 0, 1.5 1.5, 2 2, 3 3), (4 5, 6 7))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.IsNull(unpatched);
        }

        [Test]
        public void TestDelete()
        {
            var p1 = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");
            MultiLineString p2 = null;

            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.IsNull(patched);


            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestCreateModifyDiff()
        {
            var p = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");
            var p2 = ReadWkt<MultiLineString>("MULTILINESTRING((0 0, 1.5 1.5, 2 2, 3 3), (4 5, 6 7))");
            var n = (MultiLineStringDiff)Differ.CreateDiff(p, p2);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Modify, n.Operation);
            Assert.AreEqual(2, n.Value.Count);

        }

        [Test]
        public void TestCreateDeleteDiff()
        {
            var p = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");

            var n = (MultiLineStringDiff)Differ.CreateDiff(p, null);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Delete, n.Operation);
            Assert.AreEqual(2, n.Value.Count);
        }

        [Test]
        public void TestCreateInsertDiff()
        {
            var p = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");

            var n = (MultiLineStringDiff)Differ.CreateDiff(null, p);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Insert, n.Operation);
            Assert.AreEqual(2, n.Value.Count);
        }

        [Test]
        public void TestApplyModify()
        {
            var p = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");
            var diff = new MultiLineStringDiff()
            {
                Operation = Operation.Modify,
                Index = 0,
                Value = new List<LineStringDiff>()
                {
                    new LineStringDiff()
                    {
                        Index = 0,
                        Operation = Operation.Modify,
                        Value = new List<PointDiff>()
                        {
                            new PointDiff() {Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 0, Y = 0}},
                            new PointDiff(){Index = 0, Operation = Operation.Modify, Value = new CoordinateDelta(){X = 0.5, Y = 0.5}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 4, Y = 4}}
                        }
                    },
                    new LineStringDiff()
                    {
                        Index = 1,
                        Operation = Operation.Delete,
                        Value = new List<PointDiff>()
                        {
                            new PointDiff() {Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y = 10}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 20, Y = 20}}
                        }
                    },
                    new LineStringDiff()
                    {
                        Index = 1,
                        Operation = Operation.Insert,
                        Value = new List<PointDiff>()
                        {
                            new PointDiff() {Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 4, Y = 5}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 6, Y = 7}}
                        }
                    }
                }
            };
            var n = diff.Apply(p);
            var p2 = ReadWkt<MultiLineString>("MULTILINESTRING((0 0, 1.5 1.5, 2 2, 3 3), (4 5, 6 7))");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }



        [Test]
        public void TestApplyDelete()
        {
            var p = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");
            var diff = new MultiLineStringDiff()
            {
                Operation = Operation.Delete,
                Index = 0,
                Value = new List<LineStringDiff>()
                {
                    new LineStringDiff()
                    {
                        Index = 0,
                        Operation = Operation.Delete,
                        Value = new List<PointDiff>()
                        {
                            new PointDiff() {Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 0, Y = 0}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 0.5, Y = 0.5}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 4, Y = 4}}
                        }
                    },
                    new LineStringDiff()
                    {
                        Index = 1,
                        Operation = Operation.Delete,
                        Value = new List<PointDiff>()
                        {
                            new PointDiff() {Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y = 10}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 20, Y = 20}}
                        }
                    }
                }
            };
            var n = diff.Apply(p);
            Assert.IsNull(n);
        }

        [Test]
        public void TestApplyCreate()
        {
            MultiLineString p = null;
            var diff = new MultiLineStringDiff()
            {
                Operation = Operation.Insert,
                Index = 0,
                Value = new List<LineStringDiff>()
                {
                    new LineStringDiff()
                    {
                        Index = 0,
                        Operation = Operation.Insert,
                        Value = new List<PointDiff>()
                        {
                            new PointDiff() {Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y = 1}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 2, Y = 2}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 3, Y = 3}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 4, Y = 4}}
                        }
                    },
                    new LineStringDiff()
                    {
                        Index = 1,
                        Operation = Operation.Insert,
                        Value = new List<PointDiff>()
                        {
                            new PointDiff() {Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 10, Y = 10}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 20, Y = 20}}
                        }
                    }
                }
            };


            var n = diff.Apply(p);

            var p2 = ReadWkt<MultiLineString>("MULTILINESTRING((1 1, 2 2, 3 3, 4 4), (10 10, 20 20))");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }
    }
}
