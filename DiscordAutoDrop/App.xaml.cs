using DiscordAutoDrop.Utilities;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace DiscordAutoDrop
{
   public partial class App
   {
      public App()
      {
         ShutdownMode = ShutdownMode.OnExplicitShutdown;
         Startup += OnStartup;
      }

      private static async void OnStartup( object sender, StartupEventArgs args )
      {
         using ( var mutex = new Mutex( true, "DiscordAutoDrop", out bool created ) )
         {
            if ( created )
            {
               using ( var main = new Main() )
               {
                  if ( await main.StartupAsync() )
                  {
                     main.ShowDialog();
                  }
               }
            }
            else
            {
               var current = Process.GetCurrentProcess();
               foreach ( var process in Process.GetProcessesByName( current.ProcessName ) )
               {
                  if ( process.Id != current.Id )
                  {
                     NativeMethods.SetForegroundWindow( process.MainWindowHandle );
                     break;
                  }
               }
            }
         }
         ( (App)sender ).Shutdown();
      }
   }
}
