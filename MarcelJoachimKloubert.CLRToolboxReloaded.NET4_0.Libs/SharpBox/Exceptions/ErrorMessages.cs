namespace AppLimit.CloudComputing.SharpBox.Exceptions
{
    internal static class ErrorMessages
    {
        internal static class ResourceManager
        {
            internal static string GetString(string key)
            {
                switch (key)
                {
                    case "ErrorCouldNotContactStorageService":
                        return "Couldn't contact storage service";

                    case "ErrorCouldNotRetrieveDirectoryList":
                        return "Couldn't retrieve child elements from the server";

                    case "ErrorCreateProviderInstanceFailed":
                        return "Instancing the cloud storage provider failed, verify the inner exception";

                    case "ErrorFileNotFound":
                        return "File not found";

                    case "ErrorInsufficientDiskSpace":
                        return "No more space in cloud storage available";

                    case "ErrorInvalidConsumerKeySecret":
                        return "The gived consumer Key/Secret are invalid";

                    case "ErrorInvalidCredentialsOrConfiguration":
                        return "The gived credentials or configuration format does not fits to the storage provider";

                    case "ErrorInvalidFileOrDirectoryName":
                        return "Invalid file or directory name";

                    case "ErrorInvalidParameters":
                        return "One or more parameters are invalid";

                    case "ErrorLimitExceeded":
                        return "Configured file size limit for transfer exceeded";

                    case "ErrorNoValidProviderFound":
                        return "No valid storage provider was found for the given configuration type";

                    case "ErrorOpenedConnectionNeeded":
                        return "The operation needs an opened connection to the cloud storage, please call the open method before!";

                    case "ErrorTransferAbortedManually":
                        return "The datatransfer was interrupted from the application during a callback";
                }

                return "n/a";
            }
        }
    }
}