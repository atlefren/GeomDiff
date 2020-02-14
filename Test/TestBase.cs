using GeoAPI.Geometries;
using GeomDiff.Differ;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Test
{
    public class TestBase
    {
        protected readonly Differ Differ = new Differ();

        public static TGeomType ReadWkt<TGeomType>(string wkt)
            where TGeomType : Geometry
            => (TGeomType)ReadWkt(wkt);

        public static IGeometry ReadWkt(string wkt)
            => wkt != null ? GetReader().Read(wkt) : null;

        public static string WriteWkt(IGeometry geom, int dimension = 2)
            => geom != null ? GetWriter(dimension).Write(geom) : null;


        private static WKTWriter GetWriter(int dimension) => new WKTWriter(dimension) { };
        private static WKTReader GetReader() => new WKTReader { DefaultSRID = 4326 };

    }
}
