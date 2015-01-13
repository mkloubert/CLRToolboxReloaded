// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40 || PORTABLE45)
#define KNOWS_FILE_STREAM
#endif

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.IO;
using MarcelJoachimKloubert.CLRToolbox.IO.Console;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// Class for accessing global services and data.
    /// </summary>
    public static class GlobalServices
    {
        #region Fields (3)

        private static ConfigProvider _confProvider;
        private static TempProvider _tempProvider;
        private static VariableStorageProvider _varProvider;

        #endregion Fields (3)

        #region Constructors (1)

        /// <summary>
        /// Initializes the <see cref="GlobalServices" /> class.
        /// </summary>
        static GlobalServices()
        {
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

        #region Delegates and events (3)

        /// <summary>
        /// Describes a method / function that provides a repository with configuration data.
        /// </summary>
        /// <returns>The configuration repository.</returns>
        public delegate IConfigRepository ConfigProvider();

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

        #endregion Delegates and events (3)

        #region Properties (6)

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

        #endregion Properties (5)

        #region Methods (6)

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

        #endregion Methods (6)
    }
}