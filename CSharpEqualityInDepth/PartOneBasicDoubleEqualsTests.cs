using NUnit.Framework;

namespace CSharpEqualityInDepth
{
  // Equality in C# is an odd beast. At first glance, it seems very simple. Value types are compared by
  // by value, objects are compared by reference and the comparison method can be overridden by a class
  // if it wants to. For example, the String class compares the string contents, rather than comparing
  // by reference. There are also sensible "best practice" rules suggested by Microsoft, such as avoiding
  // comparing mutable objects by value, only by reference. All is not as it seems though as the following
  // series of NUnit tests show. All the tests pass and thus demonstrate some aspect of equality in
  // C#.
  // These tests form part one of the C# In Depth series on http://davidarno.org.
  [TestFixture]
  public class PartOneBasicDoubleEqualsTests
  {
    // Two strings are equal if they contain the same content, as shown by this first test. The two
    // variables, x and y, have different references, yet they are equal.
    [Test]
    public void TwoStringsWithTheSameContent_AreEqual()
    {
      string x = "1234";
      string y = '1' + "234";
      Assert.IsTrue(x == y);
    }

    // The following test proves that x and y have different references. The values may be the same,
    // but a reference comparison failes.
    [Test]
    public void TwoStringsWithTheSameContent_MayHaveDifferentReferences()
    {
      string x = "1234";
      string y = '1' + "234";
      Assert.IsFalse(ReferenceEquals(x, y));
    }

    // Two strings are only equal if the contents have the same case. The following two strings
    // aren't equal.
    [Test]
    public void TwoStringsDifferingByCase_AreNotEqual()
    {
      object x = "Hello there";
      object y = "hello there";
      Assert.IsFalse(x == y);
    }

    // The "==" operator can be defined on a type-by-type basis. The version used is determined at
    // compile-time, based on the type of the left-hand value. So whilst String overrides the default
    // class definition of "==", if the variables are defined as Objects and are assigned string values,
    // the Object version of "==" is used. As the references are different, the two aren't now equal.
    [Test]
    public void TwoIdenticalStringsExpressedAsObjects_AreNotEqual()
    {
      object x = "1234";
      object y = '1' + "234";
      Assert.IsFalse(x == y);
    }

    // The C# compiler is pretty smart when it comes to working out types though. So if "var" is used,
    // freeing up the compiler to choose the best type (in this case String), the correct version of
    // "==" gets used.
    [Test]
    public void TwoStringsReferencedAsVars_AreEqual()
    {
      var x = "hello";
      var y = 'h' + "ello";
      Assert.IsTrue(x == y);
    }

    // C# supports the idea of boxing, ie converting primitive value types, like int, bool and float,
    // to objects and back again. As a consequence, it is possible to compare two primitives by reference.
    // Because each item gets boxed to an object in turn though, they will always have different addresses
    // and so the reference comparison will always fail.
    [Test]
    public void TwoIdenticalIntValues_WontHaveTheSameReference()
    {
      int x = 1;
      int y = x;
      Assert.IsFalse(ReferenceEquals(x, y));
    }

    // Once more though, how the items are defined can effect functionality. Define x and y as object,
    // rather than int, and they'll have the same reference as x is assigned the reference to a pre-boxed x. 
    [Test]
    public void TwoIdenticalBoxedIntValues_WillHaveTheSameReference()
    {
      object x = 1;
      object y = x;
      Assert.IsTrue(ReferenceEquals(x, y));
    }

    // Again, var is your friend here as the compiler assigns the best-fit types to x and y and so they
    // end up as ints as expected.
    [Test]
    public void TwoIdenticalIntVars_WontHaveTheSameReference()
    {
      var x = 1;
      var y = x;
      Assert.IsFalse(ReferenceEquals(x, y));
    }
  }
}
