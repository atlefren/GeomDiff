using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Models.Enums;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public abstract class BaseListDiff<TDiffedComponent> : BaseDiff<List<TDiffedComponent>>
    where TDiffedComponent: IDiff
    {
        public override bool HasZ()
            => Value.Any(v => v.HasZ());

        protected List<IDiff> GetDiffs() 
            => Value.Cast<IDiff>().ToList();

        protected static TGeometry[] CastArray<TGeometry>(IEnumerable<IGeometry> geometries)
            => geometries.Cast<TGeometry>().ToArray();

        protected static List<TGeometry> CastList<TGeometry>(IEnumerable<IGeometry> geometries)
            => geometries.Cast<TGeometry>().ToList();

        protected static List<Point> GetPoints(IGeometry geometry)
            => geometry.Coordinates.Select(c => new Point(c)).ToList();

        protected static Coordinate[] ToCoordinates(List<IGeometry> points)
            => points.Select(p => p.Coordinate).ToArray();

        protected TObj ReverseListDiff<TObj>(int? index = null)
            where TObj : IDiffWithValue<List<TDiffedComponent>>, new()
            => Copy<TObj>(ReverseDiffs(), index);
        
        protected List<TDiffedComponent> ReverseDiffs()
        {
            var offset = 0;
            var reversed = new List<TDiffedComponent>();
            foreach (var diff in Value)
            {
                var index = diff.Index + offset;
                offset += GetOffsetChange(diff.Operation);
                reversed.Add((TDiffedComponent)diff.Reverse(index));
            }

            return reversed;
        }

        private static int GetOffsetChange(Operation operation)
        {
            switch (operation)
            {
                case Operation.Insert:
                    return 1;
                case Operation.Delete:
                    return -1;
                default:
                    return 0;
            }
        }

    }
}
