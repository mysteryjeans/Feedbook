using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace Feedbook.Helper
{
    internal static class PageExtension
    {
        private static readonly Dictionary<NavigationService, Dictionary<string, object>> sessions = new Dictionary<NavigationService, Dictionary<string, object>>();

        public static void SetSession(this Page page, string key, object value)
        {
            if (!sessions.ContainsKey(page.NavigationService))
                sessions[page.NavigationService] = new Dictionary<string, object>();

            sessions[page.NavigationService][key] = value;
        }

        public static object GetSession(this Page page, string key)
        {
            Dictionary<string, object> session;
            if (sessions.ContainsKey(page.NavigationService) && (session = sessions[page.NavigationService]).ContainsKey(key))
                return session[key];

            return null;
        }
    }
}
