using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public static class CollectionDiff
    {
        public static IGeometry[] Patch(IGeometry geometry, List<IDiff> diffs,
            Func<List<IGeometry>, List<IDiff>, List<IGeometry>> patchList)
            => patchList(
                geometry == null
                    ? new List<IGeometry>()
                    : ((GeometryCollection) geometry).Geometries.ToList(),
                diffs).ToArray();
    }
}
