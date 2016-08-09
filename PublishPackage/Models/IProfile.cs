using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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
                return "Profile Name : " + ProfileName + "\r\n\r\n"
                    + "Source Path : " + SourcePath + "\r\n\r\n"
                    + "Source DB : " + SourceDBPath + "\r\n\r\n"
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
        dynamic data { get; set; }

        bool IsProgressive { get; }
        void Start();
        Panel GetComponent();
        IOperationStep GetNextInstance();

        event EventHandler OnComplete;

        bool IsFirstStep { get; }
        bool IsLastStep { get; }
        bool LockPreviousStep { get; }
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

    public class LegelPageAccept : IOperationStep
    {
        CheckBox cb;
        public bool IsProgressive { get; }

        public dynamic data { get; set; }

        public IOperationStep PreviousStep { get; set; }

        public bool IsFirstStep
        {
            get
            {
                return true;
            }
        }

        public bool IsLastStep { get; }

        public bool LockPreviousStep
        {
            get
            {
                return true;
            }
        }

        public event EventHandler OnComplete;

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            RichTextBox rtb = new RichTextBox();
            rtb.Name = "rtb1";
            rtb.Left = 10;
            rtb.Top = 20;
            rtb.BorderStyle = BorderStyle.None;
            rtb.BackColor = System.Drawing.SystemColors.Window;
            rtb.Width = panel.Width - 40;
            rtb.Height = panel.Height - 200;
            rtb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            rtb.ReadOnly = true;
            rtb.Text = @"This software is currently avaiable for only beta version. Use this sowftware on your own risk.";

            panel.Controls.Add(rtb);

            cb = new CheckBox();
            cb.Left = 10;
            cb.Top = panel.Height - 75;
            cb.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            cb.Text = "I Accept";

            panel.Controls.Add(cb);

            return panel;
        }

        public IOperationStep GetNextInstance()
        {
            if (!cb.Checked)
                throw new Exception("To continue, you must accept agreement");
            return new ProfileSelect();
        }

        public void Start()
        {
            
        }
    }

    public class ProfileSelect : IOperationStep
    {
        ComboBox cbx;
        public IOperationStep PreviousStep { get; set; }

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

            cbx = new ComboBox();
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
                var profile = (s as ComboBox).SelectedValue as IProfile;

                if (profile == null)
                {
                    rtb.Text = @"There is no profile configured, Put profiles in ""Profiles"" folder in "".json"" format.

Sample JSON:
{
    ""profileName"" : ""Profile 1"",
    ""profileType"" : ""Archive"",
    ""sourcePath"" : ""D:\\PublishedSitesPath"",
    ""sourceDBPath"" : ""data source=192.168.0.10;initial catalog=dbName;integrated security=False;User Id=sa;Password=testpassword"",
    ""versionFolderPath"" : ""D:\\version""
}

Use profileType always ""Archive""
                ";
                }
                else
                {
                    rtb.Text = ((s as ComboBox).SelectedValue as IProfile).InfoString;
                }
            };

            return panel;
        }

        public IProfile Profile { get; set; }

        public bool IsProgressive { get; }

        public dynamic data { get; set; }

        public bool IsFirstStep { get { return true; } }

        public bool IsLastStep { get; }

        public bool LockPreviousStep
        {
            get
            {
                return true;
            }
        }

        public IOperationStep GetNextInstance()
        {
            var obj = new VersionSelect();

            var p = (cbx.SelectedValue as ArchiveProfile);

            if (p == null)
                throw new Exception("Profile required");

            obj.data = new ExpandoObject();
            obj.data.ProfileName = p.ProfileName;
            obj.data.SourcePath = p.SourcePath;
            obj.data.SourceDBPath = p.SourceDBPath;
            obj.data.VersionFolderPath = p.VersionFolderPath;
            return obj;
        }

        private List<IProfile> GetProfileList()
        {
            List<IProfile> list = new List<IProfile>();

            string dir = System.IO.Directory.GetCurrentDirectory() + "\\Profiles";

            if (!Directory.Exists(dir))
                return list;

            var files = System.IO.Directory.GetFiles(dir, "*.json");

            foreach (var file in files)
            {
                try
                {
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(System.IO.File.ReadAllText(file));

                    if (obj == null)
                        continue;

                    if (obj.profileType == "Archive")
                    {
                        var p = new ArchiveProfile();
                        p.ProfileName = obj.profileName;
                        p.SourcePath = obj.sourcePath;
                        p.SourceDBPath = obj.sourceDBPath;
                        p.VersionFolderPath = obj.versionFolderPath;

                        list.Add(p);
                    }
                }
                catch
                {

                }
            }

            return list;
        }
    }

    public class VersionSelect : IOperationStep
    {
        ComboBox cbx;
        public bool IsProgressive { get; }

        public IOperationStep PreviousStep { get; set; }

        public dynamic data { get; set; }

        public bool IsFirstStep { get; }

        public bool IsLastStep { get; }

        public bool LockPreviousStep { get; }

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

            cbx = new ComboBox();
            cbx.Name = "Combo1";
            cbx.Left = 10;
            cbx.Top = 60;
            cbx.Width = panel.Width - 40;
            cbx.DropDownStyle = ComboBoxStyle.DropDownList;
            cbx.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            cbx.DataSource = this.GetVersionList();
            cbx.DisplayMember = "VersionName";
            cbx.ValueMember = "Path";

            panel.Controls.Add(cbx);

            return panel;
        }

        public void Start()
        {
            if (cbx.Items.Count > 0)
                cbx.SelectedIndex = cbx.Items.Count - 1;
        }

        private System.Data.DataTable GetVersionList()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("VersionName");
            dt.Columns.Add("Path");

            string VersionFolderPath = this.data.VersionFolderPath;

            var files = Directory.GetFiles(VersionFolderPath, "*.zip");

            foreach (var f in files)
            {
                FileInfo fi = new FileInfo(f);

                dt.Rows.Add(fi.Name.Replace(fi.Extension, ""), fi.FullName);
            }

            return dt;
        }


        public IOperationStep GetNextInstance()
        {
            var obj = new OptionSelect();

            obj.data = this.data;
            obj.data.LastVersionFile = (cbx.SelectedValue as string);

            return obj;
        }
    }

    public class OptionSelect : IOperationStep
    {
        TextBox txtBox;
        public bool IsProgressive { get;}

        public IOperationStep PreviousStep { get; set; }

        public dynamic data { get; set; }

        public bool IsFirstStep { get; }

        public bool IsLastStep { get; }

        public bool LockPreviousStep { get; }

        public event EventHandler OnComplete;

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            var groupBox1 = new System.Windows.Forms.GroupBox();
            var checkBox1 = new System.Windows.Forms.CheckBox();
            var checkBox2 = new System.Windows.Forms.CheckBox();

            //groupBox1.Controls.Add(checkBox2);
            //groupBox1.Controls.Add(checkBox1);
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

            Label lbl = new Label();
            lbl.Text = "Version name";
            lbl.Left = 10;
            lbl.Top = 40;
            groupBox1.Controls.Add(lbl);

            txtBox = new TextBox();
            txtBox.Location = new System.Drawing.Point(10, 80);
            txtBox.Left = 10;
            txtBox.Top = 80;
            txtBox.Width = groupBox1.Width - 20;
            txtBox.Height = 21;
            txtBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            groupBox1.Controls.Add(txtBox);

            panel.Controls.Add(groupBox1);

            return panel;
        }

        public void Start()
        {

        }


        public IOperationStep GetNextInstance()
        {
            if (string.IsNullOrWhiteSpace(txtBox.Text))
            {
                throw new Exception("Fill version name");
            }

            var obj = new ExecuteOperation();

            obj.data = this.data;
            obj.data.VersionName = txtBox.Text;

            return obj;
        }
    }

    public class ExecuteOperation : IOperationStep
    {
        System.ComponentModel.BackgroundWorker bg;
        public bool IsProgressive { get { return true; } }

        public IOperationStep PreviousStep { get; set; }

        public dynamic data { get; set; }

        public bool IsFirstStep { get; }

        public bool IsLastStep { get; }

        public bool LockPreviousStep { get { return true; } }

        public event EventHandler OnComplete;

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public Panel GetComponent()
        {
            ProgressPanel p = new ProgressPanel();
            Panel panel = p.GetComponent();

            bg = new System.ComponentModel.BackgroundWorker();
            bg.DoWork += (s, e) =>
            {
                bg.ReportProgress(0, "Preparing...");

                var path = System.IO.Path.GetTempPath();
                string tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                System.IO.Directory.CreateDirectory(tempDirectory);

                var sourcePath = this.data.SourcePath;

                var tempOldPath = tempDirectory + "\\old";

                bool LastVersionSelected = false;
                if (this.data.LastVersionFile != null)
                {
                    Directory.CreateDirectory(tempOldPath);
                    System.IO.Compression.ZipFile.ExtractToDirectory(this.data.LastVersionFile, tempOldPath);
                    LastVersionSelected = true;
                }

                var tempNewpath = tempDirectory + "\\current";

                Directory.CreateDirectory(tempNewpath);

                var targetpath = tempNewpath + "\\code";

                Directory.CreateDirectory(targetpath);

                DirectoryCopy(sourcePath, targetpath, true);

                bg.ReportProgress(25, "Comparing...");

                ISqlReader reader = new SqlReaderByDatabase();
                SqlDatabase database = reader.Get(this.data.SourceDBPath);

                var str = database.GetJsonString();

                var targetDBPath = tempDirectory + "\\db";

                Directory.CreateDirectory(targetDBPath);

                var file = File.CreateText(targetDBPath + "\\script.json");

                file.Write(str);

                file.Close();

                IFolderReader fReader = new FolderReaderFromSystem();
                var folder2 = fReader.Get(targetpath);

                PFolder folder1 = null;
                SqlDatabase databaseOld = null;

                if (LastVersionSelected)
                {
                    if (File.Exists(tempOldPath + "\\codeStructure.json"))
                    {
                        IFolderReader fReader2 = new FolderReaderFromJson();
                        folder1 = fReader2.Get(tempOldPath + "\\codeStructure.json");
                    }
                    else
                    {
                        folder1 = fReader.Get(tempOldPath + "\\code");
                    }

                    ISqlReader reader2 = new SqlReaderByJson();
                    databaseOld = reader2.Get(tempOldPath + "\\db\\script.json");
                }

                var folderCompare = PFolderCompareResult.Compare(folder1, folder2);

                var dbCompare = SqlDatabaseCompareResult.Compare(databaseOld, database);

                var comparePath = tempDirectory + "\\compareResult";

                Directory.CreateDirectory(comparePath);

                file = File.CreateText(comparePath + "\\codeCompareResult.txt");
                file.Write(folderCompare.GetScript());
                file.Close();

                file = File.CreateText(comparePath + "\\codeStructure.json");
                file.Write(folder2.GetJsonString());
                file.Close();

                var compareDBPath = comparePath + "\\db";

                Directory.CreateDirectory(compareDBPath);

                file = File.CreateText(compareDBPath + "\\script.sql");
                file.Write(dbCompare.GetScript());
                file.Close();

                file = File.CreateText(compareDBPath + "\\script.json");
                file.Write(database.GetJsonString());
                file.Close();

                var compareTargetpath = comparePath + "\\code";

                Directory.CreateDirectory(compareTargetpath);

                var fileList = folderCompare.GetFileList();

                foreach (var f in fileList)
                {
                    var folderName = GetFolderName(compareTargetpath + f);
                    SafeCreateDirectory(folderName);

                    File.Copy(targetpath + f, compareTargetpath + f, true);
                }

                bg.ReportProgress(70, "Making archive...");

                string VersionFolderPath = this.data.VersionFolderPath;

                if (!Directory.Exists(VersionFolderPath))
                    Directory.CreateDirectory(VersionFolderPath);

                System.IO.Compression.ZipFile.CreateFromDirectory(comparePath, VersionFolderPath + "\\" + this.data.VersionName + ".zip");

                Directory.Delete(tempDirectory, true);

                bg.ReportProgress(100);
                System.Threading.Thread.Sleep(1000);
            };

            bg.ProgressChanged += (s, e) =>
            {
                string msg = (e.UserState as string);

                if (msg != null)
                {
                    p.Update(e.ProgressPercentage, msg);
                }
                else
                {
                    p.Update(e.ProgressPercentage);
                }
            };

            bg.RunWorkerCompleted += (s, e) =>
            {
                if (this.OnComplete != null)
                    this.OnComplete(this, null);
            };

            bg.WorkerReportsProgress = true;

            return panel;
        }

        private void SafeCreateDirectory(string folderName)
        {
            DirectoryInfo di = new DirectoryInfo(folderName);

            if (!di.Exists)
            {
                if (!di.Parent.Exists)
                {
                    SafeCreateDirectory(di.Parent.Name);
                }

                Directory.CreateDirectory(folderName);
            }
        }

        private string GetFolderName(string f)
        {
            FileInfo fi = new FileInfo(f);
            return fi.DirectoryName;
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
        public bool IsProgressive { get; }

        public dynamic data { get; set; }

        public IOperationStep PreviousStep { get; set; }

        public bool IsFirstStep { get; }

        public bool IsLastStep { get { return true; } }

        public bool LockPreviousStep { get { return true; } }

        public event EventHandler OnComplete;

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            Label lbl = new Label();
            lbl.Text = "Completed";
            lbl.Top = 20;
            lbl.Left = 10;

            panel.Controls.Add(lbl);

            return panel;
        }

        public IOperationStep GetNextInstance()
        {
            Application.Exit();
            return null;
            //throw new NotImplementedException();
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
            lbl.Width = panel.Width - 20;
            lbl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

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

        private void SetStatus(string msg)
        {
            //if (progressBar.InvokeRequired)
            //    progressBar.Invoke(() => { progressBar.Value = Math.Min(Math.Max(p, 0), 100); });
            //else
            statusLabel.Text = msg;
        }

        public void Update(int p)
        {
            SetProgress(p);
        }

        public void Update(string msg)
        {
            SetStatus(msg);
        }

        public void Update(int p, string msg)
        {
            SetProgress(p);
            SetStatus(msg);
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
