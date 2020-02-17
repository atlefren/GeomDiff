using System.Collections.Generic;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using GeomDiff.Models.Exceptions;

namespace GeomDiff.Differ
{
    public interface IDiffer
    {
        IDiff CreateDiff(Change change);
    }

    public class Differ
    {
        private readonly Dictionary<string, IDiffer> _differs = new Dictionary<string, IDiffer>()
        {
            {"Point",  new PointDiffer()},
            {"LineString", new LineStringDiffer() },
            {"Polygon", new PolygonDiffer() },
            {"MultiPoint",  new MultiPointDiffer()},
            {"MultiLineString", new MultiLineStringDiffer() },
            {"MultiPolygon", new MultiPolygonDiffer() }
        };

        public static Operation GetOperation(IGeometry oldGeometry, IGeometry newGeometry)
        {
            if (oldGeometry != null && newGeometry != null)
            {
                return Operation.Modify;
            }

            return oldGeometry == null 
                ? Operation.Insert 
                : Operation.Delete;
        }


        public IDiff CreateDiff(IGeometry oldGeometry, IGeometry newGeometry)
        {
            if (oldGeometry == null && newGeometry == null)
            {
                return null;
            }

            if (!GeometriesEqual(oldGeometry, newGeometry))
            {
                throw new GeometryTypeException(oldGeometry.GeometryType, newGeometry.GeometryType);
            }

            var geometryType = GetGeomType(oldGeometry, newGeometry);

            _differs.TryGetValue(geometryType, out var differ);
            if (differ == null)
            {
                throw new GeometryTypeException($"Geometry type not supported: {geometryType}");
            }

            return differ.CreateDiff(new Change()
            {
                Index = 0,
                Operation = GetOperation(oldGeometry, newGeometry),
                PreviousValue = oldGeometry,
                NewValue = newGeometry
            });

        }

        private static bool GeometriesEqual(IGeometry geometry1, IGeometry geometry2)
            => geometry1 == null || geometry2 == null || geometry1.GeometryType == geometry2.GeometryType;


        private static string GetGeomType(IGeometry oldGeometry, IGeometry newGeometry)
        {
            if (oldGeometry != null)
            {
                return oldGeometry.GeometryType;
            }
            if (newGeometry != null)
            {
                return newGeometry.GeometryType;
            }
            return "none";
        }
    }
}
