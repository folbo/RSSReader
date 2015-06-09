﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
using ATOMUltimate.Model;

namespace ATOMUltimate
{
    public static class SubscriptionManager
    {
        public static List<Atom> Feeds = new List<Atom>();

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

        private static Atom DeserializeContent(Stream content)
        {
            var serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");
            var atom = (Atom) serializer.Deserialize(content);
            return atom;
        }

        private static Atom ParseFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");
            var fs = new FileStream(filePath, FileMode.Open);

            var atom = (Atom) serializer.Deserialize(fs);

            return atom;
        }

        public static void SaveFeedToFile(Atom feed)
        {
            if (feed == null) return;

            if (!Directory.Exists(RelativePath))
                Directory.CreateDirectory(RelativePath);

            string filename = feed.Title;
            string filepath = RelativePath + filename;
            FileStream fs = new FileStream(filepath, FileMode.Create);

            XmlSerializer serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");

            serializer.Serialize(fs, feed);

            fs.Close();
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}