using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using ATOMUltimate.Model;

namespace XmlReaderTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Serialize();
            Deserialize();
        }

        private static void Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");


            var atom = new Atom()
            {
                Title = "asd",
                Link = new Link()
                {
                    Name = "Google",
                    Url = @"http://www.google.pl"
                },
                Author = new Author()
                {
                    Name = "STurski",
                    Email = "qaz@mailnesia.com"
                },
                Description = "opis",
                Generator = "Renia lewa i prawa",
                Language = "pl-PL",
                LastBuildDate = DateTime.Today,
                Updated = DateTime.Today,
                Items = new[]
                {
                    new Item() {Title = "pierwszy"},
                    new Item() {Title = "drugi"},
                }
            };


            serializer.Serialize(Console.Out, atom);

            Console.ReadKey();
        }


        private static void Deserialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");


            Stream stream = new FileStream(@"..\..\AtomExample.xml", FileMode.Open);

            var atom = serializer.Deserialize(stream);


            Console.ReadKey();
        }
    }
}