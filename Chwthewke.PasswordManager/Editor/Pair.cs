namespace Chwthewke.PasswordManager.Editor
{
    public struct Pair<TA,TB>
    {
        public TA First { get; private set; }
        public TB Second { get; private set; }

        public Pair( TA first, TB second ) : this( )
        {
            First = first;
            Second = second;
        }

        public bool Equals( Pair<TA, TB> other )
        {
            return Equals( other.First, First ) && Equals( other.Second, Second );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( obj.GetType( ) != typeof ( Pair<TA, TB> ) ) return false;
            return Equals( ( Pair<TA, TB> ) obj );
        }

        public override int GetHashCode( )
        {
            unchecked
            {
                return ( First.GetHashCode( ) * 397 ) ^ Second.GetHashCode( );
            }
        }
    }
}