using GeoAPI.Geometries;
using GeomDiff.Models;
using NetTopologySuite.Geometries;
using GeomDiff.Models.Enums;

namespace GeomDiff.Diff
{
    public class PointDiff : BaseDiff<CoordinateDelta>
    {
        public override string GeometryType { get; } = "Point";

        public override IDiff Reverse(int? index = null)
            => Copy<PointDiff>(Operation == Operation.Modify ? Value.Reverse() : Value, index);

        public override bool HasZ()
            => Value.Z.HasValue && !double.IsNaN(Value.Z.Value);

        protected override IGeometry ApplyPatch(IGeometry geometry)
            => new Point(Value.Patch(geometry == null ? GetEmptyCoordinate() : geometry.Coordinate));
        
        private static Coordinate GetEmptyCoordinate()
            => new Coordinate(0, 0,  0);

    }
}
