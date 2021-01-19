using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISProject_rjy
{
    public class MapLayer
    {
        public string Name;
        public string Type; //Tiff,Polygon,Point
        public string FilePath;
        public bool Visible;
        private float _MinX, _MinY, _MaxX, _MaxY;

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
