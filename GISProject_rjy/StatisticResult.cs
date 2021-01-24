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
    //该窗体用于显示统计分析结果
    public partial class StatisticResult : Form
    {
        public StatisticResult(List<float[]> result, List<string[]> county)
        {
            InitializeComponent();
            for (int i = 0; i < result.Count(); i++)
            {
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = result[i][0];
                this.dataGridView1.Rows[index].Cells[2].Value = result[i][1];
                this.dataGridView1.Rows[index].Cells[3].Value = result[i][2];
                this.dataGridView1.Rows[index].Cells[4].Value = result[i][3];
                for (int j = 0; j < county.Count(); j++)
                {
                    if (county[j][0] == result[i][0].ToString())
                    {
                        this.dataGridView1.Rows[index].Cells[1].Value = county[j][1];
                        break;
                    }
                }
            }

        }
    }
}
