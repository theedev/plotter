using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter
{
    public partial class FormColour : Form
    {
        const string ConfigFileName = "Config.ini";
        public FormColour()
        {
            InitializeComponent();
        }

        private void FormColour_Load(object sender, EventArgs e)
        {
            LoadList();
        }

        private void LoadList()
        {
            listBox1.Items.Clear();
            foreach (KeyValuePair<String, Color> colour in plotter.Colours)
            {
                listBox1.Items.Add(colour.Key);
            }
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }
            
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.BackColor = plotter.Colours[listBox1.SelectedItem.ToString()];
            textBox1.Text = listBox1.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!listBox1.Items.Contains(textBox1.Text))
                {
                    plotter.Colours.Add(textBox1.Text, dlg.Color);
                    LoadList();
                    plotter.Saver(ConfigFileName);
                    plotter.Loader(ConfigFileName);
                    plotter.ReloadCols();
                }
                else
                {
                    plotter.Colours.Remove(textBox1.Text);
                    plotter.Colours.Add(textBox1.Text, dlg.Color);
                    LoadList();
                    plotter.Saver(ConfigFileName);
                    plotter.Loader(ConfigFileName);
                    plotter.ReloadCols();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            plotter.Colours.Remove(listBox1.SelectedItem.ToString());
            LoadList();
            plotter.Saver(ConfigFileName);
            plotter.Loader(ConfigFileName);
            plotter.ReloadCols();
        }
    }
}
