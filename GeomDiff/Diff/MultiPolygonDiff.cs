﻿using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace GeomDiff.Diff
{
    public class MultiPolygonDiff : BaseListDiff<PolygonDiff>
    {
        public override string GeometryType { get; } = "MultiPolygon";

        public override IDiff Reverse(int? index = null) 
            => ReverseListDiff<MultiPolygonDiff>(index);

        protected override IGeometry ApplyPatch(IGeometry geometry)
            => new MultiPolygon(PatchMulti<IPolygon>(geometry));
    }
}
