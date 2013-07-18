﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Justin.BI.OLAP.Entities
{
    public class Cube
    {
        public Cube() : this("", "") { }
        public Cube(string id)
            : this(id, id)
        {

        }
        public Cube(string id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.Measures = new List<Measure>();
            this.Dimensions = new List<Dimension>();
        }
        public string ID { get; private set; }
        public string Name { get; private set; }
        public List<Measure> Measures { get; set; }
        public List<Dimension> Dimensions { get; set; }

        public string TableName { get; set; }
    }
    public class Measure
    {
        public Measure() : this("", "") { }
        public Measure(string id)
            : this(id, id)
        {

        }
        public Measure(string id, string name)
        {
            this.ID = id;
            this.Name = name;
            AggregationFunction = Microsoft.AnalysisServices.AggregationFunction.Sum;
        }
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string ColumnName { get; set; }
        public Microsoft.AnalysisServices.AggregationFunction AggregationFunction { get; set; }
    }
    public class Dimension
    {
        public Dimension() : this("", "") { }
        public Dimension(string id)
            : this(id, id)
        {

        }
        public Dimension(string id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.Hierarchies = new List<Hierarchy>();
            this.Levels = new List<Level>();
        }
        public string ID { get; private set; }
        public string Name { get; private set; }


        public List<Level> Levels { get; set; }
        public List<Hierarchy> Hierarchies { get; set; }

        public string FKColumn { get; set; }
    }

    public class Hierarchy
    {
        public Hierarchy() : this("", "") { }
        public Hierarchy(string id)
            : this(id, id)
        {

        }
        public Hierarchy(string id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.Levels = new List<Level>();
        }
        public string ID { get; private set; }

        public string Name { get; private set; }

        public List<Level> Levels { get; set; }
    }

    public class Level
    {
        public Level() : this("", "") { }
        public Level(string id)
            : this(id, id)
        {

        }
        public Level(string id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string SourceTable { get; set; }


        public string KeyColumn { get; set; }
        public string NameColumn { get; set; }
        public string OrderColumn { get; set; }

    }

    public class Solution
    {
        public Solution() : this("", "") { }
        public Solution(string id)
            : this(id, id)
        {

        }
        public Solution(string id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.Cubes = new List<Cube>();
        }
        public string ID { get; private set; }
        public string Name { get; private set; }
        public List<Cube> Cubes { get; set; }

    }
}
