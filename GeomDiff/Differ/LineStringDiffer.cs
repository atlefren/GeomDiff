using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;
using NetTopologySuite.Geometries;

namespace GeomDiff.Differ
{

    public class LineStringDiffer: SequenceDiffer, IDiffer
    {
        protected override IDiffer ComponentDiffer { get; set; } = new PointDiffer();
        
        public IDiff CreateDiff(Change change) => 
            new LineStringDiff()
            {
                Operation = change.Operation,
                Index = change.Index,
                Value = GetValue<PointDiff>(change)
            };

        protected override List<IGeometry> ToComponents(IGeometry geometry) 
            => geometry.Coordinates.Select(c => new Point(c)).Cast<IGeometry>().ToList();
    }
}
