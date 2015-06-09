using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ATOMUltimate.Model
{
    [XmlRoot(ElementName = "feed")]
    public class Atom
    {
        [XmlElement(ElementName = "author")]
        public Person[] Author { get; set; }

        [XmlElement(ElementName = "category")]
        public Category[] Category { get; set; }

        [XmlElement(ElementName = "contributor")]
        public Person[] Contributor { get; set; }

        [XmlElement(ElementName = "generator")]
        public Generator Generator { get; set; }

        [XmlElement(ElementName = "icon")]
        public string Icon { get; set; }

        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "link")]
        public Link[] Link { get; set; }

        [XmlElement(ElementName = "logo")]
        public string Logo { get; set; }

        [XmlElement(ElementName = "rights")]
        public string Rights { get; set; }

        [XmlElement(ElementName = "subtitle")]
        public string Subtitle { get; set; }

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "updated")]
        public DateTime Updated { get; set; }
        
        [XmlElement(ElementName = "entry")]
        public Entry[] Entries { get; set; }
    }

    public class Link
    {
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }

        [XmlAttribute(AttributeName = "rel")]
        public string Rel { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "hreflang")]
        public string HrefLang { get; set; }

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "length")]
        public uint Length { get; set; }
    }

    public class Person
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "uri")]
        public string Uri { get; set; }

        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
    }

    public class Entry
    {
        [XmlElement(ElementName = "author")]
        public Person[] Author { get; set; } //done

        [XmlElement(ElementName = "category")]
        public Category[] Category { get; set; } //done

        [XmlElement(ElementName = "content")]
        public Content Content { get; set; } //done

        [XmlElement(ElementName = "contributor")]
        public Person[] Contributor { get; set; } //done

        [XmlElement(ElementName = "id")]
        public string Id { get; set; } //done

        [XmlElement(ElementName = "link")]
        public Link[] Link { get; set; } //done

        [XmlElement(ElementName = "published")]
        public DateTime? Published { get; set; } //done

        public bool PublishedSpecified
        {
            get
            {
                return Published.HasValue;
            }
        }

        [XmlElement(ElementName = "rights")]
        public string Rights { get; set; } //done

        [XmlElement(ElementName = "source")]
        public string Source { get; set; } //done

        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; } //done

        [XmlElement(ElementName = "title")]
        public string Title { get; set; } //done

        [XmlElement(ElementName = "updated")]
        public DateTime Updated { get; set; } //done
    }

    [Serializable]
    public class Content : IXmlSerializable
    {
        [XmlAttribute(AttributeName = "type")]
        public TextTypeType? Type { get; set; }

        [XmlAttribute(AttributeName = "src")]
        public string Src { get; set; }
        
        [XmlText]
        public string Value { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            string type = reader.GetAttribute("type");
            switch (type)
            {
                case "text":
                    Type = TextTypeType.text;
                    break;
                case "html":
                    Type = TextTypeType.html;
                    break;
                case "xhtml":
                    Type = TextTypeType.xhtml;
                    break;
            }

            Src = reader.GetAttribute("src");

            Value = reader.ReadInnerXml();
        }

        public void WriteXml(XmlWriter writer)
        {
            if(Type != null)
                writer.WriteAttributeString("type",Type.ToString());
            writer.WriteAttributeString("src", Src);

            writer.WriteString(Value);
        }
    }

    public class Category
    {
        [XmlAttribute(AttributeName = "term")]
        public string Term { get; set; } //done, required

        [XmlAttribute(AttributeName = "scheme")]
        public string Scheme { get; set; } //done, optional

        [XmlAttribute(AttributeName = "label")]
        public string Label { get; set; } //done, optional
    }

    public class Generator
    {
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public enum TextTypeType
    {
        text,
        html,
        xhtml
    }
}
