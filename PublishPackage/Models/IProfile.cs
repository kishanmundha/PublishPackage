using System;
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

        public string SourcePath { get; set; }
        public string SourceDBPath { get; set; }
        public string VersionFolderPath { get; set; }
    }

    public enum ProfileType
    {
        Archive,
        Extract
    }

    public interface IOperationStep
    {
        IOperationStep PreviousStep { get; set; }
        IOperationStep NextStep { get; set; }

        void Open(IOperationStep PreviousStep, object data);
        bool GoNext();

        bool CanClose { get; set; }
        void Start();
        Panel GetComponent();
        IOperationStep GetNextInstance();
    }

    public class ProfileSelect : IOperationStep
    {
        public IOperationStep NextStep
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

        public bool GoNext()
        {
            if (NextStep != null)
                NextStep.Open(this, null);
            throw new NotImplementedException();
        }

        public void Open(IOperationStep PreviousStep, object data)
        {
            this.PreviousStep = PreviousStep;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //panel.BorderStyle = BorderStyle.FixedSingle;

            ComboBox cbx = new ComboBox();
            cbx.Name = "Combo1";
            cbx.Left = 10;
            cbx.Top = 20;
            cbx.DropDownStyle = ComboBoxStyle.DropDownList;
            cbx.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panel.Controls.Add(cbx);

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
    }

    public class VersionSelect : IOperationStep
    {
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

        public IOperationStep NextStep
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

        public IOperationStep PreviousStep { get; set; }

        public Panel GetComponent()
        {
            Panel panel = new Panel();
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            return panel;
        }

        public bool GoNext()
        {
            throw new NotImplementedException();
        }

        public void Open(IOperationStep PreviousStep, object data)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }


        public IOperationStep GetNextInstance()
        {
            throw new NotImplementedException();
        }
    }

    public class OptionSelect : IOperationStep
    {
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

        public IOperationStep NextStep
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

        public Panel GetComponent()
        {
            throw new NotImplementedException();
        }

        public bool GoNext()
        {
            throw new NotImplementedException();
        }

        public void Open(IOperationStep PreviousStep, object data)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }


        public IOperationStep GetNextInstance()
        {
            throw new NotImplementedException();
        }
    }

    public class ExecuteOperation : IOperationStep
    {
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

        public IOperationStep NextStep
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

        public Panel GetComponent()
        {
            throw new NotImplementedException();
        }

        public bool GoNext()
        {
            throw new NotImplementedException();
        }

        public void Open(IOperationStep PreviousStep, object data)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }


        public IOperationStep GetNextInstance()
        {
            throw new NotImplementedException();
        }
    }

    public class TestClass
    {
        public void run()
        {
            ProfileSelect ps = new ProfileSelect();
            ps.Profile = new ArchiveProfile();

            var vs = new VersionSelect();
            ps.NextStep = vs;
            ps.NextStep.Open(ps, ps.Profile);

            var os = new OptionSelect();
            vs.NextStep = os;
            os.Open(vs, null);

            var es = new ExecuteOperation();
            os.NextStep = es;
            es.Open(os, null);
        }
    }
}
