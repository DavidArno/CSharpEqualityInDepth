using System;

namespace CSharpEqualityInDepth
{
  public class PartThreeIEquatableTests
  {
    private class XY : IEquatable<XY>
    {
      public readonly double X;
      public readonly double Y;

      public XY(double x, double y)
      {
        X = x;
        Y = y;
      }

      public bool Equals(XY other)
      {
        return X == other.X && Y == other.Y;
      }
    }
  }
}
