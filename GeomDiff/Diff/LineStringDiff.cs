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

        protected override IGeometry ApplyPatch(IGeometry geometry)
        {
            var existingElements = geometry != null ? GetPoints(geometry) : new List<Point>();
            var newElements = PatchList(CastList<IGeometry>(existingElements), GetDiffs());

            return new LineString(ToCoordinates(newElements));
        }
    }
}
