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
using System.Windows.Data;
using Feedbook.Model;
using System.Threading;
using Feedbook.Services;
using Feedbook.Helper;
using System.Collections.ObjectModel;
using CoreSystem.RefTypeExtension;

namespace Feedbook.Automation
{
    internal class GetFriendListConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var friends = new ObservableCollection<ISocialUser>();
            var dispatcher = Util.GetDispatcher();
            var account = value as Account;
            if (account != null)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                       (object o) =>
                       {
                           try
                           {
                               var service = SocialService.GetSocialService(account);
                               var friendList = service.GetFriends();
                               dispatcher.BeginInvoke(new Action(() => friends.AddRange(friendList)));
                           }
                           catch (Exception ex)
                           {
                              dispatcher.BeginInvoke(new Action(() => FBMessageBox.ShowError("WOops! error occurred while downloading friends list", ex)));
                           }
                       }));
            }

            return friends;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
