using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class RingDiff : BaseListDiff<PointDiff>
    {
        public override string GeometryType { get; } = "LinearRing";

        public override IDiff Reverse(int? index = null) 
            => ReverseListDiff<RingDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geometry)
        {
            var existingElements = geometry != null ? GetPoints(geometry) : new List<Point>();
            return new LinearRing(ToCoordinates(PatchList(existingElements)));
        }
    }
}
