using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;

namespace GeomDiff.Differ
{
    public class PointDiffer: IDiffer
    {
        private static Coordinate GetCoordinate(IGeometry p) 
            => p?.Coordinate;

        public IDiff CreateDiff(Change change)
        {
            return new PointDiff()
            {
                Operation = change.Operation,
                Index = change.Index,
                Value = CoordinateDelta.Create(GetCoordinate(change.PreviousValue), GetCoordinate(change.NewValue))
            };
        }
    }
}
