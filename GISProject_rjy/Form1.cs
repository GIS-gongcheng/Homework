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
            InitializeComponent();
        }

        private void 打开shp文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            }
        }

        private void 打开Tiff文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

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


    }
}
