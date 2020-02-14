using GeoAPI.Geometries;
using GeomDiff.Models.Enums;

namespace GeomDiff.Models
{
    public class Change
    {
        public IGeometry PreviousValue { get; set; }
        public IGeometry NewValue { get; set; }
        public Operation Operation { get; set; }
        public int Index { get; set; }
        public IGeometry GetValue()
            => Operation == Operation.Insert 
                ? NewValue 
                : PreviousValue;
    }
}
