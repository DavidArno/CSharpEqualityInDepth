using System;
using NUnit.Framework;

namespace CSharpEqualityInDepth
{
  [TestFixture]
  public class PartThreeIEquatableTests
  {
    private class XY : IEquatable<XY>
    {
      public readonly int X;
      public readonly int Y;

      public XY(int x, int y)
      {
        X = x;
        Y = y;
      }

      public bool Equals(XY other)
      {
        return X == other.X && Y == other.Y;
      }
    }

    [Test]
    public void TwoXYsWithSameXAndY_AreEqual()
    {
      var xy1 = new XY(1, 2);
      var xy2 = new XY(1, 2);
      Assert.IsTrue(xy1.Equals(xy2));
    }

    [Test]
    public void TwoXYsWithDifferentX_AreNotEqual()
    {
      var xy1 = new XY(1, 2);
      var xy2 = new XY(2, 2);
      Assert.IsFalse(xy1.Equals(xy2));
    }

    [Test]
    public void TwoXYsWithDifferentY_AreNotEqual()
    {
      var xy1 = new XY(1, 2);
      var xy2 = new XY(1, 3);
      Assert.IsFalse(xy1.Equals(xy2));
    }

    [Test]
    public void TwoObjectsContainingXYsWithSameXAndY_AreNotEqual()
    {
      object xy1 = new XY(1, 2);
      object xy2 = new XY(1, 2);
      Assert.IsFalse(xy1.Equals(xy2));
    }

    private class XY2 : IEquatable<XY2>
    {
      public readonly int X;
      public readonly int Y;

      public XY2(int x, int y)
      {
        X = x;
        Y = y;
      }

      public bool Equals(XY2 other)
      {
        return X == other.X && Y == other.Y;
      }

      public override bool Equals(object other)
      {
        return Equals(other as XY2);
      }

      public override int GetHashCode()
      {
        return X ^ Y;
      }
    }

    [Test]
    public void TwoObjectsContainingXY2sWithSameXAndY_AreEqual()
    {
      object xy1 = new XY2(1, 2);
      object xy2 = new XY2(1, 2);
      Assert.IsTrue(xy1.Equals(xy2));
    }

    [Test]
    public void TwoObjectsContainingXY2sWithSameXAndY_AreNotDoubleEquals()
    {
      var xy1 = new XY2(1, 2);
      var xy2 = new XY2(1, 2);
      Assert.IsFalse(xy1 == xy2);
    }

    private class XY3 : IEquatable<XY3>
    {
      public readonly int X;
      public readonly int Y;

      public XY3(int x, int y)
      {
        X = x;
        Y = y;
      }

      public bool Equals(XY3 other)
      {
        return X == other.X && Y == other.Y;
      }

      public override bool Equals(object other)
      {
        return Equals(other as XY3);
      }

      public override int GetHashCode()
      {
        return X ^ Y;
      }

      public static bool operator ==(XY3 lhs, object rhs)
      {
        return lhs.Equals(rhs);
      }

      public static bool operator !=(XY3 lhs, object rhs) 
      { 
        return !(lhs == rhs); 
      }
    }

    [Test]
    public void TwoObjectsContainingXY3sWithSameXAndY_AreDoubleEquals()
    {
      var xy1 = new XY3(1, 2);
      var xy2 = new XY3(1, 2);
      Assert.IsTrue(xy1 == xy2);
    }
  }
}
