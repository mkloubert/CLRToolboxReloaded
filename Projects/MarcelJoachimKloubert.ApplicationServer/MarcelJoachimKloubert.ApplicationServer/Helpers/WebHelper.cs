// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.ApplicationServer.Helpers
{
    internal static class WebHelper
    {
        #region Methods (1)

        internal static string GetContentTypeByFileExtension(string ext)
        {
            switch ((ext ?? string.Empty).ToLower().Trim())
            {
                case "bmp":
                    return "image/bmp";
                
                case "eot":
                    return "application/vnd.ms-fontobject";

                case "gif":
                    return "image/gif";

                case "ico":
                case "icon":
                    return "image/x-icon";

                case "jpg":
                case "jpeg":
                    return "image/jpeg";

                case "png":
                    return "image/png";

                case "svg":
                    return "image/svg+xml";

                case "ttf":
                    return "application/x-font-ttf";

                case "woff":
                    return "application/font-woff";
            }

            return "application/octet-stream";
        }

        #endregion Methods (1)
    }
}