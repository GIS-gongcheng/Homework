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
    }
}
