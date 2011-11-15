using System;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;
using Feedbook.Model;
using System.IO;
using System.Text;
using Feedbook.Store;
using System.Runtime.Serialization;
using CoreSystem.RefTypeExtension;
using System.Collections.Generic;
using Feedbook.Logging;
using Feedbook.Helper;

namespace Feedbook
{
    internal static class DataStore
    {
        private const string CHANNELS_KEY = "CHANNELS";

        private const string TWITTER_ACCOUNTS_KEY = "TWITTER_ACCOUNTS";

        private const string GBUZZ_ACCOUNTS_KEY = "GBUZZ_ACCOUNTS";

        private const string PINNED_CATEGORIES_KEY = "PINNED_CATEGORIES";        

        public readonly static ObservableCollection<Channel> Channels;

        public readonly static ObservableCollection<Account> TwitterAccounts;

        public readonly static ObservableCollection<Account> GBuzzAccounts;

        public readonly static ObservableCollection<CategoryFeed> FavoriteCategories;       

        static DataStore()
        {
            Channels = GetObject<ObservableCollection<Channel>>(CHANNELS_KEY) ?? new ObservableCollection<Channel>();
            TwitterAccounts = GetObject<ObservableCollection<Account>>(TWITTER_ACCOUNTS_KEY) ?? new ObservableCollection<Account>();
            GBuzzAccounts = GetObject<ObservableCollection<Account>>(GBUZZ_ACCOUNTS_KEY) ?? new ObservableCollection<Account>();
            FavoriteCategories = GetObject<ObservableCollection<CategoryFeed>>(PINNED_CATEGORIES_KEY) ?? new ObservableCollection<CategoryFeed>();
        }

        public static void Save()
        {
            SaveObject<ObservableCollection<Channel>>(CHANNELS_KEY, Channels);
            SaveObject<ObservableCollection<Account>>(TWITTER_ACCOUNTS_KEY, TwitterAccounts);
            SaveObject<ObservableCollection<Account>>(GBUZZ_ACCOUNTS_KEY, GBuzzAccounts);
            SaveObject<ObservableCollection<CategoryFeed>>(PINNED_CATEGORIES_KEY, FavoriteCategories);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static T GetObject<T>(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.ContainsKey(key))
            {
                string serializedObject = IsolatedStorageSettings.ApplicationSettings[key].ToString();
                try { return Util.Deserialize<T>(serializedObject); }
                catch { }
            }

            return default(T);
        }

        public static void SaveObject<T>(string key, T objectToSave)
        {
            string serializedObject = Util.Serialize(objectToSave);
            IsolatedStorageSettings.ApplicationSettings[key] = serializedObject;
        }         
    }
}
