using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class LineStringDiff : BaseListDiff<PointDiff>
    {
        public override string GeometryType { get; } = "LineString";

        public override IDiff Reverse(int? index = null)
            => ReverseListDiff<LineStringDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geom)
        {
            var existingElements = geom != null ? GetPoints(geom) : new List<Point>();
            var diffs = GetDiffs();
            var newElements = PatchList(CastList<IGeometry>(existingElements), diffs);

            return new LineString(ToCoordinates(newElements));
        }
    }
}
