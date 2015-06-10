using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;
using ATOMUltimate.Model;

namespace ATOMUltimate
{
    public static class SubscriptionManager
    {
        public static List<Atom> Feeds = new List<Atom>();
        public static bool ShouldSync = true;

        private const string RelativePath = "feedbase/";

        public static void Initialize()
        {
            if (!Directory.Exists(RelativePath))
            {
                Directory.CreateDirectory(RelativePath);
                return; // nie było katalogu więc i plików nie będzie
            }

            //load saved feeds into list
            var feedFiles = Directory.GetFiles(RelativePath);

            foreach (var file in feedFiles)
            {
                Feeds.Add(ParseFile(file));
            }
        }

        private static readonly Regex UrlRegex =
            new Regex(
                @"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");

        private static bool CheckUri(string word)
        {
            if (word.IndexOfAny(@":.\/".ToCharArray()) != -1)
            {
                if (Uri.IsWellFormedUriString(word, UriKind.Absolute))
                {
                    // The string is an Absolute URI
                    return true;
                }
                else if (UrlRegex.IsMatch(word))
                {
                    Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri)
                    {
                        // rebuild it it with http to turn it into an Absolute URI
                        uri = new Uri(@"http://" + word, UriKind.Absolute);
                    }

                    if (uri.IsAbsoluteUri)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void Subscribe(string url)
        {
            if (!CheckUri(url))
            {
                throw new Exception();
            }

            Uri uri = new UriBuilder(url).Uri;
            using (var client = new WebClient())
            {
                Stream stream = client.OpenRead(uri);

                //udpate collection

                var feed = DeserializeContent(stream);
                stream.Close();
                if (Feeds.Select(atom => atom.Title).Contains(feed.Title))
                {
                    return;
                }
                if (feed.Link.FirstOrDefault(link => link.Rel.ToLower() == "self") == null)
                {
                    feed.Link.Add(new Link()
                    {
                        Rel = "self",
                        Href = uri.AbsoluteUri
                    });
                }


                Feeds.Add(feed);

                SaveFeedToFile(feed);
            }
        }

        public static void Unsubscribe(Atom atom)
        {
            Feeds.Remove(atom);
            File.Delete(RelativePath+RemoveSpecialCharacters(atom.Title));
        }

        private static Atom DeserializeContent(Stream content)
        {
            //todo sprawdzanie czy pasuje do xsd
            var serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");
            var atom = (Atom) serializer.Deserialize(content);
            return atom;
        }

        private static Atom ParseFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                return DeserializeContent(fs);
            }
        }

        public static void SaveFeedToFile(Atom feed)
        {
            if (feed == null) return;

            if (!Directory.Exists(RelativePath))
                Directory.CreateDirectory(RelativePath);

            string filename = RemoveSpecialCharacters(feed.Title);
            string filepath = RelativePath + filename;
            FileStream fs = new FileStream(filepath, FileMode.Create);

            XmlSerializer serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");

            serializer.Serialize(fs, feed);

            fs.Close();
        }

        public static void Sync(Atom feed)
        {
            //znajdź link do feeda  (rel="self")
            var link = feed.Link.FirstOrDefault(l => l.Rel == "self");
            if (link == null)
            {
                throw new Exception("Nie można odnaleźć linku do feeda.");
            }

            string url = link.Href;

            if (!CheckUri(url))
            {
                throw new Exception();
            }

            Uri uri = new UriBuilder(url).Uri;

            //pobierz najnowszy content ze strony
            using (var client = new WebClient())
            {
                try
                {
                    Stream stream;
                    stream = client.OpenRead(uri);
                    var update = DeserializeContent(stream);

                    stream.Close();

                    //wydłub entry których jeszcze nie ma w pliku
                    var newEntries = update.Entries.Where(e => feed.Entries.All(o => o.Id != e.Id));
                    feed.Entries.InsertRange(0, newEntries);
                }
                catch (Exception)
                {
                    MessageBox.Show("Nie można się połączyć z serwerem. Sprawdź połączenie internetowe ");
                }
            }

            //zapisz plik
            SaveFeedToFile(feed);
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}