using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
      private HotkeyManager _hotkeyManager;

      ~Main()
      {
         _hotkeyManager?.Dispose();
      }

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

         _hotkeyManager = new HotkeyManager();
         _hotkeyManager.HotkeyFired += OnHotkeyFired;

         splash.Close();
      }

      public void ShowDialog()
      {
         _vm = new MainViewModel( _hotkeyManager );
         _vm.DiscordCommands.Add( new DiscordCommandViewModel() );

         _window = new MainWindow
         {
            DataContext = _vm
         };

         _window.ShowDialog();
      }

      private void OnHotkeyFired( object sender, HotkeyFiredEventArgs e )
      {
         var command = _vm.DiscordCommands.FirstOrDefault( x => x.HotkeyId == e.HotkeyId );
         if ( command != null )
         {
            FireCommand( command.DiscordCommand );
         }
      }

      private void FireCommand( string command )
      {
         _messageBox.Invoke();
         var handle = _discord.Properties.NativeWindowHandle;
         using ( new WindowTemporaryForgrounder( handle, true ) )
         {
            SendKeys.SendWait( $"!{command}" );
            SendKeys.SendWait( "{Enter}" );
         }
      }
   }
}
