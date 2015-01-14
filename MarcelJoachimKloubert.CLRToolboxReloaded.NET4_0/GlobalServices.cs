// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40 || PORTABLE45)
#define KNOWS_FILE_STREAM
#endif

using MarcelJoachimKloubert.CLRToolbox.Collections;
using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.IO;
using MarcelJoachimKloubert.CLRToolbox.IO.Console;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// Class for accessing global services and data.
    /// </summary>
    public static class GlobalServices
    {
        #region Fields (4)

        private static CollectionBuilderProvider _collBuilderProvider;
        private static ConfigProvider _confProvider;
        private static TempProvider _tempProvider;
        private static VariableStorageProvider _varProvider;

        #endregion Fields (4)

        #region Constructors (1)

        /// <summary>
        /// Initializes the <see cref="GlobalServices" /> class.
        /// </summary>
        static GlobalServices()
        {
            SetCollections(new CollectionBuilder());
            SetConfig(new KeyValuePairConfigRepository());
            SetVars(new SynchronizedDictionary<string, object>(keyComparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                        emptyIsNull: true)));

#if KNOWS_FILE_STREAM
            SetTemp(new CommonTempDataManager());
#else
            SetTemp(new MemoryTempDataManager());
#endif
        }

        #endregion Constructors (1)

        #region Delegates and events (5)

        /// <summary>
        /// Describes a function / methods that provides an object that builds collections.
        /// </summary>
        /// <returns>The collection builder.</returns>
        public delegate ICollectionBuilder CollectionBuilderProvider();

        /// <summary>
        /// Describes a method / function that provides a repository with configuration data.
        /// </summary>
        /// <returns>The configuration repository.</returns>
        public delegate IConfigRepository ConfigProvider();

        /// <summary>
        /// Describes a method / function that provides the default value for a variable storage.
        /// </summary>
        /// <param name="vars">The underlying storage.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The default value.</returns>
        public delegate object DefaultVariableValueProvider(IDictionary<string, object> vars,
                                                            string name, Type targetType);

        /// <summary>
        /// Describes a method / function that provides a manager that handles temporary data.
        /// </summary>
        /// <returns>The temp data manager.</returns>
        public delegate ITempDataManager TempProvider();

        /// <summary>
        /// Describes a function / method that provides a storage for variables and their data.
        /// </summary>
        /// <returns>The variable storage.</returns>
        public delegate IDictionary<string, object> VariableStorageProvider();

        #endregion Delegates and events (5)

        #region Properties (7)

        /// <summary>
        /// Gets the object that builds collections.
        /// </summary>
        public static ICollectionBuilder Collections
        {
            get { return _collBuilderProvider(); }
        }

        /// <summary>
        /// Gets the global configuration.
        /// </summary>
        public static IConfigRepository Config
        {
            get { return _confProvider(); }
        }

        /// <summary>
        /// Gets the global console.
        /// </summary>
        public static IConsole Console
        {
            get { return GlobalConsole.Current; }
        }

        /// <summary>
        /// Gets the global converter.
        /// </summary>
        public static IConverter Converter
        {
            get { return GlobalConverter.Current; }
        }

        /// <summary>
        /// Gets the global service locator.
        /// </summary>
        public static IServiceLocator Services
        {
            get { return ServiceLocator.Current; }
        }

        /// <summary>
        /// Gets the manager that handles temporary data.
        /// </summary>
        public static ITempDataManager Temp
        {
            get { return _tempProvider(); }
        }

        /// <summary>
        /// Gets the global variables.
        /// </summary>
        public static IDictionary<string, object> Vars
        {
            get { return _varProvider(); }
        }

        #endregion Properties (7)

        #region Methods (26)

        /// <summary>
        /// Calls the <see cref="IConverter.ChangeType{T}(object, IFormatProvider)" /> method of <see cref="GlobalServices.Converter" />.
        /// </summary>
        /// <typeparam name="TResult">Target type.</typeparam>
        /// <param name="input">The input value.</param>
        /// <param name="provider">The optional format provider to use.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="InvalidCastException">Cast operation failed.</exception>
        public static TResult ChangeTo<TResult>(object input,
                                                IFormatProvider provider = null)
        {
            return Converter.ChangeType<TResult>(input,
                                                 provider);
        }

        /// <summary>
        /// Calls the <see cref="IConverter.ChangeType(Type, object, IFormatProvider)" /> method of <see cref="GlobalServices.Converter" />.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="provider">The optional format provider to use.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="InvalidCastException">Cast operation failed.</exception>
        public static object ChangeTo(Type targetType, object input,
                                      IFormatProvider provider = null)
        {
            return Converter.ChangeType(targetType, input,
                                        provider);
        }

        /// <summary>
        /// Calls the <see cref="ICollectionBuilder.CreateDictionary{TKey, TValue}(bool)" /> method of <see cref="GlobalServices.Collections" />
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="isSynchronized">List should be thread safe or not.</param>
        /// <returns>The new dictionary.</returns>
        public static IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(bool isSynchronized = false)
        {
            return Collections.CreateDictionary<TKey, TValue>(isSynchronized: isSynchronized);
        }

        /// <summary>
        /// Calls the <see cref="ICollectionBuilder.CreateDictionary{TKey, TValue}(IEqualityComparer{TKey}, bool)" />
        /// method of <see cref="GlobalServices.Collections" />
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="keyComparer">The key comparer to use.</param>
        /// <param name="isSynchronized">List should be thread safe or not.</param>
        /// <returns>The new dictionary.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyComparer" /> is <see langword="null" />.
        /// </exception>
        public static IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEqualityComparer<TKey> keyComparer, bool isSynchronized = false)
        {
            return Collections.CreateDictionary<TKey, TValue>(keyComparer: keyComparer,
                                                              isSynchronized: isSynchronized);
        }

        /// <summary>
        /// Calls the <see cref="ICollectionBuilder.CreateList{T}(bool)" /> method of <see cref="GlobalServices.Collections" />
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="isSynchronized">List should be thread safe or not.</param>
        /// <returns>The new list.</returns>
        public static IList<T> CreateList<T>(bool isSynchronized = false)
        {
            return Collections.CreateList<T>(isSynchronized: isSynchronized);
        }

        /// <summary>
        /// Calls the <see cref="ITempDataManager.CreateStream()" /> method of <see cref="GlobalServices.Temp" />.
        /// </summary>
        /// <returns>The new temp stream.</returns>
        public static Stream CreateTempStream()
        {
            return Temp.CreateStream();
        }

        /// <summary>
        /// Calls the <see cref="ITempDataManager.CreateStream(IEnumerable{byte}, bool)" /> method of <see cref="GlobalServices.Temp" />.
        /// </summary>
        /// <param name="initialBlob">The inital data.</param>
        /// <param name="startFromBeginning">
        /// Set position of new stream to beginning or not.
        /// </param>
        /// <returns>The new temp stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="initialBlob" /> is <see langword="null" />.
        /// </exception>
        public static Stream CreateTempStream(IEnumerable<byte> initialBlob,
                                              bool startFromBeginning = true)
        {
            return Temp.CreateStream(initialBlob,
                                     startFromBeginning);
        }

        /// <summary>
        /// Calls the <see cref="ITempDataManager.CreateStream(Stream, int?, bool)" /> method of <see cref="GlobalServices.Temp" />.
        /// </summary>
        /// <param name="initialData">The streams that contains the inital data.</param>
        /// <param name="bufferSize">
        /// The buffer size in bytes that should be used to read <paramref name="initialData" />.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <param name="startFromBeginning">
        /// Set position of new stream to beginning or not.
        /// </param>
        /// <returns>The new temp stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="initialData" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="initialData" /> cannot be read.
        /// </exception>
        public static Stream CreateTempStream(Stream initialData, int? bufferSize = null,
                                              bool startFromBeginning = true)
        {
            return Temp.CreateStream(initialData, bufferSize,
                                     startFromBeginning);
        }

        /// <summary>
        /// Calls the <see cref="IServiceLocator.GetAllInstances{S}(object)" /> method of <see cref="GlobalServices.Services" />.
        /// </summary>
        /// <typeparam name="S">Type of the service.</typeparam>
        /// <param name="key">
        /// Key of the service.
        /// <see langword="null" /> indicates to get the default service.
        /// </param>
        /// <returns>All instances of the service.</returns>
        public static object GetAllInstances<S>(object key = null)
        {
            return Services.GetAllInstances<S>(key);
        }

        /// <summary>
        /// Calls the <see cref="IServiceLocator.GetAllInstances(Type, object)" /> method of <see cref="GlobalServices.Services" />.
        /// </summary>
        /// <param name="serviceType">Typ des Dienstes.</param>
        /// <param name="key">
        /// Key of the service.
        /// <see langword="null" /> indicates to get the default service.
        /// </param>
        /// <returns>All instances of the service.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<object> GetAllInstances(Type serviceType, object key = null)
        {
            return Services.GetAllInstances(serviceType, key);
        }

        /// <summary>
        /// Calls the <see cref="IServiceLocator.GetInstance{S}(object)" /> method of <see cref="GlobalServices.Services" />.
        /// </summary>
        /// <typeparam name="S">Type of the service.</typeparam>
        /// <param name="key">
        /// Key of the service.
        /// <see langword="null" /> indicates to get the default service.
        /// </param>
        /// <returns>The instance of the service.</returns>
        /// <exception cref="ServiceActivationException">
        /// Error while locating service instance, e.g. not found.
        /// </exception>
        public static object GetInstance<S>(object key = null)
        {
            return Services.GetInstance<S>(key);
        }

        /// <summary>
        /// Calls the <see cref="IServiceLocator.GetInstance(Type, object)" /> method of <see cref="GlobalServices.Services" />.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">
        /// Key of the service.
        /// <see langword="null" /> indicates to get the default service.
        /// </param>
        /// <returns>The instance of the service.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ServiceActivationException">
        /// Error while locating service instance, e.g. not found.
        /// </exception>
        public static object GetInstance(Type serviceType, object key = null)
        {
            return Services.GetInstance(serviceType, key);
        }

        /// <summary>
        /// Returns a value from <see cref="GlobalServices.Vars" />.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="name" /> was not found.
        /// </exception>
        public static object GetVar(string name)
        {
            object result;
            if (TryGetVar(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// Returns a value from <see cref="GlobalServices.Vars" />.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="name" /> was not found.
        /// </exception>
        public static TResult GetVar<TResult>(string name)
        {
            TResult result;
            if (TryGetVar<TResult>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// Sets the function / method that provides the value for the <see cref="GlobalServices.Collections" /> property.
        /// </summary>
        /// <param name="provider">The new provider.</param>
        public static void SetCollectionProvider(CollectionBuilderProvider provider)
        {
            _collBuilderProvider = provider;
        }

        /// <summary>
        /// Sets the new value for the <see cref="GlobalServices.Collections" /> property.
        /// </summary>
        /// <param name="newValue">The new value</param>
        public static void SetCollections(ICollectionBuilder newValue)
        {
            SetCollectionProvider(newValue != null ? new CollectionBuilderProvider(() => newValue)
                                                   : null);
        }

        /// <summary>
        /// Sets the new value for the <see cref="GlobalServices.Config" /> property.
        /// </summary>
        /// <param name="newValue">The new value</param>
        public static void SetConfig(IConfigRepository newValue)
        {
            SetConfigProvider(newValue != null ? new ConfigProvider(() => newValue)
                                               : null);
        }

        /// <summary>
        /// Sets the function / method that provides the value for the <see cref="GlobalServices.Config" /> property.
        /// </summary>
        /// <param name="provider">The new provider.</param>
        public static void SetConfigProvider(ConfigProvider provider)
        {
            _confProvider = provider;
        }

        /// <summary>
        /// Sets the new value for the <see cref="GlobalServices.Temp" /> property.
        /// </summary>
        /// <param name="newValue">The new value</param>
        public static void SetTemp(ITempDataManager newValue)
        {
            SetTempProvider(newValue != null ? new TempProvider(() => newValue)
                                             : null);
        }

        /// <summary>
        /// Sets the function / method that provides the value for the <see cref="GlobalServices.Temp" /> property.
        /// </summary>
        /// <param name="provider">The new provider.</param>
        public static void SetTempProvider(TempProvider provider)
        {
            _tempProvider = provider;
        }

        /// <summary>
        /// Sets the function / method that provides the value for the <see cref="GlobalServices.Vars" /> property.
        /// </summary>
        /// <param name="provider">The new provider.</param>
        public static void SetVarProvider(VariableStorageProvider provider)
        {
            _varProvider = provider;
        }

        /// <summary>
        /// Sets the new value for the <see cref="GlobalServices.Vars" /> property.
        /// </summary>
        /// <param name="newValue">The new value</param>
        public static void SetVars(IDictionary<string, object> newValue)
        {
            SetVarProvider(newValue != null ? new VariableStorageProvider(() => newValue)
                                            : null);
        }

        /// <summary>
        /// Tries to get a value from <see cref="GlobalServices.Vars" />.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The variable where to write the value to.</param>
        /// <param name="defValue">
        /// The value for <paramref name="value" /> if value was not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        public static bool TryGetVar(string name, out object value, object defValue = null)
        {
            return TryGetVar(name: name,
                             value: out value,
                             defValueProvider: (d, n, t) => defValue);
        }

        /// <summary>
        /// Tries to get a value from <see cref="GlobalServices.Vars" />.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The variable where to write the value to.</param>
        /// <param name="defValue">
        /// The value for <paramref name="value" /> if value was not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        public static bool TryGetVar<TResult>(string name, out TResult value, TResult defValue = default(TResult))
        {
            return TryGetVar<TResult>(name: name,
                                      value: out value,
                                      defValueProvider: (d, n, t) => defValue);
        }

        /// <summary>
        /// Tries to get a value from <see cref="GlobalServices.Vars" />.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The variable where to write the value to.</param>
        /// <param name="defValueProvider">
        /// The function / method that provides the value for <paramref name="value" />
        /// if value was not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defValueProvider" /> is <see langword="null" />.
        /// </exception>
        public static bool TryGetVar(string name, out object value, DefaultVariableValueProvider defValueProvider)
        {
            if (defValueProvider == null)
            {
                throw new ArgumentNullException("defValueProvider");
            }

            var dict = Vars;

            object temp;
            var result = dict.TryGetValue(name, out temp);

            value = result ? temp
                           : defValueProvider(dict, name, typeof(object));
            return result;
        }

        /// <summary>
        /// Tries to get a value from <see cref="GlobalServices.Vars" />.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The variable where to write the value to.</param>
        /// <param name="defValueProvider">
        /// The function / method that provides the value for <paramref name="value" />
        /// if value was not found.
        /// </param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defValueProvider" /> is <see langword="null" />.
        /// </exception>
        public static bool TryGetVar<TResult>(string name, out TResult value,
                                              DefaultVariableValueProvider defValueProvider)
        {
            if (defValueProvider == null)
            {
                throw new ArgumentNullException("defValueProvider");
            }

            object temp;
            var result = TryGetVar(name: name,
                                   value: out temp,
                                   defValueProvider: defValueProvider);

            value = ChangeTo<TResult>(temp);
            return result;
        }

        #endregion Methods (26)
    }
}