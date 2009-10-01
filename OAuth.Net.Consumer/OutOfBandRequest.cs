using System;

namespace OAuth.Net.Consumer
{
    public static class OutOfBandRequest
    {
        public static readonly Uri CallbackUri = new Uri("oob://");

        public static bool IsOutOfBandCallback(Uri callbackUri)
        {
            return callbackUri == null || OutOfBandRequest.CallbackUri.Equals(callbackUri);
        }
    }
}
