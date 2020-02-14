using System.Collections.Generic;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using GeomDiff.Models.Excepions;

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

            if (oldGeometry == null)
            {
                return Operation.Insert;
            }
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (newGeometry == null)
            {
                return Operation.Delete;
            }

            return Operation.Noop;
        }


        public IDiff CreateDiff(IGeometry oldGeom, IGeometry newGeom)
        {
            if (oldGeom == null && newGeom == null)
            {
                return null;
            }

            if (!GeometriesEqual(oldGeom, newGeom))
            {
                throw new GeometryTypeException(oldGeom.GeometryType, newGeom.GeometryType);
            }

            var geomType = GetGeomType(oldGeom, newGeom);

            _differs.TryGetValue(geomType, out var differ);
            if (differ == null)
            {
                throw new GeometryTypeException($"Geometry type not supported: {geomType}");
            }

            return differ.CreateDiff(new Change()
            {
                Index = 0,
                Operation = GetOperation(oldGeom, newGeom),
                PreviousValue = oldGeom,
                NewValue = newGeom
            });

        }

        private static bool GeometriesEqual(IGeometry geom1, IGeometry geom2)
            => geom1 == null || geom2 == null || geom1.GeometryType == geom2.GeometryType;


        private static string GetGeomType(IGeometry oldGeom, IGeometry newGeom)
        {
            if (oldGeom != null)
            {
                return oldGeom.GeometryType;
            }
            if (newGeom != null)
            {
                return newGeom.GeometryType;
            }
            return "none";
        }
    }
}
