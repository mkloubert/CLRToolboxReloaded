// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Resources
{
    /// <summary>
    /// Describes an object that locates resources.
    /// </summary>
    public interface IResourceLocator : IObject
    {
        #region Operations (1)

        /// <summary>
        /// Tries to return the stream of a resource.
        /// </summary>
        /// <param name="resourceName">the name of the resource.</param>
        /// <returns>The stream or <see langword="null" /> if not found.</returns>
        /// <remarks>
        /// <paramref name="resourceName" /> is handled case insensitive.
        /// </remarks>
        Stream TryGetResourceStream(string resourceName);

        #endregion Operations (1)
    }
}