using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using OSGeo.OGR;
using OSGeo.OSR;
using OSGeo.GDAL;
using System.Diagnostics;

namespace GISProject_rjy
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            Gdal.AllRegister();
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            Ogr.RegisterAll();
            InitializeComponent();
            
        }

        //用来指示是否把shp文件导入了数据库
        bool ImportProvince = false;
        bool ImportCounty = false;
        bool ImportCyclone = false;

        private void 打开shp文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开矢量文件";
            ofd.Filter = @"Shapefile(*.shp)|*.shp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName; //文件路径
                string name = ofd.SafeFileName; //文件名
                MapLayer mapLayer = new MapLayer(name, "Shp", path);
                //获取外接矩形
                DataSource ds = Ogr.Open(path, 0);
                Layer layer = ds.GetLayerByIndex(0);
                Envelope ext = new Envelope();
                layer.GetExtent(ext, 1);
                mapLayer.SetExtent((float)ext.MinX, (float)ext.MinY, (float)ext.MaxX, (float)ext.MaxY);
                //添加新图层至地图
                if (tVLayers.Nodes.Count == 0)
                    tVLayers.Nodes.Add("图层");
                tVLayers.Nodes[0].Nodes.Add(name);
                tVLayers.ExpandAll();
                mapControl._MapLayers.Add(mapLayer);
                mapControl.Extent(mapControl._MapLayers[mapControl._MapLayers.Count - 1]);
            }

            /****
            string shpfilePath = "";
            openShapefileDialog.Filter = "shapefiles(*.shp)|*.shp|All files(*.*)|*.*"; //打开文件路径
            if (openShapefileDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(openShapefileDialog.OpenFile());
                shpfilePath = openShapefileDialog.FileName;
                string[] sFileNameSplit = shpfilePath.Split('\\'); //获取文件名称
                string sFileName = sFileNameSplit[sFileNameSplit.Length - 1].Split('.')[0];
                string sFileType = ReadShpFileType(br, shpfilePath);
                //mapControl.InputShapefile(br, sFileName, shpfilePath);
                br.Close();
                TreeNode tnNew = new TreeNode(sFileName); //treeview添加节点
                tVLayers.Nodes.Add(tnNew);
                tnNew.Checked = true;
                tVLayers.SelectedNode = tnNew; //聚焦于该新的图层

                mapControl.AddLayer(sFileName, sFileType, shpfilePath);
            }****/
        }

        private void 打开Tiff文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开栅格文件";
            ofd.Filter = @"Tiff(*.tif)|*.tif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName; //文件路径
                string name = ofd.SafeFileName; //文件名
                MapLayer mapLayer = new MapLayer(name, "Tiff", path);
                //获取外接矩形
                Dataset ds = Gdal.Open(path, Access.GA_ReadOnly);
                double[] adfGeoTransform = new double[6];
                float minX, minY, maxX, maxY;
                ds.GetGeoTransform(adfGeoTransform);
                minX = (float)(adfGeoTransform[0] + adfGeoTransform[2] * ds.RasterYSize);
                minY = (float)(adfGeoTransform[3] + adfGeoTransform[5] * ds.RasterYSize);
                maxX = (float)(adfGeoTransform[0] + adfGeoTransform[1] * ds.RasterXSize);
                maxY = (float)(adfGeoTransform[3] + adfGeoTransform[4] * ds.RasterXSize);
                mapLayer.SetExtent(minX, minY, maxX, maxY);
                if (tVLayers.Nodes.Count == 0)
                    tVLayers.Nodes.Add("图层");
                tVLayers.Nodes[0].Nodes.Add(name);
                tVLayers.ExpandAll();
                mapControl._MapLayers.Add(mapLayer);
                mapControl.Extent(mapControl._MapLayers[mapControl._MapLayers.Count - 1]);
            }
        }

        private void 读取图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // 暂时扔在这，后面要放在读取文件的类里面
        private void ReadSld(string FilePath)
        {
            XmlDocument sldDoc = new XmlDocument();
            sldDoc.Load(FilePath);
        }

        private void tVLayers_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            if (e.Node.Parent == null || e.Node == null)
                return;
            tVLayers.SelectedNode = e.Node;
            layerMenuStrip.Show(tVLayers, e.X, e.Y);
        }

        private void 加载图层样式SLDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sldFilePath = "";
            openSldFileDialog.Filter = "Styled Layer Descriptor(*.sld)|*.sld"; // 打开文件路径
            if (openSldFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 选择文件
                sldFilePath = openSldFileDialog.FileName;
                string[] sFileNameSplit = sldFilePath.Split('\\'); //获取文件名称
                string sFileName = sFileNameSplit[sFileNameSplit.Length - 1].Split('.')[0];

                // 读取SLD
                ReadSld(sldFilePath);
            }
            //mapControl.DrawSld();
        }

        private void 上移图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tncur = tVLayers.SelectedNode;
            int i = tncur.Index;
            if (tncur.Index == 0) return;
            TreeNode newnode = (TreeNode)tncur.Clone();
            tncur.Parent.Nodes.Insert(tncur.PrevNode.Index, newnode);
            tncur.Remove();
            tncur = newnode;
            tVLayers.SelectedNode = tVLayers.Nodes[0].Nodes[i - 1];
            mapControl.MoveUpLayer(i);
        }

        private void 删除图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapControl.DeleteLayer(tVLayers.SelectedNode.Index);
            tVLayers.Nodes[0].Nodes.Remove(tVLayers.SelectedNode);
        }

        private void 投影变换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpatialReference ss;
            
        }

        private string ReadShpFileType(BinaryReader br, string filePath)
        {
            br.ReadBytes(24); //跳过24个字节
            int sFileLength = br.ReadInt32(); //文件长度 <0代表数据长度未知
            int sFileBanben = br.ReadInt32(); //文件版本
            int sShapeType = br.ReadInt32(); //集合类型
            if (sShapeType == 1)
                return "Point";
            else if (sShapeType == 5)
                return "Polygon";
            else
                return "Tiff";
        }

        private void tVLayers_MouseUp(object sender, MouseEventArgs e)
        {
            //鼠标抬起产生菜单
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = tVLayers.GetNodeAt(ClickPoint);
                if (CurrentNode == null || CurrentNode.Parent == null)
                    return;
                tVLayers.SelectedNode = CurrentNode;
                layerMenuStrip.Show(tVLayers, ClickPoint);
            }
        }

        private void 缩放至图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = tVLayers.SelectedNode.Text;
            for (int i = 0; i < mapControl._MapLayers.Count(); i++)
            {
                if (mapControl._MapLayers[i].Name == name)
                {
                    mapControl.Extent(mapControl._MapLayers[i]);
                    break;
                }
            }
        }

        private void 导入数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("矢量文件路径中请不要出现中文字符，否则可能出错！");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开矢量文件";
            ofd.Filter = @"Shapefile(*.shp)|*.shp";
            //以生成.bat批处理文件的方式导入数据
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName; //文件路径
                string name = ofd.SafeFileName; //文件名
                string bat = "cd D:\r\n D:\r\n cd D:\\PostgreSQL\\9.6\\bin\r\n" +
                    "shp2pgsql -d -g \"geom\" -s 4326 -W ";
                if(name == "Province_Hainan.shp")
                {
                    bat += "GBK \"" + path+ "\" Province_Hainan > D:\\province.sql\r\n" +
                        "psql -h 127.0.0.1 -p 5432 -U postgres -w -d mypostdb -f D:\\province.sql";
                    ImportProvince = true;
                    
                }
                else if(name == "County_Hainan.shp")
                {
                    bat += "UTF-8 \"" + path + "\" County_Hainan > D:\\county.sql\r\n" +
                        "psql -h 127.0.0.1 -p 5432 -U postgres -w -d mypostdb -f D:\\county.sql";
                    ImportCounty = true;
                }
                else if(name == "cyclone.shp")
                {
                    bat += "UTF-8 \"" + path + "\" cyclone > D:\\cyclone.sql\r\n" +
                        "psql -h 127.0.0.1 -p 5432 -U postgres -w -d mypostdb -f D:\\cyclone.sql";
                    ImportCyclone = true;
                }
                else
                {
                    MessageBox.Show("本系统暂不支持其他shapefile数据导入PostGIS数据库！");
                }
                File.WriteAllText("test.bat", bat);
                Process.Start("test.bat");
                MessageBox.Show("Shapefile文件已经成功导入数据库！");
            }
        }

        

        private void 导出ProvinceHainanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ImportProvince == true)
            {
                string bat = "cd D:\r\n D:\r\n cd D:\\PostgreSQL\\9.6\\bin\r\n" +
                    "pgsql2shp -f D:\\data\\test\\province_hainan.shp -u postgres mypostdb province_hainan";
                File.WriteAllText("test.bat", bat);
                Process.Start("test.bat");
                string path = "D:\\data\\test\\province_hainan.shp";//文件路径
                string name = "province_hainan.shp";//文件名
                FileStream F = new FileStream(path,FileMode.OpenOrCreate, FileAccess.ReadWrite);
                MapLayer mapLayer = new MapLayer(name, "Shp", path);
                //获取外接矩形
                DataSource ds = Ogr.Open(path, 0);
                Layer layer = ds.GetLayerByIndex(0);
                Envelope ext = new Envelope();
                layer.GetExtent(ext, 1);
                mapLayer.SetExtent((float)ext.MinX, (float)ext.MinY, (float)ext.MaxX, (float)ext.MaxY);
                //添加新图层至地图
                if (tVLayers.Nodes.Count == 0)
                    tVLayers.Nodes.Add("图层");
                tVLayers.Nodes[0].Nodes.Add(name);
                tVLayers.ExpandAll();
                mapControl._MapLayers.Add(mapLayer);
                mapControl.Extent(mapControl._MapLayers[mapControl._MapLayers.Count - 1]);
                F.Close();
            }
            else
            {
                MessageBox.Show("请先将矢量数据导入数据库！");
            }
            
        }

        private void 导出CountyHainanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImportCounty == true)
            {
                string bat = "cd D:\r\n D:\r\n cd D:\\PostgreSQL\\9.6\\bin\r\n" +
                    "pgsql2shp -f D:\\data\\test\\county_hainan.shp -u postgres mypostdb county_hainan";
                File.WriteAllText("test.bat", bat);
                Process.Start("test.bat");
                string path = "D:\\data\\test\\county_hainan.shp";//文件路径
                string name = "county_hainan.shp";//文件名
                FileStream F = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                MapLayer mapLayer = new MapLayer(name, "Shp", path);
                //获取外接矩形
                DataSource ds = Ogr.Open(path, 0);
                Layer layer = ds.GetLayerByIndex(0);
                Envelope ext = new Envelope();
                layer.GetExtent(ext, 1);
                mapLayer.SetExtent((float)ext.MinX, (float)ext.MinY, (float)ext.MaxX, (float)ext.MaxY);
                //添加新图层至地图
                if (tVLayers.Nodes.Count == 0)
                    tVLayers.Nodes.Add("图层");
                tVLayers.Nodes[0].Nodes.Add(name);
                tVLayers.ExpandAll();
                mapControl._MapLayers.Add(mapLayer);
                mapControl.Extent(mapControl._MapLayers[mapControl._MapLayers.Count - 1]);
                F.Close();
            }
            else
            {
                MessageBox.Show("请先将矢量数据导入数据库！");
            }
        }

        private void 导出CycloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImportCyclone == true)
            {
                string bat = "cd D:\r\n D:\r\n cd D:\\PostgreSQL\\9.6\\bin\r\n" +
                    "pgsql2shp -f D:\\data\\test\\cyclone.shp -u postgres mypostdb cyclone";
                File.WriteAllText("test.bat", bat);
                Process.Start("test.bat");
                string path = "D:\\data\\test\\cyclone.shp";//文件路径
                string name = "cyclone.shp";//文件名
                FileStream F = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                MapLayer mapLayer = new MapLayer(name, "Shp", path);
                //获取外接矩形
                DataSource ds = Ogr.Open(path, 0);
                Layer layer = ds.GetLayerByIndex(0);
                Envelope ext = new Envelope();
                layer.GetExtent(ext, 1);
                mapLayer.SetExtent((float)ext.MinX, (float)ext.MinY, (float)ext.MaxX, (float)ext.MaxY);
                //添加新图层至地图
                if (tVLayers.Nodes.Count == 0)
                    tVLayers.Nodes.Add("图层");
                tVLayers.Nodes[0].Nodes.Add(name);
                tVLayers.ExpandAll();
                mapControl._MapLayers.Add(mapLayer);
                mapControl.Extent(mapControl._MapLayers[mapControl._MapLayers.Count - 1]);
                F.Close();
            }
            else
            {
                MessageBox.Show("请先将矢量数据导入数据库！");
            }
        }

        /**private void 将结果导入数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DBConnector dbConnector = new DBConnector();
            //添加新表
            dbConnector.AddDEMTable();
            List<float[]> info = new List<float[]>();
            //向新表中插入数据
            dbConnector.InsertDEMInfo(info);
            MessageBox.Show("统计结果已经成功导入数据库！");
            dbConnector.DBClose();
        }**/
    }
}
