using System;
using GeoAPI.Geometries;

namespace GeomDiff.Models
{
    public class CoordinateDelta
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

        public Coordinate Patch(Coordinate coordinate)
            => new Coordinate(coordinate.X + (X ?? 0), coordinate.Y + (Y ?? 0), coordinate.Z + (Z ?? 0));
        
        public CoordinateDelta Reverse()
            => new CoordinateDelta()
            {
                X = -X,
                Y = -Y,
                Z = -Z
            };
        
        public static CoordinateDelta Create(Coordinate oldCoordinate, Coordinate newCoordinate)
        {

            if (oldCoordinate != null && newCoordinate != null)
            {
                return new CoordinateDelta()
                {
                    X = newCoordinate.X - oldCoordinate.X,
                    Y = newCoordinate.Y - oldCoordinate.Y,
                    Z = newCoordinate.Z - oldCoordinate.Z
                };
            }

            if (oldCoordinate == null && newCoordinate != null)
            {
                return ToDiff(newCoordinate);
            }
            if (oldCoordinate != null && newCoordinate == null)
            {
                return ToDiff(oldCoordinate);
            }

            throw new Exception("Cannot create a diff from two empty coordinates!");
        }

        public static CoordinateDelta ToDiff(Coordinate coordinate)
            => new CoordinateDelta() { X = coordinate.X, Y = coordinate.Y, Z = coordinate.Z };

    }
}
