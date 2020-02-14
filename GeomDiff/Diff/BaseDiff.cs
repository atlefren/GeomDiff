using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Models.Enums;
using GeomDiff.Models.Excepions;

namespace GeomDiff.Diff
{
    public interface IDiff
    {
        int Index { get; set; }
        Operation Operation { get; set; }
        string GeometryType { get; }
        IGeometry Apply(IGeometry geom);
        IGeometry Undo(IGeometry geom);
        IDiff Reverse(int? index = null);

        bool HasZ();

    }

    public interface IDiffWithValue<TDiffedComponent> : IDiff
    {
        TDiffedComponent Value { get; set; }
    }

    public abstract class BaseDiff<TDiffedComponent> : IDiffWithValue<TDiffedComponent>
    {
        public abstract string GeometryType { get; }
        public int Index { get; set; }
        public TDiffedComponent Value { get; set; }
        public Operation Operation { get; set; }
        
        public abstract IDiff Reverse(int? index = null);

        public abstract bool HasZ();

        public IGeometry Apply(IGeometry geom)
        {
            if (Operation == Operation.Delete)
            {
                return null;
            }
            CheckGeomType(geom);
            return ApplyPatch(geom);
        }

        public IGeometry Undo(IGeometry geom)
            => Reverse().Apply(geom);
        

        public void CheckGeomType(IGeometry geometry)
        {
            if (geometry != null && geometry.GeometryType != GeometryType)
            {
                throw new GeometryTypeException($"Expected {GeometryType}, got {geometry.GeometryType}!");
            }
        }

        protected TObj Copy<TObj>(TDiffedComponent value, int? index = null)
            where TObj : IDiffWithValue<TDiffedComponent>, new()
            => new TObj()
            {
                Operation = FlipOperation(),
                Index = index ?? Index,
                Value = value
            };

        protected abstract IGeometry ApplyPatch(IGeometry geom);

        protected List<IGeometry> PatchList(List<IGeometry> existingElements, List<IDiff> diffs)
        {
            var newElements = new List<IGeometry>();

            var max = Math.Max(existingElements.Count - 1, diffs.Max(v => v.Index));

            for (var index = 0; index <= max; index++)
            {
                var inserts = Util.GetDiffs(index, Operation.Insert, diffs);
                newElements.AddRange(inserts.Select(insert => insert.Apply(null)));


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

                newElements.Add(modify != null ? modify.Apply(element) : element);
            }

            return newElements;

        }

        protected Operation FlipOperation()
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (Operation)
            {
                case Operation.Delete:
                    return Operation.Insert;
                case Operation.Insert:
                    return Operation.Delete;
                default:
                    return Operation;
            }
        }

        public override string ToString()
        {
            return $"{Index}: {Operation.ToString()} = {Value.ToString()}";
        }

    }
}
