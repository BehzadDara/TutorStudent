
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TutorStudent.Domain.Interfaces;

namespace TutorStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class AATestController : ControllerBase
    {
        private readonly ITestSingletonInterface _testSingletonInterface;
        private readonly ITestTransientInterface _testTransientInterface;
        private readonly ITestScopedInterface _testScopedInterface;
        private readonly ITestSingletonInterface _testSingleton2Interface;
        private readonly ITestTransientInterface _testTransient2Interface;
        private readonly ITestScopedInterface _testScoped2Interface;
        public AATestController(
            ITestSingletonInterface testSingletonInterface,
            ITestTransientInterface testTransientInterface,
            ITestScopedInterface testScopedInterface,
            ITestSingletonInterface testSingleton2Interface,
            ITestTransientInterface testTransient2Interface,
            ITestScopedInterface testScoped2Interface)
        {
            _testSingletonInterface = testSingletonInterface;
            _testTransientInterface = testTransientInterface;
            _testScopedInterface = testScopedInterface;
            _testSingleton2Interface = testSingleton2Interface;
            _testTransient2Interface = testTransient2Interface;
            _testScoped2Interface = testScoped2Interface;
        }

        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            var message1 = _testSingletonInterface.TestMethod();
            var message2 = _testSingleton2Interface.TestMethod();

            var message3 = _testTransientInterface.TestMethod();
            var message4 = _testTransient2Interface.TestMethod();

            var message5 = _testScopedInterface.TestMethod();
            var message6 = _testScoped2Interface.TestMethod();

            return Ok(message1 + "     " + message2 + "\n" +
                      message3 + "     " + message4 + "\n" +
                      message5 + "     " + message6 + "\n");
        }
    }
}
