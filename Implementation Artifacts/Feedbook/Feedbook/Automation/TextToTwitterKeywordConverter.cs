using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Web;
using System.Xml.Linq;
using CoreSystem.RefTypeExtension;
using Feedbook.Model;
using Feedbook.Helper;

namespace Feedbook.Automation
{
    internal class TextToTwitterKeywordConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueStr = value as string;
            if (valueStr != null)
            {
                try
                {
                    var lines = valueStr.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = SplitAction(lines[i], ' ', (spaceWord) =>
                                   SplitAction(spaceWord, ',', (commaWord) =>
                                   SplitAction(commaWord, ';', (semiColonWord) =>
                                   SplitAction(semiColonWord, '=', (equalWord) =>
                                   SplitAction(equalWord, '>', (greaterWord) =>
                                   SplitAction(greaterWord, '<', (lessWord) =>
                                       {
                                           var words = lessWord.Split('|');
                                           //------
                                           StringBuilder lineBuilder = new StringBuilder("<Run Text=\"{}");
                                           for (int j = 0; j < words.Length; j++)
                                           {
                                               var word = words[j];
                                               if (word != null && word.Length > 1 && word[0] == '@')
                                               {
                                                   lineBuilder.Append("\"/>");
                                                   lineBuilder.Append("<Hyperlink NavigateUri=\"{}");
                                                   lineBuilder.Append(HttpUtility.HtmlEncode(word.Substring(1)));
                                                   lineBuilder.Append("\" TargetName=\"TwitterProfile\"><Run Text=\"{}");
                                                   lineBuilder.Append(HttpUtility.HtmlEncode(word.Substring(1)));
                                                   lineBuilder.Append("\"/></Hyperlink><Run Text=\"{}");
                                               }
                                               else if (word != null && word.Length > 1 && word[0] == '#')
                                               {
                                                   lineBuilder.Append("\"/>");
                                                   lineBuilder.Append("<Hyperlink NavigateUri=\"");
                                                   lineBuilder.Append(HttpUtility.HtmlEncode(word));
                                                   lineBuilder.Append("\" TargetName=\"TwitterSearch\"><Run Text=\"{}");
                                                   lineBuilder.Append(HttpUtility.HtmlEncode(word));
                                                   lineBuilder.Append("\"/></Hyperlink><Run Text=\"{}");
                                               }
                                               else if (word != null && (word.StartsWith("http://") || word.StartsWith("https://")) && Uri.IsWellFormedUriString(word, UriKind.RelativeOrAbsolute))
                                               {
                                                   lineBuilder.Append("\"/>");
                                                   lineBuilder.Append("<Hyperlink NavigateUri=\"{}");
                                                   lineBuilder.Append(HttpUtility.HtmlEncode(word));
                                                   lineBuilder.Append("\"><Run Text=\"{}");
                                                   lineBuilder.Append(HttpUtility.HtmlEncode(word));
                                                   lineBuilder.Append("\"/></Hyperlink><Run Text=\"{}");
                                               }
                                               else
                                               {
                                                   if (j > 0) lineBuilder.Append("|");
                                                   lineBuilder.Append(HttpUtility.HtmlEncode(word));
                                               }
                                           }
                                           lineBuilder.Append("\"/>");
                                           return lineBuilder.ToString();
                                       }
                                       ))))));
                    }

                    string xamlString = "<TextBlock xml:space=\"preserve\" TextWrapping=\"Wrap\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" SnapsToDevicePixels=\"True\" Background=\"{x:Null}\">" + string.Join("<LineBreak/>", lines) + "</TextBlock>";

                    try
                    {
                        MemoryStream memSTream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xamlString));
                        return XamlReader.Load(memSTream);
                    }
                    catch (Exception ex)
                    {
                        ex.Data["Feed"] = value;
                        ex.Data["FeedXAML"] = xamlString;
                        this.Log("Error occurred while loading twitter feed after converting to XAML", ex);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data["Value"] = value;
                    this.Log("Error occurred while converting text of twitter feed into XAML", ex);
                }
            }

            try { return Content.ToPainText(value as string); }
            catch { }

            return value;
        }

        private string SplitAction(string value, char seperator, Func<string, string> action)
        {
            var words = value.Split(seperator);
            for (int i = 0; i < words.Length; i++)
                words[i] = action(words[i]);

            return string.Join(HttpUtility.HtmlEncode(seperator.ToString()), words);
        }



        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
