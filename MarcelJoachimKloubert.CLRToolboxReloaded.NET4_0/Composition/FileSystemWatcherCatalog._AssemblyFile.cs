// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.ComponentModel.Composition.Hosting;

using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Composition
{
    partial class FileSystemWatcherCatalog
    {
        private sealed class _AssemblyFile : IEquatable<_AssemblyFile>
        {
            #region Fields (5)

            internal Assembly Assembly;
            internal AssemblyCatalog Catalog;
            internal string File;
            internal byte[] Hash = new byte[0];
            internal long Length = long.MinValue;

            #endregion Fields (5)

            #region Methods (3)

            public override bool Equals(object other)
            {
                if (other is _AssemblyFile)
                {
                    return this.Equals((_AssemblyFile)other);
                }

                return base.Equals(other);
            }

            public bool Equals(_AssemblyFile other)
            {
                if (other == null)
                {
                    return false;
                }

                return other.File == this.File &&
                       other.Hash.SequenceEqual(this.Hash) &&
                       other.Length == this.Length;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            #endregion Methods (3)
        }
    }
}