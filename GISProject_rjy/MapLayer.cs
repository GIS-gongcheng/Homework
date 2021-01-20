using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Data;

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
    }
}
