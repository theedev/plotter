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
        const string configFileName = "Config.ini";
        public FormColour()
        {
            InitializeComponent();
        }

        private void formColour_Load(object sender, EventArgs e)
        {
            loadList();
        }

        private void loadList()
        {
            listBox1.Items.Clear();
            foreach (KeyValuePair<String, Color> colour in PlotterFunctions.colours)
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
            panel1.BackColor = PlotterFunctions.colours[listBox1.SelectedItem.ToString()];
            textBox1.Text = listBox1.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!listBox1.Items.Contains(textBox1.Text))
                {
                    PlotterFunctions.colours.Add(textBox1.Text, dlg.Color);
                    loadList();
                    PlotterFunctions.saver(configFileName);
                    PlotterFunctions.loader(configFileName);
                    PlotterFunctions.reloadColours();
                }
                else
                {
                    PlotterFunctions.colours.Remove(textBox1.Text);
                    PlotterFunctions.colours.Add(textBox1.Text, dlg.Color);
                    loadList();
                    PlotterFunctions.saver(configFileName);
                    PlotterFunctions.loader(configFileName);
                    PlotterFunctions.reloadColours();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PlotterFunctions.colours.Remove(listBox1.SelectedItem.ToString());
            loadList();
            PlotterFunctions.saver(configFileName);
            PlotterFunctions.loader(configFileName);
            PlotterFunctions.reloadColours();
        }
    }
}
