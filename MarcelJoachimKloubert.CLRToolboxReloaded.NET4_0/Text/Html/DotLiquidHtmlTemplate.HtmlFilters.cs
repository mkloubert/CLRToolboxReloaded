// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.Web;

namespace MarcelJoachimKloubert.CLRToolbox.Text.Html
{
    partial class DotLiquidHtmlTemplate : HtmlTemplateBase
    {
        internal static class HtmlFilters
        {
            #region Methods (6)

            private static string ConvertToString(object obj)
            {
                return obj.AsString() ?? string.Empty;
            }

            public static string encode_html(object val)
            {
                return HttpUtility.HtmlEncode(ConvertToString(val));
            }

            public static string encode_html_attrib(object val)
            {
                return HttpUtility.HtmlAttributeEncode(ConvertToString(val));
            }

            public static string encode_js(object val)
            {
                return HttpUtility.JavaScriptStringEncode(ConvertToString(val));
            }

            public static string encode_json(object val)
            {
                return val.ToJson();
            }

            public static string encode_url(object val)
            {
                return HttpUtility.UrlEncode(ConvertToString(val));
            }

            #endregion Methods (6)
        }
    }
}