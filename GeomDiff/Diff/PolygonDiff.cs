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
        {
            if (polygon == null)
            {
                return new List<LinearRing>();
            }
            var shell = (LinearRing) polygon.Shell;
            var holes = CastList<LinearRing>(polygon.Holes);
            
            return new List<LinearRing>() { shell }.Concat(holes);
        }


        public override IDiff Reverse(int? index = null)
            => ReverseListDiff<PolygonDiff>(index);


        protected override IGeometry ApplyPatch(IGeometry geom)
        {
            var existingElements = CastList<IGeometry>(GetRings((Polygon) geom));

            var diffs = Value.Cast<IDiff>().ToList();
            var newElements = PatchList(existingElements, diffs);

            var shell = (LinearRing) newElements[0];
            var holes = CastArray<ILinearRing>(newElements.Skip(1));

            return new Polygon(shell, holes);
        }
    }
}
