using System;
using System.Collections.Generic;
using System.Text;

using OAuth.Net.Common;

namespace OAuth.Net.Components
{
    /// <summary>
    /// Implements a thread safe in memory callback store.
    /// </summary>
    public class InMemoryCallbackStore : ICallbackStore
    {
        private Dictionary<IRequestToken, Uri> CallbackStore = new Dictionary<IRequestToken, Uri>();

        #region ICallbackStore Members

        /// <summary>
        /// Appends a new callback to the store.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="callbackUri"></param>
        /// <returns>true if the callbackUri is succesfully appended or false if there already exists a callbackUri for the token</returns>
        public bool AddCallback(IRequestToken token, Uri callbackUri)
        {
            lock (CallbackStore)
            {
                if (CallbackStore.ContainsKey(token))
                    return false;
                else
                {
                    CallbackStore.Add(token, callbackUri);
                    return true;
                }
            }
        }

        /// <summary>
        /// Determins if the store contains a callback for the supplied token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ContainsCallback(IRequestToken token)
        {
            lock (CallbackStore)
            {
                return CallbackStore.ContainsKey(token);
            }
        }

        /// <summary>
        /// Removes the callbackUri from the store for the supplied token.
        /// This method should be used to obtain the callback Uri when redirecting the user to ensure 
        /// that it is removed from the store.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>The callback URI or null if none found.</returns>
        public Uri RemoveCallback(IRequestToken token)
        {
            Uri uri = null;

            lock (CallbackStore)
            {                
                CallbackStore.TryGetValue( token, out uri );
                CallbackStore.Remove( token );                
            }

            return uri;
        }

        #endregion
    }
}
