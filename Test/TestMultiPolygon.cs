using System.Collections.Generic;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using NetTopologySuite.Geometries;

using NUnit.Framework;

namespace Test
{

    [TestFixture]
    public class TestMultiPolygon : TestBase
    {
        [Test]
        public void TestModify()
        {
            var p1 = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");
            var p2 = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 0.5 1.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5)), ((30 30, 30 40, 40 40, 40 30, 30 30), (35 35, 35 36, 36 36, 36 35, 35 35)))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestInsert()
        {
            MultiPolygon p1 = null;
            var p2 = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 0.5 1.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5)), ((30 30, 30 40, 40 40, 40 30, 30 30), (35 35, 35 36, 36 36, 36 35, 35 35)))");
            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.AreEqual(WriteWkt(p2), WriteWkt(patched));

            var unpatched = diff.Undo(p2);
            Assert.IsNull(unpatched);
        }

        [Test]
        public void TestDelete()
        {
            var p1 = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");
            MultiPolygon p2 = null;

            var diff = Differ.CreateDiff(p1, p2);

            var patched = diff.Apply(p1);
            Assert.IsNull(patched);


            var unpatched = diff.Undo(p2);
            Assert.AreEqual(WriteWkt(p1), WriteWkt(unpatched));
        }

        [Test]
        public void TestCreateModifyDiff()
        {
            var p = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");
            var p2 = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 0.5 1.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5)), ((30 30, 30 40, 40 40, 40 30, 30 30), (35 35, 35 36, 36 36, 36 35, 35 35)))");
            var n = (MultiPolygonDiff)Differ.CreateDiff(p, p2);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Modify, n.Operation);
            Assert.AreEqual(2, n.Value.Count);

        }

        [Test]
        public void TestCreateDeleteDiff()
        {
            var p = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");

            var n = (MultiPolygonDiff)Differ.CreateDiff(p, null);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Delete, n.Operation);
            Assert.AreEqual(2, n.Value.Count);
        }

        [Test]
        public void TestCreateInsertDiff()
        {
            var p = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");

            var n = (MultiPolygonDiff)Differ.CreateDiff(null, p);

            Assert.AreEqual(0, n.Index);
            Assert.AreEqual(Operation.Insert, n.Operation);
            Assert.AreEqual(2, n.Value.Count);
        }

        [Test]
        public void TestApplyModify()
        {
            var p = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");
            var diff = new MultiPolygonDiff()
            {
                Operation = Operation.Modify,
                Index = 0,
                Value = new List<PolygonDiff>()
                {
                    new PolygonDiff() {Index = 0, Operation = Operation.Modify, Value = new List<RingDiff>()
                    {
                        new RingDiff(){Index = 0, Operation = Operation.Modify, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y=1}},
                            new PointDiff(){Index = 1, Operation = Operation.Modify, Value = new CoordinateDelta(){X = -0.5, Y=0.5}},

                        }},
                        new RingDiff(){Index = 1, Operation = Operation.Delete, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y=3}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 4, Y=4}},
                            new PointDiff(){Index = 2, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 5, Y=5}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y=3}}
                        }}
                    }},
                    new PolygonDiff() {Index = 1, Operation = Operation.Delete, Value = new List<RingDiff>()
                    {
                        new RingDiff(){Index = 0, Operation = Operation.Delete, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y=10}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y=20}},
                            new PointDiff(){Index = 2, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 20, Y=20}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 20, Y=10}},
                            new PointDiff(){Index = 4, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y=10}}
                        }}
                    }},
                    new PolygonDiff() {Index = 1, Operation = Operation.Insert, Value = new List<RingDiff>()
                    {
                        new RingDiff(){Index = 0, Operation = Operation.Insert, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 30, Y=30}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 30, Y=40}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 40, Y=40}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 40, Y=30}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 30, Y=30}}
                        }},
                        new RingDiff(){Index = 1, Operation = Operation.Insert, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 35, Y=35}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 35, Y=36}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 36, Y=36}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 36, Y=35}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 35, Y=35}}
                        }}
                    }},
                }
            };



            var n = diff.Apply(p);

            var p2 = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 0.5 1.5, 2 2, 0 0), (5 5, 6 6, 7 7, 5 5)), ((30 30, 30 40, 40 40, 40 30, 30 30), (35 35, 35 36, 36 36, 36 35, 35 35)))");
            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));
        }



        [Test]
        public void TestApplyDelete()
        {
            var p = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");
            var diff = new MultiPolygonDiff()
            {
                Operation = Operation.Delete,
                Index = 0,
                Value = new List<PolygonDiff>()
                {
                    new PolygonDiff() {Index = 0, Operation = Operation.Delete, Value = new List<RingDiff>()
                    {
                        new RingDiff(){Index = 0, Operation = Operation.Delete, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 0, Y=0}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 1, Y=1}},
                            new PointDiff(){Index = 2, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 2, Y=2}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 0, Y=0}},

                        }},
                        new RingDiff(){Index = 1, Operation = Operation.Delete, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y=3}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 4, Y=4}},
                            new PointDiff(){Index = 2, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 5, Y=5}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 3, Y=3}}
                        }},
                        new RingDiff(){Index = 2, Operation = Operation.Delete, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 5, Y=5}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 6, Y=6}},
                            new PointDiff(){Index = 2, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 7, Y=7}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 5, Y=5}}
                        }}
                    }},
                    new PolygonDiff() {Index = 1, Operation = Operation.Delete, Value = new List<RingDiff>()
                    {
                        new RingDiff(){Index = 0, Operation = Operation.Delete, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y=10}},
                            new PointDiff(){Index = 1, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y=20}},
                            new PointDiff(){Index = 2, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 20, Y=20}},
                            new PointDiff(){Index = 3, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 20, Y=10}},
                            new PointDiff(){Index = 4, Operation = Operation.Delete, Value = new CoordinateDelta(){X = 10, Y=10}}
                        }}
                    }}
                }
            };
            var n = diff.Apply(p);
            Assert.IsNull(n);
        }

        [Test]
        public void TestApplyCreate()
        {
            MultiPolygon p = null;
            var diff = new MultiPolygonDiff()
            {
                Operation = Operation.Insert,
                Index = 0,
                Value = new List<PolygonDiff>()
                {
                    new PolygonDiff() {Index = 0, Operation = Operation.Insert, Value = new List<RingDiff>()
                    {
                        new RingDiff(){Index = 0, Operation = Operation.Insert, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 0, Y=0}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 1, Y=1}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 2, Y=2}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 0, Y=0}},

                        }},
                        new RingDiff(){Index = 1, Operation = Operation.Insert, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 3, Y=3}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 4, Y=4}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 5, Y=5}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 3, Y=3}}
                        }},
                        new RingDiff(){Index = 2, Operation = Operation.Insert, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 5, Y=5}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 6, Y=6}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 7, Y=7}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 5, Y=5}}
                        }}
                    }},
                    new PolygonDiff() {Index = 1, Operation = Operation.Insert, Value = new List<RingDiff>()
                    {
                        new RingDiff(){Index = 0, Operation = Operation.Insert, Value = new List<PointDiff>()
                        {
                            new PointDiff(){Index = 0, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 10, Y=10}},
                            new PointDiff(){Index = 1, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 10, Y=20}},
                            new PointDiff(){Index = 2, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 20, Y=20}},
                            new PointDiff(){Index = 3, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 20, Y=10}},
                            new PointDiff(){Index = 4, Operation = Operation.Insert, Value = new CoordinateDelta(){X = 10, Y=10}}
                        }}
                    }}
                }
            };

            var n = diff.Apply(p);
            var p2 = ReadWkt<MultiPolygon>("MULTIPOLYGON(((0 0, 1 1, 2 2, 0 0), (3 3, 4 4, 5 5, 3 3), (5 5, 6 6, 7 7, 5 5)), ((10 10, 10 20, 20 20, 20 10, 10 10)))");

            Assert.AreEqual(WriteWkt(p2), WriteWkt(n));

        }
    }

}
