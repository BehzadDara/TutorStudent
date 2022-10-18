using System;
using System.Collections.Generic;
using System.Text;

namespace TutorStudent.Domain.Interfaces
{
    public interface ITestInterface
    {
        public int TestNumber { get; }
        public string TestMethod();
    }
    public interface ITestSingletonInterface : ITestInterface { }
    public interface ITestTransientInterface : ITestInterface { }
    public interface ITestScopedInterface : ITestInterface { }
}
