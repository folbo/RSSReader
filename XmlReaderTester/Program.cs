using System;
using System.Text;
using System.Xml.Serialization;
using ATOMUltimate.Model;

namespace XmlReaderTester
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (Atom));

            var atom = new Atom()
            {
                Title = "asd"
            };


            serializer.Serialize(Console.Out, atom);

            Console.ReadKey();
        }
    }
}
