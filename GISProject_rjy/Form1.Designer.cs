namespace GISProject_rjy
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开shp文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开Tiff文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导入数据库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出数据库shp文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出ProvinceHainanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出CountyHainanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出CycloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.投影变换ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.统计分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.制图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tVLayers = new System.Windows.Forms.TreeView();
            this.openSldFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.layerMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.上移图层ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载图层样式SLDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除图层ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.缩放至图层ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openShapefileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mapControl = new GISProject_rjy.MapControl();
            this.menuStrip1.SuspendLayout();
            this.layerMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.投影变换ToolStripMenuItem,
            this.统计分析ToolStripMenuItem,
            this.制图ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1524, 39);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem,
            this.导入数据库ToolStripMenuItem,
            this.导出数据库shp文件ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(82, 35);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开shp文件ToolStripMenuItem,
            this.打开Tiff文件ToolStripMenuItem});
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(356, 44);
            this.打开ToolStripMenuItem.Text = "打开";
            // 
            // 打开shp文件ToolStripMenuItem
            // 
            this.打开shp文件ToolStripMenuItem.Name = "打开shp文件ToolStripMenuItem";
            this.打开shp文件ToolStripMenuItem.Size = new System.Drawing.Size(348, 44);
            this.打开shp文件ToolStripMenuItem.Text = "打开Shapefile文件";
            this.打开shp文件ToolStripMenuItem.Click += new System.EventHandler(this.打开shp文件ToolStripMenuItem_Click);
            // 
            // 打开Tiff文件ToolStripMenuItem
            // 
            this.打开Tiff文件ToolStripMenuItem.Name = "打开Tiff文件ToolStripMenuItem";
            this.打开Tiff文件ToolStripMenuItem.Size = new System.Drawing.Size(348, 44);
            this.打开Tiff文件ToolStripMenuItem.Text = "打开Tiff文件";
            this.打开Tiff文件ToolStripMenuItem.Click += new System.EventHandler(this.打开Tiff文件ToolStripMenuItem_Click);
            // 
            // 导入数据库ToolStripMenuItem
            // 
            this.导入数据库ToolStripMenuItem.Name = "导入数据库ToolStripMenuItem";
            this.导入数据库ToolStripMenuItem.Size = new System.Drawing.Size(356, 44);
            this.导入数据库ToolStripMenuItem.Text = "导入数据库";
            this.导入数据库ToolStripMenuItem.Click += new System.EventHandler(this.导入数据库ToolStripMenuItem_Click);
            // 
            // 导出数据库shp文件ToolStripMenuItem
            // 
            this.导出数据库shp文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.导出ProvinceHainanToolStripMenuItem,
            this.导出CountyHainanToolStripMenuItem,
            this.导出CycloneToolStripMenuItem});
            this.导出数据库shp文件ToolStripMenuItem.Name = "导出数据库shp文件ToolStripMenuItem";
            this.导出数据库shp文件ToolStripMenuItem.Size = new System.Drawing.Size(356, 44);
            this.导出数据库shp文件ToolStripMenuItem.Text = "导出数据库shp文件";
            // 
            // 导出ProvinceHainanToolStripMenuItem
            // 
            this.导出ProvinceHainanToolStripMenuItem.Name = "导出ProvinceHainanToolStripMenuItem";
            this.导出ProvinceHainanToolStripMenuItem.Size = new System.Drawing.Size(386, 44);
            this.导出ProvinceHainanToolStripMenuItem.Text = "导出Province_Hainan";
            this.导出ProvinceHainanToolStripMenuItem.Click += new System.EventHandler(this.导出ProvinceHainanToolStripMenuItem_Click);
            // 
            // 导出CountyHainanToolStripMenuItem
            // 
            this.导出CountyHainanToolStripMenuItem.Name = "导出CountyHainanToolStripMenuItem";
            this.导出CountyHainanToolStripMenuItem.Size = new System.Drawing.Size(386, 44);
            this.导出CountyHainanToolStripMenuItem.Text = "导出County_Hainan";
            this.导出CountyHainanToolStripMenuItem.Click += new System.EventHandler(this.导出CountyHainanToolStripMenuItem_Click);
            // 
            // 导出CycloneToolStripMenuItem
            // 
            this.导出CycloneToolStripMenuItem.Name = "导出CycloneToolStripMenuItem";
            this.导出CycloneToolStripMenuItem.Size = new System.Drawing.Size(386, 44);
            this.导出CycloneToolStripMenuItem.Text = "导出Cyclone";
            this.导出CycloneToolStripMenuItem.Click += new System.EventHandler(this.导出CycloneToolStripMenuItem_Click);
            // 
            // 投影变换ToolStripMenuItem
            // 
            this.投影变换ToolStripMenuItem.Name = "投影变换ToolStripMenuItem";
            this.投影变换ToolStripMenuItem.Size = new System.Drawing.Size(130, 35);
            this.投影变换ToolStripMenuItem.Text = "投影变换";
            this.投影变换ToolStripMenuItem.Click += new System.EventHandler(this.投影变换ToolStripMenuItem_Click);
            // 
            // 统计分析ToolStripMenuItem
            // 
            this.统计分析ToolStripMenuItem.Name = "统计分析ToolStripMenuItem";
            this.统计分析ToolStripMenuItem.Size = new System.Drawing.Size(130, 35);
            this.统计分析ToolStripMenuItem.Text = "统计分析";
            // 
            // 制图ToolStripMenuItem
            // 
            this.制图ToolStripMenuItem.Name = "制图ToolStripMenuItem";
            this.制图ToolStripMenuItem.Size = new System.Drawing.Size(82, 35);
            this.制图ToolStripMenuItem.Text = "制图";
            // 
            // tVLayers
            // 
            this.tVLayers.Location = new System.Drawing.Point(8, 52);
            this.tVLayers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tVLayers.Name = "tVLayers";
            this.tVLayers.Size = new System.Drawing.Size(322, 918);
            this.tVLayers.TabIndex = 1;
            this.tVLayers.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tVLayers_NodeMouseClick);
            this.tVLayers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tVLayers_MouseUp);
            // 
            // openSldFileDialog
            // 
            this.openSldFileDialog.FileName = "openSldFileDialog";
            // 
            // layerMenuStrip
            // 
            this.layerMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.layerMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.上移图层ToolStripMenuItem,
            this.加载图层样式SLDToolStripMenuItem,
            this.删除图层ToolStripMenuItem,
            this.缩放至图层ToolStripMenuItem});
            this.layerMenuStrip.Name = "layerMenuStrip";
            this.layerMenuStrip.Size = new System.Drawing.Size(325, 156);
            // 
            // 上移图层ToolStripMenuItem
            // 
            this.上移图层ToolStripMenuItem.Name = "上移图层ToolStripMenuItem";
            this.上移图层ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.上移图层ToolStripMenuItem.Text = "上移图层";
            this.上移图层ToolStripMenuItem.Click += new System.EventHandler(this.上移图层ToolStripMenuItem_Click);
            // 
            // 加载图层样式SLDToolStripMenuItem
            // 
            this.加载图层样式SLDToolStripMenuItem.Name = "加载图层样式SLDToolStripMenuItem";
            this.加载图层样式SLDToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.加载图层样式SLDToolStripMenuItem.Text = "加载图层样式（SLD）";
            this.加载图层样式SLDToolStripMenuItem.Click += new System.EventHandler(this.加载图层样式SLDToolStripMenuItem_Click);
            // 
            // 删除图层ToolStripMenuItem
            // 
            this.删除图层ToolStripMenuItem.Name = "删除图层ToolStripMenuItem";
            this.删除图层ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.删除图层ToolStripMenuItem.Text = "删除图层";
            this.删除图层ToolStripMenuItem.Click += new System.EventHandler(this.删除图层ToolStripMenuItem_Click);
            // 
            // 缩放至图层ToolStripMenuItem
            // 
            this.缩放至图层ToolStripMenuItem.Name = "缩放至图层ToolStripMenuItem";
            this.缩放至图层ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.缩放至图层ToolStripMenuItem.Text = "缩放至图层";
            this.缩放至图层ToolStripMenuItem.Click += new System.EventHandler(this.缩放至图层ToolStripMenuItem_Click);
            // 
            // openShapefileDialog
            // 
            this.openShapefileDialog.FileName = "openFileDialog1";
            // 
            // mapControl
            // 
            this.mapControl.BackColor = System.Drawing.SystemColors.Window;
            this.mapControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mapControl.Location = new System.Drawing.Point(338, 52);
            this.mapControl.MapLayers = new GISProject_rjy.MapLayer[0];
            this.mapControl.Margin = new System.Windows.Forms.Padding(2);
            this.mapControl.Name = "mapControl";
            this.mapControl.Size = new System.Drawing.Size(1188, 918);
            this.mapControl.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1524, 959);
            this.Controls.Add(this.mapControl);
            this.Controls.Add(this.tVLayers);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximumSize = new System.Drawing.Size(1550, 1030);
            this.MinimumSize = new System.Drawing.Size(1550, 1030);
            this.Name = "Form1";
            this.Text = "GIS工程";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.layerMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开shp文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开Tiff文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导入数据库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 投影变换ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 统计分析ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 制图ToolStripMenuItem;
        private System.Windows.Forms.TreeView tVLayers;
        private MapControl mapControl;
        private System.Windows.Forms.OpenFileDialog openSldFileDialog;
        private System.Windows.Forms.ContextMenuStrip layerMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem 上移图层ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载图层样式SLDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除图层ToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openShapefileDialog;
        private System.Windows.Forms.ToolStripMenuItem 缩放至图层ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出数据库shp文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出ProvinceHainanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出CountyHainanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出CycloneToolStripMenuItem;
    }
}

