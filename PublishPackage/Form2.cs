﻿using System;
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
        Models.IOperationStep operation;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            operation = new Models.ProfileSelect();
            panel = operation.GetComponent();
            panel.Width = this.Width;
            panel.Height = this.Height - panel1.Height;

            this.Controls.Add(panel);
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            //this.Text = "This (" + this.Width + ", " + this.Height + ") - Child (" + ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var NextOperation = operation.GetNextInstance();
                NextOperation.PreviousStep = operation;
                this.Controls.Remove(panel);

                operation = NextOperation;
                panel = operation.GetComponent();
                panel.Width = this.Width;
                panel.Height = this.Height - panel1.Height;

                this.Controls.Add(panel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
