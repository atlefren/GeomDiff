using System.Collections.Generic;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;

namespace GeomDiff.Differ
{
    public class MultiPolygonDiffer : SequenceDiffer, IDiffer
    {
        protected override IDiffer ComponentDiffer { get; set; } = new PolygonDiffer();
        public IDiff CreateDiff(Change change) =>
            new MultiPolygonDiff()
            {
                Operation = change.Operation,
                Index = change.Index,
                Value = GetValue<PolygonDiff>(change)
            };

        protected override List<IGeometry> ToComponents(IGeometry geometry) 
            => GetGeometries(geometry);
    }
}
