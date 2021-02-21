using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

using Core.Models;
using Core.Expansions;

namespace Core.Services.AppState
{
    public abstract class AppObject : IEnumerable<KeyValuePair<UserStruct, string>>
    {
        public event Action<AppObject, string> OnConnectionAdd;

        public event Action<UserStruct, string> OnConnectionAddOtherData;

        public event Action<AppObject, string> OnConnectionRemove;

        public event Action<UserStruct, string> OnConnectionRemoveOtherData;

        public long Id { get; set; }

        protected readonly Dictionary<UserStruct, string> UsersCache;

        protected readonly DictionaryKeyKey<UserStruct?, long?> UserIdCache;
        protected long UserIdCounter;

        public AppObject()
        {
            UsersCache = new Dictionary<UserStruct, string>();
            UserIdCache = new DictionaryKeyKey<UserStruct?, long?>();
            UserIdCounter = 0;
        }

        public int Count => UsersCache.Count;

        public string this [UserStruct user]
        {
            get
            {
                return UsersCache[user];
            }

            set
            {
                var past = UsersCache[user];

                UsersCache[user] = value;

                if (past.IsNotNull())
                {
                    OnConnectionRemove?.Invoke(this, past);
                    OnConnectionRemoveOtherData?.Invoke(user, past);
                }                                

                if (value.IsNotNull())
                {
                    OnConnectionAdd?.Invoke(this, value);
                    OnConnectionAddOtherData?.Invoke(user, value);
                }
            }
        }

        public long? GetId(UserStruct user)
        {
            return UserIdCache[user];
        }

        public UserStruct? GetUser(long id)
        {
            return UserIdCache[id];
        }

        public bool ContainsUser(UserStruct user)
        {
            return UsersCache.ContainsKey(user);
        }

        public void AddUser(UserStruct user, string connection)
        {
            lock (UsersCache)
            {
                UsersCache.Add(user, connection);

                UserIdCounter++;
                UserIdCache.Add(user, UserIdCounter);
            }

            if (connection.IsNotNull())
            {
                OnConnectionAdd?.Invoke(this, connection);
                OnConnectionAddOtherData?.Invoke(user, connection);
            }
        }

        public void RemoveUser(UserStruct user)
        {
            var connection = UsersCache[user];

            lock (UsersCache)
            {
                UsersCache.Remove(user);

                UserIdCache.Remove(user);
            }

            if (connection.IsNotNull())
            {
                OnConnectionRemove?.Invoke(this, connection);
                OnConnectionRemoveOtherData?.Invoke(user, connection);
            }
        }

        public abstract bool IsEmpty();

        public abstract Task NotifyDeath();

        public IEnumerable<string> GetOthersConnections(UserStruct user)
        {
            return this.Where(x => !x.Key.Equals(user) && x.Value.IsNotNull()).Select(x => x.Value);
        }

        public IEnumerable<string> GetAllConnections()
        {
            return this.Where(x => x.Value.IsNotNull()).Select(x => x.Value);
        }

        public IEnumerator<KeyValuePair<UserStruct, string>> GetEnumerator()
        {
            lock (UsersCache)
            {
                foreach (var i in UsersCache)
                    yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
