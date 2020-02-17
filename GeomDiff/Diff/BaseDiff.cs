using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Models.Enums;
using GeomDiff.Models.Exceptions;

namespace GeomDiff.Diff
{
    public interface IDiff
    {
        int Index { get; set; }
        Operation Operation { get; set; }
        string GeometryType { get; }
        IGeometry Apply(IGeometry geometry);
        IGeometry Undo(IGeometry geometry);
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

        public IGeometry Apply(IGeometry geometry)
        {
            if (Operation == Operation.Delete)
            {
                return null;
            }
            CheckGeomType(geometry);
            return ApplyPatch(geometry);
        }

        public IGeometry Undo(IGeometry geometry)
            => Reverse().Apply(geometry);
        
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

        protected abstract IGeometry ApplyPatch(IGeometry geometry);

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

    }
}
