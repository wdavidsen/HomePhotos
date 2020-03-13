using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SCS.HomePhotos.Web.Test.Mocks
{
#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments

    /// <summary>
    /// A mock session for testing.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Http.ISession" />
    public sealed class MockHttpSession : ISession
    {
        Dictionary<string, object> sessionStorage = new Dictionary<string, object>();
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object this[string name]
        {
            get { return sessionStorage[name]; }
            set { sessionStorage[name] = value; }
        }

        /// <summary>
        /// A unique identifier for the current session. This is not the same as the session cookie
        /// since the cookie lifetime may not be the same as the session entry lifetime in the data store.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        string ISession.Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Indicate whether the current session has loaded.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        bool ISession.IsAvailable
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Enumerates all the keys, if any.
        /// </summary>
        IEnumerable<string> ISession.Keys
        {
            get { return sessionStorage.Keys; }
        }
        /// <summary>
        /// Remove all entries from the current session, if any.
        /// The session cookie is not removed.
        /// </summary>
        void ISession.Clear()
        {
            sessionStorage.Clear();
        }
        /// <summary>
        /// Store the session in the data store. This may throw if the data store is unavailable.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        Task ISession.CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load the session from the data store. This may throw if the data store is unavailable.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        Task ISession.LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove the given key from the session if present.
        /// </summary>
        /// <param name="key"></param>
        void ISession.Remove(string key)
        {
            sessionStorage.Remove(key);
        }

        /// <summary>
        /// Set the given key and value in the current session. This will throw if the session
        /// was not established prior to sending the response.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ISession.Set(string key, byte[] value)
        {
            sessionStorage[key] = value;
        }

        /// <summary>
        /// Retrieve the value of the given key, if present.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ISession.TryGetValue(string key, out byte[] value)
        {
            if (sessionStorage.ContainsKey(key))
            {
                value = Encoding.ASCII.GetBytes(sessionStorage[key].ToString());
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
