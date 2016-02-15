using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server29._10
{
    
    [XmlRootAttribute("visible_objects")]
    public class VisibleObjects : BaseCommand
    {
        public enum types { WALL };
        public class MapObject
        {
            private int row;
            private int col;
            private types type;

            [XmlElement("type")]
            public types Type
            {
                get { return type; }
                set { type = value; }
            }

            [XmlElement("row")]
            public int Row
            {
                get { return row; }
                set { row = value; }
            }

            [XmlElement("col")]
            public int Col
            {
                get { return col; }
                set { col = value; }
            }

            public MapObject(types t, int row, int col)
            {
                this.type = t;
                this.row = row;
                this.col = col;
            }
            public MapObject()
            {
                this.type = types.WALL;
                this.row = 0;
                this.col = 0;
            }
        }

        private List<MapObject> mapObjects;
        [XmlArray("map_objects")]
        [XmlArrayItem("map_object")]
        public List<MapObject> MapObjects
        {
            get {return mapObjects;}
        }

        public VisibleObjects()
        {
            id = 8;
            mapObjects = new List<MapObject>();
        }
        public VisibleObjects(List<MapObject> list)
        {
            id = 8;
            mapObjects = list;
        }           
    }
}
