using System;
using System.Collections.Generic;
using System.Text;
using TutorStudent.Domain.DependencyInjectionAttribute;
using TutorStudent.Domain.Interfaces;

namespace TutorStudent.Infrastructure.Implementations
{

    [SingletonDependency(ServiceType = (typeof(ITestSingletonInterface)))]
    public class TestClass1 : ITestSingletonInterface
    {
        public int TestNumber { get; } = (new Random()).Next();
        public string TestMethod()
        {
            return $"class 1 : {TestNumber}";
        }
    }

    [TransientDependency(ServiceType = (typeof(ITestTransientInterface)))]
    public class TestClass2 : ITestTransientInterface
    {
        public int TestNumber { get; } = (new Random()).Next();
        public string TestMethod()
        {
            return $"class 2 : {TestNumber}";
        }
    }

    [ScopedDependency(ServiceType = (typeof(ITestScopedInterface)))]
    public class TestClass3 : ITestScopedInterface
    {
        public int TestNumber { get; } = (new Random()).Next();
        public string TestMethod()
        {
            return $"class 3 : {TestNumber}";
        }
    }
}
