using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSGeo.OGR;
using OSGeo.OSR;
using OSGeo.GDAL;
using System.IO;

namespace GISProject_rjy
{
    public partial class MapControl : UserControl
    {
        public MapControl()
        {
            InitializeComponent();
            this.MouseWheel += MapControl_MouseWheel;
        }

        #region 字段

        //运行时属性变量
        public List<MapLayer> _MapLayers = new List<MapLayer>(); //图层集合
        private float _DisplayScale = 1F; //显示比例尺倒数

        //内部变量
        private float mOffsetX = 0F, mOffsetY = 0F; //窗口左上点地图坐标
        private PointF mMouseLocation = new PointF(); //鼠标当前位置
        private const float mcZoomRatio = 1.5F; //缩放系数
        private Bitmap mFeatures;
        #endregion

        #region 属性
        //运行时属性

        /// <summary>
        /// 获取或设置图层集合
        /// </summary>
        [Browsable(false)]
        public MapLayer[] MapLayers
        {
            get { return _MapLayers.ToArray(); }
            set
            {
                _MapLayers.Clear();
                _MapLayers.AddRange(value);
            }
        }

        /// <summary>
        /// 获取显示比例尺倒数
        /// </summary>
        [Browsable(false)]
        public double DisplayScale
        {
            get { return _DisplayScale; }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 将地图坐标转换为屏幕坐标
        /// </summary>
        /// <param name="point">地图坐标</param>
        /// <returns></returns>
        public PointF FromMapPoint(PointF point)
        {
            float sPointX = (point.X - mOffsetX) / _DisplayScale;
            float sPointY = (mOffsetY - point.Y) / _DisplayScale;
            return new PointF(sPointX, sPointY);
        }

        /// <summary>
        /// 将屏幕坐标转换为地图坐标
        /// </summary>
        /// <param name="point">屏幕坐标</param>
        /// <returns></returns>
        public PointF ToMapPoint(PointF point)
        {
            float x = point.X * _DisplayScale + mOffsetX;
            float y = mOffsetY - point.Y * _DisplayScale;
            PointF sPoint = new PointF(x, y);
            return sPoint;
        }

        /// <summary>
        /// 以指定点为中心放大
        /// </summary>
        /// <param name="center">缩放中心</param>
        /// <param name="ratio">缩放系数</param>
        public void ZoomByCenter(PointF center, float ratio)
        {
            float sDisplayScale;
            sDisplayScale = _DisplayScale / ratio;

            float sOffsetX, sOffsetY;
            sOffsetX = mOffsetX + (1 - 1 / ratio) * (center.X - mOffsetX);
            sOffsetY = mOffsetY + (1 - 1 / ratio) * (center.Y - mOffsetY);

            mOffsetX = sOffsetX;
            mOffsetY = sOffsetY;
            _DisplayScale = sDisplayScale;

            //触发事件
            if (DisplayScaleChanged != null)
                DisplayScaleChanged(this);
        }

        /// <summary>
        /// 缩放至图层
        /// </summary>
        /// <param name="layer">选中图层</param>
        /// <returns></returns>
        public void Extent(MapLayer layer)
        {
            float[] MBR = new float[4];
            layer.GetExtent(MBR);
            float minX = MBR[0];
            float minY = MBR[1];
            float maxX = MBR[2];
            float maxY = MBR[3];
            mOffsetX = MBR[0];
            mOffsetY = MBR[3];
            if (maxX - minX == 0 && maxY - minY == 0)
                return;
            else if (maxY - minY == 0)
            {
                _DisplayScale = (maxX - minX) / 780;
                mOffsetY = minY - _DisplayScale * 445;
            }
            else if ((maxX - minX) / (maxY - minY) > 1.7528)
            {
                _DisplayScale = (maxX - minX) / 780;
            }
            else
                _DisplayScale = (maxY - minY) / 445;
            Refresh();
            //触发事件
            if (DisplayScaleChanged != null)
                DisplayScaleChanged(this);
        }


        public void InputShapefile(BinaryReader br, string fileName, string shpfilePath)
        {
            string dbfFilePath = shpfilePath.Remove(shpfilePath.Length - fileName.Length - 4) + fileName + ".dbf";
            _MapLayers[_MapLayers.Count - 1].DT = GetDbfData(dbfFilePath);
            Refresh();
        }

        public void AddLayer(MapLayer layer)
        {
            _MapLayers.Add(layer);
        }

        public void AddLayer(string name, string type, string filePath)
        {
            MapLayer newLayer = new MapLayer(name, type, filePath);
            _MapLayers.Add(newLayer);
        }

        public void MoveUpLayer(int Layerid)
        {
            MapLayer curlayer = _MapLayers[Layerid];
            _MapLayers.RemoveAt(Layerid);
            _MapLayers.Insert(Layerid - 1, curlayer);
            Refresh();
        }

        public void DeleteLayer(int Layerid)
        {
            _MapLayers.RemoveAt(Layerid);
            Refresh();
        }

        #endregion

        #region 事件

        private void mapControl_Load(object sender, EventArgs e)
        {
            mFeatures = new Bitmap(this.Width, this.Height);    //初始化 勿删
            //SelectFeature();
        }

        public delegate void DisplayScaleChangedHandle(object sender);

        /// <summary>
        /// 显示比例尺发生了变化
        /// </summary>
        [Browsable(true), Description("显示比例尺发生了变化")]
        public event DisplayScaleChangedHandle DisplayScaleChanged;

        #endregion

        #region 私有函数

        private DataTable GetDbfData(string tablePath)
        {
            dbfReader sDBFReader = new dbfReader();
            if (sDBFReader.Open(tablePath))
                return sDBFReader.GetDataTable();
            else
                return new DataTable();
        }


        //绘制Point要素
        private void DrawPoint(Graphics g, Geometry geom, int index, int fid)
        {
            if (geom != null)
            {
                //提取坐标信息
                string GeomWkt;
                geom.ExportToWkt(out GeomWkt);
                string[] str = GeomWkt.Split(new char[3] { ' ', '(', ')' });
                PointF point = new PointF();
                point.X = Convert.ToSingle(str[2]);
                point.Y = Convert.ToSingle(str[3]);
                //计算屏幕坐标
                PointF ScreenPoint = FromMapPoint(point);

                if (MapLayers[index].Style.Styles.Count == 0)
                {
                    g.FillEllipse(new SolidBrush(Color.Blue), new RectangleF((float)(ScreenPoint.X - 2), (float)(ScreenPoint.Y - 2), 4, 4));
                }

                else
                {
                    SolidBrush brush;
                    RectangleF rect;
                    Pen pen;
                    // 按照Style绘制
                    // 遍历规则
                    List<LayerStyle.FeatureTypeStyle.Rule> Rules = MapLayers[index].Style.Styles[0].Rules;
                    for (int i = 0; i < Rules.Count; ++i)
                    {
                        if (Rules[i].FiltersAnd.Count != 0)
                        {
                            bool RuleEnabled = true;
                            // 判断条件
                            for (int j = 0; j < Rules[i].FiltersAnd.Count; ++j)
                            {
                                string filter = "FID = " + Convert.ToString(fid);
                                double value = Convert.ToDouble(MapLayers[index].DT.Rows[fid][
                                    Rules[i].FiltersAnd[j].PropertyName]);
                                if (Rules[i].FiltersAnd[j].Criterion == "ge")
                                {
                                    if (value < Rules[i].FiltersAnd[j].Literal)
                                    {
                                        RuleEnabled = false;
                                        break;
                                    }
                                }
                                else if (Rules[i].FiltersAnd[j].Criterion == "le")
                                {
                                    if (value > Rules[i].FiltersAnd[j].Literal)
                                    {
                                        RuleEnabled = false;
                                        break;
                                    }
                                }
                            }
                            if (RuleEnabled)
                            {
                                // 画点
                                if(Rules[i].PointSymbol.Fill.Enabled)
                                {
                                    brush = new SolidBrush(Color.FromArgb(Convert.ToInt32(255 * Rules[i].PointSymbol.Fill.Opacity), 
                                        ColorTranslator.FromHtml(Rules[i].PointSymbol.Fill.Color)));
                                    string sizestr = Convert.ToString(MapLayers[index].DT.Rows[fid][
                                        Rules[i].PointSymbol.SizePropertyName]);
                                    if (sizestr == "Infinity")
                                        sizestr = "100";
                                    float size = Convert.ToSingle(sizestr);
                                    rect = new RectangleF((float)(ScreenPoint.X - size * 0.5), (float)(ScreenPoint.Y - size * 0.5), size, size);
                                    g.FillEllipse(brush, rect);
                                    if (Rules[i].PointSymbol.Stroke.Enabled)
                                    {
                                        pen = new Pen(Color.FromArgb(Convert.ToInt32(255 * Rules[i].PointSymbol.Stroke.Opacity),
                                            ColorTranslator.FromHtml(Rules[i].PointSymbol.Stroke.Color)), (float)Rules[i].PointSymbol.Stroke.Width);
                                        g.DrawEllipse(pen, rect);
                                    }
                                }
                            }
                        }
                        if (Rules[i].FiltersOr.Count != 0)
                        {
                            // 示例数据里没有
                        }

                    }
                }

            }
        }

        //绘制Polygon要素
        private void DrawPolygon(Graphics g, Geometry geom, int index)
        {
            if (geom != null)
            {
                //提取坐标信息
                string GeomWkt;
                geom.ExportToWkt(out GeomWkt);
                string[] str = GeomWkt.Split(new char[4] { ' ', '(', ')', ',' });
                List<PointF> ScreenPoints = new List<PointF>();
                for (int k = 3; k < str.Count() - 3; k += 2)
                {
                    PointF point = new PointF();
                    point.X = Convert.ToSingle(str[k]);
                    point.Y = Convert.ToSingle(str[k + 1]);
                    ScreenPoints.Add(FromMapPoint(point));
                }
                if (MapLayers[index].Style.Styles.Count == 0)
                {
                    //绘制
                    g.FillPolygon(new SolidBrush(Color.Orange), ScreenPoints.ToArray());
                    g.DrawPolygon(new Pen(Color.Black), ScreenPoints.ToArray());
                }
                else // 按Style绘制
                {
                    // 遍历规则
                    List<LayerStyle.FeatureTypeStyle.Rule> Rules = MapLayers[index].Style.Styles[0].Rules;
                    for (int i = 0; i < Rules.Count; ++i)
                    {
                        if (Rules[i].PolygonSymbol.Stroke.Enabled)
                        {
                            SolidBrush brush;
                            Pen pen;
                            if (Rules[i].PolygonSymbol.Fill.Enabled)
                            {
                                brush = new SolidBrush(Color.FromArgb(Convert.ToInt32(255 * Rules[i].PolygonSymbol.Fill.Opacity),
                                ColorTranslator.FromHtml(Rules[i].PolygonSymbol.Fill.Color)));
                                g.FillPolygon(brush, ScreenPoints.ToArray());
                            }
                            pen = new Pen(Color.FromArgb(Convert.ToInt32(255 * Rules[i].PolygonSymbol.Stroke.Opacity),
                                ColorTranslator.FromHtml(Rules[i].PolygonSymbol.Stroke.Color)), (float)Rules[i].PolygonSymbol.Stroke.Width);
                            g.DrawPolygon(pen, ScreenPoints.ToArray());
                        }
                    }
                }
            }
        }

        //绘制MultiPolygon要素
        private void DrawMultiPolygon(Graphics g, Geometry geom, int index)
        {
            if (geom != null)
            {
                Geometry sub_geom;
                for (int j = 0; j < geom.GetGeometryCount(); j++)
                {
                    sub_geom = geom.GetGeometryRef(j);
                    if (sub_geom != null)
                    {
                        DrawPolygon(g, sub_geom, index);
                    }
                }
            }
        }

        //绘制矢量图层
        private void DrawShpLayer(Graphics g, MapLayer curLayer, int index)
        {
            Ogr.RegisterAll();
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", @".\gdal\data");
            DataSource ds = Ogr.Open(curLayer.FilePath, 0);
            //遍历每个图层
            for (int i = 0; i < ds.GetLayerCount(); i++)
            {
                Layer layer = ds.GetLayerByIndex(i);
                Feature feature;
                //遍历图层中每个要素
                int fid = 0;
                while ((feature = layer.GetNextFeature()) != null)
                {
                    Geometry geom = feature.GetGeometryRef();
                    if (geom != null)
                    {
                        string type = geom.GetGeometryName();
                        if (type == "POLYGON")
                            DrawPolygon(g, geom, index);
                        else if (type == "MULTIPOLYGON")
                            DrawMultiPolygon(g, geom, index);
                        else if (type == "POINT")
                            DrawPoint(g, geom, index, fid);
                        ++fid;
                    }
                }
            }
        }

        //绘制tiff图层
        private void DrawTiffLayer(Graphics g, MapLayer curLayer, int index)
        {
            //PictureBox pictureBox = new PictureBox();
            float[] MBR = new float[4];
            curLayer.GetExtent(MBR);
            int width, height;
            //根据比例尺确定图片大小和位置
            PointF locationPoint = FromMapPoint(new PointF(MBR[0], MBR[3]));
            if (locationPoint.Y < this.Height && locationPoint.X < this.Width)
            {
                width = (int)((MBR[2] - MBR[0]) / _DisplayScale);
                height = (int)((MBR[3] - MBR[1]) / _DisplayScale);
                Dataset ds = Gdal.Open(curLayer.FilePath, Access.GA_ReadOnly);
                int imgWidth = ds.RasterXSize; 
                int imgHeight = ds.RasterYSize; 
                float[] r = new float[width * height];
                Band band = ds.GetRasterBand(1);
                band.ReadRaster(0, 0, imgWidth, imgHeight, r, width, height, 0, 0);
                //获取NoData值
                double nodata;
                int handle = 1;
                band.GetNoDataValue(out nodata, out handle);
                double[] MinMax = { 0, 0 };
                band.ComputeRasterMinMax(MinMax, 0);
                int i, j;
                for (i = 0; i < width; i++)
                {
                    for (j = 0; j < height; j++)
                    {
                        float value = r[i + j * width];    //像元值
                        if (value > nodata + 1)
                        {
                            int color = 0;
                            if (MinMax[0] == -32768)
                            {
                                if (value > MinMax[0])
                                {
                                    color = (int)(1.0 * (value + 128) / (MinMax[1] + 128) * 255.0);
                                }
                            }
                            else
                                value = (int)((value - MinMax[0]) / (MinMax[1] - MinMax[0]) * 255.0);
                            newColor = Color.FromArgb(value, value, value);
                        }
                        else if (MapLayers[index].Style.Styles[0].Rules[0].RasterSymbol.ColorMap.Count == 0)
                        {
                            if (MinMax[0] == -32768)
                            {
                                if (value == MinMax[0])
                                {
                                    value = 0;
                                }
                                else
                                {
                                    value = (int)(1.0 * (value + 128) / (MinMax[1] + 128) * 255.0);
                                }
                            }
                            else
                                value = (int)((value - MinMax[0]) / (MinMax[1] - MinMax[0]) * 255.0);
                            newColor = Color.FromArgb(value, value, value);
                            newColor = Color.FromArgb(Convert.ToInt32(255 * curLayer.Style.Styles[0].Rules[0].RasterSymbol.Opacity), newColor);
                        }
                        else // 按照Style绘制
                        {
                            for (int eCount = 0; eCount < curLayer.Style.Styles[0].Rules[0].RasterSymbol.ColorMap.Count - 1; ++eCount)
                            {
                                if (value >= curLayer.Style.Styles[0].Rules[0].RasterSymbol.ColorMap[eCount].Quantity &&
                                    value <= curLayer.Style.Styles[0].Rules[0].RasterSymbol.ColorMap[eCount + 1].Quantity)
                                {
                                    newColor = Color.FromArgb(Convert.ToInt32(255 * curLayer.Style.Styles[0].Rules[0].RasterSymbol.ColorMap[eCount].Opacity),
                                        ColorTranslator.FromHtml(curLayer.Style.Styles[0].Rules[0].RasterSymbol.ColorMap[eCount].Color));
                                    break;
                                }
                            }
                        }

                    }
                }
            }
        }
        #endregion

        #region 母版事件处理

        //鼠标滑轮
        private void MapControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                PointF sCenterPoint = new PointF(this.ClientSize.Width / 2, this.ClientSize.Height / 2);//屏幕中心点
                PointF sCenterPointOnMap = ToMapPoint(sCenterPoint);
                ZoomByCenter(sCenterPointOnMap, mcZoomRatio);
                Refresh();
            }
            else
            {
                PointF sCenterPoint = new PointF(this.ClientSize.Width / 2, this.ClientSize.Height / 2);//屏幕中心点
                PointF sCenterPointOnMap = ToMapPoint(sCenterPoint);
                ZoomByCenter(sCenterPointOnMap, 1 / mcZoomRatio);
                Refresh();
            }
        }

        private void MapControl_Load_1(object sender, EventArgs e)
        {
            mFeatures = new Bitmap(this.Width, this.Height);    //初始化 勿删
            //SelectFeature();
        }

        private void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mMouseLocation.X = e.Location.X;
                mMouseLocation.Y = e.Location.Y;
            }
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF sPreMouseLocation = new PointF(mMouseLocation.X, mMouseLocation.Y);
                PointF sPrePoint = ToMapPoint(sPreMouseLocation);
                PointF sCurMouseLocation = new PointF(e.Location.X, e.Location.Y);
                PointF sCurPoint = ToMapPoint(sCurMouseLocation);
                mOffsetX = mOffsetX + sPrePoint.X - sCurPoint.X;
                mOffsetY = mOffsetY + sPrePoint.Y - sCurPoint.Y;
                Refresh();
                mMouseLocation.X = e.Location.X;
                mMouseLocation.Y = e.Location.Y;
            }
        }

        //母版重绘
        private void MapControl_Paint_1(object sender, PaintEventArgs e)
        {
            for (int i = _MapLayers.Count() - 1; i >= 0; i--)
            {
                if (_MapLayers[i].Type == "Shp")
                    DrawShpLayer(e.Graphics, _MapLayers[i], i);
                else if (_MapLayers[i].Type == "Tiff")
                {
                    DrawTiffLayer(e.Graphics, _MapLayers[i], i);
                }
            }
        }
        #endregion
    }
}
