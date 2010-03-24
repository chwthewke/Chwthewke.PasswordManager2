using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace Chwthewke.PasswordManager.Migration
{
    public class LegacyItemLoader : ILegacyItemLoader
    {
        private const string SettingsFqn = "Chwthewke.PasswordManager.WpfGui.Properties.Settings";
        private const string SettingTag = "setting";
        private const string SettingNameAttribute = "name";
        private const string SavedPasswordsAttributeValue = "SavedPasswords";
        private const string ValueTag = "value";

        public IEnumerable<LegacyItem> Load( TextReader reader )
        {
            try
            {
                string legacySettingsString = ReadSettingsString( reader );
                return ParseLegacyItems( legacySettingsString );
            }
            catch ( InvalidDataException e )
            {
                Console.WriteLine( e );
                return Enumerable.Empty<LegacyItem>( );
            }
        }

        private string ReadSettingsString( TextReader reader )
        {
            XElement xml = XElement.Load( reader );

            var appSettingsNode = xml.Descendants( SettingsFqn ).FirstOrDefault( );
            if ( appSettingsNode == null )
                throw new InvalidDataException( string.Format( "Missing {0} tag.", SettingsFqn ) );

            var settingNode = appSettingsNode.Elements( SettingTag )
                .FirstOrDefault( e => e.Attribute( SettingNameAttribute ) != null &&
                                      e.Attribute( SettingNameAttribute ).Value == SavedPasswordsAttributeValue );
            if ( settingNode == null )
                throw new InvalidDataException(
                    string.Format( "Missing {0} {1}={2} tag.",
                                   SettingTag, SettingNameAttribute, SavedPasswordsAttributeValue ) );

            var valueNode = settingNode.Element( ValueTag );
            if ( valueNode == null )
                throw new InvalidDataException( string.Format( "Missing {0} tag.", ValueTag ) );

            return valueNode.Value;
        }

        private IEnumerable<LegacyItem> ParseLegacyItems( string value )
        {
            string line;
            using ( TextReader reader = new StringReader( value ) )
                while ( ( line = reader.ReadLine( ) ) != null )
                {
                    if ( string.IsNullOrWhiteSpace( line ) )
                        continue;
                    string[ ] parts = line.Split( ',' );
                    if ( parts.Length < 3 )
                        continue;
                    string key = parts[ 0 ];
                    int typeBit;
                    if ( !int.TryParse( parts[ 1 ], out typeBit ) )
                        continue;
                    yield return new LegacyItem( key, typeBit == 0 );
                }
        }
    }
}