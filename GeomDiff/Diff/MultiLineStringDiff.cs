using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class MultiLineStringDiff : BaseListDiff<LineStringDiff>
    {
        public override string GeometryType { get; } = "MultiLineString";

        public override bool HasZ()
            => Value.Any(v => v.HasZ());

        public override IDiff Reverse(int? index = null) 
            => ReverseListDiff<MultiLineStringDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geometry)
            => new MultiLineString(CastArray<ILineString>(CollectionDiff.Patch(geometry, GetDiffs(), PatchList)));
        
    }
}
