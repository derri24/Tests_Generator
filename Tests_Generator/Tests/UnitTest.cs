using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests_Generator;

namespace Tests;

[TestClass]
public class UnitTest
{
    private string ReadFileContent(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open);
        StreamReader streamReader = new StreamReader(fileStream);
        var content = streamReader.ReadToEnd();
        streamReader.Close();
        fileStream.Close();
        return content;
    }

    private static Dictionary<string, List<string>> GetDataDictionary(string content)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(content);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        Dictionary<string, List<string>> dataDictionary = new Dictionary<string, List<string>>();
        var classNames = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        foreach (var className in classNames)
        {
            List<string> methodNames = new List<string>();
            foreach (var method in className.Members)
                methodNames.Add(((MethodDeclarationSyntax) method).Identifier.ToString());
            dataDictionary.Add(className.Identifier.ToString(), methodNames);
        }
        return dataDictionary;
    }

    
    [TestMethod]
    public void CountClassesTest()
    {
        var oneClassContent = ReadFileContent("files\\OneClass.cs");
        var firstResultClasses = TestsGenerator.GetTestDictionary(oneClassContent);
        Assert.AreEqual(1, firstResultClasses.Keys.Count);

        var twoClassesContent = ReadFileContent("files\\TwoClasses.cs");
        var secondResultClasses = TestsGenerator.GetTestDictionary(twoClassesContent);
        Assert.AreEqual(2, secondResultClasses.Keys.Count);
    }

    
    [TestMethod]
    public void SameMethodNamesTest()
    {
        var oneDuplicatesMethodName = ReadFileContent("files\\OneDuplicatesMethodName.cs");
        var dataDictionary = GetDataDictionary(oneDuplicatesMethodName);
        var firstDuplicates = dataDictionary.GroupBy(methodName => methodName)
            .Where(methodName => methodName.Count() > 1).ToList();
        Assert.AreEqual(0, firstDuplicates.Count);

        var twoDuplicatesMethodName = ReadFileContent("files\\TwoDuplicatesMethodName.cs");
        var secondDataDictionary = GetDataDictionary(twoDuplicatesMethodName);
        var secondDuplicates = secondDataDictionary.GroupBy(methodName => methodName)
            .Where(methodName => methodName.Count() > 1).ToList();
        Assert.AreEqual(0, secondDuplicates.Count);
    }

    
    [TestMethod]
    public void ResultContentTest()
    {
        var oneClassContent = ReadFileContent("files\\OneClass.cs");
        var testsContent = TestsGenerator.GetTestDictionary(oneClassContent);
        Assert.AreEqual(1, testsContent.Keys.Count);
        Assert.IsTrue(testsContent.Values.First().Contains("ATest()"));
        Assert.IsTrue(testsContent.Values.First().Contains("BTest()"));
        Assert.IsTrue(testsContent.Values.First().Contains("CTest()"));
    }
}