using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class MultiPolygonDiff : BaseListDiff<PolygonDiff>
    {
        public override string GeometryType { get; } = "MultiPolygon";

        public override IDiff Reverse(int? index = null) 
            => ReverseListDiff<MultiPolygonDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geom)
            => new MultiPolygon(CastArray<IPolygon>(CollectionDiff.Patch(geom, GetDiffs(), PatchList)));
    }
}
