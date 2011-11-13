using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal interface IBaselinePasswordDocument
    {
        string Key { get; }

        int Iteration { get; }

        Guid? PasswordGenerator { get; }

        Guid? MasterPasswordId { get; }

        DateTime? CreatedOn { get; }

        string Note { get; }

        PasswordDigestDocument Document { get; }
    }

    internal class BaselinePasswordDocument : IBaselinePasswordDocument
    {
        public BaselinePasswordDocument( PasswordDigestDocument baseline )
        {
            if ( baseline == null )
                throw new ArgumentNullException( "baseline" );
            _baseline = baseline;
        }

        public string Key
        {
            get { return _baseline.Key; }
        }

        public int Iteration
        {
            get { return _baseline.Iteration; }
        }

        public Guid? PasswordGenerator
        {
            get { return _baseline.PasswordGenerator; }
        }

        public Guid? MasterPasswordId
        {
            get { return _baseline.MasterPasswordId; }
        }

        public DateTime? CreatedOn
        {
            get { return _baseline.CreatedOn; }
        }

        public string Note
        {
            get { return _baseline.Note; }
        }

        public PasswordDigestDocument Document
        {
            get { return _baseline; }
        }

        private readonly PasswordDigestDocument _baseline;
    }

    internal class NewPasswordDocument : IBaselinePasswordDocument
    {
        public string Key
        {
            get { return string.Empty; }
        }

        public int Iteration
        {
            get { return 1; }
        }

        public Guid? PasswordGenerator
        {
            get { return null; }
        }

        public Guid? MasterPasswordId
        {
            get { return null; }
        }

        public DateTime? CreatedOn
        {
            get { return null; }
        }

        public string Note
        {
            get { return string.Empty; }
        }

        public PasswordDigestDocument Document
        {
            get { return null; }
        }
    }
}