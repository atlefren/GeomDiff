using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class MultiPointDiff : BaseListDiff<PointDiff>
    {
        public override string GeometryType { get; } = "MultiPoint";

        public override IDiff Reverse(int? index = null)
            => ReverseListDiff<MultiPointDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geometry)
            => new MultiPoint(PatchMulti<IPoint>(geometry));
        
    }
}
