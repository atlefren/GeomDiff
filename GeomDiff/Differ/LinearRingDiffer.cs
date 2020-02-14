using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;
using NetTopologySuite.Geometries;

namespace GeomDiff.Differ
{
    public class LinearRingDiffer : SequenceDiffer, IDiffer
    {
        protected override IDiffer ComponentDiffer { get; set; } = new PointDiffer();
        protected override List<IGeometry> ToComponents(IGeometry geometry)
            => geometry.Coordinates.Select(c => new Point(c)).Cast<IGeometry>().ToList();

        public IDiff CreateDiff(Change change) => new RingDiff()
        {
            Operation = change.Operation,
            Index = change.Index,
            Value = GetValue<PointDiff>(change)
        };
    }
}
