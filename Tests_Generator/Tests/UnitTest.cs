using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests_Generator;

namespace Tests;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public void CountClassesTest()
    {
        var oneClassContent =FileSystem.ReadFile("files\\OneClass.cs");
        var firstResultClasses = TestsGenerator.GetTestDictionary(oneClassContent);
        Assert.AreEqual(1, firstResultClasses.Keys.Count);

        var twoClassesContent = FileSystem.ReadFile("files\\TwoClasses.cs");
        var secondResultClasses = TestsGenerator.GetTestDictionary(twoClassesContent);
        Assert.AreEqual(2, secondResultClasses.Keys.Count);
    }

    [TestMethod]
    public void SameMethodNamesTest()
    {
        var oneDuplicatesMethodName =FileSystem.ReadFile("files\\OneDuplicatesMethodName.cs");
        var firstTestsContent = TestsGenerator.GetTestDictionary(oneDuplicatesMethodName);
        Assert.IsFalse(firstTestsContent.Values.First().Contains("ThirdMethodTest"));
        
        var twoDuplicatesMethodName = FileSystem.ReadFile("files\\TwoDuplicatesMethodName.cs");
        var secondTestsContent = TestsGenerator.GetTestDictionary(twoDuplicatesMethodName);
        Assert.IsFalse(secondTestsContent.Values.First().Contains("ThirdMethodTest"));
        Assert.IsFalse(secondTestsContent.Values.First().Contains("FirstMethodTest"));
    }
    
    [TestMethod]
    public void ResultContentTest()
    {
        var oneClassContent = FileSystem.ReadFile("files\\OneClass.cs");
        var testsContent = TestsGenerator.GetTestDictionary(oneClassContent);
        Assert.AreEqual(1, testsContent.Keys.Count);
        Assert.IsTrue(testsContent.Values.First().Contains("ATest()"));
        Assert.IsTrue(testsContent.Values.First().Contains("BTest()"));
        Assert.IsTrue(testsContent.Values.First().Contains("CTest()"));
    }
}