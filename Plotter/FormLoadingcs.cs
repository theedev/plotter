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
    public partial class FormLoadingcs : Form
    {
        public FormLoadingcs()
        {
            InitializeComponent();
        }
        public FormLoadingcs(bool isUnknown, string text)
        {
            InitializeComponent();
            this.Text = text;
            if (isUnknown)
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
            }
        }

        private void formLoadingcs_Load(object sender, EventArgs e)
        {

        }

        public void setProgress(int prog)
        {
            progressBar1.Value = prog;
        }
    }
}
