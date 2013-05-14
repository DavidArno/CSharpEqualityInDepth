using NUnit.Framework;

namespace CSharpEqualityInDepth
{
  // The two ways of checking for equality in C# - using == and .Equals() can be "overridden". It is important
  // to remember though that override and overload are two very different things in C#: .Equals() can be both
  // overriden and overloaded, whereas == can only be overloaded. This set of tests demonstrate the significant
  // effects this subtle difference can have on equality comparisons.
  [TestFixture]
  public class PartTwoEqualsAndDoubelEqualsAreNotEqualTests
  {
    // To demonstrate .Equals() overriding, we'll start by defining two test classes.
    // EqualsBaseClass is simply a base class that can be used to demonstrate the effect of polymorphism
    // on an Equals() override.
    private class EqualsBaseClass { }

    // EqualToEverythingClass overrides the Equals() method, such that it is equal to everything. We must
    // override the GetHashCode() method too as some .NET classes rely a simple rule: if A.Equals(B) is true,
    // then A and B must have the same hash value. The reverse doesn't need to apply of course, but it does
    // in this highly contrived case.
    private class EqualToEverythingClass : EqualsBaseClass
    {
      public override bool Equals(object obj)
      {
        return true;
      }

      public override int GetHashCode()
      {
        return 0;
      }
    }

    // If we override Equals in a class, we'd expect an instance of that class to use it. The first test
    // confirms this is the case.
    [Test]
    public void OverridingEquals_EffectsEquality()
    {
      var x = new EqualToEverythingClass();
      Assert.IsTrue(x.Equals(null));
    }

    // As Equals() is overriden, rather than overloaded, polymorphism should come in to play when determining
    // which version of Equals() to use. In other words, in the following test, x is declared as being of
    // type EqualsBaseClass at compile-time, but at run-time, it is assigned an instance of 
    // EqualToEverythingClass. Therefore, the latter's version is invoked by the Assert.
    [Test]
    public void EqualsOverride_IsPolymorphicOverride()
    {
      EqualsBaseClass x = new EqualToEverythingClass();
      Assert.IsTrue(x.Equals(null));
    }

    // Overriding Equals() does not effect == as the next test shows.
    [Test]
    public void OverridingEquals_DoesntEffectDoubleEquals()
    {
      var x = new EqualToEverythingClass();
      Assert.IsFalse(x == null);
    }

    private class BaseClass {}

    private class EqualToAndNotEqualToEverythingClass : BaseClass
    {
      public static bool operator ==(EqualToAndNotEqualToEverythingClass lhs, object rhs)
      {
        return true;
      }

      public static bool operator !=(EqualToAndNotEqualToEverythingClass lhs, object rhs)
      {
        return true;
      }
    }

    [Test]
    public void OverloadOfDoubleEqualsUsed_WhenLhsIsCorrectType()
    {
      var x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsTrue(x == null);
    }

    [Test]
    public void OverloadOfNotEqualsUsed_WhenLhsIsCorrectType()
    {
      var x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsTrue(x != x);
    }

    [Test]
    public void OverloadOfDoubleEqualsNotUsed_WhenLhsIsBaseType()
    {
      BaseClass x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsFalse(x == null);
    }

    [Test]
    public void OverloadOfNotEqualsNotUsed_WhenLhsIsCorrectType()
    {
      BaseClass x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsFalse(x != x);
    }
  }
}
