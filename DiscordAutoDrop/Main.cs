using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordAutoDrop.MVVM;
using DiscordAutoDrop.Splash;
using DiscordAutoDrop.Utilities;
using DiscordAutoDrop.ViewModels;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Patterns;

namespace DiscordAutoDrop
{
   internal sealed class Main
   {
      private AutomationElement _discord;
      private IInvokePattern _messageBox;

      private MainWindow _window;
      private MainViewModel _vm;

      public async Task StartupAsync()
      {
         var splash = new SplashScreen();
         splash.Show();

         using ( var inspectLauncher = new InspectLauncher() )
         {
            splash.DisplayTask( LoadingTask.LaunchingInspect );
            Debug.Assert( await Task.Factory.StartNew( inspectLauncher.EnsureInspectLaunched ) );
         }

         using ( var discordFinder = new DiscordFinder() )
         {
            splash.DisplayTask( LoadingTask.InitializingUIAutomation );
            await Task.Factory.StartNew( discordFinder.Initialize );

            splash.DisplayTask( LoadingTask.FindingDiscord );
            _discord = await discordFinder.FindDiscordAsync();

            if ( _discord != null )
            {
               splash.DisplayTask( LoadingTask.FindingDiscordMessageBox );
               _messageBox = await discordFinder.FindDiscordMessageBoxAsync( _discord );
            }
         }

         splash.Close();
      }

      public void ShowDialog()
      {
         _vm = new MainViewModel
         {
            MainButtonCommand = new RelayCommand( OnMainButtonPressed )
         };

         _window = new MainWindow
         {
            DataContext = _vm
         };

         _window.ShowDialog();
      }

      private void OnMainButtonPressed()
      {
         _messageBox.Invoke();
         var handle = _discord.Properties.NativeWindowHandle;
         using ( new WindowTemporaryForgrounder( handle, true ) )
         {
            SendKeys.SendWait( "!toot" );
            SendKeys.SendWait( "{Enter}" );
         }
      }
   }
}
