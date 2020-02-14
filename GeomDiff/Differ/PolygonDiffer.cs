using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;

namespace GeomDiff.Differ
{
    public class PolygonDiffer : SequenceDiffer, IDiffer
    {
        protected override IDiffer ComponentDiffer { get; set; } = new LinearRingDiffer();

        protected override List<IGeometry> ToComponents(IGeometry geometry)
            => GetRings((IPolygon) geometry).Cast<IGeometry>().ToList();

        private static IEnumerable<ILinearRing> GetRings(IPolygon polygon)
            =>  new List<ILinearRing>() { polygon.Shell }.Concat(polygon.Holes.ToList());
        

        public IDiff CreateDiff(Change change) => 
            new PolygonDiff() {
                Operation = change.Operation,
                Index = change.Index,
                Value = GetValue<RingDiff>(change)
            };
    }
}
