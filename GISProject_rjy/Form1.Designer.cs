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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开shp文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开Tiff文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导入数据库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.投影变换ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.统计分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.制图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.mapControl1 = new GISProject_rjy.MapControl();
            this.读取图层ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.投影变换ToolStripMenuItem,
            this.统计分析ToolStripMenuItem,
            this.制图ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(566, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem,
            this.导入数据库ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(43, 22);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开shp文件ToolStripMenuItem,
            this.打开Tiff文件ToolStripMenuItem});
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.打开ToolStripMenuItem.Text = "打开";
            // 
            // 打开shp文件ToolStripMenuItem
            // 
            this.打开shp文件ToolStripMenuItem.Name = "打开shp文件ToolStripMenuItem";
            this.打开shp文件ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.打开shp文件ToolStripMenuItem.Text = "打开Shapefile文件";
            this.打开shp文件ToolStripMenuItem.Click += new System.EventHandler(this.打开shp文件ToolStripMenuItem_Click);
            // 
            // 打开Tiff文件ToolStripMenuItem
            // 
            this.打开Tiff文件ToolStripMenuItem.Name = "打开Tiff文件ToolStripMenuItem";
            this.打开Tiff文件ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.打开Tiff文件ToolStripMenuItem.Text = "打开Tiff文件";
            this.打开Tiff文件ToolStripMenuItem.Click += new System.EventHandler(this.打开Tiff文件ToolStripMenuItem_Click);
            // 
            // 导入数据库ToolStripMenuItem
            // 
            this.导入数据库ToolStripMenuItem.Name = "导入数据库ToolStripMenuItem";
            this.导入数据库ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.导入数据库ToolStripMenuItem.Text = "导入数据库";
            // 
            // 投影变换ToolStripMenuItem
            // 
            this.投影变换ToolStripMenuItem.Name = "投影变换ToolStripMenuItem";
            this.投影变换ToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            this.投影变换ToolStripMenuItem.Text = "投影变换";
            // 
            // 统计分析ToolStripMenuItem
            // 
            this.统计分析ToolStripMenuItem.Name = "统计分析ToolStripMenuItem";
            this.统计分析ToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            this.统计分析ToolStripMenuItem.Text = "统计分析";
            // 
            // 制图ToolStripMenuItem
            // 
            this.制图ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.读取图层ToolStripMenuItem});
            this.制图ToolStripMenuItem.Name = "制图ToolStripMenuItem";
            this.制图ToolStripMenuItem.Size = new System.Drawing.Size(43, 22);
            this.制图ToolStripMenuItem.Text = "制图";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(6, 26);
            this.treeView1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(128, 364);
            this.treeView1.TabIndex = 1;
            // 
            // mapControl1
            // 
            this.mapControl1.BackColor = System.Drawing.SystemColors.Window;
            this.mapControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mapControl1.Location = new System.Drawing.Point(136, 26);
            this.mapControl1.MapLayers = new GISProject_rjy.MapLayer[0];
            this.mapControl1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.Size = new System.Drawing.Size(428, 364);
            this.mapControl1.TabIndex = 2;
            // 
            // 读取图层ToolStripMenuItem
            // 
            this.读取图层ToolStripMenuItem.Name = "读取图层ToolStripMenuItem";
            this.读取图层ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.读取图层ToolStripMenuItem.Text = "读取图层";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 394);
            this.Controls.Add(this.mapControl1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.TreeView treeView1;
        private MapControl mapControl1;
        private System.Windows.Forms.ToolStripMenuItem 读取图层ToolStripMenuItem;
    }
}

