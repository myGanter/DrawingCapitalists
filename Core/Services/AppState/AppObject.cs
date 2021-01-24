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

        public event Action<AppObject, string> OnConnectionRemove;

        public long Id { get; set; }

        protected readonly Dictionary<UserStruct, string> UsersCache;

        public AppObject()
        {
            UsersCache = new Dictionary<UserStruct, string>();
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

                if (past.IsNotNull())
                    OnConnectionRemove?.Invoke(this, past);

                UsersCache[user] = value;

                if (value.IsNotNull())
                    OnConnectionAdd?.Invoke(this, value);
            }
        }

        public void AddUser(UserStruct user, string connection)
        {
            lock (UsersCache)
            {
                UsersCache.Add(user, connection);
            }

            if (connection.IsNotNull())
                OnConnectionAdd?.Invoke(this, connection);
        }

        public void RemoveUser(UserStruct user)
        {
            var connection = UsersCache[user];

            lock (UsersCache)
            {
                UsersCache.Remove(user);
            }

            if (connection.IsNotNull())
                OnConnectionRemove?.Invoke(this, connection);
        }

        public abstract bool IsEmpty();

        public abstract Task NotifyDeath();

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
