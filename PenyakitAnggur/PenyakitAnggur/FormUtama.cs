using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PenyakitAnggur
{
    public partial class FormUtama : Form
    {
        private Training training;
        private Testing testing;

        public FormUtama()
        {
            InitializeComponent();
            
            panelWindow.Controls.Clear();
            training = new Training();
            training.TopLevel = false;
            panelWindow.Controls.Add(training);
            training.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            training.Dock = DockStyle.Fill;
            training.Show();
            

        }

        private void testingToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            panelWindow.Controls.Clear();
            training = new Training();
            training.TopLevel = false;
            panelWindow.Controls.Add(training);
            training.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            training.Dock = DockStyle.Fill;
            training.Show();
        }

        private void pengujianToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            panelWindow.Controls.Clear();
            testing = new Testing();
            testing.TopLevel = false;
            panelWindow.Controls.Add(testing);
            testing.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            testing.Dock = DockStyle.Fill;
            testing.Show();
        }

    }
}
