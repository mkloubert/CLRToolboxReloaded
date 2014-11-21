// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using System;
using System.Text.RegularExpressions;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    partial class WebUrlHandler
    {
        private sealed class HandlerItem
        {
            #region Fields (2)

            internal readonly HandlerAction ACTION;
            internal readonly Regex REG_EX;

            #endregion Fields (2)

            #region Constructors (1)

            #region Events and delegates (1)

            internal delegate void HandlerAction(Match match, HttpRequestEventArgs e, ref bool found);

            #endregion

            internal HandlerItem(Regex regex,
                                 HandlerAction action)
            {
                this.REG_EX = regex;
                this.ACTION = action;
            }

            #endregion Constructors (1)
        }
    }
}