using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISProject_rjy
{
    class Projection
    {
        class ProjectionParameter
        {
            
        }
        public string GEOGCS; // 地理坐标系
        public string DATUM; // 大地基准面
        public string SPHEROID; // 椭球体定义
        public string PRIMEM; // 本初子午线
        public string UNIT; // 单位
        public string AUTHORITY; // 权威定义
        public string PROJCS; // 投影定义
        public string PROJECTION; // 投影类型
        ProjectionParameter PARAMETER; // 投影参数
    }
}
