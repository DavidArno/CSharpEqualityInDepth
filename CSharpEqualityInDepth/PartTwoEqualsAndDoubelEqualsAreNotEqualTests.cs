using NUnit.Framework;

namespace CSharpEqualityInDepth
{
  // The two ways of checking for equality in C# - using == and .Equals() - can be modified for individual types. 
  // It is important to remember though that override and overload are two very different things in C#: .Equals() 
  // can be both overridden and overloaded, whereas == can only be overloaded. This set of tests demonstrate the 
  // significant effects this subtle difference can have on equality comparisons.
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

    // As Equals() is overridden, rather than overloaded, polymorphism should come in to play when determining
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

    // To demonstrate "==" overloading, we'll need some new classes. Again we start by defining a simple
    // base class.
    private class BaseClass {}

    // Next is the definition of the class that will be tested. There are a few things to note here. 1. The
    // code itself is smellier than an overripe Vieux Boulogne cheese as both == and != will always be true.
    // However it suits our purposes in these tests to do this, so hopefully you'll forgive this coding crime.
    // 2. The way operators are overloaded in C# is messy. They must be defined as static methods in the class
    // they affect. I do not know the exact details, but I assume that some sort of syntactic sugar is used
    // by the compiler to affect which method is used to perform the comparison. 3. Certain operators come as
    // a pair and if you overload one, you must overload the other too. "==" and "!=" are one such pair. Why
    // "!=" isn't inferred from "==" is a mystery to me. Microsoft could at least allowed it to default to 
    // inferred, even if there are edge cases that require it to be explicitly defined.
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

    // The first test compares a value to null, using the above class. By making it return true always,
    // the test can compare against null (which hopefully doesn't have its own implementation of ==), thus
    // guaranteeing we are testing our overload.
    [Test]
    public void OverloadOfDoubleEqualsUsed_WhenLhsIsCorrectType()
    {
      var x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsTrue(x == null);
    }

    // The next test confirms that, since we had to define "!=", at least it's being used. Incidentally, the
    // compiler issues a warning here, as it suggests that x != x is a pointless comparison, for of course
    // it will be false. It fails to take in to account just how bad some folk's code can be! :)
    [Test]
    public void OverloadOfNotEqualsUsed_WhenLhsIsCorrectType()
    {
      var x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsTrue(x != x);
    }

    // Because "==" is a static overload, rather than an override, it isn't polymorphic. In other words,
    // if x is defined as BaseClass, then the compiler will choose the version of "==" appropriate to that
    // at compile time. The fact that x is an instance of EqualToAndNotEqualToEverythingClass at runtime
    // doesn't effect it.
    [Test]
    public void OverloadOfDoubleEqualsNotUsed_WhenLhsIsBaseType()
    {
      BaseClass x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsFalse(x == null);
    }

    // The same applies to "!=". The version to use is chosen at compile-time and sanity returns to the !=
    // comparison.
    [Test]
    public void OverloadOfNotEqualsNotUsed_WhenLhsIsBaseType()
    {
      BaseClass x = new EqualToAndNotEqualToEverythingClass();
      Assert.IsFalse(x != x);
    }
  }
}
