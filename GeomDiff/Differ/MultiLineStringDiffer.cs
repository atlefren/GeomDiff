using System.Collections.Generic;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;

namespace GeomDiff.Differ
{
    public class MultiLineStringDiffer : SequenceDiffer, IDiffer
    {
        protected override IDiffer ComponentDiffer { get; set; } = new LineStringDiffer();

        public IDiff CreateDiff(Change change) =>
            new MultiLineStringDiff()
            {
                Operation = change.Operation,
                Index = change.Index,
                Value = GetValue<LineStringDiff>(change)
            };

        protected override List<IGeometry> ToComponents(IGeometry geometry)
            => GetGeometries(geometry);
    }
}
