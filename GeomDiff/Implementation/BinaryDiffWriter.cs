using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using GeomDiff.Models.Exceptions;

namespace GeomDiff.Implementation
{
    public static class BinaryDiffWriter
    {
        public static byte[] Write(IDiff diff)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            WriteHeader(diff, writer);
            Write(diff, writer, diff.HasZ());
            writer.Flush();
            stream.Flush();
            return stream.ToArray();
        }

        private static void WriteHeader(IDiff diff, BinaryWriter writer)
        {
            WriteUint((int) Util.GetGeometryType(diff.GeometryType), writer);
            writer.Write(diff.HasZ());
        }

        private static void Write(IDiff diff, BinaryWriter writer, bool hasZ)
        {
            WriteUint((int) diff.Operation, writer);

            switch (diff)
            {
                case PointDiff pointDiff:
                    Write(pointDiff, writer, hasZ);
                    break;
                case LineStringDiff lineStringDiff:
                    Write(lineStringDiff, writer, hasZ);
                    break;
                case RingDiff ringDiff:
                    Write(ringDiff, writer, hasZ);
                    break;
                case PolygonDiff polygonDiff:
                    Write(polygonDiff, writer, hasZ);
                    break;
                case MultiPointDiff multiPointDiff:
                    Write(multiPointDiff, writer, hasZ);
                    break;
                case MultiLineStringDiff multiLineStringDiff:
                    Write(multiLineStringDiff, writer, hasZ);
                    break;
                case MultiPolygonDiff multiPolygonDiff:
                    Write(multiPolygonDiff, writer, hasZ);
                    break;
                default:
                    throw new GeometryTypeException($"Geometry type not supported: {diff.GeometryType}");
            }
        }

        private static void Write(PointDiff diff, BinaryWriter writer, bool hasZ)
            => WriteCoordinate(diff.Value, writer, hasZ);

        private static void Write(LineStringDiff diff, BinaryWriter writer, bool hasZ)
            => WriteList(diff.Value.Cast<IDiff>().ToList(), writer, hasZ);

        private static void Write(RingDiff diff, BinaryWriter writer, bool hasZ)
            => WriteList(diff.Value.Cast<IDiff>().ToList(), writer, hasZ);

        private static void Write(PolygonDiff diff, BinaryWriter writer, bool hasZ)
            => WriteList(diff.Value.Cast<IDiff>().ToList(), writer, hasZ);

        private static void Write(MultiPointDiff diff, BinaryWriter writer, bool hasZ)
            => WriteList(diff.Value.Cast<IDiff>().ToList(), writer, hasZ);

        private static void Write(MultiLineStringDiff diff, BinaryWriter writer, bool hasZ)
            => WriteList(diff.Value.Cast<IDiff>().ToList(), writer, hasZ);

        private static void Write(MultiPolygonDiff diff, BinaryWriter writer, bool hasZ)
            => WriteList(diff.Value.Cast<IDiff>().ToList(), writer, hasZ);

        private static void WriteCoordinate(CoordinateDelta coordinate, BinaryWriter writer, bool hasZ)
        {
            writer.Write(coordinate.X ?? double.NaN);
            writer.Write(coordinate.Y ?? double.NaN);
            if (hasZ)
            {
                writer.Write(coordinate.Z ?? double.NaN);
            }
        }

        private static void WriteList(IReadOnlyCollection<IDiff> diffs, BinaryWriter writer, bool hasZ)
        {
            WriteUint(diffs.Count, writer);
            foreach (var diff in diffs)
            {
                WriteUint(diff.Index, writer);
                Write(diff, writer, hasZ);
            }
        }

        private static void WriteUint(int number, BinaryWriter writer)
            => writer.Write(number);
        
        

    }
}
