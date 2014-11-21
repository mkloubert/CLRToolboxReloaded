// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    /// <summary>
    /// Marks a type as a default web interface module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
                    AllowMultiple = false)]
    public sealed class DefaultWebInterfaceModuleAttribute : Attribute
    {
    }
}