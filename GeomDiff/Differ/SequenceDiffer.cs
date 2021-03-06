﻿using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Implementation;
using GeomDiff.Models;
using GeomDiff.Models.Enums;

namespace GeomDiff.Differ
{
    public abstract class SequenceDiffer
    {
        private readonly MyersDiff<IGeometry> _differ = new MyersDiff<IGeometry>(AreEqual, GetDistance);

        protected abstract IDiffer ComponentDiffer { get; set; }

        protected abstract List<IGeometry> ToComponents(IGeometry geometry);

        protected List<TDiff> GetValue<TDiff>(Change change)
            => ToDiffs(GetChanges(change)).Cast<TDiff>().ToList();

        protected List<IGeometry> GetGeometries(IGeometry geometry)
            => ((IGeometryCollection) geometry).Geometries.ToList();

        private static bool AreEqual(IGeometry geometry1, IGeometry geometry2)
        {
            var c1 = geometry1.Coordinates.ToList();
            var c2 = geometry2.Coordinates.ToList();

            return c1.Count == c2.Count && c1.Zip(c2, (a, b) => new {A = a, B = b}).All(pair => pair.A.Equals3D(pair.B));
        } 
        
        private static double GetDistance(IGeometry geometry1, IGeometry geometry2) 
            => geometry1.Distance(geometry2);

        private IEnumerable<IDiff> ToDiffs(IEnumerable<Change> changes)
            => changes.Select(change => ComponentDiffer.CreateDiff(change));

        private IEnumerable<Change> GetChanges(Change change)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (change.Operation)
            {
                case Operation.Modify:
                    return _differ.GetChanges(
                        ToComponents(change.PreviousValue),
                        ToComponents(change.NewValue));
                case Operation.Delete:

                    return ToComponents(change.PreviousValue)
                        .Select((p, index) => new Change()
                        {
                            Index = index,
                            Operation = Operation.Delete,
                            PreviousValue = p,
                            NewValue = null
                        });


                case Operation.Insert:
                    return ToComponents(change.NewValue)
                        .Select((p, index) => new Change()
                        {
                            Index = index,
                            Operation = Operation.Insert,
                            PreviousValue = null,
                            NewValue = p
                        });
            }
            return null;
        }
    }
}
