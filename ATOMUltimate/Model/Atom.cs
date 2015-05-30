using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ATOMUltimate.Model
{
    [XmlRoot(ElementName = "feed")]
    public class Atom
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "link")]
        public Link Link { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "lastBuildDate")]
        public DateTime LastBuildDate { get; set; }
        [XmlElement(ElementName = "updated")]
        public DateTime Updated { get; set; }
        [XmlElement(ElementName = "language")]
        public string Language { get; set; }
        [XmlElement(ElementName = "generator")]
        public string Generator { get; set; }
        [XmlElement(ElementName = "item")]
        public Item[] Items { get; set; }

        public Author Author { get; set; }

    }

    public class Link
    {
        [XmlAttribute(AttributeName = "href")]
        public string Url { get; set; }
        [XmlText]
        public string Name { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Item
    {
        public string Title { get; set; }
        public Link Link { get; set; }
    }
}
