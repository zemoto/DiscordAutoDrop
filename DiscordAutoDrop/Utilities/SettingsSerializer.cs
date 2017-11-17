using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class SettingsSerializer
   {
      private const string XmlFileName = "DiscordDropSettings.xml";
      private readonly string _xmlFilePath;

      public SettingsSerializer()
      {
         _xmlFilePath = Path.Combine( Directory.GetCurrentDirectory(), XmlFileName );
      }

      public void Serialize( Settings settings )
      {
         try
         {
            var xmlStream = File.Open( _xmlFilePath, FileMode.Create, FileAccess.Write );
            var writerSettings = new XmlWriterSettings
            {
               Indent = true,
               NewLineHandling = NewLineHandling.Entitize
            };

            var serializer = new XmlSerializer( typeof( Settings ) );
            using ( var xmlWriter = XmlWriter.Create( xmlStream, writerSettings ) )
            {
               serializer.Serialize( xmlWriter, settings );
            }
         }
         catch ( Exception ex )
         {
            Debug.WriteLine( ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message );
         }
      }

      public Settings Deserialize()
      {
         try
         {
            if ( File.Exists( _xmlFilePath ) )
            {
               var serializer = new XmlSerializer( typeof( Settings ) );
               using ( Stream xmlStream = File.Open( _xmlFilePath, FileMode.Open, FileAccess.Read ) )
               {
                  return serializer.Deserialize( xmlStream ) as Settings;
               }
            }
         }
         catch { /* Pass through on error */ }
         return null;
      }
   }
}
