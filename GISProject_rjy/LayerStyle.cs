using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GISProject_rjy
{
    public class LayerStyle
    {
        /// <summary>
        /// 导入SLD生成的图层风格
        /// </summary>
            public class FeatureTypeStyle
            {
                public class Rule
                {
                public class Filter
                {
                    public string PropertyName;
                    public string Criterion; // ge, le
                    public double Literal;

                    public Filter(string pName, string criterion, double literal)
                    {
                        PropertyName = pName;
                        Criterion = criterion;
                        Literal = literal;
                    }
                }
                    public class PointSymbolizer
                    {
                        public class PolygonFill
                        {
                            public bool Enabled = false;
                            public string Color = "#FFFFFF";
                            public double Opacity = 1d;
                        }
                        public class PolygonStroke
                        {
                            public bool Enabled = false;
                            public string Color = "#000000";
                            public double Width = 1d;
                            public double Opacity = 1d;
                        }
                    public string Name;
                    public PolygonFill Fill = new PolygonFill();
                    public PolygonStroke Stroke = new PolygonStroke();
                    public string SizePropertyName;
                    }
                    public class PolygonSymbolizer
                    {
                        public class PolygonFill
                        {
                            public bool Enabled = false;
                            public string Color = "#FFFFFF";
                            public double Opacity = 1d;
                        }
                        public class PolygonStroke
                        {
                            public bool Enabled = false;
                            public string Color = "#000000";
                            public double Width = 1d;
                            public double Opacity = 1d;
                        }
                        public PolygonFill Fill = new PolygonFill();
                        public PolygonStroke Stroke = new PolygonStroke();
                    }
                public class RasterSymbolizer
                {
                    public class ColorMapEntry
                    {
                        public string Color = "FFFFFF";
                        public double Opacity = 1d;
                        public double Quantity;

                        public ColorMapEntry() { }
                        public ColorMapEntry(string color, double opacity, double quantity)
                        {
                            Color = color;
                            Opacity = opacity;
                            Quantity = quantity;
                        }
                    }
                    public bool Enabled = false;
                    List<ColorMapEntry> ColorMap = new List<ColorMapEntry>();

                    public void AddColorMapEntry(string color, double opacity, double quantity)
                    {
                        ColorMap.Add(new ColorMapEntry(color, opacity, quantity));
                    }
                }
                    public string Name;
                    public string Title;
                public PointSymbolizer PointSymbol = new PointSymbolizer();
                public PolygonSymbolizer PolygonSymbol = new PolygonSymbolizer();
                public RasterSymbolizer RasterSymbol = new RasterSymbolizer();
                public List<Filter> FiltersAnd = new List<Filter>();
                public List<Filter> FiltersOr = new List<Filter>();

                public void AddFilterAnd(string pName, string criterion, double literal)
                {
                    FiltersAnd.Add(new Filter(pName, criterion, literal));
                }
                public void AddFilterOr(string pName, string criterion, double literal)
                {
                    FiltersOr.Add(new Filter(pName, criterion, literal));
                }
            }
                public string FeatureTypeName;
                public string SemanticTypeIdentifier;
                public List<Rule> Rules = new List<Rule>();

            public void AddRule()
                {
                    Rules.Add(new Rule());
                }
            }
            public List<FeatureTypeStyle> Styles = new List<FeatureTypeStyle>();

            public void AddStyle()
            {
                Styles.Add(new FeatureTypeStyle());
            }

        
    }
}
