﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublishPackage.Models
{
    public interface IProfile
    {
        ProfileType ProfileType { get; }

        string InfoString { get; }
    }

    public class ArchiveProfile : IProfile
    {
        ProfileType IProfile.ProfileType
        {
            get
            {
                return ProfileType.Archive;
            }
        }

        public string ProfileName { get; set; }
        public string SourcePath { get; set; }
        public string SourceDBPath { get; set; }
        public string VersionFolderPath { get; set; }

        public string InfoString
        {
            get
            {
                return "Profile Name : " + ProfileName + "\r\n"
                    + "Source Path : " + SourcePath + "\r\n"
                    + "Source DB : " + SourceDBPath + "\r\n"
                    + "VersionFolder Path : " + VersionFolderPath;
            }
        }

        public override string ToString()
        {
            return this.ProfileName;
        }
    }

    public enum ProfileType
    {
        Archive,
        Extract
    }

    public interface IOperationStep
    {
        IOperationStep PreviousStep { get; set; }

        bool CanClose { get; set; }
        void Start();
        Panel GetComponent();
        IOperationStep GetNextInstance();

        event EventHandler OnComplete;
    }

    /*
     * * Make archive *
     *
     * Select profile
     * Select Last version
     * Version Name and options
     * ProgressBar - Extract Last version files to temp1, Copy Published code and db generated script to temp2, get compare result
     * Show compare result (link for map renamed result)
     * ProgressBar - Make arcvhie
     */

    public class ProfileSelect : IOperationStep
    {
        public IOperationStep PreviousStep
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler OnComplete;

        public void Start()
        {
            
        }

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            Label lbl = new Label();
            lbl.Text = "Select profile";
            lbl.Top = 20;
            lbl.Left = 10;

            panel.Controls.Add(lbl);

            ComboBox cbx = new ComboBox();
            cbx.Name = "Combo1";
            cbx.Left = 10;
            cbx.Top = 60;
            cbx.Width = panel.Width - 40;
            cbx.DropDownStyle = ComboBoxStyle.DropDownList;
            cbx.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            cbx.DataSource = this.GetProfileList();

            panel.Controls.Add(cbx);

            RichTextBox rtb = new RichTextBox();
            rtb.Name = "rtb1";
            rtb.Left = 10;
            rtb.Top = 100;
            rtb.BorderStyle = BorderStyle.None;
            rtb.Width = panel.Width - 40;
            rtb.Height = panel.Height - 100;
            rtb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            panel.Controls.Add(rtb);

            cbx.SelectedValueChanged += (s, e) =>
            {
                rtb.Text = ((s as ComboBox).SelectedValue as IProfile).InfoString;
            };

            return panel;
        }

        public IProfile Profile { get; set; }

        public bool CanClose
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IOperationStep GetNextInstance()
        {
            return new VersionSelect();
        }

        private List<IProfile> GetProfileList()
        {
            List<IProfile> list = new List<IProfile>();

            var profile = new ArchiveProfile();
            profile.ProfileName = "Test profile";
            list.Add(profile);

            var profile2 = new ArchiveProfile();
            profile2.ProfileName = "Test profile 2";
            list.Add(profile2);

            return list;
        }
    }

    public class VersionSelect : IOperationStep
    {
        public bool CanClose { get; set; }

        public IOperationStep PreviousStep { get; set; }

        public event EventHandler OnComplete;

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            Label lbl = new Label();
            lbl.Text = "Select version";
            lbl.Top = 20;
            lbl.Left = 10;

            panel.Controls.Add(lbl);

            ComboBox cbx = new ComboBox();
            cbx.Name = "Combo1";
            cbx.Left = 10;
            cbx.Top = 60;
            cbx.Width = panel.Width - 40;
            cbx.DropDownStyle = ComboBoxStyle.DropDownList;
            cbx.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panel.Controls.Add(cbx);

            return panel;
        }

        public void Start()
        {
            
        }


        public IOperationStep GetNextInstance()
        {
            return new OptionSelect();
        }
    }

    public class OptionSelect : IOperationStep
    {
        public bool CanClose { get; set; }

        public IOperationStep PreviousStep { get; set; }

        public event EventHandler OnComplete;

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            var groupBox1 = new System.Windows.Forms.GroupBox();
            var checkBox1 = new System.Windows.Forms.CheckBox();
            var checkBox2 = new System.Windows.Forms.CheckBox();

            groupBox1.Controls.Add(checkBox2);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Location = new System.Drawing.Point(10, 20);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(panel.Width - 40, panel.Height - 80);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Options";
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(10, 32);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(98, 21);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;

            checkBox2.AutoSize = true;
            checkBox2.Location = new System.Drawing.Point(10, 59);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(98, 21);
            checkBox2.TabIndex = 2;
            checkBox2.Text = "checkBox2";
            checkBox2.UseVisualStyleBackColor = true;

            panel.Controls.Add(groupBox1);

            return panel;
        }

        public void Start()
        {
            
        }


        public IOperationStep GetNextInstance()
        {
            return new ExecuteOperation();
        }
    }

    public class ExecuteOperation : IOperationStep
    {
        System.ComponentModel.BackgroundWorker bg;
        public bool CanClose { get; set; }

        public IOperationStep PreviousStep { get; set; }

        public event EventHandler OnComplete;

        public Panel GetComponent()
        {
            ProgressPanel p = new ProgressPanel();
            Panel panel = p.GetComponent();

            bg = new System.ComponentModel.BackgroundWorker();
            bg.DoWork += (s, e) =>
            {
                int a = 0;
                for (var i = 0; i < 10000; i++)
                {
                    for (var j = 0; j < 100000; j++)
                        a++;

                    //p.Update((i * 100) / 10000);
                    bg.ReportProgress((i * 100) / 10000);
                }

                bg.ReportProgress(100);
            };

            bg.ProgressChanged += (s, e) =>
            {
                p.Update(e.ProgressPercentage);
            };

            bg.RunWorkerCompleted += (s, e) =>
            {
                if (this.OnComplete != null)
                    this.OnComplete(this, null);
            };

            bg.WorkerReportsProgress = true;

            return panel;
        }

        public void Start()
        {
            bg.RunWorkerAsync();
        }

        public IOperationStep GetNextInstance()
        {
            return new ShowCompareResult();
        }
    }

    public class ShowCompareResult : IOperationStep
    {
        public bool CanClose { get; set; }

        public IOperationStep PreviousStep { get; set; }

        public event EventHandler OnComplete;

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            return panel;
        }

        public IOperationStep GetNextInstance()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            
        }
    }

    public class ProgressPanel
    {
        private ProgressBar progressBar;
        private Label statusLabel;
        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            Label lbl = new Label();
            statusLabel = lbl;
            lbl.Text = "Progress";
            lbl.Top = 80;
            lbl.Left = 10;

            panel.Controls.Add(lbl);

            ProgressBar pb = new ProgressBar();
            progressBar = pb;
            pb.Top = 120;
            pb.Left = 10;
            pb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pb.Width = panel.Width - 40;

            panel.Controls.Add(pb);

            return panel;
        }

        private void SetProgress(int p)
        {
            //if (progressBar.InvokeRequired)
            //    progressBar.Invoke(() => { progressBar.Value = Math.Min(Math.Max(p, 0), 100); });
            //else
                progressBar.Value = Math.Min(Math.Max(p, 0), 100);
        }

        public void Update(int p)
        {
            SetProgress(p);
        }

        public void Update(string msg)
        {

        }

        public void Update(int p, string msg)
        {

        }
    }

    public class TestClass
    {
        public void run()
        {
            //ProfileSelect ps = new ProfileSelect();
            //ps.Profile = new ArchiveProfile();

            //var vs = new VersionSelect();
            //ps.NextStep = vs;
            //ps.NextStep.Open(ps, ps.Profile);

            //var os = new OptionSelect();
            //vs.NextStep = os;
            //os.Open(vs, null);

            //var es = new ExecuteOperation();
            //os.NextStep = es;
            //es.Open(os, null);
        }
    }
}
