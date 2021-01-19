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
        #endregion

        #region 事件
        public delegate void DisplayScaleChangedHandle(object sender);

        /// <summary>
        /// 显示比例尺发生了变化
        /// </summary>
        [Browsable(true), Description("显示比例尺发生了变化")]
        public event DisplayScaleChangedHandle DisplayScaleChanged;

        #endregion

        #region 私有函数
        //绘制点图层
        private void DrawPointLayer(Graphics g, MapLayer curLayer)
        {
            Ogr.RegisterAll();
            DataSource ds = Ogr.Open(curLayer.FilePath, 0);
            //遍历每个图层
            for (int i = 0; i < ds.GetLayerCount(); i++)
            {
                Layer layer = ds.GetLayerByIndex(i);
                Feature feature;
                //遍历图层中每个要素
                while ((feature = layer.GetNextFeature()) != null)
                {
                    Geometry geom = feature.GetGeometryRef();
                    if (geom != null)
                    {
                        //提取坐标信息
                        string GeomWkt;
                        geom.ExportToWkt(out GeomWkt);
                        string[] str = GeomWkt.Split(' ');
                        PointF point = new PointF();
                        point.X = Convert.ToSingle(str[0].Substring(6));
                        point.Y = Convert.ToSingle(str[1].Substring(0, str[1].Length-1));
                        //计算屏幕坐标
                        PointF ScreenPoint = FromMapPoint(point);
                        //绘制
                        g.FillEllipse(new SolidBrush(Color.Blue), new RectangleF((float)(ScreenPoint.X - 2 ), (float)(ScreenPoint.Y - 2), 4, 4));
                    }
                }
            }
        }

        //绘制Polygon图层
        private void DrawPolygonLayer(Graphics g, MapLayer curLayer)
        {
            Ogr.RegisterAll();
            DataSource ds = Ogr.Open(curLayer.FilePath, 0);
            //遍历每个图层
            for (int i = 0; i < ds.GetLayerCount(); i++)
            {
                Layer layer = ds.GetLayerByIndex(i);
                Feature feature;
                //遍历图层中每个要素
                while ((feature = layer.GetNextFeature()) != null)
                {
                    Geometry geom = feature.GetGeometryRef();
                    if (geom != null)
                    {
                        //提取坐标信息
                        string GeomWkt;
                        geom.ExportToWkt(out GeomWkt);
                        string[] str1 = GeomWkt.Split(',');
                        List<PointF> ScreenPoints = new List<PointF>();
                        string[] str2 = str1[0].Split(' ');
                        PointF point = new PointF();
                        point.X = Convert.ToSingle(str2[0].Substring(9));
                        point.Y = Convert.ToSingle(str2[1]);
                        ScreenPoints.Add(FromMapPoint(point));
                        for(int k=1;k<str1.Count()-1;k++)
                        {
                            str2 = str1[k].Split(' ');
                            point.X = Convert.ToSingle(str2[0]);
                            point.Y = Convert.ToSingle(str2[1]);
                            ScreenPoints.Add(FromMapPoint(point));
                        }
                        str2 = str1[str1.Count() - 1].Split(' ');
                        point.X = Convert.ToSingle(str2[0]);
                        point.Y = Convert.ToSingle(str2[1].Substring(0, str2[1].Length-2));
                        ScreenPoints.Add(FromMapPoint(point));
                        //绘制
                        g.FillPolygon(new SolidBrush(Color.Cyan), ScreenPoints.ToArray());                   
                    }
                }
            }
        }

        //绘制MultiPolygon图层
        private void DrawMultiPolygonLayer(Graphics g, MapLayer curLayer)
        {
            Ogr.RegisterAll();
            DataSource ds = Ogr.Open(curLayer.FilePath, 0);
            //遍历每个图层
            for (int i = 0; i < ds.GetLayerCount(); i++)
            {
                Layer layer = ds.GetLayerByIndex(i);
                Feature feature;
                //遍历图层中每个要素
                while ((feature = layer.GetNextFeature()) != null)
                {
                    Geometry geom = feature.GetGeometryRef();
                    if (geom != null)
                    {
                        Geometry sub_geom;
                        for (int j = 0; j < geom.GetGeometryCount(); j++)
                        {
                            sub_geom = geom.GetGeometryRef(j);
                            if (sub_geom != null)
                            {
                                //提取坐标信息
                                string GeomWkt;
                                sub_geom.ExportToWkt(out GeomWkt);
                                string[] str1 = GeomWkt.Split(',');
                                List<PointF> ScreenPoints = new List<PointF>();
                                string[] str2 = str1[0].Split(' ');
                                PointF point = new PointF();
                                point.X = Convert.ToSingle(str2[0].Substring(9));
                                point.Y = Convert.ToSingle(str2[1]);
                                ScreenPoints.Add(FromMapPoint(point));
                                for (int k = 1; k < str1.Count() - 1; k++)
                                {
                                    str2 = str1[k].Split(' ');
                                    point.X = Convert.ToSingle(str2[0]);
                                    point.Y = Convert.ToSingle(str2[1]);
                                    ScreenPoints.Add(FromMapPoint(point));
                                }
                                str2 = str1[str1.Count() - 1].Split(' ');
                                point.X = Convert.ToSingle(str2[0]);
                                point.Y = Convert.ToSingle(str2[1].Substring(0, str2[1].Length - 2));
                                ScreenPoints.Add(FromMapPoint(point));
                                //绘制
                                g.FillPolygon(new SolidBrush(Color.Cyan), ScreenPoints.ToArray());
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

        //母版重绘
        private void MapControl_Paint(object sender, PaintEventArgs e)
        {
            for(int i=0;i<_MapLayers.Count;i++)
            {
                if(_MapLayers[i].Type == "POINT")
                {
                    DrawPointLayer(e.Graphics, _MapLayers[i]);
                }
                else if (_MapLayers[i].Type == "POLYGON")
                {
                    DrawPolygonLayer(e.Graphics, _MapLayers[i]);
                }
                else if (_MapLayers[i].Type == "MULTIPOLYGON")
                {
                    DrawMultiPolygonLayer(e.Graphics, _MapLayers[i]);
                }
                else if (_MapLayers[i].Type == "TIFF")
                {
                    
                }
            }
        }
        #endregion
    }
}
