using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Models;
using GeomDiff.Models.Enums;

namespace GeomDiff.Implementation
{
   
    public class MyersDiff<TComponent>
    where TComponent: IGeometry
    {
        private readonly Func<IGeometry, IGeometry, bool> _areEqual;
        
        private readonly Merger _merger;

        public MyersDiff(Func<IGeometry, IGeometry, bool> areEqual, Func<IGeometry, IGeometry, double> getDistance)
        {
            _areEqual = areEqual;
            _merger = new Merger(areEqual, getDistance);
        }

        public List<Change> GetChanges(List<TComponent> t1, List<TComponent> t2)
        {

            var n = t1.Count;
            var m = t2.Count;

            var max = m + n;
            var offset = max + 1;

            var v = Enumerable.Repeat(0, 2 * max + 1).ToList();

            var patches = Enumerable.Repeat(new List<Change>(), 2 * max + 1).ToList();

            for (var d = 0; d < max + 1; d++)
            {
                for (var k = -d; k < d + 1; k += 2)
                {
                    int x;
                    int y;
                    List<Change> patch;
                    if (k == -d || (k != d && v[k - 1 + offset] < v[k + 1 + offset]))
                    {
                        x = v[k + 1 + offset];
                        y = x - k;
                        patch = new List<Change>(patches[k + 1 + offset]);
                        if (y <= t2.Count && y > 0)
                        {
                            patch.Add(new Change()
                            {
                                Operation = Operation.Insert,
                                NewValue = t2[y - 1],
                                Index = x
                            });
                        }
                    }
                    else
                    {
                        x = v[k - 1 + offset] + 1;
                        patch = new List<Change>(patches[k - 1 + offset]);
                        if (x <= t1.Count && x > 0)
                        {
                            patch.Add(new Change()
                            {
                                Operation = Operation.Delete,
                                PreviousValue = t1[x - 1],
                                Index = x - 1
                            });

                        }
                    }

                    y = x - k;
                    while (x < n && y < m && _areEqual(t1[x], t2[y]))
                    {
                        patch.Add(new Change()
                        {
                            Operation = Operation.Noop,
                            Index = x,
                            PreviousValue = t1[x]

                        });

                        x += 1;
                        y += 1;

                    }
                    v[k + offset] = x;
                    patches[k + offset] = patch;
                    // ReSharper disable once InvertIf
                    if (x >= n && y >= m)
                    {
                        return _merger.MergeDiffs(patch);
                    }

                }
            }

            return null;

        }

    }
}
