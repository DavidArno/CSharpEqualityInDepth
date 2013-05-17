using System;
using NUnit.Framework;

namespace CSharpEqualityInDepth
{
  // With .NET 2.0, Microsoft introduced the IEquatable interface. It introduced a typesafe Equals() method
  // that could be used to speed up comparisons in the new generic collections. It speeded up those 
  // comparisons by avoiding the need for casting and boxing, which the Object.Equals() suffered from. The
  // IEquatable interface is a dangerous beast though as it can lure the unwary into thinking it rationalised
  // equality in C#. This set of tests sets out to demonstrate how quite the opposite occurred: it simply
  // made things worse.
  [TestFixture]
  public class PartThreeIEquatableTests
  {
    // To start, we define a simple immutable value class that implements IEquatable: XY. The class implements
    // .Equals(), wich is true if the X and Y values are equal.
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

    // Two separate instances of XY are equal if their X's and Y's are equal.
    [Test]
    public void TwoXYsWithSameXAndY_AreEqual()
    {
      var xy1 = new XY(1, 2);
      var xy2 = new XY(1, 2);
      Assert.IsTrue(xy1.Equals(xy2));
    }

    // If the X values are different, then they are not equal.
    [Test]
    public void TwoXYsWithDifferentX_AreNotEqual()
    {
      var xy1 = new XY(1, 2);
      var xy2 = new XY(2, 2);
      Assert.IsFalse(xy1.Equals(xy2));
    }

    // If the Y values are different, again they are not equal.
    [Test]
    public void TwoXYsWithDifferentY_AreNotEqual()
    {
      var xy1 = new XY(1, 2);
      var xy2 = new XY(1, 3);
      Assert.IsFalse(xy1.Equals(xy2));
    }

    // However, there is a problem lurking below the surface. The IEquatable.Equals
    // method is an overload, not an override of Object.Equals. The compiler must
    // therefore choose - at compile-time - the appropriate version of Equals() to
    // use. If we define xy1 as an object at compile time, then Object.Equals() is
    // used for the comparison. This causes the comparison to be a by-reference
    // comparison.
    [Test]
    public void TwoObjectsContainingXYsWithSameXAndY_AreNotEqual()
    {
      object xy1 = new XY(1, 2);
      XY xy2 = new XY(1, 2);
      Assert.IsFalse(xy1.Equals(xy2));
    }

    // To overcome this problem, it is necessary to override Object.Equals in our
    // class. The new version of the XY class (XY2) does just that.
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

    // Now when we compare two XY2 as objects, they are correctly tested for equality by value.
    [Test]
    public void TwoObjectsContainingXY2sWithSameXAndY_AreEqual()
    {
      object xy1 = new XY2(1, 2);
      XY2 xy2 = new XY2(1, 2);
      Assert.IsTrue(xy1.Equals(xy2));
    }

    // Of course, just because xy1.Equals(xy2) doesn't mean that xy1 == xy2. The == operator
    // still compares by reference,
    [Test]
    public void TwoObjectsContainingXY2sWithSameXAndY_AreNotDoubleEquals()
    {
      var xy1 = new XY2(1, 2);
      var xy2 = new XY2(1, 2);
      Assert.IsFalse(xy1 == xy2);
    }

    // So let's define a final version of XY, XY3, which handles equality as near as possible to
    // perfectly as C# will allow. We therefore add == and != operators.
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

    // Now xy1 == xy2 is true.
    [Test]
    public void TwoVarsContainingXY3sWithSameXAndY_AreDoubleEquals()
    {
      var xy1 = new XY3(1, 2);
      var xy2 = new XY3(1, 2);
      Assert.IsTrue(xy1 == xy2);
    }

    // Why the comment "as near as possible to [perfect] as C# will allow"? Well
    // if we define xy1 as object, the wrong version of == gets used and compare by
    // reference sneaks back in unwanted, but unavoidable. 
    [Test]
    public void TwoObjectsContainingXY3sWithSameXAndY_AreNotDoubleEquals()
    {
      object xy1 = new XY3(1, 2);
      var xy2 = new XY3(1, 2);
      Assert.IsFalse(xy1 == xy2);
    }
  }
}
