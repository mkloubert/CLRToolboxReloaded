// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Workflows;
using NUnit.Framework;
using System;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Execution
{
    [TestFixture]
    public class Workflows
    {
        #region CLASS: AttributeTestWorkflow

        public class AttributeTestWorkflow : AttributeWorkflowBase
        {
            #region Fields (1)

            public readonly StringBuilder STRING = new StringBuilder();

            #endregion Fields (1)

            #region Methods (8)

            [WorkflowStart]
            [NextWorkflowStep("Step_02")]
            public void Step_00(IWorkflowExecutionContext ctx)
            {
                this.STRING.Clear();

                ctx.Result = 23979;
            }

            [WorkflowStart("wurst")]
            [NextWorkflowStep("Step_01", "Wurst")]
            public void Step_00_Wurst(IWorkflowExecutionContext ctx)
            {
                this.STRING.Clear();

                this.STRING.Append("Wurst::");
            }

            [NextWorkflowStep("Step_02", "Wurst")]
            protected void Step_01(IWorkflowExecutionContext ctx)
            {
                this.STRING.Append("01");
            }

            [NextWorkflowStep("Step_03 ", "Wurst ")]
            [NextWorkflowStep("Step_03")]
            protected void Step_02(IWorkflowExecutionContext ctx)
            {
                this.STRING.Append("02");
            }

            [NextWorkflowStep(" Step_04 ", "  WURSt ")]
            [NextWorkflowStep("   Step_04")]
            protected void Step_03(IWorkflowExecutionContext ctx)
            {
                this.STRING.Append("03");
            }

            [NextWorkflowStep(" Step_08  ")]
            protected void Step_04(IWorkflowExecutionContext ctx)
            {
                this.STRING.Append("04");

                ctx.Next = (c) =>
                    {
                        this.STRING.Append("06");

                        c.Result = 5979;

                        c.Next = c.Index == 4 ? new WorkflowAction(Step_08) : new WorkflowAction(this.Step_09);
                        c.NextVars["obj"] = this;
                    };
            }

            protected static void Step_08(IWorkflowExecutionContext ctx)
            {
                ctx.GetPreviousVar<AttributeTestWorkflow>("OBj")
                   .STRING
                   .Append("08");
            }

            [NextWorkflowStep("Step_10", "wUrSt")]
            protected void Step_09(IWorkflowExecutionContext ctx)
            {
                AttributeTestWorkflow wf;
                if (ctx.TryGetPreviousVar<AttributeTestWorkflow>(name: "obj", value: out wf) == false)
                {
                    wf = this;
                }

                wf.STRING
                  .Append("09");

                ctx.Result = 23979;

                ctx.Next = ctx.Index == 6 ? new WorkflowAction(wf.Step_09) : new WorkflowAction(wf.Step_10);
            }

            public void Step_10(IWorkflowExecutionContext ctx)
            {
                this.STRING.Append("10");
            }

            #endregion Methods (8)
        }

        #endregion CLASS: AttributeTestWorkflow

        #region CLASS: AttributeTestWorkflow_Wurst

        public class AttributeTestWorkflow_Wurst : AttributeTestWorkflow
        {
            #region Properties (1)

            public override string ContractName
            {
                get { return "WURST"; }
            }

            #endregion Properties (1)
        }

        #endregion CLASS: AttributeTestWorkflow_Wurst

        #region Methods (2)

        [Test]
        public void AttributeWorkflowBase()
        {
            var wf1 = new AttributeTestWorkflow();
            var wf2 = new AttributeTestWorkflow_Wurst();

            var result1 = wf1.Execute();
            var result2 = wf2.Execute();

            Assert.AreEqual(result1, 5979);
            Assert.AreEqual(wf1.STRING.ToString(), "0203040608");

            Assert.AreEqual(result2, 23979);
            Assert.AreEqual(wf2.STRING.ToString(), "Wurst::0102030406090910");
        }
        
        [Test]
        public void DelegateWorkflowTest()
        {
            var str = string.Empty;

            var step3 = new WorkflowAction(ctx =>
                {
                    ctx.Result = ctx.GetResult<int>() + 3;

                    str += "03";
                });

            var step2 = new WorkflowAction(ctx =>
                {
                    ctx.Result = ctx.GetResult<int>() + 2;
                    str += "02";

                    ctx.Next = step3;
                });

            var step1 = new WorkflowAction(ctx =>
                {
                    ctx.Result = 1;
                    str += "01";

                    ctx.Next = step2;
                });


            var wf1 = DelegateWorkflow.Create(step1);
            var res1 = wf1.Execute();
            
            Assert.AreEqual(str, "010203");
            Assert.AreEqual(res1, 6);

            var wf2 = new DelegateWorkflow((wf, c) => step1);
            var res2 = wf2.Execute();

            Assert.AreEqual(str, "010203010203");
            Assert.AreEqual(res2, 6);
        }

        #endregion Methods (1)
    }
}