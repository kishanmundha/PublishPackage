using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublishPackage
{
    public partial class frmContentView : Form
    {
        public frmContentView(string Title, string Content)
        {
            InitializeComponent();

            this.Text = Title;
            this.richTextBox1.Text = Content;
        }
    }
}
