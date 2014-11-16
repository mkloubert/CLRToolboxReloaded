// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    /// <summary>
    /// A workflow that is based on attributes.
    /// </summary>
    public abstract class AttributeWorkflowBase : WorkflowBase
    {
        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public AttributeWorkflowBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeWorkflowBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        public AttributeWorkflowBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeWorkflowBase" /> class.
        /// </summary>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>Object will NOT work thread safe.</remarks>
        public AttributeWorkflowBase(object sync)
            : base(sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeWorkflowBase" /> class.
        /// </summary>
        /// <remarks>Object will NOT work thread safe.</remarks>
        public AttributeWorkflowBase()
            : base()
        {
        }

        #endregion Constructors

        #region Properties (2)

        /// <summary>
        /// Gets the name of the (default) contract.
        /// </summary>
        public virtual string ContractName
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the underlying object that contains the attributes.
        /// </summary>
        public virtual object Object
        {
            get { return this; }
        }

        #endregion

        #region Methods (3)

        /// <inheriteddoc />
        protected override IEnumerable<WorkflowFunc> GetFunctionIterator()
        {
            var contract = WorkflowAttributeBase.ParseContractName(this.ContractName);

            var obj = this.Object;

            var type = obj.GetType();
            var allMethods = GetMethodsByType(type);

            var occuredErrors = new List<Exception>();

            MethodInfo currentMethod = null;
            {
                var methodsWithAttribs = allMethods.Where(m =>
                    {
                        // search for 'WorkflowStartAttribute'
                        var attribs = m.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.Execution.Workflows.WorkflowStartAttribute),
                                                            true)
                                       .OfType<WorkflowStartAttribute>();

                        return attribs.Where(a => a.Contract == contract)
                                      .Any();
                    });

                currentMethod = methodsWithAttribs.SingleOrDefault();
            }

            var execVars = this.CreateVarStorage();
            var hasBeenCanceled = false;
            long index = -1;
            Exception lastError = null;
            IReadOnlyDictionary<string, object> previousVars = null;
            object result = null;
            var sync = new object();
            var throwErrors = true;
            while ((hasBeenCanceled == false) &&
                   (currentMethod != null))
            {
                yield return (args) =>
                    {
                        var ctx = new WorkflowExecutionContext(isSynchronized: false,
                                                               sync: sync)
                        {
                            Cancel = false,
                            ContinueOnError = false,
                            Errors = new ReadOnlyListWrapper<Exception>(occuredErrors),
                            ExecutionArguments = new ReadOnlyListWrapper<object>(args ?? new object[] { null }),
                            ExecutionVars = execVars,
                            HasBeenCanceled = hasBeenCanceled,
                            Index = ++index,
                            LastError = lastError,
                            NextVars = this.CreateVarStorage(),
                            PreviousVars = previousVars,
                            Result = result,
                            ThrowErrors = throwErrors,
                            Workflow = this,
                            WorkflowVars = this.Vars,
                        };

                        // first try to find method for next step
                        MethodInfo nextMethod = null;
                        {
                            // search for 'NextWorkflowStepAttribute'
                            var attribs = currentMethod.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.Execution.Workflows.NextWorkflowStepAttribute),
                                                                            true)
                                                       .OfType<NextWorkflowStepAttribute>();

                            var nextStep = attribs.Where(a => a.Contract == contract)
                                                  .SingleOrDefault();

                            if (nextStep != null)
                            {
                                var allPossibleNextMethods = allMethods.Where(m => m.Name == nextStep.Member);

                                // first: try find with parameter
                                nextMethod = allPossibleNextMethods.Where(m => m.GetParameters().Length > 0)
                                                                   .SingleOrDefault();

                                if (nextMethod == null)
                                {
                                    // now find WITHOUT parameter
                                    nextMethod = allPossibleNextMethods.Where(m => m.GetParameters().Length < 1)
                                                                       .Single();
                                }
                            }
                        }

                        Exception le = null;
                        try
                        {
                            // execution
                            InvokeWorkflowMethod(obj, currentMethod,
                                                 ctx,
                                                 occuredErrors,
                                                 out le);
                        }
                        finally
                        {
                            lastError = le;
                        }

                        // next action?
                        var nextAction = ctx.Next;
                        if (nextAction == null)
                        {
                            currentMethod = nextMethod;
                        }
                        else
                        {
                            obj = nextAction.Target;
                            currentMethod = nextAction.Method;
                        }

                        if (currentMethod != null)
                        {
                            type = currentMethod.ReflectedType;
                            allMethods = GetMethodsByType(type);
                        }

                        previousVars = new ReadOnlyDictionaryWrapper<string, object>(ctx.NextVars);
                        throwErrors = ctx.ThrowErrors;

                        if (ctx.Cancel)
                        {
                            hasBeenCanceled = true;
                            ctx.HasBeenCanceled = hasBeenCanceled;
                        }

                        result = ctx.Result;

                        return ctx;
                    };
            }

            if (throwErrors &&
                (occuredErrors.Count > 0))
            {
                throw new AggregateException(occuredErrors);
            }
        }

        private static IEnumerable<MethodInfo> GetMethodsByType(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.Instance | BindingFlags.Static);
        }

        private static void InvokeWorkflowMethod(object obj, MethodInfo method,
                                                 IWorkflowExecutionContext ctx,
                                                 ICollection<Exception> occuredErrors,
                                                 out Exception lastError)
        {
            lastError = null;

            try
            {
                object[] methodParams = null;
                if (method.GetParameters().Length > 0)
                {
                    methodParams = new object[] { ctx };
                }

                method.Invoke(obj, methodParams);
            }
            catch (Exception ex)
            {
                lastError = ex;
                occuredErrors.Add(ex);

                if (ctx.ContinueOnError == false)
                {
                    throw;
                }
            }
        }

        #endregion
    }
}