//
// QueryTests.cs
//
// Author:
//       Ventsislav Mladenov <vmladenov.mladenov@gmail.com>
//
// Copyright (c) 2013 Ventsislav Mladenov
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using NUnit.Framework;
using System.Xml.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Query;

namespace MonoDevelop.VersionControl.TFS.Tests
{
    [TestFixture]
    public class QueryTests
    {
        [Test]
        public void Parse1()
        {
            XElement el = XElement.Parse(@"<f>SELECT [System.Id], [System.WorkItemType], [System.AssignedTo], [System.CreatedBy], [Microsoft.VSTS.Common.Priority], [System.Title], [System.Description] 
FROM WorkItems 
WHERE 
    [System.TeamProject] = @project 
AND [System.State] &lt;&gt; 'Closed' 
AND [Microsoft.VSTS.Common.Issue] = 'Yes' 
ORDER BY [Microsoft.VSTS.Common.Priority], [System.Id]</f>");
            var parser = new LexalParser(el.Value);
            var nodes = parser.Process();

            Assert.IsTrue(nodes[0].NodeType == NodeType.Field);
            Assert.IsTrue(string.Equals(((FieldNode)nodes[0]).Field, "System.TeamProject"));

            Assert.IsTrue(nodes[1].NodeType == NodeType.Condition);
            Assert.IsTrue(((ConditionalNode)nodes[1]).Condition == Condition.Equals);

            Assert.IsTrue(nodes[2].NodeType == NodeType.Parameter);
            Assert.IsTrue(string.Equals(((ParameterNode)nodes[2]).ParameterName, "project"));

            Assert.IsTrue(nodes[3].NodeType == NodeType.Operator);
            Assert.IsTrue(((OperatorNode)nodes[3]).Operator == Operator.And);

            Assert.IsTrue(nodes[4].NodeType == NodeType.Field);
            Assert.IsTrue(nodes[5].NodeType == NodeType.Condition);
            Assert.IsTrue(nodes[6].NodeType == NodeType.Constant);
            Assert.IsTrue(nodes[7].NodeType == NodeType.Operator);
            Assert.IsTrue(nodes[8].NodeType == NodeType.Field);
            Assert.IsTrue(nodes[9].NodeType == NodeType.Condition);
            Assert.IsTrue(nodes[10].NodeType == NodeType.Constant);
        }

        [Test]
        public void Parse2()
        {
            XElement el = XElement.Parse(@"<f>select [System.Id], [Phoenix.DueDate], [Phoenix.MagicDueDate], [System.WorkItemType], [System.State], [System.Title], [System.IterationPath] 
from WorkItems 
where ([System.State] = 'New' 
    or [System.State] = 'Active') 
    and ([System.AssignedTo] = 'Ventsislav Mladenov' or [System.TeamProject] = @project) 
order by [System.Id]</f>");
            var parser = new LexalParser(el.Value);
            var nodes = parser.Process();
            Assert.IsTrue(nodes[0].NodeType == NodeType.StartGroup);

            Assert.IsTrue(nodes[1].NodeType == NodeType.Field);
            Assert.IsTrue(nodes[2].NodeType == NodeType.Condition);
            Assert.IsTrue(nodes[3].NodeType == NodeType.Constant);
            Assert.IsTrue(nodes[4].NodeType == NodeType.Operator);
            Assert.IsTrue(nodes[5].NodeType == NodeType.Field);
            Assert.IsTrue(nodes[6].NodeType == NodeType.Condition);
            Assert.IsTrue(nodes[7].NodeType == NodeType.Constant);
            Assert.IsTrue(nodes[8].NodeType == NodeType.EndGroup);

            Assert.IsTrue(nodes[9].NodeType == NodeType.Operator);

            Assert.IsTrue(nodes[10].NodeType == NodeType.StartGroup);
            Assert.IsTrue(nodes[11].NodeType == NodeType.Field);
            Assert.IsTrue(nodes[12].NodeType == NodeType.Condition);
            Assert.IsTrue(nodes[13].NodeType == NodeType.Constant);
            Assert.IsTrue(nodes[14].NodeType == NodeType.Operator);
            Assert.IsTrue(nodes[15].NodeType == NodeType.Field);
            Assert.IsTrue(nodes[16].NodeType == NodeType.Condition);
            Assert.IsTrue(nodes[17].NodeType == NodeType.Parameter);
            Assert.IsTrue(nodes[18].NodeType == NodeType.EndGroup);
        }

        [Test]
        public void Optimize1()
        {
            XElement el = XElement.Parse(@"<f>SELECT [System.Id], [System.WorkItemType], [Microsoft.VSTS.Common.Rank], [System.Title], [System.State], [System.AssignedTo], [Microsoft.VSTS.Common.RoughOrderOfMagnitude], [Microsoft.VSTS.Common.ExitCriteria], [System.Description] 
FROM WorkItems 
WHERE [System.TeamProject] = @project 
  AND [System.WorkItemType] = 'Scenario' 
  AND [System.State] = 'Active' 
ORDER BY [Microsoft.VSTS.Common.Rank], [System.State], [System.Id]</f>");
            var parser = new LexalParser(el.Value);
            var nodes = parser.Process();
            var output = NodeManager.Optimize(nodes);
            Assert.IsTrue(output[0].NodeType == NodeType.Condition);
            Assert.IsTrue(((ConditionalNode)output[0]).Right.NodeType == NodeType.Parameter);

            Assert.IsTrue(output[1].NodeType == NodeType.Operator);

            Assert.IsTrue(output[2].NodeType == NodeType.Condition);
            Assert.IsTrue(((ConditionalNode)output[2]).Right.NodeType == NodeType.Constant);

            Assert.IsTrue(output[3].NodeType == NodeType.Operator);

            Assert.IsTrue(output[4].NodeType == NodeType.Condition);
            Assert.IsTrue(((ConditionalNode)output[4]).Right.NodeType == NodeType.Constant);
        }

        [Test]
        public void RPN1()
        {
            XElement el = XElement.Parse(@"<f>select [System.Id], [Phoenix.DueDate], [Phoenix.MagicDueDate], [System.WorkItemType], [System.State], [System.Title], [System.IterationPath] 
from WorkItems 
where ([System.State] = 'New' 
    or [System.State] = 'Active') 
    and ([System.AssignedTo] = 'Ventsislav Mladenov' or [System.TeamProject] = @project) 
order by [System.Id]</f>");
            var parser = new LexalParser(el.Value);
            var nodes = parser.Process();
            var output = NodeManager.Optimize(nodes);
            var rpn = new RPNTransformer(output);
            var res = rpn.ConvertToRPN();
            var rpnToXml = new RPNToXml(res);
            Console.WriteLine(rpnToXml.Process());
        }
    }
}