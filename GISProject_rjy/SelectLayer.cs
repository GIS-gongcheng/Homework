using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GISProject_rjy
{
    public partial class SelectLayer : Form
    {
        public string cb1;
        public string cb2;
        public string cb3;
        public SelectLayer(MapControl mapControl)
        {
            InitializeComponent();
            foreach (MapLayer layer in mapControl._MapLayers)
            {
                if (layer.Type == "Shp")
                    comboBox1.Items.Add(layer.Name);
                else
                    comboBox2.Items.Add(layer.Name);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb1 = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb2 = comboBox2.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb3 = comboBox3.Text;
        }
    }
}
