using System.Configuration;

namespace DataStructures
{
    public class BookSection : ConfigurationSection
    {
        [ConfigurationProperty("books")]
        [ConfigurationCollection(typeof(BookCollection), AddItemName = "add")]
        public BookCollection Books
        {
            get { return (BookCollection)this["books"]; }
        }
    }

    public class BookCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new BookElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BookElement)element).Number;
        }
    }

    public class BookElement : ConfigurationElement
    {
        [ConfigurationProperty("shortName", IsRequired = true)]
        public string ShortName
        {
            get { return (string)this["shortName"]; }
            set { this["shortName"] = value; }
        }

        [ConfigurationProperty("longName", IsRequired = true)]
        public string LongName
        {
            get { return (string)this["longName"]; }
            set { this["longName"] = value; }
        }

        [ConfigurationProperty("number", IsRequired = true)]
        public int Number
        {
            get { return (int)this["number"]; }
            set { this["number"] = value; }
        }

        [ConfigurationProperty("color", IsRequired = true)]
        public string Color
        {
            get { return (string)this["color"]; }
            set { this["color"] = value; }
        }

        [ConfigurationProperty("chapters", IsRequired = true)]
        public string Chapters
        {
            get { return (string)this["chapters"]; }
            set { this["chapters"] = value; }
        }
    }
}
