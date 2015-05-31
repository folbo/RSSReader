﻿using System;
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
                Link = new Link[]
                {
                    new Link()
                    {
                        Title = "Google",
                        Href = @"http://www.google.pl"
                    }
                },
                Author = new Person[]
                {
                    new Person()
                    {
                        Name = "STurski",
                        Email = "qaz@mailnesia.com"
                    }
                },
               Category = new Category[]
               {
                   new Category()
                   {
                       Label = "Kosiarki",
                       Scheme = "scheme",
                       Term = "term"
                   }
               }
            };


            serializer.Serialize(Console.Out, atom);

            Console.ReadKey();
        }


        private static void Deserialize()
        {
            var serializer = new XmlSerializer(typeof (Atom), @"http://www.w3.org/2005/Atom");


            Stream stream = new FileStream(@"..\..\AtomExample.xml", FileMode.Open);

            var atom = serializer.Deserialize(stream);


            Console.ReadKey();
        }
    }
}