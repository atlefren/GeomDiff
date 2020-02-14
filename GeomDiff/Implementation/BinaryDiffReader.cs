using System;
using System.Collections.Generic;
using System.IO;
using GeomDiff.Diff;
using GeomDiff.Models;
using GeomDiff.Models.Enums;
using GeomDiff.Models.Excepions;


namespace GeomDiff.Implementation
{
    public static class BinaryDiffReader
    {
        private static readonly Dictionary<GeometryType, Func<BinaryReader, bool, int, IDiff>> Readers
            = new Dictionary<GeometryType, Func<BinaryReader, bool, int, IDiff>>()
            {
                {GeometryType.Point, ReadPointDiff},
                {GeometryType.LineString, ReadLineStringDiff},
                {GeometryType.Polygon, ReadPolygonDiff},
                {GeometryType.MultiPoint, ReadMultiPointDiff },
                {GeometryType.MultiLineString, ReadMultiLineStringDiff},
                {GeometryType.MultiPolygon, ReadMultiPolygonDiff}
            };

        public static IDiff Read(byte[] binaryPatch)
        {
            using (var reader = new BinaryReader(new MemoryStream(binaryPatch)))
            {
                var geometryType = ReadGeometryType(reader);
                var readZ = reader.ReadBoolean();
                return Read(reader, geometryType, readZ);
            }
        }

        private static GeometryType ReadGeometryType(BinaryReader reader)
            =>  (GeometryType) reader.ReadUInt32();
        
        private static Operation ReadOperationType(BinaryReader reader)
            =>  (Operation) reader.ReadUInt32();
        
        private static IDiff Read(BinaryReader binaryReader, GeometryType geometryType, bool readZ)
            => Readers.TryGetValue(geometryType, out var reader) 
                ? reader(binaryReader, readZ, 0) 
                : throw new GeometryTypeException($"Geometry type not supported: {geometryType}");
        
        private static PointDiff ReadPointDiff(BinaryReader reader, bool readZ, int index)
            => ReadObj<PointDiff, CoordinateDelta>(reader, readZ, ReadCoordinateDelta, index);

        private static LineStringDiff ReadLineStringDiff(BinaryReader reader, bool readZ, int index)
            => ReadObj<LineStringDiff, List<PointDiff>>(reader, readZ, GetListReader(ReadPointDiff), index);

        private static RingDiff ReadRingDiff(BinaryReader reader, bool readZ, int index)
            => ReadObj<RingDiff, List<PointDiff>>(reader, readZ, GetListReader(ReadPointDiff), index);

        private static PolygonDiff ReadPolygonDiff(BinaryReader reader, bool readZ, int index)
            => ReadObj<PolygonDiff, List<RingDiff>>(reader, readZ, GetListReader(ReadRingDiff), index);

        private static MultiPointDiff ReadMultiPointDiff(BinaryReader reader, bool readZ, int index)
            => ReadObj<MultiPointDiff, List<PointDiff>>(reader, readZ, GetListReader(ReadPointDiff), index);

        private static MultiLineStringDiff ReadMultiLineStringDiff(BinaryReader reader, bool readZ, int index)
            => ReadObj<MultiLineStringDiff, List<LineStringDiff>>(reader, readZ, GetListReader(ReadLineStringDiff), index);

        private static MultiPolygonDiff ReadMultiPolygonDiff(BinaryReader reader, bool readZ, int index)
            => ReadObj<MultiPolygonDiff, List<PolygonDiff>>(reader, readZ, GetListReader(ReadPolygonDiff), index);

        private static CoordinateDelta ReadCoordinateDelta(BinaryReader reader, bool readZ)
            => new CoordinateDelta()
            {
                X = reader.ReadDouble(),
                Y = reader.ReadDouble(),
                Z = readZ ? reader.ReadDouble() : double.NaN
            };

        private static Func<BinaryReader, bool, List<TElement>> GetListReader<TElement>(Func<BinaryReader, bool, int, TElement> read)
            => (reader, readZ) => ReadElements(reader, readZ, read);

        private static TObj ReadObj<TObj, TDiffedComponent>(BinaryReader reader, bool readZ, Func<BinaryReader, bool, TDiffedComponent> read, int index = 0)
            where TObj : IDiffWithValue<TDiffedComponent>, new() =>
            new TObj()
            {
                Operation = ReadOperationType(reader),
                Index = index,
                Value = read(reader, readZ)
            };

        private static List<TElement> ReadElements<TElement>(BinaryReader reader, bool readZ,
            Func<BinaryReader, bool, int, TElement> read)
        {
            var numElements = reader.ReadUInt32();
            var diffs = new List<TElement>();
            for (var i = 0; i < numElements; i++)
            {
                var pointIndex = (int)reader.ReadUInt32();
                diffs.Add(read(reader, readZ, pointIndex));
            }

            return diffs;
        }
    }
}
