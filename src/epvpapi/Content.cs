﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace epvpapi
{
    /// <summary>
    /// Represents formatted vBulletin content
    /// </summary>
    public class Content 
    {
        /// <summary>
        /// Represents a content element such as spoilers, quotes, images, links...
        /// </summary>
        public class Element
        {
            /// <summary>
            /// Tag of the element that triggers the interpretation
            /// </summary>
            public string Tag { get; set; }
            public string Value { get; set; }

            /// <summary>
            /// Elements being wrapped by this element
            /// </summary>
            public List<Element> Childs { get; set; } 

            /// <summary>
            /// The plain representation that includes the element tag and the plain values of the child elements.
            /// Commonly used for posting content to the forum since vBulletin needs to interpret the raw content first
            /// </summary>
            public virtual string Plain
            {
                get { return String.Format("[{0}]{1}{2}[/{0}]", Tag, Value, String.Join(String.Empty, Childs.Select(childContent => childContent.Plain))); }
            }

            public Element(string tag = null, string value = null)
            {
                Tag = tag;
                Value = value;
                Childs = new List<Element>();
            }

            /// <summary>
            /// Tries to parse the plain element code. For example, a spoiler may be formatted this way:
            /// [spoiler]This is a spoiler[/spoiler] where the code parses the tag (spoiler) and the value within the tags
            /// </summary>
            /// <param name="input"> Text to parse </param>
            /// <param name="contentElement"> Element representing the parsed results </param>
            /// <returns> true if the input was parsed, false if the input couldn't be parsed </returns>
            public static bool TryParse(string input, out Element contentElement)
            {
                contentElement = new Element();
                var match = new Regex(@"(?:\[([a-zA-Z]+)\]){1}(.+)(?:\[\/\1\]){1}").Match(input);
                // 0 - everything, 1 - Tag, 2 - value
                if (match.Groups.Count != 3) return false;

                contentElement = new Element(match.Groups[1].Value, match.Groups[2].Value);
                return true;
            }

            /// <summary>
            /// Filters all elements and child events by the given type
            /// </summary>
            /// <typeparam name="T"> Type of the element to parse deriving from <c>Element</c> </typeparam>
            /// <returns> List of all elements that matched the given tag within all child nodes </returns>
            public List<T> Filter<T>() where T : Element, new()
            {
                var filteringElement = new T();
                var concatenatedList = new List<T>();
                foreach (var child in Childs)
                {
                    if (child.Tag.Equals(filteringElement.Tag, StringComparison.InvariantCultureIgnoreCase))
                        concatenatedList.Add(child as T);
                    concatenatedList.AddRange(child.Filter<T>());
                }

                return concatenatedList;
            }

            public class PlainText : Element
            {
                public override string Plain
                {
                    get { return (string) Value; }
                }

                public PlainText() :
                    this("")
                { }


                public PlainText(string value) :
                    base("", value)
                { }
            }

            public class ItalicText : Element
            {
                public ItalicText() :
                    this("")
                { }

                public ItalicText(string value) :
                    base("I", value)
                { }
            }

            public class UnderlinedText : Element
            {
                public UnderlinedText() :
                    this("")
                { }

                public UnderlinedText(string value) :
                    base("U", value)
                { }
            }

            public class BoldText : Element
            {
                public BoldText() :
                    this("")
                { }

                public BoldText(string value) :
                    base("B", value)
                { }
            }

            public class StruckThroughText : Element
            {
                public StruckThroughText() :
                    this("")
                { }

                public StruckThroughText(string value) :
                    base("STRIKE", value)
                { }
            }

            public class CenteredText : Element
            {
                public CenteredText() :
                    this("")
                { }

                public CenteredText(string value) :
                    base("CENTER", value)
                { }
            }

            public class LeftAlignedText : Element
            {
                public LeftAlignedText() :
                    this("")
                { }

                public LeftAlignedText(string value) :
                    base("LEFT", value)
                { }
            }

            public class RightAlignedText : Element
            {
                public RightAlignedText() :
                    this("")
                { }

                public RightAlignedText(string value) :
                    base("RIGHT", value)
                { }
            }

            public class JustifiedText : Element
            {
                public JustifiedText() :
                    this("")
                { }

                public JustifiedText(string value) :
                    base("JUSTIFY", value)
                { }
            }

            public class Spoiler : Element
            {
                public string Title { get; set; }

                public Spoiler() :
                    this("")
                { }

                public Spoiler(string value) :
                    base("spoiler", value)
                { }
            }

            public class Image : Element
            {
                public Image() :
                    this("")
                { }

                public Image(string value) :
                    base("img", value)
                { }
            }

            public class Link : Element
            {
                public Link() :
                    this("")
                { }

                public Link(string value) :
                    base("url", value)
                { }
            }

            public class GenericCode : Element
            {
                public GenericCode() :
                    this("")
                { }

                public GenericCode(string value) :
                    base("CODE", value)
                { }
            }

            public class IndentedText : Element
            {
                public IndentedText() :
                    this("")
                { }

                public IndentedText(string value) :
                    base("INDENT", value)
                { }
            }

            public class Quote : Element
            {
                public User Author { get; set; }

                public override string Plain
                {
                    get { return String.Format("[{0}={1}]{2}{3}[/{0}]", Tag, Author.Name, Value, String.Join(String.Empty, Childs.Select(childContent => childContent.Plain))); }
                }

                public Quote(User author):
                    base("quote")
                {
                    Author = author;
                }

                public Quote() :
                    this(new User())
                { }
            }
        }

        /// <summary>
        /// Contents of the post
        /// </summary>
        public List<Element> Elements { get; set; }

        public List<Element.PlainText> PlainTexts
        {
            get { return Filter<Element.PlainText>(); }
        }

        public List<Element.Spoiler> Spoilers
        {
            get { return Filter<Element.Spoiler>(); }
        }

        public List<Element.Quote> Quotes
        {
            get { return Filter<Element.Quote>(); }
        }

        public List<Element.Image> Images
        {
            get { return Filter<Element.Image>(); }
        }

        public List<Element.Link> Links
        {
            get { return Filter<Element.Link>(); } 
        }

        public List<Element.BoldText> BoldText
        {
            get { return Filter<Element.BoldText>(); }
        }

        public List<Element.ItalicText> ItalicText
        {
            get { return Filter<Element.ItalicText>(); }
        }

        public List<Element.UnderlinedText> UnderlinedText
        {
            get { return Filter<Element.UnderlinedText>(); }
        }

        public List<Element.StruckThroughText> StruckThrough
        {
            get { return Filter<Element.StruckThroughText>(); }
        }

        public Content(List<Element> elements)
        {
            Elements = elements;       
        }

        public Content(string plainStringContent):
            this(new List<Element>() { new Element.PlainText(plainStringContent)} )
        { }

        public Content() :
            this(new List<Element>())
        { }

        public List<T> Filter<T>() where T : Element, new()
        {
            var filteringElement = new T();
            var concatenatedList = new List<T>();
            foreach (var element in Elements)
            {
                if (element.Tag.Equals(filteringElement.Tag, StringComparison.InvariantCultureIgnoreCase))
                    concatenatedList.Add(element as T);
                concatenatedList.AddRange(element.Filter<T>());
            }

            return concatenatedList;
        }

        public override string ToString()
        {
            return String.Join(String.Empty, Elements.Select(content => content.Plain));
        }
    }
}
