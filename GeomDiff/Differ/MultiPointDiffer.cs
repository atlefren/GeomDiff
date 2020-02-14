using System.Collections.Generic;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;

namespace GeomDiff.Differ
{
    public class MultiPointDiffer : SequenceDiffer, IDiffer
    {
        protected override IDiffer ComponentDiffer { get; set; } = new PointDiffer();

        public IDiff CreateDiff(Change change) =>
            new MultiPointDiff()
            {
                Operation = change.Operation,
                Index = change.Index,
                Value = GetValue<PointDiff>(change)
            };

        
        protected override List<IGeometry> ToComponents(IGeometry geometry) 
            => GetGeometries(geometry);
    }
}
