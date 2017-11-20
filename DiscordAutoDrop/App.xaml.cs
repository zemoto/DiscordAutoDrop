using DiscordAutoDrop.Utilities;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace DiscordAutoDrop
{
   public partial class App
   {
      private Main _main;

      public App()
      {
         ShutdownMode = ShutdownMode.OnExplicitShutdown;
         Startup += OnStartup;
         Exit += OnExit;
      }

      private async void OnStartup( object sender, StartupEventArgs args )
      {
         using ( var mutex = new Mutex( true, "DiscordAutoDrop", out bool created ) )
         {
            if ( created )
            {
               _main = new Main();
               await _main.StartupAsync();
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
               ( (App)sender ).Shutdown();
            }
         }
      }

      private void OnExit( object sender, ExitEventArgs e )
      {
         _main?.Dispose();
      }
   }
}
