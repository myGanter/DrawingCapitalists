using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.AppState
{
    public class AppObjects : IEnumerable<KeyValuePair<long, AppObject>>
    {
        private readonly Dictionary<long, AppObject> ObjectCache;

        private readonly Dictionary<string, long> ConnectionObjectCache;

        public AppObjects()
        {
            ObjectCache = new Dictionary<long, AppObject>();
            ConnectionObjectCache = new Dictionary<string, long>();
        }

        public AppObject this [long id]
        {
            get
            {
                ObjectCache.TryGetValue(id, out AppObject res);
                return res;
            }
        }

        public AppObject this [string connection]
        {
            get
            {
                if (ConnectionObjectCache.TryGetValue(connection, out long id))
                {
                    ObjectCache.TryGetValue(id, out AppObject res);
                    return res;
                }

                return null;
            }
        }

        private void AddConnectionObject(AppObject obj, string connection)
        {
            lock (ConnectionObjectCache)
            {
                ConnectionObjectCache.Add(connection, obj.Id);
            }
        }

        private void RemoveConnectionObject(AppObject obj, string connection)
        {
            lock (ConnectionObjectCache)
            {
                ConnectionObjectCache.Remove(connection);
            }
        }

        public long AddObject(AppObject obj)
        {
            lock (ObjectCache)
            {
                long id = 1;

                if (ObjectCache.Count > 0)
                {
                    id = ObjectCache.Max(x => x.Key) + 1;
                }

                obj.Id = id;
                obj.OnConnectionAdd += AddConnectionObject;
                obj.OnConnectionRemove += RemoveConnectionObject;

                ObjectCache.Add(id, obj);

                return id;
            }
        }

        public void RemoveObject(long id)
        {
            lock (ObjectCache)
            {
                ObjectCache.Remove(id);
            }
        }

        public IEnumerator<KeyValuePair<long, AppObject>> GetEnumerator()
        {
            lock (ObjectCache)
            {
                foreach (var i in ObjectCache)
                    yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
