using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using ATOMUltimate.Model;

namespace ATOMUltimate
{
    public static class SubscriptionManager
    {
        public static List<Atom> Feeds;

        public static void AddFeed(string url)
        {
            using (var client = new WebClient())
            {
                var source = client.DownloadString(url);
                var feed = ParseContent(source);
                Feeds.Add(feed);
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
