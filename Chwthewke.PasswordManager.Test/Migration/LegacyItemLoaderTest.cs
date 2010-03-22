using System.Collections.Generic;
using System.IO;
using Chwthewke.PasswordManager.Migration;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Migration
{
    [ TestFixture ]
    public class LegacyItemLoaderTest
    {
        [ Test ]
        public void LoadLegacyItems( )
        {
            // Setup
            const string configFragment =
                @"    <userSettings>
        <Chwthewke.PasswordManager.WpfGui.Properties.Settings>
            <setting name=""SavedPasswords"" serializeAs=""String"">
                <value>twitter,1,aGfY7GGuhWxD5RWemgaGoRujIqQxdfB/8NiczzGP1fIVOlOLpieySPZC+I8dmLHU0hM0v0jwmslG0i8dWbuxig==
orange,0,PVOAs/nF37894oBc3gUgs3wQVmjxnd4YLp9hRAJ/AAKDqvUc/vtqaHfzVKCfQPDPqu1k624XD+EzI9oxg/jJBA==
</value>
            </setting>
        </Chwthewke.PasswordManager.WpfGui.Properties.Settings>
    </userSettings>";
            TextReader reader = new StringReader( configFragment );
            LegacyItemLoader loader = new LegacyItemLoader( );
            // Exercise
            IEnumerable<LegacyItem> items = loader.Load( reader );
            // Verify
            LegacyItem[ ] expected = new[ ]
                                         {
                                             new LegacyItem( "twitter", false ),
                                             new LegacyItem( "orange", true )
                                         };

            Assert.That( items, Is.EquivalentTo( expected ) );
        }
    }
}