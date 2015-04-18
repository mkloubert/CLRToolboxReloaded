// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Composition
{
    /// <summary>
    /// A part catalog that uses public assembly keys to trust assemblies or types.
    /// </summary>
    public sealed class StrongNameCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        #region Fields (2)

        private readonly AggregateCatalog _CATALOGS = new AggregateCatalog();
        private readonly PublicKeyProvider _PUBLIC_KEY_PROVIDER;

        #endregion Fields (2)

        #region Constructors (1)

        /// <summary>
        /// Creates a new instance of the <see cref="StrongNameCatalog" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the public keys to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public StrongNameCatalog(PublicKeyProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PUBLIC_KEY_PROVIDER = provider;
        }

        #endregion Constructors (1)

        #region Events and delegates (3)

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
        /// Describes a function / method that provides the public keys for an instance of that class.
        /// </summary>
        /// <param name="catalog">The underlying catalog.</param>
        /// <returns>The public keys.</returns>
        public delegate IEnumerable<byte[]> PublicKeyProvider(StrongNameCatalog catalog);

        #endregion Events and delegates (3)

        #region Properties (1)

        /// <inheriteddoc />
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this._CATALOGS.Parts; }
        }

        #endregion Properties (1)

        #region Methods (17)

        /// <summary>
        /// Adds an assembly to that catalog.
        /// </summary>
        /// <param name="asm">The assembly to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="asm" /> is NO trusted assembly.
        /// </exception>
        public void AddAssembly(Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            if (this.IsTrusted(asm) == false)
            {
                throw new InvalidOperationException();
            }

            this._CATALOGS
                .Catalogs.Add(new AssemblyCatalog(asm));
        }

        /// <summary>
        /// Adds all trusted assemblies of a directory.
        /// </summary>
        /// <param name="path">The path of the directory to add.</param>
        /// <returns>The added files.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// <paramref name="path" /> does NOT exist.
        /// </exception>
        public IEnumerable<FileInfo> AddDirectory(string path)
        {
            return this.AddDirectory(new DirectoryInfo(path));
        }

        /// <summary>
        /// Adds all trusted assemblies of a directory.
        /// </summary>
        /// <param name="dir">The directory to add.</param>
        /// <returns>The added files.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dir" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// <paramref name="dir" /> does NOT exist.
        /// </exception>
        public IEnumerable<FileInfo> AddDirectory(DirectoryInfo dir)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }

            foreach (var file in dir.EnumerateFiles()
                                    .Where(x => this.IsTrusted(x)))
            {
                this.AddFile(file);
                yield return file;
            }
        }

        /// <summary>
        /// Adds an assembly file.
        /// </summary>
        /// <param name="path">The path of the file to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// <paramref name="path" /> does NOT exist.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// File is NO trusted assembly.
        /// </exception>
        public void AddFile(string path)
        {
            this.AddFile(new FileInfo(path));
        }

        /// <summary>
        /// Adds an assembly file.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="file" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// <paramref name="file" /> does NOT exist.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// File is NO trusted assembly.
        /// </exception>
        public void AddFile(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (this.IsTrusted(file) == false)
            {
                throw new InvalidOperationException();
            }

            this.AddAssembly(Assembly.LoadFile(Path.GetFullPath(file.FullName)));
        }

        /// <summary>
        /// Adds a type to that catalog.
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="type" /> is NO type of a trusted assembly.
        /// </exception>
        public void AddType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (this.IsTrusted(type) == false)
            {
                throw new InvalidOperationException();
            }

            this._CATALOGS
                .Catalogs.Add(new TypeCatalog(type));
        }

        /// <summary>
        /// Creates a new instance from a list of public keys.
        /// </summary>
        /// <param name="pubKeys">The list of public keys.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pubKeys" /> is <see langword="null" />.
        /// </exception>
        public static StrongNameCatalog Create(IEnumerable<byte[]> pubKeys)
        {
            if (pubKeys == null)
            {
                throw new ArgumentNullException("pubKeys");
            }

            return new StrongNameCatalog((c) => pubKeys);
        }

        /// <summary>
        /// Creates a new <see cref="CompositionContainer" /> instance based on that catalog.
        /// </summary>
        /// <param name="providers">The optional export providers to add.</param>
        /// <returns>The created instance.</returns>
        /// <remarks>The new instance will NOT be thread safe.</remarks>
        public CompositionContainer CreateContainer(params ExportProvider[] providers)
        {
            return this.CreateContainer(isThreadSafe: false,
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

        /// <summary>
        /// Creates a new instance from a list of Base64 strings that represent public keys.
        /// </summary>
        /// <param name="base64List">The list of Base64 values / strings.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="base64List" /> is <see langword="null" />.
        /// </exception>
        public static StrongNameCatalog CreateFromBase64(IEnumerable<string> base64List)
        {
            if (base64List == null)
            {
                throw new ArgumentNullException("base64List");
            }

            return Create(base64List.Select(x =>
                {
                    if (string.IsNullOrWhiteSpace(x))
                    {
                        return null;
                    }

                    return Convert.FromBase64String(x.Trim());
                }));
        }

        /// <summary>
        /// Creates a new instance from a list of hex strings that represent public keys.
        /// </summary>
        /// <param name="hexList">The list of hex values / strings.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="hexList" /> is <see langword="null" />.
        /// </exception>
        public static StrongNameCatalog CreateFromHexList(IEnumerable<string> hexList)
        {
            if (hexList == null)
            {
                throw new ArgumentNullException("hexList");
            }

            return Create(hexList.Select(x =>
                {
                    if (string.IsNullOrWhiteSpace(x))
                    {
                        return null;
                    }

                    x = x.Trim();
                    return Enumerable.Range(0, x.Length)
                                     .Where(y => y % 2 == 0)
                                     .Select(y => Convert.ToByte(x.Substring(y, 2).ToLower(), 16))
                                     .ToArray();
                }));
        }

        /// <summary>
        /// Returns the list of underlying catalogs.
        /// </summary>
        /// <returns>The list of underlying catalogs.</returns>
        public IEnumerable<ComposablePartCatalog> GetCatalogs()
        {
            return this._CATALOGS
                       .Catalogs;
        }

        /// <summary>
        /// Returns all public keys.
        /// </summary>
        /// <returns>The public keys.</returns>
        public IEnumerable<byte[]> GetPublicKeys()
        {
            return (this._PUBLIC_KEY_PROVIDER(this) ?? Enumerable.Empty<byte[]>()).OfType<byte[]>();
        }

        /// <summary>
        /// Checks if an assembly is trusted.
        /// </summary>
        /// <param name="asm">The assembly to check.</param>
        /// <returns>Is trusted or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public bool IsTrusted(Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            return this.IsTrusted(asm.GetName());
        }

        /// <summary>
        /// Checks if an assembly is trusted.
        /// </summary>
        /// <param name="asmName">The assembly name to check.</param>
        /// <returns>Is trusted or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asmName" /> is <see langword="null" />.
        /// </exception>
        public bool IsTrusted(AssemblyName asmName)
        {
            if (asmName == null)
            {
                throw new ArgumentNullException("asmName");
            }

            var pk = asmName.GetPublicKey();
            if (pk != null)
            {
                return this.GetPublicKeys()
                           .Any(x => x.SequenceEqual(pk));
            }

            return false;
        }

        /// <summary>
        /// Checks if an assembly file is trusted.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>Is trusted or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="file" /> is <see langword="null" />.
        /// </exception>
        public bool IsTrusted(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            return this.IsTrusted(AssemblyName.GetAssemblyName(file.FullName));
        }

        /// <summary>
        /// Checks if a type is trusted.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Is trusted or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> is <see langword="null" />.
        /// </exception>
        public bool IsTrusted(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return this.IsTrusted(type.Assembly);
        }

        #endregion Methods (17)
    }
}