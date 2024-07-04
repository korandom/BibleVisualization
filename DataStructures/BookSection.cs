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
            return ((BookElement)element).ShortName;
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
    }
}
