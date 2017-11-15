using System.Windows;

namespace DiscordAutoDrop
{
   public partial class App
   {
      public App()
      {
         Startup += OnStartup;
      }

      private static async void OnStartup( object sender, StartupEventArgs args )
      {
         using ( var main = new Main() )
         {
            if ( await main.StartupAsync() )
            {
               main.ShowDialog();
            }
         }
            
         ( (App)sender ).Shutdown();
      }
   }
}
