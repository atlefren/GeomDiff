using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public static class CollectionDiff
    {
        public static IGeometry[] Patch(IGeometry geom, List<IDiff> diffs, Func<List<IGeometry>, List<IDiff>, List<IGeometry>> patchList)
        {
            if (geom == null)
            {
                return patchList(new List<IGeometry>(), diffs).ToArray();
            }

            var existingElements = ((GeometryCollection) geom).Geometries.ToList();
            return patchList(existingElements, diffs).ToArray();
        }
    }
}
