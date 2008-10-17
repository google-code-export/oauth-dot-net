using System;
using System.Collections.Generic;
using System.Text;

namespace OAuth.Net.Common
{
    public interface IOutOfBoundConsumerStore
    {
        /// <summary>
        /// Adds the supplied consumer to the consumer store associating this consumer with a particular Uri. 
        /// If the consumer conflicts with a consumer already in the store, then <c>false</c> is returned.
        /// </summary>
        /// <param name="consumer">The consumer to store</param>
        /// <param name="uri">The Uri to assciate the consumer with</param>
        /// <returns><c>true</c>, if the consumer was stored</returns>
        bool Add(IConsumer consumer, Uri uri);

        /// <summary>
        /// Gets the consumer associated to the particular Uri
        /// </summary>
        /// <param name="uri">The Uri to search by</param>
        /// <returns>The consumer registered with the consumer key if successful, or
        /// <c>null</c> if there is no consumer registered with the supplied key</returns>
        IConsumer GetByUri(Uri uri);

        /// <summary>
        /// Removes the association between a consumer and Uri.
        /// </summary>        
        /// <param name="uri">The Uri the consumer is associated with.</param>        
        /// <returns><c>true</c>, if the association was succesfull removed
        /// from the store. This will return <c>false</c> if the assocaition
        /// did not exist in the store.</returns>
        bool Remove(Uri uri);
    }
}
