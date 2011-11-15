using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Linq;

namespace Feedbook.Store
{
    internal class IsolatedStorageSettings : Dictionary<string, object>
    {
        private const string ISOLATED_FILE_NAME = "ApplicationSettingData.xml";

        private static IsolatedStorageSettings applicationSettings;
        public static IsolatedStorageSettings ApplicationSettings
        {
            get
            {
                if (applicationSettings == null)
                    applicationSettings = GetApplicationSettings(ISOLATED_FILE_NAME);

                return applicationSettings;
            }
        }

        private IsolatedStorageSettings() { }

        public void Save()
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = userStore.OpenFile(ISOLATED_FILE_NAME, System.IO.FileMode.Truncate))
                {
                    using (var threadSafeStream = IsolatedStorageFileStream.Synchronized(stream))
                    {
                        using (var writer = new StreamWriter(threadSafeStream))
                        {
                            var xappSettings = new XElement("applicationSettings");
                            foreach (var key in this.Keys)
                            {
                                xappSettings.Add(new XElement("add",
                                                               new XAttribute("key", key),
                                                               new XElement("value", this[key])));
                            }

                            writer.Write(xappSettings.ToString());
                            writer.Close();
                        }
                    }
                }
            }
        }

        public static IsolatedStorageSettings GetApplicationSettings(string storageFileName)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = userStore.OpenFile(storageFileName, System.IO.FileMode.OpenOrCreate))
                {
                    using (var threadSafeStream = IsolatedStorageFileStream.Synchronized(stream))
                    {
                        using (var reader = new StreamReader(threadSafeStream))
                        {
                            var data = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(data))
                            {
                                try
                                {
                                    var applicationSettings = new IsolatedStorageSettings();
                                    var xappSettings = XElement.Parse(data);
                                    foreach (var entry in xappSettings.Elements("add"))
                                    {
                                        var key = (string)entry.Attribute("key");
                                        var value = (string)entry.Element("value");
                                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                                            applicationSettings.Add(key, value);
                                    }

                                    return applicationSettings;
                                }
                                catch { }
                            }

                            return new IsolatedStorageSettings();
                        }
                    }
                }
            }
        }        
    }
}
