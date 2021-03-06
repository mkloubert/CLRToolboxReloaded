﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE45)
#define KNOWS_REFLECTED_TYPE_PROPERTY
#endif

#if !(NET40 || PORTABLE40 || MONO40)
#define METHOD_BASE_HAS_CREATE_DELEGATE
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Helpers;
using MarcelJoachimKloubert.CLRToolbox.Scripting.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Scripting
{
    /// <summary>
    /// A basic object for execute a script.
    /// </summary>
    public abstract partial class ScriptExecutorBase : DisposableObjectBase, IScriptExecutor
    {
        #region Constructors (2)

        /// <inheriteddoc />
        protected ScriptExecutorBase(object sync)
            : base(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ScriptExecutorBase()
            : this(sync: new object())
        {
        }

        #endregion Constructors

        #region Delegates and Events (3)

        /// <summary>
        /// Describes a procedure with a variable number of parameters.
        /// </summary>
        /// <param name="args">The parameters for the procedure.</param>
        public delegate void SimpleAction(params object[] args);

        /// <summary>
        /// Describes a function with a variable number of parameters and a variable result type.
        /// </summary>
        /// <param name="args">The parameters for the function.</param>
        /// <returns>The result of the function.</returns>
        public delegate object SimpleFunc(params object[] args);

        /// <summary>
        /// Describes a function with a variable number of parameters and a nullable <see cref="bool" /> as result type.
        /// </summary>
        /// <param name="args">The parameters for the function.</param>
        /// <returns>The result of the function.</returns>
        public delegate bool? SimplePredicate(params object[] args);

        #endregion Delegates and Events

        #region Methods (10)

        /// <inheriteddoc />
        public IScriptExecutionContext Execute(string src,
                                               bool autoStart = true,
                                               bool debug = false)
        {
            ScriptExecutionContext result;

            lock (this._SYNC)
            {
                this.ThrowIfDisposed();

                result = new ScriptExecutionContext();
                result.Executor = this;
                result.Source = src;
                result.IsDebug = debug;

                result.StartAction = delegate()
                {
                    var onExecCtx = new OnExecuteContext();
                    try
                    {
                        onExecCtx.IsDebug = result.IsDebug;
                        onExecCtx.Source = result.Source;

                        onExecCtx.StartTime = GlobalServices.Now;
                        this.OnExecute(onExecCtx);
                    }
                    finally
                    {
                        result.Result = onExecCtx.ScriptResult;
                    }
                };

                if (autoStart)
                {
                    result.Start();
                }
            }

            return result;
        }

        /// <summary>
        /// Exports types and methods from an assembly that are marked with <see cref="ExportScriptTypeAttribute" />
        /// and/or <see cref="ExportScriptFuncAttribute" /> attributes.
        /// </summary>
        /// <param name="asm">The assembly where to search in.</param>
        /// <param name="exportedFuncs">
        /// The variable where to save the exported functions.
        /// The key is the alias.
        /// The value is the delegate.
        /// </param>
        /// <param name="exportedTypes">
        /// The variable where to save the exported types.
        /// The key is the type.
        /// The value is the alias.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        protected virtual void ExportTypesAndFunctions(Assembly asm,
                                                       out IDictionary<string, Delegate> exportedFuncs,
                                                       out IDictionary<Type, string> exportedTypes)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            exportedFuncs = new Dictionary<string, Delegate>();
            exportedTypes = new Dictionary<Type, string>();

            if (this.IsTrustedAssembly(asm) == false)
            {
                // assembly is not trusted
                return;
            }

            ReflectionHelper.GetTypes(asm)
               .ForEach(ctx =>
                   {
                       var obj = ctx.State.Executor;

                       var type = ctx.Item;
                       if (obj.IsTrustedType(type) == false)
                       {
                           // type is not trusted
                           return;
                       }

                       var allExpTypeAttribs = ReflectionHelper.GetCustomAttributes<global::MarcelJoachimKloubert.CLRToolbox.Scripting.Export.ExportScriptTypeAttribute>(type)
                                                               .ToArray();
                       if (allExpTypeAttribs.Length != 1)
                       {
                           var expTypeAttrib = allExpTypeAttribs[0];
                           if (string.IsNullOrWhiteSpace(expTypeAttrib.Alias))
                           {
                               ctx.State.ExportedTypes[type] = null;
                           }
                           else
                           {
                               ctx.State.ExportedTypes[type] = expTypeAttrib.Alias.Trim();
                           }
                       }

                       var allMethods = ReflectionHelper.GetMethods(type);

                       object instanceOfType = null;
                       allMethods.ForEach(ctx2 =>
                           {
                               var obj2 = ctx2.State.Executor;

                               var method = ctx2.Item;
                               if (obj2.IsTrustedMethod(method))
                               {
                                   // method is not trusted
                                   return;
                               }

                               var allExpFuncAttribs = ReflectionHelper.GetCustomAttributes<global::MarcelJoachimKloubert.CLRToolbox.Scripting.Export.ExportScriptFuncAttribute>(method)
                                                                       .ToArray();
                               if (allExpFuncAttribs.Length != 1)
                               {
                                   return;
                               }

                               var delegateType = method.TryGetDelegateTypeFromMethod();
                               if (delegateType == null)
                               {
                                   return;
                               }

                               if ((method.IsStatic == false) &&
                                   (instanceOfType == null))
                               {
                                   instanceOfType = Activator.CreateInstance(type);
                               }

                               Delegate @delegate;
                               if (method.IsStatic)
                               {
#if METHOD_BASE_HAS_CREATE_DELEGATE
                                   @delegate = method.CreateDelegate(delegateType);
#else
                                   @delegate = Delegate.CreateDelegate(delegateType,
                                                                       method);
#endif
                               }
                               else
                               {
#if METHOD_BASE_HAS_CREATE_DELEGATE
                                   @delegate = method.CreateDelegate(delegateType,
                                                                     instanceOfType);
#else
                                   @delegate = Delegate.CreateDelegate(delegateType,
                                                                       instanceOfType,
                                                                       method);
#endif
                               }

                               var expFuncAttrib = allExpFuncAttribs[0];
                               if (string.IsNullOrWhiteSpace(expFuncAttrib.Alias))
                               {
                                   ctx2.State.ExportedFuncs[method.Name] = @delegate;
                               }
                               else
                               {
                                   ctx2.State.ExportedFuncs[expFuncAttrib.Alias.Trim()] = @delegate;
                               }
                           }, actionState: new
                           {
                               Executor = obj,
                               ExportedFuncs = ctx.State.ExportedFuncs,
                           });
                   }, actionState: new
                   {
                       Executor = this,
                       ExportedFuncs = exportedFuncs,
                       ExportedTypes = exportedTypes,
                   });
        }

        /// <inheriteddoc />
        public ScriptExecutorBase ExposeType<T>(string alias = null)
        {
            return this.ExposeType(typeof(T), alias);
        }

        /// <inheriteddoc />
        public ScriptExecutorBase ExposeType(Type type, string alias = null)
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();

                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }

                this._EXPOSED_TYPES[type] = string.IsNullOrWhiteSpace(alias) ? null : alias.Trim();
            }

            return this;
        }

        /// <summary>
        /// Checks if an assembly is trusted for that script executor or not.
        /// </summary>
        /// <param name="asm">The assembly to check.</param>
        /// <returns>Is trusted or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        protected virtual bool IsTrustedAssembly(Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            return true;
        }

        /// <summary>
        /// Checks if a method is trusted for that script executor or not.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <returns>Is trusted or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method" /> is <see langword="null" />.
        /// </exception>
        protected virtual bool IsTrustedMethod(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            Type reflType = null;
#if KNOWS_REFLECTED_TYPE_PROPERTY
            reflType = method.ReflectedType;
#endif

            if ((reflType != null) &&
                this.IsTrustedType(reflType))
            {
                return true;
            }

            return this.IsTrustedType(method.DeclaringType);
        }

        /// <summary>
        /// Checks if a type is trusted for that script executor or not.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Is trusted or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> is <see langword="null" />.
        /// </exception>
        protected virtual bool IsTrustedType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return this.IsTrustedAssembly(ReflectionHelper.GetAssembly(type));
        }

        /// <summary>
        /// The logic for the <see cref="ScriptExecutorBase.Execute(string, bool, bool)" /> method.
        /// </summary>
        /// <param name="context">The execution context.</param>
        protected abstract void OnExecute(OnExecuteContext context);

        /// <inheriteddoc />
        public ScriptExecutorBase SetFunction(string funcName, Delegate func)
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();

                if (funcName == null)
                {
                    throw new ArgumentNullException("funcName");
                }

                var name = funcName.Trim();
                if (name == string.Empty)
                {
                    throw new ArgumentException("funcName");
                }

                if (func == null)
                {
                    throw new ArgumentNullException("func");
                }

                this._FUNCS[name] = func;
            }

            return this;
        }

        /// <inheriteddoc />
        public ScriptExecutorBase SetVariable(string varName, object value)
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();

                if (varName == null)
                {
                    throw new ArgumentNullException("varName");
                }

                var name = varName.Trim();
                if (name == string.Empty)
                {
                    throw new ArgumentException("varName");
                }

                if (value.IsNull())
                {
                    value = null;
                }

                this._VARS[name] = value;
            }

            return this;
        }

        #endregion Methods
    }
}