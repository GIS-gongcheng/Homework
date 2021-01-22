using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using OSGeo.OGR;
using OSGeo.OSR;
using OSGeo.GDAL;

namespace GISProject_rjy
{
    public class MapLayer
    {
        public string Name;
        public string Type; // Tiff,Polygon,Point
        public string FilePath;
        public bool Visible;
        public float _MinX, _MinY, _MaxX, _MaxY;
        //public bool Transformed = true; // 是否转换为Web Mercator投影
        public DataTable DT = new DataTable();   //图层的属性数据表
        public LayerStyle Style = new LayerStyle();

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
            nsp.AddNamespace("sld", "http://www.opengis.net/sld");
            nsp.AddNamespace("ogc", "http://www.opengis.net/ogc");
            nsp.AddNamespace("gml", "http://www.opengis.net/gml");
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
                                    for (int rrCount = 0; rrCount < Rule[rCount].ChildNodes.Count; ++rrCount)
                                    {
                                        if (Rule[rCount].ChildNodes[rrCount].Name == "sld:Graphic")
                                        {
                                            XmlNodeList Graphics = Rule[rCount].ChildNodes[rrCount].ChildNodes;
                                            for (int gCount = 0; gCount < Graphics.Count; ++gCount)
                                            {
                                                if (Graphics[gCount].Name == "sld:Mark")
                                                {
                                                    XmlNodeList Symbols = Graphics[gCount].ChildNodes;
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
                                                            XmlNodeList FillAttributes = Symbols[sCount].ChildNodes;
                                                            for (int aCount = 0; aCount < FillAttributes.Count; ++aCount)
                                                            {
                                                                if (FillAttributes[aCount].Attributes["name"].Value == "fill")
                                                                {
                                                                    string Color = FillAttributes[aCount].FirstChild.Value;
                                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Fill.Color = Color;
                                                                }
                                                                else if (FillAttributes[aCount].Attributes["name"].Value == "fill-opacity")
                                                                {
                                                                    double Opacity = Convert.ToDouble(FillAttributes[aCount].FirstChild.Value);
                                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Fill.Opacity = Opacity;
                                                                }
                                                            }
                                                        }
                                                        // 轮廓
                                                        else if (Symbols[sCount].Name == "sld:Stroke")
                                                        {
                                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Stroke.Enabled = true;
                                                            XmlNodeList StrokeAttributes = Symbols[sCount].ChildNodes;
                                                            for (int aCount = 0; aCount < StrokeAttributes.Count; ++aCount)
                                                            {
                                                                if (StrokeAttributes[aCount].Attributes["name"].Value == "stroke")
                                                                {
                                                                    string Color = StrokeAttributes[aCount].FirstChild.Value;
                                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Stroke.Color = Color;
                                                                }
                                                                else if (StrokeAttributes[aCount].Attributes["name"].Value == "stroke-opacity")
                                                                {
                                                                    double Opacity = Convert.ToDouble(StrokeAttributes[aCount].FirstChild.Value);
                                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Stroke.Opacity = Opacity;
                                                                }
                                                                else if (StrokeAttributes[aCount].Attributes["name"].Value == "stroke-width")
                                                                {
                                                                    double Width = Convert.ToDouble(StrokeAttributes[aCount].FirstChild.Value);
                                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.Stroke.Width = Width;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (Graphics[gCount].Name == "sld:Size")
                                                {
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PointSymbol.SizePropertyName
                                                        = Graphics[gCount].FirstChild.FirstChild.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                                // 多边形标识
                                else if (Rule[rCount].Name == "sld:PolygonSymbolizer")
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
                                            XmlNodeList FillAttributes = Symbols[sCount].ChildNodes;
                                            for (int aCount = 0; aCount < FillAttributes.Count; ++aCount)
                                            {
                                                if (FillAttributes[aCount].Attributes["name"].Value == "fill")
                                                {
                                                    string Color = FillAttributes[aCount].FirstChild.Value;
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Fill.Color = Color;
                                                }
                                                else if (FillAttributes[aCount].Attributes["name"].Value == "fill-opacity")
                                                {
                                                    double Opacity = Convert.ToDouble(FillAttributes[aCount].FirstChild.Value);
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Fill.Opacity = Opacity;
                                                }
                                            }
                                        }
                                        // 轮廓
                                        else if (Symbols[sCount].Name == "sld:Stroke")
                                        {
                                            style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Enabled = true;
                                            XmlNodeList StrokeAttributes = Symbols[sCount].ChildNodes;
                                            for (int aCount = 0; aCount < StrokeAttributes.Count; ++aCount)
                                            {
                                                if (StrokeAttributes[aCount].Attributes["name"].Value == "stroke")
                                                {
                                                    string Color = StrokeAttributes[aCount].FirstChild.Value;
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Color = Color;
                                                }
                                                else if (StrokeAttributes[aCount].Attributes["name"].Value == "stroke-opacity")
                                                {
                                                    double Opacity = Convert.ToDouble(StrokeAttributes[aCount].FirstChild.Value);
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Opacity = Opacity;
                                                }
                                                else if (StrokeAttributes[aCount].Attributes["name"].Value == "stroke-width")
                                                {
                                                    double Width = Convert.ToDouble(StrokeAttributes[aCount].FirstChild.Value);
                                                    style.Styles[0].Rules[style.Styles[0].Rules.Count - 1].PolygonSymbol.Stroke.Width = Width;
                                                }
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

        public void TransformToWebMercator()
        {
            DataSource ds = Ogr.Open(FilePath, 0);
            Layer layer = ds.GetLayerByIndex(0);
            SpatialReference sr = ds.GetLayerByIndex(0).GetSpatialRef();
            SpatialReference Mercator = new SpatialReference("");
            Mercator.ImportFromEPSG(3857); // Web Mercator
            Mercator.SetMercator(0d, 0d, 1d, 0d, 0d);
            CoordinateTransformation ct = new CoordinateTransformation(sr, Mercator);
            //Geometry geom = layer.
        }
    }
}
