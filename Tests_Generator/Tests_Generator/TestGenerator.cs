﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tests_Generator;

public static class TestsGenerator
{
    private static List<string> RenameDuplicates(List<string> methodNames, List<IGrouping<string, string>> duplicates)
    {
        for (int i = 0; i < duplicates.Count; i++)
        {
            var methodIndex = 0;
            for (int j = 0; j < methodNames.Count; j++)
            {
                if (duplicates[i].Key == methodNames[j])
                {
                    methodIndex++;
                    methodNames[j] += methodIndex;
                }
            }
        }
        return methodNames;
    }

    private static int countTests = 1;
    private static string GenerateTestContent(List<string> methodNames)
    {
        var duplicates = methodNames.GroupBy(methodName => methodName)
            .Where(methodName => methodName.Count() > 1).ToList();
        if (duplicates.Count() > 0)
            methodNames = RenameDuplicates(methodNames, duplicates);

        string methods = "";
        foreach (var methodName in methodNames)
        {
            methods += 
                @"
    [TestMethod]
    public void " + methodName + @"Test()
    {
        Assert.Fail(""autogenerated"");
    }";
            methods += '\n';
        }

        string testsContent =
            @"using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenerateTestProject;

[TestClass]

public class UnitTest" +countTests+ "{" + 
            methods + "}";
        
        countTests++;
        return testsContent;
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

    public static Dictionary<string, string> GetTestDictionary(string content)
    {
        Dictionary<string, string> testContents = new Dictionary<string, string>();
        var dataDictionary = GetDataDictionary(content);
        foreach (var dataDictionaryElement in dataDictionary)
        {
            var testContent = GenerateTestContent(dataDictionaryElement.Value);
            testContents.Add(dataDictionaryElement.Key, testContent);
        }
        return testContents;
    }
}