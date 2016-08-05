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
    public partial class Form2 : Form
    {
        Panel panel;
        Panel CurrentPanel;
        Models.IOperationStep operation;
        System.Threading.Thread nextAnimation;

        List<Panel> PanelSteps = new List<Panel>();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //nextAnimation = new System.Threading.Thread(() => {
            //});

            operation = new Models.ProfileSelect();
            Next();
            //panel = operation.GetComponent();
            //panel.Width = this.Width;
            //panel.Height = this.Height - panel1.Height;

            //this.Controls.Add(panel);
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            //this.Text = "This (" + this.Width + ", " + this.Height + ") - Child (" + ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                AutoNext();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Previous();
        }

        #region Helper

        private void RegisterEvents()
        {
            operation.OnComplete += (s, e) =>
            {
                AutoNext();
            };
        }

        private void AutoNext()
        {
            var NextOperation = operation.GetNextInstance();
            NextOperation.PreviousStep = operation;
            operation = NextOperation;
            RegisterEvents();
            Next();
        }

        private void Next()
        {
            if(CurrentPanel != null)
            {
                CurrentPanel.Left -= CurrentPanel.Width;
            }

            Panel panel = operation.GetComponent();
            panel.Width = this.Width;
            panel.Height = this.Height - panel1.Height;
            panel.Left = 0;

            this.Controls.Add(panel);

            PanelSteps.Add(panel);

            CurrentPanel = panel;
        }

        private void Previous()
        {
            if (PanelSteps.Count == 1)
                return;

            this.Controls.Remove(CurrentPanel);
            PanelSteps.Remove(PanelSteps.Last());

            CurrentPanel = PanelSteps.Last();
            operation = operation.PreviousStep;

            CurrentPanel.Left = 0;
        }


        #endregion
    }
}
