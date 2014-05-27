// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization.Json
{
    /// <summary>
    /// Describes a JSON result that returns an additional parameter list.
    /// </summary>
    public sealed class JsonParamResult : JsonResult<IDictionary<string, object>>
    {
    }
}