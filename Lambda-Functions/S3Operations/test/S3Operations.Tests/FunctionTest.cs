using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using S3Operations;

namespace S3Operations.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestToUpperFunction()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var upperCase = await function.FunctionHandler("hello world", context);

            Assert.Equal("HELLO WORLD", upperCase);
            await Task.CompletedTask;
        }
    }
}
