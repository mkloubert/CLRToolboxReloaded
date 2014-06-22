// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.FileBox.Server.IO
{
    [Export(typeof(global::MarcelJoachimKloubert.FileBox.Server.IO.IDirectories))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class Directories : IDirectories
    {
        #region Fields (1)

        private readonly Global _GLOBAL;

        #endregion Fields (1)

        #region Constructors (1)

        [ImportingConstructor]
        internal Directories(Global app)
        {
            this._GLOBAL = app;
        }

        #endregion Constructors (1)

        #region Properties (1)

        public string Files
        {
            get { return this._GLOBAL.UserFileDirectory; }
        }

        public string Temp
        {
            get { return this._GLOBAL.TempDirectory; }
        }

        #endregion Properties (1)
    }
}