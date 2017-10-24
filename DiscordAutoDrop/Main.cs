using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
      private const string XmlFileName = "DiscordCommand.xml";

      private readonly XmlSerializer<ObservableCollection<DiscordCommandViewModel>> _serializer;
      private readonly CommandRateLimiter _rateLimiter;

      private AutomationElement _discord;
      private IInvokePattern _messageBox;

      private MainWindow _window;
      private MainViewModel _vm;
      private HotkeyManager _hotkeyManager;

      public Main()
      {
         var xmlFilePath = Path.Combine( Directory.GetCurrentDirectory(), XmlFileName );
         _serializer = new XmlSerializer<ObservableCollection<DiscordCommandViewModel>>( xmlFilePath );

         _rateLimiter = new CommandRateLimiter( FireCommand );
      }

      ~Main()
      {
         _serializer.Serialize( _vm.DiscordCommands );
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

         splash.DisplayTask( LoadingTask.LoadingSavedDiscordCommands );
         var commands = await Task.Factory.StartNew( _serializer.Deserialize );
      
         splash.DisplayTask( LoadingTask.RegisteringSavedHotkeys );
         _hotkeyManager = new HotkeyManager();
         _hotkeyManager.HotkeyFired += OnHotkeyFired;
         _vm = new MainViewModel( _hotkeyManager );

         if ( commands != null )
         {
            foreach ( var command in commands )
            {
               if ( _hotkeyManager.TryRegister( command.HotKey, command.Modifier, out int id ) )
               {
                  command.HotkeyId = id;
                  _vm.DiscordCommands.Add( command );
               }
            }
         }
         splash.Close();
      }

      public void ShowDialog()
      {
         if ( !_vm.DiscordCommands.Any() )
         {
            _vm.DiscordCommands.Add( new DiscordCommandViewModel() );
         }

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
            _rateLimiter.QueueCommand( command.DiscordCommand );
         }
      }

      private void FireCommand( string command )
      {
         _messageBox.Invoke();
         var handle = _discord.Properties.NativeWindowHandle;
         using ( new WindowTemporaryForgrounder( handle ) )
         {
            SendKeys.SendWait( command );
            SendKeys.SendWait( "{Enter}" );
         }
      }
   }
}
