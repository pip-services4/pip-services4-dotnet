using PipServices4.Expressions.Mustache;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PipServices4.Expressions.Test.Mustache
{
    public class MustacheTemplateTest
    {
        [Fact]
        public void TestTemplate1()
        {
            var template = new MustacheTemplate();
            template.Template = "Hello, {{{NAME}}}{{ #if ESCLAMATION }}!{{/if}}{{{^ESCLAMATION}}}.{{{/ESCLAMATION}}}";
            Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>
            {
                { "NAME", "Alex"},
                { "ESCLAMATION", "1" }
            };

            var result = template.EvaluateWithVariables(variables);
            Assert.Equal("Hello, Alex!", result);

            template.DefaultVariables["name"] = "Mike";
            template.DefaultVariables["esclamation"] = false;

            result = template.Evaluate();
            Assert.Equal("Hello, Mike.", result);
        }
    }
}
