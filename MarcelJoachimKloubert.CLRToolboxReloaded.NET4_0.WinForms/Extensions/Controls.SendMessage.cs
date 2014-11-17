// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Windows.Forms
{
    static partial class ClrToolboxWinFormsExtensionMethods
    {
        #region Methods (4)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Control.DefWndProc(ref Message)" />
        public static void SendMessage(this Control ctrl, ref Message msg)
        {
            if (ctrl == null)
            {
                throw new ArgumentNullException("ctrl");
            }

            var msgTypeRef = typeof(global::System.Windows.Forms.Message).MakeByRefType();

            // find 'Control.DefWndProc(ref Message)' method
            var defWndProcMethod = ctrl.GetType()
                                       .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                       .First(m =>
                                           {
                                               if (m.Name != "DefWndProc")
                                               {
                                                   return false;
                                               }

                                               var p = m.GetParameters();
                                               return (p.Length == 1) &&
                                                      msgTypeRef.Equals(p[0].ParameterType);
                                           });

            var @params = new object[] { msg };
            defWndProcMethod.Invoke(ctrl, @params);

            msg = (Message)@params[0];
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Control.DefWndProc(ref Message)" />
        public static Message SendMessage(this Control ctrl, IntPtr hWnd, int msg, int wparam, int lparam)
        {
            return SendMessage(ctrl, hWnd, msg, (IntPtr)wparam, (IntPtr)lparam);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Control.DefWndProc(ref Message)" />
        public static Message SendMessage(this Control ctrl, int hWnd, int msg, int wparam, int lparam)
        {
            return SendMessage(ctrl, (IntPtr)hWnd, msg, (IntPtr)wparam, (IntPtr)lparam);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Control.DefWndProc(ref Message)" />
        public static Message SendMessage(this Control ctrl, IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var result = Message.Create(hWnd, msg, wparam, lparam);
            SendMessage(ctrl, ref result);

            return result;
        }

        #endregion Methods (3)
    }
}