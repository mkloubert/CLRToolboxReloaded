// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Configuration
{
    #region DELEGATE: DefaultConfigValueProvider

    /// <summary>
    /// Describes a method or function that provides a default value especially for an <see cref="IConfigRepository" /> object.
    /// </summary>
    /// <typeparam name="T">Type of the default value.</typeparam>
    /// <param name="category">The name of the category.</param>
    /// <param name="name">The name of the value.</param>
    /// <returns>The default value.</returns>
    public delegate T DefaultConfigValueProvider<T>(string category, string name);

    #endregion DELEGATE: DefaultConfigValueProvider

    #region INTERFACE: IConfigRepository

    /// <summary>
    /// Describes a repository that provides configuration data stored in categories as key/value pairs.
    /// </summary>
    public interface IConfigRepository : IObject
    {
        #region Data Members (2)

        /// <summary>
        /// Gets if that repository can be written.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Gets if that repository can be written.
        /// </summary>
        bool CanWrite { get; }

        #endregion  Data Members (2)

        #region Operations (13)

        /// <summary>
        /// Clears all categories and values.
        /// </summary>
        /// <returns>Repository was cleared (write operation was done) or not.</returns>
        /// <exception cref="InvalidOperationException">Repository cannot be written.</exception>
        bool Clear();

        /// <summary>
        /// Checks if a value exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <returns>Value exists or not.</returns>
        bool ContainsValue(string name, string category = null);

        /// <summary>
        /// Deletes a value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The optional category.</param>
        /// <returns>Value was deleted or not.</returns>
        /// <exception cref="InvalidOperationException">Repository cannot be written.</exception>
        bool DeleteValue(string name, string category = null);

        /// <summary>
        /// Returns the list of category names.
        /// </summary>
        /// <returns>The current list of category names.</returns>
        IEnumerable<string> GetCategoryNames();

        /// <summary>
        /// Returns a config value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The optional category.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value does not exist.</exception>
        /// <exception cref="InvalidOperationException">Repository cannot be read.</exception>
        object GetValue(string name, string category = null);

        /// <summary>
        /// Returns a config value strong typed.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="category">The optional category.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value does not exist.</exception>
        /// <exception cref="InvalidOperationException">Repository cannot be read.</exception>
        T GetValue<T>(string name, string category = null);

        /// <summary>
        /// Makes that config repository read-only.
        /// </summary>
        /// <returns>The read-only version of that repository.</returns>
        IConfigRepository MakeReadOnly();

        /// <summary>
        /// Sets a config value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="category">The optional category.</param>
        /// <returns>Value was set (write operation was done) or not.</returns>
        /// <exception cref="InvalidOperationException">Repository cannot be written.</exception>
        bool SetValue(string name, object value, string category = null);

        /// <summary>
        /// Sets a config value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="category">The optional category.</param>
        /// <returns>Value was set (write operation was done) or not.</returns>
        /// <exception cref="InvalidOperationException">Repository cannot be written.</exception>
        bool SetValue<T>(string name, T value, string category = null);

        /// <summary>
        /// Tries to return a config value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The variable where to found value to.</param>
        /// <param name="category">The optional category.</param>
        /// <param name="defaultVal">
        /// The value that is written to <paramref name="value" /> if not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="InvalidOperationException">Repository cannot be read.</exception>
        bool TryGetValue(string name, out object value, string category = null, object defaultVal = null);

        /// <summary>
        /// Tries to return a config value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The variable where to found value to.</param>
        /// <param name="category">The optional category.</param>
        /// <param name="defaultValProvider">
        /// The logic that provides the value that is written to <paramref name="value" /> if not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValProvider" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">Repository cannot be read.</exception>
        bool TryGetValue(string name, out object value, string category, DefaultConfigValueProvider<object> defaultValProvider);

        /// <summary>
        /// Tries to return a config value.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The variable where to found value to.</param>
        /// <param name="category">The optional category.</param>
        /// <param name="defaultVal">
        /// The value that is written to <paramref name="value" /> if not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="InvalidOperationException">Repository cannot be read.</exception>
        bool TryGetValue<T>(string name, out T value, string category = null, T defaultVal = default(T));

        /// <summary>
        /// Tries to return a config value.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The variable where to found value to.</param>
        /// <param name="category">The optional category.</param>
        /// <param name="defaultValProvider">
        /// The logic that provides the value that is written to <paramref name="value" /> if not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValProvider" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">Repository cannot be read.</exception>
        bool TryGetValue<T>(string name, out T value, string category, DefaultConfigValueProvider<T> defaultValProvider);

        #endregion Operations (13)
    }

    #endregion
}