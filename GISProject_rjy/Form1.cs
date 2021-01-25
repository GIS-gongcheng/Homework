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
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
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
                //添加新图层至地图
                mapControl.AddShpLayer(path, name);               
                if (tVLayers.Nodes.Count == 0)
                    tVLayers.Nodes.Add("图层");
                tVLayers.Nodes[0].Nodes.Insert(0, name);
                tVLayers.ExpandAll();
                tVLayers.SelectedNode = tVLayers.Nodes[0].Nodes[0];
            }
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
                mapControl.AddTiffLayer(path, name);
                //添加新图层至地图
                if (tVLayers.Nodes.Count == 0)
                    tVLayers.Nodes.Add("图层");
                tVLayers.Nodes[0].Nodes.Insert(0, name);
                tVLayers.ExpandAll();
                tVLayers.SelectedNode = tVLayers.Nodes[0].Nodes[0];
            }
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
                int index = mapControl._MapLayers.Count() - 1 - tVLayers.SelectedNode.Index;
                mapControl.MapLayers[index].ReadSld(sldFilePath);
                mapControl.Refresh();
            }
        }

        private void 上移图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tncur = tVLayers.SelectedNode;
            int i = tncur.Index;
            if (i > 0)
            {
                string name = tVLayers.Nodes[0].Nodes[i].Text;
                tVLayers.Nodes[0].Nodes.RemoveAt(i);
                tVLayers.Nodes[0].Nodes.Insert(i-1, name);
                tVLayers.ExpandAll();
                tVLayers.SelectedNode = tVLayers.Nodes[0].Nodes[i - 1];
                mapControl.MoveUpLayer(i);
            }
        }

        private void 删除图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = mapControl._MapLayers.Count() - 1 - tVLayers.SelectedNode.Index;
            mapControl.DeleteLayer(index);
            tVLayers.Nodes[0].Nodes.Remove(tVLayers.SelectedNode);
        }

        private void 投影变换ToolStripMenuItem_Click(object sender, EventArgs e)
        {            
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
            int i = tVLayers.SelectedNode.Index;
            mapControl.Extent(mapControl._MapLayers[mapControl._MapLayers.Count()-1-i]);
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
                    "pgsql2shp -f D:\\province_hainan.shp -u postgres mypostdb province_hainan";
                File.WriteAllText("test.bat", bat);
                Process.Start("test.bat");
                string path = "D:\\province_hainan.shp";//文件路径
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
                    "pgsql2shp -f D:\\county_hainan.shp -u postgres mypostdb county_hainan";
                File.WriteAllText("test.bat", bat);
                Process.Start("test.bat");
                string path = "D:\\county_hainan.shp";//文件路径
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
                    "pgsql2shp -f D:\\cyclone.shp -u postgres mypostdb cyclone";
                File.WriteAllText("test.bat", bat);
                Process.Start("test.bat");
                string path = "D:\\cyclone.shp";//文件路径
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

        private void 统计分析ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SelectLayer selFrm = new SelectLayer(mapControl);
            if (selFrm.ShowDialog(this) == DialogResult.OK)
            {
                int shpIndex = 0, tifIndex = 0;
                for (int i = 0; i < mapControl._MapLayers.Count(); i++)
                {
                    if (mapControl._MapLayers[i].Name == selFrm.cb1)
                        shpIndex = i;
                    else if (mapControl._MapLayers[i].Name == selFrm.cb2)
                        tifIndex = i;
                }
                Dataset ds = Gdal.Open(mapControl._MapLayers[tifIndex].FilePath, Access.GA_ReadOnly);
                DataSource ds2 = Ogr.Open(mapControl._MapLayers[shpIndex].FilePath, 0);
                Layer layer = ds2.GetLayerByIndex(0);
                //判断二者坐标系是否一致
                SpatialReference SrsLayer = layer.GetSpatialRef();
                SpatialReference SrsRas = new SpatialReference(ds.GetProjection());
                if (SrsLayer.IsSame(SrsRas) == 1)
                {
                    Statistic statistic = new Statistic();
                    //code-ave-max-min-count(最后一列count是像元数需要删去)
                    List<float[]> result = statistic.ComputeStatistic(layer, ds);                    
                    //存入数据库
                    DBConnector dbConnector = new DBConnector();
                    string name = selFrm.cb2;
                    name = name.Substring(0, name.Length - 4);//去掉名称中的.tif
                    //添加新表
                    dbConnector.AddTable(name);
                    //向新表中插入数据
                    dbConnector.InsertInfo(result,name);
                    MessageBox.Show("统计结果已经成功导入数据库！");                   
                }
                else
                    MessageBox.Show("数据坐标系不一致！");           
            }
        }

        private void 制图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveBitmapDialog.Filter = "bitmap(*.bmp)|*.bmp|All files(*.*)|*.*";
            if (saveBitmapDialog.ShowDialog() == DialogResult.OK)
            {
                string picPath = saveBitmapDialog.FileName;
                Bitmap bmp = mapControl.GetBitmap();
                //保存到磁盘文件
                bmp.Save(picPath, System.Drawing.Imaging.ImageFormat.Bmp);
                bmp.Dispose();
                MessageBox.Show("位图导出完成！", "制图结果");
            }
        }

        private void 矢量数据重投影ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //选择投影变换图层
            SelectLayer selFrm = new SelectLayer(mapControl);
            selFrm.Text = "选择变换图层";
            selFrm.label1.Text = "选择矢量图层";
            selFrm.label2.Visible = false;
            selFrm.comboBox2.Visible = false;
            if (selFrm.ShowDialog(this) == DialogResult.OK)
            {
                int index = 0;
                for (int i = 0; i < mapControl._MapLayers.Count(); i++)
                {
                    if (mapControl._MapLayers[i].Name == selFrm.cb1)
                    {
                        index = i;
                        break;
                    }
                }
                //选择输出文件路径
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "选择重投影文件存储路径";
                sfd.Filter = @"shp(*.shp)|*.shp";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string path = sfd.FileName; //文件路径
                    string[] path2 = path.Split(new char[1] { '\\' });//文件名
                    string name = path2[path2.Count() - 1];
                    //打开需转换矢量文件
                    DataSource ds = Ogr.Open(mapControl._MapLayers[index].FilePath, 0);
                    //投影转换
                    TransformProject transform = new TransformProject();
                    transform.TransformShp(ds,path);
                    //转换结果添加至地图
                    mapControl.AddShpLayer(path, name);
                    if (tVLayers.Nodes.Count == 0)
                        tVLayers.Nodes.Add("图层");
                    tVLayers.Nodes[0].Nodes.Insert(0, name);
                    tVLayers.ExpandAll();
                    tVLayers.SelectedNode = tVLayers.Nodes[0].Nodes[0];
                    MessageBox.Show("成功转换至Web墨卡托投影！", "投影转换结果");
                }
            }
        }

        private void 栅格数据重投影ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //选择投影变换图层
            SelectLayer selFrm = new SelectLayer(mapControl);
            selFrm.Text = "选择变换图层";
            selFrm.label2.Text = "选择栅格图层";
            selFrm.label1.Visible = false;
            selFrm.comboBox1.Visible = false;
            selFrm.label3.Visible = true;
            selFrm.comboBox3.Visible = true;
            selFrm.comboBox3.Items.Add("NearestNeighbour");
            selFrm.comboBox3.Items.Add("Bilinear");
            selFrm.comboBox3.Items.Add("Cubic");
            selFrm.comboBox3.Items.Add("CubicSpline");
            if (selFrm.ShowDialog(this) == DialogResult.OK)
            {
                int index = 0;
                for (int i = 0; i < mapControl._MapLayers.Count(); i++)
                {
                    if (mapControl._MapLayers[i].Name == selFrm.cb2)
                    {
                        index = i;
                        break;
                    }
                }
                //选择输出文件路径
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "选择重投影文件存储路径";
                sfd.Filter = @"Tiff(*.tif)|*.tif";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string path = sfd.FileName; //文件路径
                    string[] path2 = path.Split(new char[1] { '\\' });//文件名
                    string name = path2[path2.Count() - 1];
                    //投影转换
                    TransformProject transform = new TransformProject();
                    float[] MBR = new float[4];
                    mapControl._MapLayers[index].GetExtent(MBR);
                    double[] dbx = { (double)MBR[0], (double)MBR[0], (double)MBR[2], (double)MBR[2] };
                    double[] dby = { (double)MBR[1], (double)MBR[3], (double)MBR[1], (double)MBR[3] };
                    Dataset ds = Gdal.Open(mapControl._MapLayers[index].FilePath, Access.GA_ReadOnly);
                    //重采样方式
                    ResampleAlg re = ResampleAlg.GRA_NearestNeighbour;
                    if (selFrm.cb3 == "Bilinear")
                        re = ResampleAlg.GRA_Bilinear;
                    else if (selFrm.cb3 == "Cubic")
                        re = ResampleAlg.GRA_Cubic;
                    else if (selFrm.cb3 == "CubicSpline")
                        re = ResampleAlg.GRA_CubicSpline;
                    transform.TransformTiff(ds, dbx, dby, path, re);
                    //转换结果添加至地图
                    mapControl.AddTiffLayer(path, name);
                    if (tVLayers.Nodes.Count == 0)
                        tVLayers.Nodes.Add("图层");
                    tVLayers.Nodes[0].Nodes.Insert(0, name);
                    tVLayers.ExpandAll();
                    tVLayers.SelectedNode = tVLayers.Nodes[0].Nodes[0];
                    MessageBox.Show("成功转换至Web墨卡托投影！", "投影转换结果");
                }
            }

        }
    }
}
