using System;


namespace GeomDiff.Models.Excepions
{
    public class GeometryTypeException : ArgumentException
    {

        public GeometryTypeException(string message) : base(message) { }

        public GeometryTypeException(string geomType1, string geomType2) : base(
            $"Cannot create patch from {geomType1} to {geomType2}!")
        { }
    }
}
