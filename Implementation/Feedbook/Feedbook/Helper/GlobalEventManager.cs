using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Feedbook.Model;
using System.Collections.Specialized;
using CoreSystem.RefTypeExtension;
using System.Collections.ObjectModel;
using Feedbook.Services;

namespace Feedbook.Helper
{
    internal class FeedEventArgs : EventArgs
    {
        public FeedSource Source { get; private set; }

        public Account Account { get; private set; }

        public Channel Channel { get; private set; }

        public Feed Feed { get; private set; }

        public FeedEventArgs(FeedSource source, Account account, Channel channel, Feed feed)
        {
            this.Source = source;
            this.Account = account;
            this.Channel = channel;
            this.Feed = feed;
        }
    }

    internal delegate void FeedEventHandler(object sender, FeedEventArgs e);

    internal static class GlobalEventManager
    {
        public static event FeedEventHandler OnFeedAdd;

        public static event FeedEventHandler OnFeedRemove;    

        public static void Initialize()
        {
            Add(FeedSource.RSS, DataStore.Channels);
            Add(FeedSource.Twitter, DataStore.TwitterAccounts);
            Add(FeedSource.GoogleBuzz, DataStore.GBuzzAccounts);
        }

        private static void Add(FeedSource source, ObservableCollection<Account> collection)
        {
            collection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
                      {
                          if (e.NewItems != null)
                              foreach (Account account in e.NewItems)
                                  Add(source, account);

                          if (e.OldItems != null)
                              foreach (Account account in e.OldItems)
                                  Remove(source, account);
                      };

            collection.ForEach(account => Add(source, account));
        }

        private static void Add(FeedSource source, ObservableCollection<Channel> collection)
        {
            collection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                    foreach (Channel channel in e.NewItems)
                        Add(source, null, channel);

                if (e.OldItems != null)
                    foreach (Channel channel in e.OldItems)
                        Remove(source, null, channel);
            };

            collection.ForEach(channel => Add(source, null, channel));
        }

        private static void Add(FeedSource source, Account account)
        {
            account.Channels.CollectionChanged += Handler(source, account);
            account.Channels.ForEach(channel => Add(source, account, channel));
        }

        private static void Remove(FeedSource source, Account account)
        {
            account.Channels.CollectionChanged -= Handler(source, account);
            account.Channels.ForEach(channel => Remove(source, account, channel));
        }

        private static void Add(FeedSource source, Account account, Channel channel)
        {
            channel.Feeds.CollectionChanged += Handler(source, account, channel);
            //channel.Feeds.ForEach(feed => RaiseOnFeedAdd(channel, new FeedEventArgs(source, account, channel, feed)));
        }

        private static void Remove(FeedSource source, Account account, Channel channel)
        {
            channel.Feeds.CollectionChanged -= Handler(source, account, channel);
            //channel.Feeds.ForEach(feed => RaiseOnFeedRemove(channel, new FeedEventArgs(source, account, channel, feed)));
        }

        private static NotifyCollectionChangedEventHandler Handler(FeedSource source, Account account)
        {
            return delegate(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                    foreach (Channel channel in e.NewItems)
                        Add(source, account, channel);

                if (e.OldItems != null)
                    foreach (Channel channel in e.OldItems)
                        Remove(source, account, channel);
            };
        }

        private static NotifyCollectionChangedEventHandler Handler(FeedSource source, Account account, Channel channel)
        {
            return delegate(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                    foreach (Feed feed in e.NewItems)
                        RaiseOnFeedAdd(sender, new FeedEventArgs(source, account, channel, feed));

                if (e.OldItems != null)
                {
                    foreach (Feed feed in e.OldItems)
                        RaiseOnFeedRemove(sender, new FeedEventArgs(source, account, channel, feed));
                }
            };
        }

        private static void RaiseOnFeedAdd(object sender, FeedEventArgs e)
        {
            if (OnFeedAdd != null)
                OnFeedAdd(sender, e);
        }


        private static void RaiseOnFeedRemove(object sender, FeedEventArgs e)
        {
            if (OnFeedRemove != null)
                OnFeedRemove(sender, e);
        }
    }
}
