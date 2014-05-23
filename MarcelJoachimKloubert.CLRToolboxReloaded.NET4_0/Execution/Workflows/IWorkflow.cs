// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    #region DELEGATE: WorkflowFunc

    /// <summary>
    /// Describes a function / methods that is a function of a workflow (step).
    /// </summary>
    /// <param name="args">The arguments for the current execution.</param>
    /// <returns>The context that is / was in use.</returns>
    public delegate IWorkflowExecutionContext WorkflowFunc(params object[] args);

    #endregion DELEGATE: WorkflowFunc

    #region INTERFACE: IWorkflow

    /// <summary>
    /// Describes a workflow.
    /// </summary>
    public interface IWorkflow : IObject, IEnumerable<WorkflowFunc>
    {
        #region Methods (2)

        /// <summary>
        /// Executes the workflow.
        /// </summary>
        /// <param name="args">The arguments for the execution.</param>
        /// <returns>The result of the execution.</returns>
        object Execute(IEnumerable<object> args);

        /// <summary>
        /// Executes the workflow.
        /// </summary>
        /// <param name="args">The arguments for the execution.</param>
        /// <returns>The result of the execution.</returns>
        object Execute(params object[] args);

        #endregion Methods (2)
    }

    #endregion INTERFACE: IWorkflow
}