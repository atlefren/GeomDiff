# GeomDiff

Library for creating diffs of IGeometry objects


#usage

```
//Create a diff as a byte array
var diff = GeometryDifferBinary.Diff(oldGeom, newGeom);

//patch a geometry using a diff
var patched = GeometryDifferBinary.Patch(oldGeom, diff); 

//unpatch
var unpatched = GeometryDifferBinary.UnPatch(newGeom, diff);
```
