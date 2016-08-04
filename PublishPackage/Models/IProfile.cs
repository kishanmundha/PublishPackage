using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IProfile Profile { get; set; }
    }

    public class VersionSelect : IOperationStep
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
            throw new NotImplementedException();
        }

        public void Open(IOperationStep PreviousStep, object data)
        {
            throw new NotImplementedException();
        }
    }

    public class OptionSelect : IOperationStep
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
            throw new NotImplementedException();
        }

        public void Open(IOperationStep PreviousStep, object data)
        {
            throw new NotImplementedException();
        }
    }

    public class ExecuteOperation : IOperationStep
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
            throw new NotImplementedException();
        }

        public void Open(IOperationStep PreviousStep, object data)
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
