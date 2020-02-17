using System;
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

        protected static List<Point> GetPoints(IGeometry geometry)
            => geometry.Coordinates.Select(c => new Point(c)).ToList();

        protected static Coordinate[] ToCoordinates(List<Point> points)
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
                reversed.Add((TDiffedComponent) diff.Reverse(index));
            }

            return reversed;
        }

        protected List<TGeometry> PatchList<TGeometry>(List<TGeometry> geometries)
            where TGeometry : IGeometry
        {
            var diffs = Value.Cast<IDiff>().ToList();
            var existingElements = geometries.Cast<IGeometry>().ToList();
            if (diffs.Count == 0)
            {
                return existingElements.Cast<TGeometry>().ToList();
            }

            var patched = new List<IGeometry>();

            var numElements = Math.Max(existingElements.Count - 1, diffs.Max(v => v.Index));

            for (var index = 0; index <= numElements; index++)
            {
                var inserts = Util.GetDiffs(index, Operation.Insert, diffs);
                patched.AddRange(inserts.Select(insert => insert.Apply(null)));


                var delete = Util.GetDiff(index, Operation.Delete, diffs);
                if (delete != null)
                {
                    continue;
                }
                var element = Util.GetAt(index, existingElements);
                if (element == null)
                {
                    continue;
                }
                var modify = Util.GetDiff(index, Operation.Modify, diffs);
                patched.Add(modify != null ? modify.Apply(element) : element);
            }

            return patched.Cast<TGeometry>().ToList();
        }

        protected TOutput[] PatchMulti<TOutput>(IGeometry geometry)
            where TOutput : IGeometry
            => PatchList(
                geometry == null
                    ? new List<IGeometry>()
                    : ((GeometryCollection) geometry).Geometries.ToList()
               ).Cast<TOutput>().ToArray();

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
