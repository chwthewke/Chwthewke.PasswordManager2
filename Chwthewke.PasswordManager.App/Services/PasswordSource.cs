using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PasswordSource
    {
        public PasswordSource( IPasswordRepository passwordRepository )
        {
            if ( passwordRepository == null )
                throw new ArgumentNullException( "passwordRepository" );

            _passwordRepository = passwordRepository;
        }

        private IPasswordRepository _passwordRepository;

        public IPasswordRepository PasswordRepository
        {
            get { return _passwordRepository; }
            set
            {
                if ( value == null )
                    throw new ArgumentNullException( "value" );

                _passwordRepository.MergeInto( value );
                _passwordRepository = value;
            }
        }
    }
}