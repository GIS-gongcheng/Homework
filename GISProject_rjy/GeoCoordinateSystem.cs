using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISProject_rjy
{
    class GeoCoordinateSystem
    {
        #region 字段

        private string _GeoCSName;           //地理坐标系名称
        private string _DatumName;           //大地基准面名称
        private double _X, _Y, _Z;           //椭球体定位参数
        private string _SpheroidName;        //椭球体名称
        private double _SemiMajor;           //椭球体长半轴
        private double _InverseFlatting;     //椭球体扁率倒数
        private string _PrimeMeridianName;   //初始经线名称
        private double _PrimeMeridian;       //初始经线
        private string _AngularUnitName;     //角度单位名称
        public enum AngularUnitType { degree = 1 };
        private AngularUnitType _AngularUnit;//角度单位
        private double _RadiansPerUnit;      //每单位的弧度

        #endregion

        #region 构造函数

        public GeoCoordinateSystem(string geoCSName, string datumName, double x, double y, double z,
            string spheroidName, double semiMajor, double inverseFlatting, string primeMeridianName,
            double primeMeridian, string angularUnitName, AngularUnitType angularUnit, double radiansPerUnit)
        {
            _GeoCSName = geoCSName;
            _DatumName = datumName;
            _X = x;
            _Y = y;
            _Z = z;
            _SpheroidName = spheroidName;
            _SemiMajor = semiMajor;
            _InverseFlatting = inverseFlatting;
            _PrimeMeridianName = primeMeridianName;
            _PrimeMeridian = primeMeridian;
            _AngularUnitName = angularUnitName;
            _AngularUnit = angularUnit;
            _RadiansPerUnit = radiansPerUnit;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置地理坐标系名称
        /// </summary>
        public string GeoCSName
        {
            get { return _GeoCSName; }
            set { _GeoCSName = value; }
        }
        /// <summary>
        /// 获取或设置大地基准面名称
        /// </summary>
        public string DatumName
        {
            get { return _DatumName; }
            set { _DatumName = value; }
        }
        /// <summary>
        /// 获取或设置椭球体定位参数X
        /// </summary>
        public double X
        {
            get { return _X; }
            set { _X = value; }
        }
        /// <summary>
        /// 获取或设置椭球体定位参数Y
        /// </summary>
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        /// <summary>
        /// 获取或设置椭球体定位参数Z
        /// </summary>
        public double Z
        {
            get { return _Z; }
            set { _Z = value; }
        }
        /// <summary>
        ///获取或设置椭球体名称
        /// </summary>
        public string SpheroidName
        {
            get { return _SpheroidName; }
            set { _SpheroidName = value; }
        }
        /// <summary>
        ///获取或设置椭球体长半轴
        /// </summary>
        public double SemiMajor
        {
            get { return _SemiMajor; }
            set { _SemiMajor = value; }
        }
        /// <summary>
        ///获取或设置椭球体扁率倒数
        /// </summary>
        public double InverseFlatting
        {
            get { return _InverseFlatting; }
            set { _InverseFlatting = value; }
        }
        /// <summary>
        ///获取或设置初始经线名称
        /// </summary>
        public string PrimeMeridianName
        {
            get { return _PrimeMeridianName; }
            set { _PrimeMeridianName = value; }
        }
        /// <summary>
        ///获取或设置初始经线
        /// </summary>
        public double PrimeMeridian
        {
            get { return _PrimeMeridian; }
            set { _PrimeMeridian = value; }
        }
        /// <summary>
        ///获取或设置角度单位名称
        /// </summary>
        public string AngularUnitName
        {
            get { return _AngularUnitName; }
            set { _AngularUnitName = value; }
        }
        /// <summary>
        ///获取或设置角度单位
        /// </summary>
        public AngularUnitType AngularUnit
        {
            get { return _AngularUnit; }
            set { _AngularUnit = value; }
        }
        /// <summary>
        ///获取或设置每单位的弧度
        /// </summary>
        public double RadiansPerUnit
        {
            get { return _RadiansPerUnit; }
            set { _RadiansPerUnit = value; }
        }

        #endregion
    }
}
