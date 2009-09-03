using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using OAuth.Net.Common;
using Microsoft.Practices.ServiceLocation;

namespace OAuth.Net.Components
{

    /// <summary>
    /// Implements the Verification Provider by hashing the token with the consumer key.
    /// The return verifier must equal this value.
    /// </summary>
    public class MD5HashVerificationProvider : IVerificationProvider
    {

        #region IVerificationProvider Members

        /// <summary>
        /// Generates a new Verification code for the given token.
        /// </summary>
        /// <param name="token"></param>        
        /// <returns></returns>
        public string Generate(IRequestToken token)
        {
            return CreateBase64MD5Hash(BuildHashValue( token ));            
        }

        /// <summary>
        /// Checks to see if the provided verifier is valid for this Request token.
        /// </summary>
        /// <param name="token"></param>        
        /// <param name="verifier"></param>
        /// <returns></returns>
        public bool IsValid(IRequestToken token, string verifier)
        {
            string hash = CreateBase64MD5Hash(BuildHashValue(token)); 
            return hash.Equals(verifier, StringComparison.Ordinal);
        }

        private string CreateBase64MD5Hash(string valueToHash)
        {
            MD5 md5Provider = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            return Convert.ToBase64String(md5Provider.ComputeHash(Encoding.Unicode.GetBytes(valueToHash)));
        }

        private string BuildHashValue(IRequestToken token)
        {
            return token.Token + GetConsumerSecret(token.ConsumerKey);
        }

        private string GetConsumerSecret(string consumerKey)
        {
            IConsumerStore store = ServiceLocator.Current.GetInstance<IConsumerStore>();
            if (store.Contains(consumerKey))
            {
                return store.GetByKey(consumerKey).Secret;
            }
            else
                throw new ArgumentException("Consumer could not be found for key " + consumerKey, "consumerKey");
        }

        #endregion
    }
}
