using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class PolygonDiff : BaseListDiff<RingDiff>
    {
        public override string GeometryType { get; } = "Polygon";

        private static IEnumerable<LinearRing> GetRings(Polygon polygon)
            => polygon == null 
                ? new List<LinearRing>() 
                : new List<LinearRing>() { (LinearRing) polygon.Shell}.Concat(CastList<LinearRing>(polygon.Holes));
        

        public override IDiff Reverse(int? index = null)
            => ReverseListDiff<PolygonDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geometry)
        {
            var existingElements = CastList<IGeometry>(GetRings((Polygon) geometry));

            var newElements = PatchList(existingElements, GetDiffs());

            var shell = (LinearRing) newElements[0];
            var holes = CastArray<ILinearRing>(newElements.Skip(1));

            return new Polygon(shell, holes);
        }
    }
}
