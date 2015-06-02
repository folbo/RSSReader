using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Windows;
using System.Xml.Serialization;
using ATOMUltimate.Model;

namespace ATOMUltimate
{
    public static class SubscriptionManager
    {
        public static List<Atom> Feeds = new List<Atom>();

        public static void Subscribe(string url)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, "temp.xml");
                }
                catch (Exception)
                {
                    MessageBox.Show("can't download feed.");
                }

                //udpate collection

                FileStream fs = new FileStream("temp.xml",FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                string content = sr.ReadToEnd();
                var feed = ParseContent(content);
                Feeds.Add(feed);

                SaveFeedToFile(feed);
            }
        }

        private static Atom ParseContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            var serializer = new XmlSerializer(typeof(Atom), @"http://www.w3.org/2005/Atom");
            var stream = GenerateStreamFromString(content);

            var atom = (Atom)serializer.Deserialize(stream);

            return atom;
        }

        public static void SaveFeedToFile(Atom feed)
        {
            if (feed == null) return;

            string relativePath = "feedbase/";
            if (!Directory.Exists(relativePath))
                Directory.CreateDirectory(relativePath);

            string filename = "_" + feed.Title;
            string filepath = relativePath + filename;
            FileStream fs = new FileStream(filepath, FileMode.Create);

            XmlSerializer serializer = new XmlSerializer(typeof(Atom), @"http://www.w3.org/2005/Atom");

            serializer.Serialize(fs, feed);
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
