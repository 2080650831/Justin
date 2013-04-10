﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Justin.Core
{
    public class AddinConfig
    {
        public MenuConfig Menu { get; set; }
    }
    public class MenuConfig
    {
        [XmlArray("New")]
        public List<MenuItem> NewItems { get; set; }
        [XmlArray("Open")]
        public List<MenuItem> OpenItems { get; set; }
        [XmlArray("Tools")]
        public List<MenuItem> ToolsItems { get; set; }
    }

    public class MenuItem
    {
        [XmlAttribute]
        public string Icon { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Text { get; set; }
        [XmlAttribute]
        public bool IsFile { get; set; }
        [XmlAttribute]
        public string Extensions { get; set; }
        [XmlAttribute]
        public string Class { get; set; }
    }
}
