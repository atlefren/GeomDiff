using GeoAPI.Geometries;
using GeomDiff.Diff;
using GeomDiff.Implementation;


namespace GeomDiff
{
    public static class GeometryDiffer
    {
        private static readonly Differ.Differ Differ = new Differ.Differ();

        public static IDiff Diff(IGeometry oldGeom, IGeometry newGeom)
            => Differ.CreateDiff(oldGeom, newGeom);

        public static IGeometry Patch(IGeometry oldGeom, IDiff patch)
            => patch.Apply(oldGeom);

        public static IGeometry UnPatch(IGeometry oldGeom, IDiff patch)
            => patch.Undo(oldGeom);
    }
    
    public static class GeometryDifferBinary
    {
        private static readonly Differ.Differ Differ = new Differ.Differ();

        public static byte[] Diff(IGeometry oldGeom, IGeometry newGeom)
            => BinaryDiffWriter.Write(Differ.CreateDiff(oldGeom, newGeom));

        public static IGeometry Patch(IGeometry oldGeom, byte[] patch)
            => BinaryDiffReader.Read(patch).Apply(oldGeom);

        public static IGeometry UnPatch(IGeometry oldGeom, byte[] patch)
            => BinaryDiffReader.Read(patch).Undo(oldGeom);
    }
}
