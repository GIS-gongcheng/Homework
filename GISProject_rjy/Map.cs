using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISProject_rjy
{
    class Map
    {
        #region 字段
        private string _Name;                   //地图名称
        private string _Description;            //地图描述
        private List<MapLayer> _Layers;                //地图所包含的图层
        private GeoCoordinateSystem _GeoCS;     //地图的地理坐标系统
        private double _Scale;                  //比例尺
        //private RectangleD _CurrentRect;        //地图当前的显示范围
        private int _SelectedLayerID;

        #endregion

        #region 构造函数
        //public Map(string name,string description,Layer[] layers,
        //   GeoCoordinateSystem geoCS,double scale,RectangleD currentRect,double tolerance )
        public Map(string name, string description = "", double scale = 1)
        {
            _Name = name;
            _Description = description;
            _Layers = new List<MapLayer>();
            _GeoCS = null;
            _Scale = scale;
            _SelectedLayerID = -1;
            //_CurrentRect = currentRect;
            //_Tolerance = tolerance;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置地图名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// 获取或设置地图描述
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary>
        /// 获取或设置地图所包含的图层
        /// </summary>
        public List<MapLayer> Layers
        {
            get { return _Layers; }

        }

        public int LayerCount
        {
            get { return _Layers.Count; }
        }

        /// <summary>
        /// 获取或设置地图的地理坐标系统
        /// </summary>
        public GeoCoordinateSystem GeoCS
        {
            get { return _GeoCS; }
            set { _GeoCS = value; }
        }

        /// <summary>
        /// 获取或设置地图比例尺
        /// </summary>
        public double Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }

        public int SelectedLayerID
        {
            get { return _SelectedLayerID; }
            set { _SelectedLayerID = value; }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(MapLayer layer)
        {
            _Layers.Add(layer);
        }


        public MapLayer SelectedLayer()
        {
            return Layers[_SelectedLayerID];
        }

        /// <summary>
        /// 改变图层的顺序
        /// </summary>
        /// <param name="preIndex">待修改顺序的图层的原来的索引号</param>
        /// <param name="afterIndex">修改后的索引号</param>
        public void ChangeLayersOrder(int preIndex, int afterIndex)
        {
            MapLayer ChangedLayer = _Layers[preIndex];
            if (preIndex < afterIndex)
            {
                for (int i = preIndex; i < afterIndex; i++)
                {
                    _Layers[i] = _Layers[i + 1];
                }
            }
            else
            {
                for (int i = preIndex; i > afterIndex; i--)
                {
                    _Layers[i] = _Layers[i - 1];
                }
            }
            _Layers[afterIndex] = ChangedLayer;
        }
        #endregion
    }
}
