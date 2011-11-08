﻿using System;
using Chwthewke.PasswordManager.Engine;
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
    }

    internal class BaselinePasswordDocument : IBaselinePasswordDocument
    {
        public BaselinePasswordDocument( PasswordDigestDocument baseline )
        {
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
    }
}