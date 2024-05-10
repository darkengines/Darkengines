using Azure.Identity;
using Darkengines.Authentication;
using Darkengines.Users.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.WebSockets {
    public class MessagingSystem {
        protected static ISet<MessagingClient> EmptyClientSet = new HashSet<MessagingClient>();
        protected ConcurrentDictionary<User, ISet<MessagingClient>> Clients { get; }
        public MessagingSystem() {
            Clients = new ConcurrentDictionary<User, ISet<MessagingClient>>();
        }
        public void AddClient(MessagingClient client) {
            Clients.AddOrUpdate(client.Identity, new HashSet<MessagingClient> { client }, (identity, set) => {
                if (!set.Contains(client)) { set.Add(client); }
                return set;
            });
        }
        public void RemoveClient(MessagingClient client) {
            Clients.AddOrUpdate(client.Identity, new HashSet<MessagingClient> { client }, (identity, set) => {
                set.Remove(client);
                return set;
            });
        }
        public ISet<MessagingClient> GetClients(User identity) {
            if (!Clients.TryGetValue(identity, out var clients)) return EmptyClientSet;
            return clients;
        }
        public IEnumerable<MessagingClient> GetClients() {
            foreach (var set in Clients.Values) {
                foreach (var client in set) {
                    yield return client;
                }
            }
        }
    }
}
