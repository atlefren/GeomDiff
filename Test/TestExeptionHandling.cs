using GeomDiff.Models.Excepions;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class TestExeptionHandling: TestBase
    {
        [Test]
        public void TestCreateDiffFromDifferentGeoms()
        {
            var geom1 = ReadWkt("POINT(1 1)");
            var geom2 = ReadWkt("LINESTRING(1 1, 2 2, 3 3)");

            var ex = Assert.Throws<GeometryTypeException>(delegate { Differ.CreateDiff(geom1, geom2); });
            Assert.AreEqual("Cannot create patch from Point to LineString!", ex.Message);

        }

        [Test]
        public void TestPassWrongTypeOfGeometry()
        {
            var point1 = ReadWkt("POINT(1 1)");
            var point2 = ReadWkt("POINT(1 1)");

            var diff = Differ.CreateDiff(point1, point2);

            var line = ReadWkt("LINESTRING(1 1, 2 2, 3 3)");


            var ex = Assert.Throws<GeometryTypeException>(delegate { diff.Apply(line); });
            Assert.AreEqual("Expected Point, got LineString!", ex.Message);

        }

        [Test]
        public void TestCreateDiffFromUnsupportedGeometry()
        {
            var gc1 = ReadWkt("GEOMETRYCOLLECTION (POINT (40 10))");
            var gc2 = ReadWkt("GEOMETRYCOLLECTION (POINT (40 11))");


            var ex = Assert.Throws<GeometryTypeException>(delegate { Differ.CreateDiff(gc1, gc2); });
            Assert.AreEqual("Geometry type not supported: GeometryCollection", ex.Message);

        }

    }
}
