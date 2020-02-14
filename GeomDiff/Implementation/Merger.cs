using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Models;
using GeomDiff.Models.Enums;

namespace GeomDiff.Implementation
{
    public class Merger
    {

        private readonly Func<IGeometry, IGeometry, bool> _areEqual;
        private readonly Func<IGeometry, IGeometry, double> _getDistance;

        public Merger(Func<IGeometry, IGeometry, bool> areEqual, Func<IGeometry, IGeometry, double> getDistance)
        {
            _areEqual = areEqual;
            _getDistance = getDistance;
        }

        public List<Change> MergeDiffs(List<Change> diffs)
        {

            var deletes = new List<Change>();
            var inserts = new List<Change>();
            var res = new List<Change>();

            foreach (var diff in Reorder(diffs))
            {
                if (diff.Operation == Operation.Delete)
                {
                    deletes.Add(diff);
                }
                else if (diff.Operation == Operation.Insert)
                {
                    inserts.Add(diff);
                    if (deletes.Count > 0 && diff.Index > deletes.Last().Index + 1)
                    {
                        res.AddRange(Merge(deletes, inserts));
                        deletes = new List<Change>();
                        inserts = new List<Change>();
                    }
                }
                else
                {
                    res.AddRange(Merge(deletes, inserts));
                    deletes = new List<Change>();
                    inserts = new List<Change>();
                    res.Add(diff);

                }

            }

            res.AddRange(Merge(deletes, inserts));
            return RemoveNoops(res).ToList();
        }


        private IEnumerable<Change> Merge(IList<Change> deletes,
            IList<Change> inserts)
        {
            var result = new List<Change>();


            if (deletes.Count < inserts.Count)
            {
                result.AddRange(MergeByDistance(deletes, inserts, true));
            }
            else if (inserts.Count < deletes.Count)
            {
                result.AddRange(MergeByDistance(inserts, deletes, false));
            }
            else
            {
                result.AddRange(deletes.Zip(inserts, (del, ins) => new {Delete = del, Insert = ins}).Select(pair => CreateModifyOperation(pair.Insert, pair.Delete)));
            }

            return result;
        }

        private IEnumerable<Change> MergeByDistance(ICollection<Change> minority,
            IList<Change> majority, bool fixIdx)
        {
            if (minority.Count > 1)
            {
                var res = new List<Change>();
                foreach (var minor in minority)
                {
                    var major = majority[0];
                    majority.RemoveAt(0);
                    res.Add(CreateModifyOperation(major, minor));
                }

                return res.Concat(majority);
                
            }


            var merged = new List<Change>(majority);
            var a = new List<Change>();
            foreach (var minor in minority)
            {
                if (merged.Count > 0)
                {
                    var idx = GetIndexOfClosest(minor, merged);
                    var major = majority[idx];
                    merged[idx] = CreateModifyOperation(major, minor);
                    if (fixIdx)
                    {
                        for (var i = 0; i < idx; i++)
                        {
                            merged[i].Index = merged[idx].Index;
                        }
                    }

                }
                else
                {
                    a.Add(minor);
                }
            }
            return a.Concat(merged);
            
        }

        private int GetIndexOfClosest(Change diff, IList<Change> diffs)
        {
            var idx = -1;
            var dist = double.PositiveInfinity;

            for (var i = 0; i < diffs.Count(); i++)
            {
                if (diff.Operation == Operation.Modify)
                {
                    continue;
                }

                var newDist = _getDistance(diff.GetValue(), diffs[i].GetValue());
                if (newDist < dist)
                {
                    dist = newDist;
                    idx = i;
                }
            }

            return idx;
        }

        private static Change CreateModifyOperation(Change a, Change b)
        {

            var delete = (a.Operation == Operation.Delete) ? a : b;
            var insert = (a.Operation == Operation.Insert) ? a : b;

            return new Change()
            {
                Index = delete.Index,
                Operation = Operation.Modify,
                PreviousValue = delete.PreviousValue,
                NewValue = insert.NewValue
            };
        }

        private static IEnumerable<Change> RemoveNoops(IEnumerable<Change> diffs)
            => diffs.Where(d => !d.Operation.Equals(Operation.Noop));
        

        private IEnumerable<Change> Reorder(IReadOnlyList<Change> diffs)
        {
            var filtered = new List<Change>();
            var j = 0;
            while (j < diffs.Count)
            {
                var current = diffs[j];
                if (j == diffs.Count - 1)
                {
                    filtered.Add(current);
                    break;
                }

                var next = diffs[j + 1];
                if (current.Operation == Operation.Noop && next.Operation == Operation.Insert &&
                    _areEqual(current.PreviousValue, next.NewValue))
                {
                    filtered.Add(next);
                    filtered.Add(current);
                    j += 2;
                }
                else if (current.Operation == Operation.Noop && next.Operation == Operation.Delete &&
                         _areEqual(current.PreviousValue, next.PreviousValue) && next.Index == current.Index + 1)
                {
                    next.Index -= 1;
                    current.Index += 1;
                    filtered.Add(next);
                    filtered.Add(current);
                    j += 2;

                }
                else
                {
                    filtered.Add(current);
                    j++;
                }
            }

            return filtered;
        }
    }
}

