// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Configuration
{
    /// <summary>
    /// A simple config repository that stores its data into dictionaries.
    /// </summary>
    public partial class KeyValuePairConfigRepository : ConfigRepositoryBase
    {
        #region Fields (1)

        /// <summary>
        /// Stores all categories and their values.
        /// </summary>
        protected readonly IDictionary<string, IDictionary<string, object>> _VALUES = new Dictionary<string, IDictionary<string, object>>();

        #endregion Fields (1)

        #region Constructors (2)

        /// <inheriteddoc />
        protected KeyValuePairConfigRepository(object sync)
            : base(isSynchronized: true,
                   sync: sync)
        {
            this._VALUES = this.CreateInitalConfigValueCollection() ?? new Dictionary<string, IDictionary<string, object>>();
        }

        /// <inheriteddoc />
        protected KeyValuePairConfigRepository()
            : this(sync: new object())
        {
        }

        #endregion Constructors (2)

        #region Methods (10)

        /// <summary>
        /// Creates the initial value for <see cref="KeyValuePairConfigRepository._VALUES" /> field.
        /// </summary>
        /// <returns>The inital value for <see cref="KeyValuePairConfigRepository._VALUES" />.</returns>
        protected virtual IDictionary<string, IDictionary<string, object>> CreateInitalConfigValueCollection()
        {
            // create default instance
            return null;
        }

        /// <inheriteddoc />
        protected override void OnClear(ref bool wasCleared)
        {
            if (this._VALUES.Count > 0)
            {
                this._VALUES.Clear();
                wasCleared = true;
            }

            if (wasCleared)
            {
                this.OnUpdated(UpdateContext.ClearAll,
                               category: null, name: null);
            }
        }

        /// <inheriteddoc />
        protected override void OnContainsValue(string category, string name, ref bool exists)
        {
            IDictionary<string, object> catValues;
            if (this._VALUES.TryGetValue(category, out catValues))
            {
                exists = catValues.ContainsKey(name);
            }
        }

        /// <inheriteddoc />
        protected override void OnDeleteValue(string category, string name, ref bool deleted)
        {
            IDictionary<string, object> catValues;
            if (this._VALUES.TryGetValue(category, out catValues))
            {
                deleted = catValues.Remove(name);
            }

            if (deleted)
            {
                this.OnUpdated(UpdateContext.DeleteValue,
                               category: category, name: name);
            }
        }

        /// <inheriteddoc />
        protected override IEnumerable<string> OnGetCategoryNames()
        {
            return this._VALUES
                       .Keys;
        }

        /// <summary>
        /// Extension of <see cref="KeyValuePairConfigRepository.OnSetValue{T}(string, string, T, ref bool, bool)" /> method.
        /// </summary>
        /// <typeparam name="T">Type of rhe value to set.</typeparam>
        /// <param name="category">The category where the value should be stored to.</param>
        /// <param name="name">The name in the category where the value should be stored to.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="valueWasSet">
        /// Defines if value was set or not.
        /// Is <see langword="false" /> by default.
        /// </param>
        /// <param name="invokeOnUpdated">
        /// Invoke <see cref="KeyValuePairConfigRepository.OnUpdated(UpdateContext, string, string)" /> method or not.
        /// </param>
        protected virtual void OnSetValue<T>(string category, string name, T value, ref bool valueWasSet, bool invokeOnUpdated)
        {
            IDictionary<string, object> catValues;
            if (this._VALUES.TryGetValue(category, out catValues) == false)
            {
                catValues = this.CreateEmptyDictionaryForCategory(category);
                if (catValues == null)
                {
                    return;
                }

                this._VALUES
                    .Add(category, catValues);
            }

            catValues[name] = this.ToConfigValue(category, name, value);
            valueWasSet = true;

            if (invokeOnUpdated &&
                valueWasSet)
            {
                this.OnUpdated(UpdateContext.SetValue,
                               category: category, name: name);
            }
        }

        /// <inheriteddoc />
        protected override void OnSetValue<T>(string category, string name, T value, ref bool valueWasSet)
        {
            this.OnSetValue<T>(category: category,
                               name: name,
                               value: value,
                               valueWasSet: ref valueWasSet,
                               invokeOnUpdated: true);
        }

        /// <inheriteddoc />
        protected override void OnTryGetValue<T>(string category, string name, ref T foundValue, ref bool valueWasFound)
        {
            IDictionary<string, object> catValues;
            if (this._VALUES.TryGetValue(category, out catValues))
            {
                object value;
                if (catValues.TryGetValue(name, out value))
                {
                    foundValue = this.FromConfigValue<T>(category, name, value);
                    valueWasFound = true;
                }
            }
        }

        /// <summary>
        /// Is invoked after data were updated.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="category">The name of the category.</param>
        /// <param name="name">The name of the value.</param>
        protected virtual void OnUpdated(UpdateContext context, string category, string name)
        {
            // dummy
        }

        /// <inheriteddoc />
        protected override void PrepareCategoryAndName(string category, string name,
                                                       out string newCategory, out string newName)
        {
            base.PrepareCategoryAndName(category, name,
                                        out newCategory, out newName);

            if (newCategory == null)
            {
                newCategory = string.Empty;
            }

            if (newName == null)
            {
                newName = string.Empty;
            }
        }

        #endregion Methods (1)
    }
}