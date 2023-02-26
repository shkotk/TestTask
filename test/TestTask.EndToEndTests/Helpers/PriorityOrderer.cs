using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestTask.EndToEndTests.Helpers;

public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var orderedCases = testCases
            .OrderBy(testCase =>
            {
                var priority = testCase.TestMethod.Method
                    .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName)
                    .SingleOrDefault()
                    ?.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority));
                return priority ?? 0;
            })
            .ThenBy(testCase => testCase.TestMethod.Method.Name);

        return orderedCases;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class TestPriorityAttribute : Attribute
{
    public TestPriorityAttribute(int priority) => Priority = priority;

    public int Priority { get; }
}