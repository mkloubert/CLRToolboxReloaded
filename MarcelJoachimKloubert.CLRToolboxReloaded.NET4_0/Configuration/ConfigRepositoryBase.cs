// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Configuration
{
    /// <summary>
    /// A basic repository that provides configuration data stored in categories as key/value pairs.
    /// </summary>
    public abstract partial class ConfigRepositoryBase : ObjectBase, IConfigRepository
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected ConfigRepositoryBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ConfigRepositoryBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected ConfigRepositoryBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ConfigRepositoryBase()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public virtual bool CanRead
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public virtual bool CanWrite
        {
            get { return true; }
        }

        #endregion Properties (2)

        #region Methods (29)

        /// <inheriteddoc />
        public bool Clear()
        {
            return this.InvokeRepoFunc((repo) =>
                {
                    repo.ThrowIfNotWritable();

                    var result = false;
                    repo.OnClear(ref result);

                    return result;
                });
        }

        /// <inheriteddoc />
        public bool ContainsValue(string name, string category = null)
        {
            return this.InvokeRepoFunc((repo, state) =>
                {
                    string configCategory;
                    string configName;
                    repo.PrepareCategoryAndName(state.Category, state.Name,
                                                out configCategory, out configName);

                    var result = false;
                    repo.OnContainsValue(configCategory, configName,
                                         ref result);

                    return result;
                }, state: new
                {
                    Category = category,
                    Name = name,
                });
        }

        /// <summary>
        /// Creates a key/value pair collection for a new category.
        /// </summary>
        /// <param name="category">The name of the category.</param>
        /// <returns>The dictionary of the category or <see langword="null" /> if no category should be created.</returns>
        protected virtual IDictionary<string, object> CreateEmptyDictionaryForCategory(string category)
        {
            return new Dictionary<string, object>();
        }

        /// <inheriteddoc />
        public bool DeleteValue(string name, string category = null)
        {
            return this.InvokeRepoFunc((repo, state) =>
                {
                    repo.ThrowIfNotWritable();

                    string configCategory;
                    string configName;
                    repo.PrepareCategoryAndName(state.Category, state.Name,
                                                out configCategory, out configName);

                    var result = false;
                    repo.OnDeleteValue(configCategory, configName,
                                       ref result);

                    return result;
                }, state: new
                {
                    Category = category,
                    Name = name,
                });
        }

        /// <inheriteddoc />
        public IEnumerable<string> GetCategoryNames()
        {
            var names = this.OnGetCategoryNames() ?? Enumerable.Empty<string>();

            return names.Select(n =>
                                {
                                    var result = n.AsString();

                                    return string.IsNullOrWhiteSpace(result) ? null : result.Trim();
                                })
                        .Distinct();
        }

        /// <inheriteddoc />
        public object GetValue(string name, string category = null)
        {
            return this.GetValue<object>(name,
                                         category: category);
        }

        /// <inheriteddoc />
        public T GetValue<T>(string name, string category = null)
        {
            return this.InvokeRepoFunc((repo, state) =>
                {
                    T result;
                    if (repo.TryGetValue<T>(state.Name, out result, state.Category) == false)
                    {
                        throw new InvalidOperationException();
                    }

                    return result;
                }, state: new
                {
                    Category = category,
                    Name = name,
                });
        }

        /// <inheriteddoc />
        public IConfigRepository MakeReadOnly()
        {
            if (this.CanWrite == false)
            {
                // already read-only
                return this;
            }

            return new ReadOnlyConfigRepositoryWrapper(this);
        }

        /// <inheriteddoc />
        public bool SetValue(string name, object value, string category = null)
        {
            return this.SetValue<object>(name, value,
                                         category: category);
        }

        /// <inheriteddoc />
        public bool SetValue<T>(string name, T value, string category = null)
        {
            return this.InvokeRepoFunc((repo, state) =>
                {
                    repo.ThrowIfNotWritable();

                    string configCategory;
                    string configName;
                    repo.PrepareCategoryAndName(state.Category, state.Name,
                                                out configCategory, out configName);

                    var result = false;
                    repo.OnSetValue<T>(configCategory, configName,
                                       state.Value,
                                       ref result);

                    return result;
                }, state: new
                {
                    Category = category,
                    Name = name,
                    Value = value,
                });
        }

        /// <inheriteddoc />
        public bool TryGetValue(string name, out object value, string category = null, object defaultVal = null)
        {
            return this.TryGetValue<object>(name,
                                            out value,
                                            category: category, defaultVal: defaultVal);
        }

        /// <inheriteddoc />
        public bool TryGetValue<T>(string name, out T value, string category = null, T defaultVal = default(T))
        {
            return this.TryGetValue<T>(name, out value,
                                       category: category,
                                       defaultValProvider: (c, n) => defaultVal);
        }

        /// <inheriteddoc />
        public bool TryGetValue(string name, out object value, string category, DefaultConfigValueProvider<object> defaultValProvider)
        {
            return this.TryGetValue<object>(name, out value,
                                            category: category,
                                            defaultValProvider: defaultValProvider);
        }

        /// <inheriteddoc />
        public bool TryGetValue<T>(string name, out T value, string category, DefaultConfigValueProvider<T> defaultValProvider)
        {
            if (defaultValProvider == null)
            {
                throw new ArgumentNullException("defaultValProvider");
            }

            var temp = default(T);
            var result = this.InvokeRepoFunc((repo, state) =>
                {
                    repo.ThrowIfNotReadable();

                    var r = false;

                    string configCategory;
                    string configName;
                    repo.PrepareCategoryAndName(state.Category, state.Name,
                                                out configCategory, out configName);

                    T foundValue = default(T);
                    repo.OnTryGetValue<T>(configCategory, configName,
                                          ref foundValue,
                                          ref r);

                    if (r)
                    {
                        temp = foundValue;
                    }
                    else
                    {
                        // not found => use default
                        temp = state.DefaultValueProvider(category: configCategory,
                                                          name: configName);
                    }

                    return r;
                }, state: new
                {
                    Category = category,
                    DefaultValueProvider = defaultValProvider,
                    Name = name,
                });

            value = temp;
            return result;
        }

        /// <summary>
        /// Converts an object that is stored in that repository to a target type.
        /// </summary>
        /// <typeparam name="T">Target type of the value.</typeparam>
        /// <param name="input">The input value.</param>
        /// <param name="category">The category from where the input value comes from.</param>
        /// <param name="name">The name where the input value is stored in.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"><paramref name="input" /> could not be converted.</exception>
        protected virtual T FromConfigValue<T>(string category, string name, object input)
        {
            var result = input;
            if (result.IsNull())
            {
                result = null;
            }

            return GlobalConverter.Current
                                  .ChangeType<T>(result);
        }

        /// <summary>
        /// Invokes logic for that config repository.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="func">The logic to invoke.</param>
        /// <returns>the result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeRepoFunc<TResult>(Func<ConfigRepositoryBase, TResult> func)
        {
            return this.InvokeRepoFunc<Func<ConfigRepositoryBase, TResult>, TResult>((repo, f) => f(repo),
                                                                                     state: func);
        }

        /// <summary>
        /// Invokes logic for that config repository.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="func">The logic to invoke.</param>
        /// <param name="state">The additional state object for <paramref name="func" />.</param>
        /// <returns>the result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeRepoFunc<TState, TResult>(Func<ConfigRepositoryBase, TState, TResult> func,
                                                          TState state)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            Func<Func<ConfigRepositoryBase, TState, TResult>, TState, TResult> funcToInvoke;
            if (this._IS_SYNCHRONIZED)
            {
                funcToInvoke = this.InvokeRepoAction_ThreadSafe;
            }
            else
            {
                funcToInvoke = this.InvokeRepoAction_NonThreadSafe;
            }

            return funcToInvoke(func, state);
        }

        private TResult InvokeRepoAction_NonThreadSafe<TState, TResult>(Func<ConfigRepositoryBase, TState, TResult> func,
                                                                        TState state)
        {
            return func(this, state);
        }

        private TResult InvokeRepoAction_ThreadSafe<TState, TResult>(Func<ConfigRepositoryBase, TState, TResult> func,
                                                                     TState state)
        {
            TResult result;

            lock (this._SYNC)
            {
                result = this.InvokeRepoAction_NonThreadSafe<TState, TResult>(func,
                                                                              state);
            }

            return result;
        }

        /// <summary>
        /// Stores the logic for <see cref="ConfigRepositoryBase.Clear()" /> method.
        /// </summary>
        /// <param name="wasCleared">Was cleared or not.</param>
        protected abstract void OnClear(ref bool wasCleared);

        /// <summary>
        /// The logic for <see cref="ConfigRepositoryBase.ContainsValue(string, string)" /> method.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="exists">
        /// Defines if value was found or not.
        /// That value is <see langword="false" /> by default.
        /// </param>
        protected virtual void OnContainsValue(string category, string name, ref bool exists)
        {
            object dummy;
            exists = this.TryGetValue(name: name,
                                      value: out dummy,
                                      category: category);
        }

        /// <summary>
        /// The logic for <see cref="ConfigRepositoryBase.DeleteValue(string, string)" /> method.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="deleted">
        /// Defines if value was deleted or not.
        /// That value is <see langword="false" /> by default.
        /// </param>
        protected abstract void OnDeleteValue(string category, string name, ref bool deleted);

        /// <summary>
        /// The logic for <see cref="ConfigRepositoryBase.GetCategoryNames()" />
        /// </summary>
        protected abstract IEnumerable<string> OnGetCategoryNames();

        /// <summary>
        /// Stores the logic for <see cref="ConfigRepositoryBase.SetValue{T}(string, T, string)" /> method.
        /// </summary>
        /// <typeparam name="T">Type of rhe value to set.</typeparam>
        /// <param name="category">The category where the value should be stored to.</param>
        /// <param name="name">The name in the category where the value should be stored to.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="valueWasSet">
        /// Defines if value was set or not.
        /// Is <see langword="false" /> by default.
        /// </param>
        protected abstract void OnSetValue<T>(string category, string name, T value, ref bool valueWasSet);

        /// <summary>
        /// Stores the logic for <see cref="ConfigRepositoryBase.TryGetValue{T}(string, out T, string, T)" /> method.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="foundValue">Defines the found value.</param>
        /// <param name="valueWasFound">
        /// Defines if value was found or not.
        /// Is <see langword="false" /> bye default.
        /// </param>
        protected abstract void OnTryGetValue<T>(string category, string name, ref T foundValue, ref bool valueWasFound);

        /// <summary>
        /// Parses a category and name value for use in this config repository.
        /// </summary>
        /// <param name="category">The category value to parse.</param>
        /// <param name="name">The name value to parse.</param>
        /// <param name="newCategory">The target category value.</param>
        /// <param name="newName">The target name value.</param>
        protected virtual void PrepareCategoryAndName(string category, string name,
                                                      out string newCategory, out string newName)
        {
            newCategory = (category ?? string.Empty).ToLower().Trim();
            if (newCategory == string.Empty)
            {
                newCategory = null;
            }

            newName = (name ?? string.Empty).ToLower().Trim();
            if (newName == string.Empty)
            {
                newName = null;
            }
        }

        /// <summary>
        /// Throws an exception if that repository is not readable.
        /// </summary>
        /// <exception cref="InvalidOperationException">Repository cannot be read.</exception>
        protected void ThrowIfNotReadable()
        {
            if (this.CanRead == false)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Throws an exception if that repository is not written.
        /// </summary>
        /// <exception cref="InvalidOperationException">Repository cannot be written.</exception>
        protected void ThrowIfNotWritable()
        {
            if (this.CanWrite == false)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="category">The category where the input value should be stored in.</param>
        /// <param name="name">The name in the category where the input value should be stored in.</param>
        /// <param name="input">The input value</param>
        /// <returns></returns>
        protected virtual object ToConfigValue(string category, string name, object input)
        {
            var result = input;
            if (result.IsNull())
            {
                result = null;
            }

            return result;
        }

        #endregion Methods (29)
    }
}