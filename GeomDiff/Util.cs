using System.Collections.Generic;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models.Enums;


namespace GeomDiff
{
    public static class Util
    {
        public static IEnumerable<IDiff> GetDiffs(int index, Operation operation, List<IDiff> diffs)
            => diffs.FindAll(d => d.Index == index && d.Operation == operation);

        public static IDiff GetDiff(int index, Operation operation, List<IDiff> diffs)
            => diffs.Find(d => d.Index == index && d.Operation == operation);

        public static TElement GetAt<TElement>(int index, IReadOnlyList<TElement> list)
            where TElement : IGeometry
            => index < list.Count ? list[index] : default;

        public static GeometryType GetGeometryType(string geometryType)
        {
            switch (geometryType)
            {
                case "Point":
                {
                    return GeometryType.Point;
                }
                case "LineString":
                {
                    return GeometryType.LineString;
                }
                case "Polygon":
                {
                    return GeometryType.Polygon;
                }
                case "MultiPoint":
                {
                    return GeometryType.MultiPoint;
                }
                case "MultiLineString":
                {
                    return GeometryType.MultiLineString;
                }
                case "MultiPolygon":
                {
                    return GeometryType.MultiPolygon;
                }
                default:
                    return GeometryType.Unknown;
            }
        }
    }

}
