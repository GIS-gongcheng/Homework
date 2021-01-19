using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

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
    }
}
