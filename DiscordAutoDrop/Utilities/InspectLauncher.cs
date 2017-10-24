using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace DiscordAutoDrop.Utilities
{
   // UI Automation does not show the entire UI tree of every app unless Inspect.exe has launched.
   // Inspect injects UI automation into every open app which allows us to effectively search
   // through all the UI trees. This allows us to find the Discord message box.
   internal sealed class InspectLauncher : IDisposable
   {
      private const string InspectFilePath = @"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\inspect.exe";
      private const string InspectProcessName = "inspect";

      public bool EnsureInspectLaunched()
      {
         bool launched = false;
         if ( Process.GetProcessesByName( InspectProcessName ).Any() )
         {
            launched = true;
         }
         else if ( File.Exists( InspectFilePath ) )
         {
            var proc = new Process
            {
               StartInfo =
               {
                  FileName = InspectFilePath,
                  WindowStyle = ProcessWindowStyle.Hidden
               }
            };
            proc.Start();
            Thread.Sleep( TimeSpan.FromSeconds( 1 ) );

            launched = true;
         }
         return launched;
      }

      public void Dispose()
      {
         var inspectProcesses = Process.GetProcessesByName( InspectProcessName );
         foreach ( var inspectProcess in inspectProcesses )
         {
            inspectProcess.Kill();
         }
      }
   }
}
