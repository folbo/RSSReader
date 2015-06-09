using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            //load saved feeds into list
            var feedFiles = Directory.GetFiles(RelativePath);

            foreach (var file in feedFiles)
            {
                Feeds.Add(ParseFile(file));
            }
        }

        public static void Subscribe(string url)
        {
            using (var client = new WebClient())
            {
                StreamReader sr;
                try
                {
                    //if()
                    Stream stream = client.OpenRead(url);
                    sr = new StreamReader(stream);
                }
                catch (Exception)
                {
                    throw;
                }

                //udpate collection

                string content = sr.ReadToEnd();
                sr.Close();

                var feed = DeserializeContent(content);
                Feeds.Add(feed);

                SaveFeedToFile(feed);
            }
        }

        private static Atom DeserializeContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            var serializer = new XmlSerializer(typeof(Atom), @"http://www.w3.org/2005/Atom");
            var stream = GenerateStreamFromString(content);
            
            var atom = (Atom)serializer.Deserialize(stream);

            return atom;
        }

        private static Atom ParseFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var serializer = new XmlSerializer(typeof(Atom), @"http://www.w3.org/2005/Atom");
            var fs = new FileStream(filePath, FileMode.Open);

            var atom = (Atom)serializer.Deserialize(fs);

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

            XmlSerializer serializer = new XmlSerializer(typeof(Atom), @"http://www.w3.org/2005/Atom");

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
