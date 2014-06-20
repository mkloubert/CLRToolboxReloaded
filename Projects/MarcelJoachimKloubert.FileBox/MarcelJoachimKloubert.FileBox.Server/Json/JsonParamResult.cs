// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.FileBox.Server.Json
{
    /// <summary>
    /// A JSON result object that uses a general dictionary for its data.
    /// </summary>
    public sealed class JsonParamResult : JsonResult<IDictionary<string, object>>
    {
    }
}