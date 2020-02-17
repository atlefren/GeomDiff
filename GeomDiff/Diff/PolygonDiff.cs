using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class PolygonDiff : BaseListDiff<RingDiff>
    {
        public override string GeometryType { get; } = "Polygon";

        private static List<ILinearRing> GetRings(Polygon polygon)
            => polygon == null 
                ? new List<ILinearRing>() 
                : new List<ILinearRing>() {polygon.Shell}.Concat(polygon.Holes).ToList();
        

        public override IDiff Reverse(int? index = null)
            => ReverseListDiff<PolygonDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geometry)
        {
            var patched = PatchList(GetRings((Polygon) geometry));

            return new Polygon(patched[0], patched.Skip(1).ToArray());
        }
    }
}
