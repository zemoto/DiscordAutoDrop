using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class XmlSerializer<T> where T : class
   {
      private readonly string _xmlFilePath;

      public XmlSerializer( string xmlFilePath )
      {
         _xmlFilePath = xmlFilePath;
      }

      public void Serialize( T objectToSerialize )
      {
         try
         {
            var xmlStream = File.Open( _xmlFilePath, FileMode.Create, FileAccess.Write );
            var settings = new XmlWriterSettings
            {
               Indent = true,
               NewLineHandling = NewLineHandling.Entitize
            };

            var serializer = new XmlSerializer( typeof( T ) );
            using ( var xmlWriter = XmlWriter.Create( xmlStream, settings ) )
            {
               serializer.Serialize( xmlWriter, objectToSerialize );
            }
         }
         catch ( Exception ex )
         {
            Debug.WriteLine( ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message );
         }
      }

      public T Deserialize()
      {
         try
         {
            if ( File.Exists( _xmlFilePath ) )
            {
               var serializer = new XmlSerializer( typeof( T ) );
               using ( Stream xmlStream = File.Open( _xmlFilePath, FileMode.Open, FileAccess.Read ) )
               {
                  return serializer.Deserialize( xmlStream ) as T;
               }
            }
         }
         catch { /* Pass through on error */ }
         return null;
      }
   }
}
