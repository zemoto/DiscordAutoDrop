using System.Windows;

namespace DiscordAutoDrop
{
   public partial class App
   {
      public App()
      {
         Startup += OnStartup;
      }

      private async void OnStartup( object sender, StartupEventArgs args )
      {
         var main = new Main();
         await main.StartupAsync();
         main.ShowDialog();

         ( (App)sender ).Shutdown();
      }
   }
}
