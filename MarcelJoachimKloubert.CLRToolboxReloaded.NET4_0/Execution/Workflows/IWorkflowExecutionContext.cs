﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    #region DELEGATE: WorkflowAction

    /// <summary>
    /// Describes an action for handling an <see cref="IWorkflowExecutionContext" /> object.
    /// </summary>
    /// <param name="context">The context object.</param>
    public delegate void WorkflowAction(IWorkflowExecutionContext context);

    #endregion DELEGATE: WorkflowAction

    #region DELEGATE: WorkflowAction<TState>

    /// <summary>
    /// Describes an action for handling an <see cref="IWorkflowExecutionContext{S}" /> object.
    /// </summary>
    /// <typeparam name="TState">Type of the state object.</typeparam>
    /// <param name="context">The context object.</param>
    public delegate void WorkflowAction<TState>(IWorkflowExecutionContext<TState> context);

    #endregion DELEGATE: WorkflowAction<TState>

    #region INTERFACE: IWorkflowExecutionContext

    /// <summary>
    /// Describes a context for a workflow execution.
    /// </summary>
    public interface IWorkflowExecutionContext : IObject
    {
        #region Data Members (17)

        /// <summary>
        /// Gets or sets if the whole operation should be canceled or not.
        /// </summary>
        bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets if the execution should be continued if execution fails.
        /// </summary>
        bool ContinueOnError { get; set; }

        /// <summary>
        /// The list of errors that have be occured until now.
        /// </summary>
        IReadOnlyList<Exception> Errors { get; }

        /// <summary>
        /// Gets the arguments that were submitted to the underlying <see cref="WorkflowFunc" /> delegate.
        /// </summary>
        IReadOnlyList<object> ExecutionArguments { get; }

        /// <summary>
        /// Gets variables that should be used over the whole workflow execution chain.
        /// </summary>
        /// <remarks>
        /// The keys of the dictionary are handled case insensitive.
        /// </remarks>
        IDictionary<string, object> ExecutionVars { get; }

        /// <summary>
        /// Gets if the whole operation has been canceled or not.
        /// </summary>
        bool HasBeenCanceled { get; }

        /// <summary>
        /// Gets the current zero based index.
        /// </summary>
        long Index { get; }

        /// <summary>
        /// Gets if that context represents the first execution in the workflow chain or not.
        /// </summary>
        bool IsFirst { get; }

        /// <summary>
        /// Gets if that context represents the last execution in the workflow chain or not.
        /// With other words: <see cref="IWorkflowExecutionContext.Next" /> is <see langword="null" />.
        /// </summary>
        bool IsLast { get; }

        /// <summary>
        /// Gets the last error or <see langword="null" /> for no error.
        /// </summary>
        Exception LastError { get; }

        /// <summary>
        /// Gets or sets the next action to invoke.
        /// <see langword="null" /> indicates that this should be the last execution.
        /// </summary>
        /// <remarks>
        /// Is reset to <see langword="null" /> before each execution.
        /// </remarks>
        WorkflowAction Next { get; set; }

        /// <summary>
        /// Gets the dictionary that defines the variables for the <see cref="IWorkflowExecutionContext.PreviousVars" />
        /// of the next execution context.
        /// </summary>
        /// <remarks>
        /// The dictionary is cleared at the beginning.
        /// The keys of the dictionary are handled case insensitive.
        /// </remarks>
        IDictionary<string, object> NextVars { get; }

        /// <summary>
        /// Gets the values that were defined via the <see cref="IWorkflowExecutionContext.NextVars" /> property
        /// of the previous execution context.
        /// </summary>
        /// <remarks>
        /// The keys of the dictionary are handled case insensitive.
        /// The value is <see langword="null" /> if that context represents the first execution.
        /// </remarks>
        IReadOnlyDictionary<string, object> PreviousVars { get; }

        /// <summary>
        /// Gets or sets the result object for an <see cref="IWorkflow.Execute(object[])" /> method.
        /// </summary>
        object Result { get; set; }

        /// <summary>
        /// Throw occured exceptions at the end if at least one execution failed.
        /// </summary>
        bool ThrowErrors { get; set; }

        /// <summary>
        /// Gets the underlying workflow.
        /// </summary>
        IWorkflow Workflow { get; }

        /// <summary>
        /// Gets the dictionary that defines the variables of <see cref="IWorkflowExecutionContext.Workflow" />.
        /// </summary>
        IDictionary<string, object> WorkflowVars { get; }

        #endregion INTERFACE: IWorkflowExecutionContext

        #region Operations (17)

        /// <summary>
        /// Returns an argument of <see cref="IWorkflowExecutionContext.ExecutionVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="index">The index in <see cref="IWorkflowExecutionContext.ExecutionArguments" />.</param>
        /// <returns>The strong typed version of the var.</returns>
        /// <exception cref="IndexOutOfRangeException">Argument not stored in <paramref name="index" />.</exception>
        T GetExecutionArgument<T>(int index);

        /// <summary>
        /// Returns a value of <see cref="IWorkflowExecutionContext.ExecutionVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <returns>The strong typed version of the var.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="name" /> does not exist.</exception>
        T GetExecutionVar<T>(string name);

        /// <summary>
        /// Returns a value of <see cref="IWorkflowExecutionContext.NextVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <returns>The strong typed version of <see cref="IWorkflowExecutionContext.NextVars" />.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="name" /> does not exist.</exception>
        T GetNextVar<T>(string name);

        /// <summary>
        /// Returns a value of <see cref="IWorkflowExecutionContext.PreviousVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <returns>The strong typed version of <see cref="IWorkflowExecutionContext.PreviousVars" />.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="name" /> does not exist.</exception>
        T GetPreviousVar<T>(string name);

        /// <summary>
        /// Returns the value of <see cref="IWorkflowExecutionContext.Result" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <returns>The strong typed version of <see cref="IWorkflowExecutionContext.Result" />.</returns>
        T GetResult<T>();

        /// <summary>
        /// Returns a value of <see cref="IWorkflowExecutionContext.Workflow" /> property strong typed.
        /// </summary>
        /// <typeparam name="W">Target type.</typeparam>
        /// <returns>The strong typed version of <see cref="IWorkflowExecutionContext.Workflow" />.</returns>
        W GetWorkflow<W>() where W : global::MarcelJoachimKloubert.CLRToolbox.Execution.Workflows.IWorkflow;

        /// <summary>
        /// Returns a value of <see cref="IWorkflowExecutionContext.WorkflowVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <returns>The strong typed version of <see cref="IWorkflowExecutionContext.WorkflowVars" />.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="name" /> does not exist.</exception>
        T GetWorkflowVar<T>(string name);

        /// <summary>
        /// Tries to return an argument of <see cref="IWorkflowExecutionContext.ExecutionArguments" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="index">The zero based index in <see cref="IWorkflowExecutionContext.ExecutionArguments" />.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValue">
        /// The default value for <paramref name="index" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Argument exists or not.</returns>
        bool TryGetExecutionArgument<T>(int index, out T value, T defaultValue = default(T));

        /// <summary>
        /// Tries to return an argument of <see cref="IWorkflowExecutionContext.ExecutionArguments" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="index">The zero based index in <see cref="IWorkflowExecutionContext.ExecutionArguments" />.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValueProvider">
        /// The logic that produces the default value for <paramref name="index" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Argument exists or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValueProvider" /> is <see langword="null" />.
        /// </exception>
        bool TryGetExecutionArgument<T>(int index, out T value, Func<int, T> defaultValueProvider);

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.ExecutionVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValue">
        /// The default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        bool TryGetExecutionVar<T>(string name, out T value, T defaultValue = default(T));

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.ExecutionVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValueProvider">
        /// The logic that produces the default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValueProvider" /> is <see langword="null" />.
        /// </exception>
        bool TryGetExecutionVar<T>(string name, out T value, Func<string, T> defaultValueProvider);

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.NextVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValue">
        /// The default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        bool TryGetNextVar<T>(string name, out T value, T defaultValue = default(T));

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.NextVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValueProvider">
        /// The logic that produces the default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValueProvider" /> is <see langword="null" />.
        /// </exception>
        bool TryGetNextVar<T>(string name, out T value, Func<string, T> defaultValueProvider);

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.PreviousVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValue">
        /// The default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        bool TryGetPreviousVar<T>(string name, out T value, T defaultValue = default(T));

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.PreviousVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValueProvider">
        /// The logic that produces the default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValueProvider" /> is <see langword="null" />.
        /// </exception>
        bool TryGetPreviousVar<T>(string name, out T value, Func<string, T> defaultValueProvider);

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.WorkflowVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValue">
        /// The default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        bool TryGetWorkflowVar<T>(string name, out T value, T defaultValue = default(T));

        /// <summary>
        /// Tries to return a value of <see cref="IWorkflowExecutionContext.WorkflowVars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValueProvider">
        /// The logic that produces the default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValueProvider" /> is <see langword="null" />.
        /// </exception>
        bool TryGetWorkflowVar<T>(string name, out T value, Func<string, T> defaultValueProvider);

        #endregion Operations
    }

    #endregion INTERFACE: IWorkflowExecutionContext

    #region INTERFACE: IWorkflowExecutionContext<TState>

    /// <summary>
    /// Describes a context for a workflow execution.
    /// </summary>
    /// <typeparam name="TState">Type of the underlying state object.</typeparam>
    public interface IWorkflowExecutionContext<TState> : IWorkflowExecutionContext
    {
        #region Data Members (2)

        /// <inheriteddoc />
        new WorkflowAction<TState> Next { get; set; }

        /// <summary>
        /// Gets the underlying state object.
        /// </summary>
        TState State { get; }

        #endregion Data Members
    }

    #endregion INTERFACE: IWorkflowExecutionContext<TState>
}