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
        System.Threading.Thread panelSlide;

        List<Panel> PanelSteps = new List<Panel>();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //nextAnimation = new System.Threading.Thread(() => {
            //});

            operation = new Models.LegelPageAccept();
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
            if (operation.IsLastStep)
            {
                Application.Exit();
                return;
            }
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
                CurrentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            }

            Panel panel = operation.GetComponent();
            panel.Width = this.Width;
            panel.Height = this.Height - panel1.Height;
            panel.Left = 0;
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            this.Controls.Add(panel);

            PanelSteps.Add(panel);

            CurrentPanel = panel;

            ChangeButtonStatus();

            operation.Start();
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
            CurrentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            ChangeButtonStatus();

            operation.Start();
        }

        private void ChangeButtonStatus()
        {
            button2.Visible = !operation.IsFirstStep && !operation.IsLastStep;
            button2.Enabled = !operation.LockPreviousStep || !operation.IsProgressive;
            button1.Enabled = !operation.IsProgressive || !operation.IsLastStep;

            button1.Text = operation.IsLastStep ? "&Finish" : "&Next";
        }


        #endregion

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (operation.IsProgressive)
            {
                e.Cancel = true;
                MessageBox.Show("Can't close on progress");
            }
        }
    }
}
