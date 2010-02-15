namespace Chwthewke.PasswordManager.Engine
{
    public class PasswordType
    {
        internal readonly Symbols _symbols;
        internal readonly int _length;

        private PasswordType( Symbols symbols, int length )
        {
            _symbols = symbols;
            _length = length;
        }

        public IPasswordFactory Factory
        {
            get { return new PasswordFactory( new Sha512( ), new ArrayBaseConverter( _symbols.Length ), _symbols, _length ); }
        }

        public static readonly PasswordType Ascii = new PasswordType( Symbols.Symbols92, 10 );
        public static readonly PasswordType AlphaNumeric = new PasswordType( Symbols.Symbols62, 12 );
    }
}