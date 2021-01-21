using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Data;
using System.Xml;

namespace GISProject_rjy
{
    public class MapLayer
    {
        public string Name;
        public string Type; // Tiff,Polygon,Point
        public string FilePath;
        public bool Visible;
        public float _MinX, _MinY, _MaxX, _MaxY;
        //public SymbolType _Symbol;                  //图层的符号
        public float _Symbolsize;                     //符号大小（单位-像素,点的大小或线宽）
        //public List<Geometry> _Features = new List<Geometry>();    //图层的要素列表
        public DataTable DT = new DataTable();   //图层的属性数据表
        public int FIDused = 0;                   //已使用过的FID编号
        //public Renderer _renderer;                 //图层的渲染器

        public LayerStyle Style = new LayerStyle();
        public bool _LableUsed = false;        //是否显示注记
        public string _LableField = "";        //注记字段
        public Color _LableColor = Color.Black;//注记颜色
        public float _LableSize = 8;          //注记大小

        public MapLayer(string name, string type, string filePath)
        {
            Name = name;
            FilePath = filePath;
            Visible = true;
            Type = type;
        }
        public void SetExtent(float minX, float minY, float maxX, float maxY)
        {
            _MinX = minX;
            _MinY = minY;
            _MaxX = maxX;
            _MaxY = maxY;
        }

        public void GetExtent(float[] MBR)
        {
            MBR[0] = _MinX;
            MBR[1] = _MinY;
            MBR[2] = _MaxX;
            MBR[3] = _MaxY;
        }

        // 读取SLD
        public void ReadSld(string FilePath)
        {
            XmlDocument sldDoc = new XmlDocument();
            sldDoc.Load(FilePath);
            XmlNamespaceManager nsp = new XmlNamespaceManager(sldDoc.NameTable);
            XmlElement root = sldDoc.DocumentElement;
            XmlNodeList FeatStyleList = root.ChildNodes[0].ChildNodes[1].ChildNodes;
            LayerStyle style = new LayerStyle();
            style.AddStyle();
            // 搜索每条FeatureTypeStyle
            for (int fsCount = 0; fsCount < FeatStyleList.Count; ++fsCount)
            {
                if (FeatStyleList[fsCount].Name == "sld:FeatureTypeStyle")
                {
                    XmlNodeList RuleList = FeatStyleList[fsCount].ChildNodes;
                    style.Styles[0].FeatureTypeName = RuleList[0].Value;
                    // 搜索每条rule
                    for (int rlCount = 0; rlCount < RuleList.Count; ++rlCount)
                    {
                        if (RuleList[rlCount].Name == "sld:Rule")
                        {
                            style.Styles[0].AddRule();
                            XmlNodeList Rule = RuleList[rlCount].ChildNodes;
                            for (int rCount = 0; rCount < Rule.Count; ++rCount)
                            {
                                // 规则名称
                                if (Rule[rCount].Name == "sld:Name")
                                {
                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].Name = Rule[rCount].Value;
                                }
                                // 规则标题
                                else if (Rule[rCount].Name == "sld:Title")
                                {
                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].Title = Rule[rCount].Value;
                                }
                                // 过滤器
                                else if (Rule[rCount].Name == "ogc:Filter")
                                {
                                    XmlNodeList Filters = Rule[rCount].ChildNodes;
                                    for (int fCount = 0; fCount < Filters.Count; ++fCount)
                                    {
                                        // 与
                                        if (Filters[fCount].Name == "ogc:And")
                                        {
                                            XmlNodeList Ands = Filters[fCount].ChildNodes;
                                            for (int aCount = 0; aCount < Ands.Count; ++aCount)
                                            {
                                                // 大于等于
                                                if (Ands[aCount].Name == "ogc:PropertyIsGreaterThanOrEqualTo")
                                                {
                                                    string PropertyName = Ands[aCount].ChildNodes[0].FirstChild.Value;
                                                    double Literal = Convert.ToDouble(Ands[aCount].ChildNodes[1].FirstChild.Value);
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].FiltersAnd.Add(new LayerStyle.FeatureTypeStyle.Rule.Filter(PropertyName, "ge", Literal));
                                                }
                                                // 小于等于
                                                else if (Ands[aCount].Name == "ogc:PropertyIsLessThanOrEqualTo")
                                                {
                                                    string PropertyName = Ands[aCount].ChildNodes[0].FirstChild.Value;
                                                    double Literal = Convert.ToDouble(Ands[aCount].ChildNodes[1].FirstChild.Value);
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].FiltersAnd.Add(new LayerStyle.FeatureTypeStyle.Rule.Filter(PropertyName, "le", Literal));
                                                }
                                            }
                                        }
                                        // 或
                                        else if (Filters[fCount].Name == "ogc:Or")
                                        {
                                            // 示例数据里没有
                                        }
                                    }
                                }
                                // 点标识
                                else if (Rule[rCount].Name == "sld:PointSymbolizer")
                                {
                                    XmlNodeList Symbols = Rule[rCount].FirstChild.FirstChild.ChildNodes;
                                    for (int sCount = 0; sCount < Symbols.Count; ++sCount)
                                    {
                                        if (Symbols[sCount].Name == "sld:WellKnownName")
                                        {
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Name = Symbols[sCount].Value;
                                        }
                                        // 填充
                                        else if (Symbols[sCount].Name == "sld:Fill")
                                        {
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Fill.Enabled = true;
                                            string Color = Symbols[sCount].FirstChild.FirstChild.Value;
                                            double Opacity = Convert.ToDouble(Symbols[sCount].LastChild.FirstChild.Value);
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Fill.Color = Color;
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Fill.Opacity = Opacity;
                                        }
                                        // 轮廓
                                        else if (Symbols[sCount].Name == "sld:Stroke")
                                        {
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Stroke.Enabled = true;
                                            string Color = Symbols[sCount].FirstChild.FirstChild.Value;
                                            double Width = Convert.ToDouble(Symbols[sCount].LastChild.FirstChild.Value);
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Stroke.Color = Color;
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Stroke.Width = Width;
                                        }
                                    }
                                    // 大小
                                    if (Rule[rCount].FirstChild.LastChild.Name == "sld:Size")
                                    {
                                        style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.SizePropertyName = Rule[rCount].FirstChild.LastChild.FirstChild.FirstChild.Value;
                                    }
                                }
                                // 多边形标识
                                else if (Rule[rCount].Name == "sld:PointSymbolizer")
                                {
                                    XmlNodeList Symbols = Rule[rCount].ChildNodes;
                                    for (int sCount = 0; sCount < Symbols.Count; ++sCount)
                                    {
                                        if (Rule[rCount].Name == "sld:Name")
                                        {
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].Name = Rule[rCount].Value;
                                        }
                                        // 填充
                                        else if (Symbols[sCount].Name == "sld:Fill")
                                        {
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Fill.Enabled = true;
                                            string Color = Symbols[sCount].ChildNodes[0].Attributes["fill"].Value;
                                            double Opacity = Convert.ToDouble(Symbols[sCount].ChildNodes[0].Attributes["fill-opacity"].Value);
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Fill.Color = Color;
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Fill.Opacity = Opacity;
                                        }
                                        // 轮廓
                                        else if (Symbols[sCount].Name == "sld:Stroke")
                                        {
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Enabled = true;
                                            if (Symbols[sCount].ChildNodes[1].Attributes["stroke"].Value != null)
                                            {
                                                string Color = Symbols[sCount].ChildNodes[1].Attributes["stroke"].Value;
                                                style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Color = Color;
                                            }
                                            if (Symbols[sCount].ChildNodes[1].Attributes["stroke-width"].Value != null)
                                            {
                                                double Width = Convert.ToDouble(Symbols[sCount].ChildNodes[1].Attributes["stroke-width"].Value);
                                                style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Width = Width;
                                            }
                                            if (Symbols[sCount].ChildNodes[1].Attributes["stroke-opacity"].Value != null)
                                            {
                                                double Opacity = Convert.ToDouble(Symbols[sCount].ChildNodes[1].Attributes["stroke-opacity"].Value);
                                                style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Opacity = Opacity;
                                            }
                                        }
                                    }
                                }
                                // 栅格标识
                                else if (Rule[rCount].Name == "sld:RasterSymbolizer")
                                {
                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].RasterSymbol.Enabled = true;
                                    XmlNodeList ColorMap = Rule[rCount].LastChild.ChildNodes;
                                    for (int cCount = 0; cCount < ColorMap.Count; ++cCount)
                                    {
                                        style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].RasterSymbol.AddColorMapEntry
                                            (ColorMap[cCount].Attributes["color"].Value,
                                            Convert.ToDouble(ColorMap[cCount].Attributes["opacity"].Value),
                                            Convert.ToDouble(ColorMap[cCount].Attributes["quantity"].Value));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Style = style;
        }
    }
}
