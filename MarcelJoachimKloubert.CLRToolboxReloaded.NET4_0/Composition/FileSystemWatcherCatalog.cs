// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.CLRToolbox.Composition
{
    /// <summary>
    /// A catalog that is updated automatically by using a <see cref="FileSystemWatcher" /> instance.
    /// </summary>
    public sealed partial class FileSystemWatcherCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        #region Fields (6)

        private readonly List<_AssemblyFile> _ASSEMBLIE_FILES = new List<_AssemblyFile>();
        private readonly AggregateCatalog _CATALOGS = new AggregateCatalog();
        private readonly bool _OWNS_WATCHER;
        private readonly object _SYNC = new object();
        private readonly FileSystemWatcher _WATCHER;

        /// <summary>
        /// Stores the default file filter value.
        /// </summary>
        public const string DEFAULT_FILTER = "*.dll";

        #endregion Fields (6)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemWatcherCatalog" /> class.
        /// </summary>
        /// <param name="path">The directory to watch.</param>
        /// <param name="filter">The filter to use.</param>
        /// <param name="includeSubDirs">Also watch sub directories or not.</param>
        public FileSystemWatcherCatalog(string path, string filter = DEFAULT_FILTER, bool includeSubDirs = false)
            : this(watcher: CreateWatcher(path, filter, includeSubDirs),
                   ownsWatcher: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemWatcherCatalog" /> class.
        /// </summary>
        /// <param name="watcher">The underlying watcher.</param>
        /// <param name="ownsWatcher">
        /// Also dispose watcher if that instance is disposed or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="watcher" /> is <see cref="ArgumentNullException" />.
        /// </exception>
        public FileSystemWatcherCatalog(FileSystemWatcher watcher, bool ownsWatcher = false)
        {
            if (watcher == null)
            {
                throw new ArgumentNullException("watcher");
            }

            this._OWNS_WATCHER = ownsWatcher;
            this._WATCHER = watcher;

            this.Init();
        }

        #endregion Constructors (2)

        #region Properties (6)

        /// <summary>
        /// Gets the path of the directory that is handled by the watcher.
        /// </summary>
        public string Directory
        {
            get { return this._WATCHER.Path; }
        }

        /// <summary>
        /// Gets or sets a value that indicates that the catalog should watch for files or not.
        /// </summary>
        public bool Enabled
        {
            get { return this._WATCHER.EnableRaisingEvents; }

            set { this._WATCHER.EnableRaisingEvents = value; }
        }

        /// <summary>
        /// Gets or sets the function that verifies a file for adding to this catalog.
        /// </summary>
        public FilePredicate FileFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function that transforms file data.
        /// </summary>
        public FileDataTransformer FileTransformer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the filter the watcher is using.
        /// </summary>
        public string Filter
        {
            get { return this._WATCHER.Filter; }
        }

        /// <inheriteddoc />
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                IQueryable<ComposablePartDefinition> result;

                lock (this._SYNC)
                {
                    result = this._CATALOGS.Parts;
                }

                return result;
            }
        }

        #endregion Properties (6)

        #region Events and delegates (9)

        private void _WATCHER_Changed(object sender, FileSystemEventArgs e)
        {
            this.InvokeForLists((catalogs, assemblyFiles, state) =>
                {
                    try
                    {
                        if (state.EventArgs.ChangeType.HasFlag(WatcherChangeTypes.Changed) == false)
                        {
                            return;
                        }

                        var file = Path.GetFullPath(state.EventArgs.FullPath);

                        byte[] asmBlob;
                        AssemblyName asmName;
                        if (state.Catalog.TryLoadAssembly(file, out asmBlob, out asmName) == false)
                        {
                            // file is no assembly or could not be loaded
                            return;
                        }

                        var newEntry = new _AssemblyFile();
                        newEntry.File = file;
                        newEntry.Hash = HashAsmBlob(asmBlob);
                        newEntry.Length = asmBlob.Length;

                        var oldEntries = assemblyFiles.Where(x => x.File == file)
                                                      .ToArray();
                        foreach (var oe in oldEntries)
                        {
                            if (oe.Equals(newEntry))
                            {
                                // are equal
                                continue;
                            }

                            var removeOldEntry = new Action(() =>
                                {
                                    using (var oc = oe.Catalog)
                                    {
                                        catalogs.Catalogs.Remove(oc);
                                        assemblyFiles.Remove(oe);
                                    }
                                });

                            if (state.Catalog.GetFileFilter()(state.Catalog, file, asmName, newEntry.Hash, newEntry.Length) == false)
                            {
                                // does not match filter

                                removeOldEntry();
                                continue;
                            }

                            if (newEntry.Assembly == null)
                            {
                                // init assembly with catalog

                                newEntry.Assembly = Assembly.Load(asmBlob);
                                newEntry.Catalog = new AssemblyCatalog(newEntry.Assembly);
                            }

                            removeOldEntry();

                            // add NEW entry
                            assemblyFiles.Add(newEntry);
                            catalogs.Catalogs.Add(newEntry.Catalog);
                        }
                    }
                    catch (Exception ex)
                    {
                        state.Catalog.OnError(ex);
                    }
                }, new
                {
                    Catalog = this,
                    EventArgs = e,
                });
        }

        private void _WATCHER_Created(object sender, FileSystemEventArgs e)
        {
            this.InvokeForLists((catalogs, assemblyFiles, state) =>
                {
                    try
                    {
                        var file = Path.GetFullPath(state.EventArgs.FullPath);

                        byte[] asmBlob;
                        AssemblyName asmName;
                        if (state.Catalog.TryLoadAssembly(file, out asmBlob, out asmName) == false)
                        {
                            // file is no assembly or could not be loaded
                            return;
                        }

                        var newEntry = new _AssemblyFile();
                        newEntry.File = file;
                        newEntry.Hash = HashAsmBlob(asmBlob);
                        newEntry.Length = asmBlob.Length;

                        if (state.Catalog.GetFileFilter()(state.Catalog, newEntry.File, asmName, newEntry.Hash, newEntry.Length) == false)
                        {
                            // does not match filter
                            return;
                        }

                        var existingEntries = assemblyFiles.Where(x => newEntry.Equals(x))
                                                           .ToArray();
                        if (existingEntries.Length > 0)
                        {
                            // overwrite existing

                            foreach (var ee in existingEntries)
                            {
                                using (var oc = ee.Catalog)
                                {
                                    // remove old catalog
                                    catalogs.Catalogs.Remove(oc);

                                    // create and add new one
                                    ee.Catalog = new AssemblyCatalog(ee.Assembly);
                                    catalogs.Catalogs.Add(ee.Catalog);
                                }
                            }
                        }
                        else
                        {
                            // add new

                            newEntry.Assembly = Assembly.Load(asmBlob);
                            newEntry.Catalog = new AssemblyCatalog(newEntry.Assembly);

                            assemblyFiles.Add(newEntry);
                            catalogs.Catalogs.Add(newEntry.Catalog);
                        }
                    }
                    catch (Exception ex)
                    {
                        state.Catalog.OnError(ex);
                    }
                }, new
                {
                    Catalog = this,
                    EventArgs = e,
                });
        }

        private void _WATCHER_Deleted(object sender, FileSystemEventArgs e)
        {
            this.InvokeForLists((catalogs, assemblyFiles, state) =>
                {
                    try
                    {
                        var file = Path.GetFullPath(state.EventArgs.FullPath);

                        var matchingEntries = assemblyFiles.Where(x => x.File == file)
                                                           .ToArray();
                        foreach (var me in matchingEntries)
                        {
                            using (var oc = me.Catalog)
                            {
                                catalogs.Catalogs.Remove(oc);
                                assemblyFiles.Remove(me);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        state.Catalog.OnError(ex);
                    }
                }, new
                {
                    Catalog = this,
                    EventArgs = e,
                });
        }

        private void _WATCHER_Renamed(object sender, RenamedEventArgs e)
        {
            this.InvokeForLists((catalogs, assemblyFiles, state) =>
                {
                    try
                    {
                        var oldFile = Path.GetFullPath(state.EventArgs.OldFullPath);
                        var file = Path.GetFullPath(state.EventArgs.FullPath);

                        byte[] asmBlob;
                        AssemblyName asmName;
                        if (state.Catalog.TryLoadAssembly(file, out asmBlob, out asmName) == false)
                        {
                            // new file is no assembly or could not be loaded
                            return;
                        }

                        var oldEntries = assemblyFiles.Where(x => x.File == oldFile)
                                                      .ToArray();
                        foreach (var oe in oldEntries)
                        {
                            if (state.Catalog.GetFileFilter()(state.Catalog, file, asmName, oe.Hash, oe.Length))
                            {
                                // update file path
                                oe.File = file;
                            }
                            else
                            {
                                // new file does not match filter => remove old entry

                                using (var oc = oe.Catalog)
                                {
                                    catalogs.Catalogs.Remove(oc);
                                    assemblyFiles.Remove(oe);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        state.Catalog.OnError(ex);
                    }
                }, new
                {
                    Catalog = this,
                    EventArgs = e,
                });
        }

        /// <inheriteddoc />
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add { this._CATALOGS.Changed += value; }

            remove { this._CATALOGS.Changed -= value; }
        }

        /// <inheriteddoc />
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add { this._CATALOGS.Changing += value; }

            remove { this._CATALOGS.Changing -= value; }
        }

        /// <summary>
        /// Is occured if an error occurs.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorsReceived;

        /// <summary>
        /// Describes a method or function that is used to transform the data of a file, e.g. if it is encrypted or compressed.
        /// </summary>
        /// <param name="catalog">The underlying catalog instance.</param>
        /// <param name="path">The full path of the file.</param>
        /// <param name="src">The stream with the source data.</param>
        /// <param name="dest">The stream where the target / transformed data should be written to.</param>
        public delegate void FileDataTransformer(FileSystemWatcherCatalog catalog, string path, Stream src, Stream dest);

        /// <summary>
        /// Describes a function or method that checks if an assembly file can be loaded/added or not.
        /// </summary>
        /// <param name="catalog">The underlying catalog instance.</param>
        /// <param name="path">The full path of the assembly file.</param>
        /// <param name="asmName">The assembly information.</param>
        /// <param name="asmHash">The assembly data.</param>
        /// <param name="asmLength">The size of the assembly.</param>
        /// <returns>Can be added / loaded or not.</returns>
        public delegate bool FilePredicate(FileSystemWatcherCatalog catalog, string path, AssemblyName asmName, byte[] asmHash, long asmLength);

        #endregion Events and delegates (9)

        #region Methods (18)

        private static bool AlwaysAddFilePredicate(FileSystemWatcherCatalog catalog, string path, AssemblyName asmName, byte[] asmHash, long asmLength)
        {
            return true;
        }

        /// <summary>
        /// Creates a new <see cref="CompositionContainer" /> instance based on that catalog.
        /// </summary>
        /// <param name="providers">The optional export providers to add.</param>
        /// <returns>The created instance.</returns>
        /// <remarks>The new instance will be thread safe by default.</remarks>
        public CompositionContainer CreateContainer(params ExportProvider[] providers)
        {
            return this.CreateContainer(isThreadSafe: true,
                                        providers: providers);
        }

        /// <summary>
        /// Creates a new <see cref="CompositionContainer" /> instance based on that catalog.
        /// </summary>
        /// <param name="isThreadSafe">Handle new instance thread safe or not.</param>
        /// <param name="providers">The optional export providers to add.</param>
        /// <returns>The created instance.</returns>
        public CompositionContainer CreateContainer(bool isThreadSafe, params ExportProvider[] providers)
        {
            return new CompositionContainer(this,
                                            providers: providers,
                                            isThreadSafe: isThreadSafe);
        }

        private static FileSystemWatcher CreateWatcher(string path, string filter, bool includeSubDirs)
        {
            var result = new FileSystemWatcher();
            result.EnableRaisingEvents = false;

            if (string.IsNullOrWhiteSpace(filter) == false)
            {
                result.Filter = filter;
            }

            result.IncludeSubdirectories = includeSubDirs;
            result.Path = Path.GetFullPath(path);

            return result;
        }

        /// <inheriteddoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            try
            {
                this.UnregisterEvents();
            }
            finally
            {
                if (disposing)
                {
                    if (this._OWNS_WATCHER)
                    {
                        this._WATCHER.Dispose();
                    }
                }
            }
        }

        private static void DummyTransformer(FileSystemWatcherCatalog catalog, string path, Stream src, Stream dest)
        {
            src.CopyTo(dest);
        }

        private static byte[] HashAsmBlob(byte[] asmBlob)
        {
            return HashCrypter.Create<SHA512CryptoServiceProvider>()
                              .Encrypt(asmBlob);
        }

        private FilePredicate GetFileFilter()
        {
            return this.FileFilter ?? AlwaysAddFilePredicate;
        }

        private FileDataTransformer GetFileTransformer()
        {
            return this.FileTransformer ?? DummyTransformer;
        }

        private void Init()
        {
            this.RegisterEvents();
        }

        private void InvokeForLists(Action<AggregateCatalog, IList<_AssemblyFile>> action)
        {
            this.InvokeForLists((catalogs, assemblyFiles, actionState) =>
                {
                    actionState.Action(catalogs, assemblyFiles);
                }, new
                {
                    Action = action,
                });
        }

        private void InvokeForLists<T>(Action<AggregateCatalog, IList<_AssemblyFile>, T> action, T actionState)
        {
            lock (this._SYNC)
            {
                action(this._CATALOGS, this._ASSEMBLIE_FILES,
                       actionState);
            }
        }

        private bool OnError(Exception ex)
        {
            var handler = this.ErrorsReceived;
            if (handler != null)
            {
                handler(this, new ErrorEventArgs(ex));
                return true;
            }

            return false;
        }

        private void RegisterEvents()
        {
            this._WATCHER.Changed += this._WATCHER_Changed;
            this._WATCHER.Created += this._WATCHER_Created;
            this._WATCHER.Deleted += this._WATCHER_Deleted;
            this._WATCHER.Renamed += this._WATCHER_Renamed;
        }

        private static AssemblyName TryGetAssemblyName(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) == false)
                {
                    path = Path.GetFullPath(path);

                    if (File.Exists(path))
                    {
                        return AssemblyName.GetAssemblyName(path);
                    }
                }
            }
            catch
            {
                // ignore errors here
            }

            return null;
        }

        private bool TryLoadAssembly(string file, out byte[] asmBlob, out AssemblyName asmName)
        {
            asmBlob = null;
            asmName = null;

            asmBlob = this.TryLoadFile(file);
            if (asmBlob.IsNullOrEmpty())
            {
                // no data loaded
                return false;
            }

            var tmpFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tmpFile, asmBlob);

                asmName = TryGetAssemblyName(tmpFile);
            }
            finally
            {
                // try to delete temp file
                try
                {
                    File.Delete(tmpFile);
                }
                catch (Exception ex)
                {
                    this.OnError(ex);
                }
            }

            return asmBlob != null &&
                   asmName != null;
        }

        private byte[] TryLoadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (var src = File.OpenRead(path))
                    {
                        using (var dest = new MemoryStream())
                        {
                            this.GetFileTransformer()(this, path, src, dest);

                            return dest.ToArray();
                        }
                    }
                }
            }
            catch
            {
                // ignore errors here
            }

            return null;
        }

        private void UnregisterEvents()
        {
            this._WATCHER.Changed -= this._WATCHER_Changed;
            this._WATCHER.Created -= this._WATCHER_Created;
            this._WATCHER.Deleted -= this._WATCHER_Deleted;
            this._WATCHER.Renamed -= this._WATCHER_Renamed;
        }

        #endregion Methods (18)
    }
}