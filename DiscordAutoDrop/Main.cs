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
      private const string XmlFileName = "DiscordDrops.xml";

      private readonly XmlSerializer<ObservableCollection<DiscordDropViewModel>> _serializer;
      private readonly DiscordDropRateLimiter _rateLimiter;

      private AutomationElement _discord;
      private ILegacyIAccessiblePattern _messageBox;

      private MainWindow _window;
      private MainViewModel _vm;
      private HotkeyManager _hotkeyManager;

      public Main()
      {
         var xmlFilePath = Path.Combine( Directory.GetCurrentDirectory(), XmlFileName );
         _serializer = new XmlSerializer<ObservableCollection<DiscordDropViewModel>>( xmlFilePath );

         _rateLimiter = new DiscordDropRateLimiter( FireDrop );
      }

      ~Main()
      {
         _serializer.Serialize( _vm.DiscordDrop );
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

         splash.DisplayTask( LoadingTask.LoadingSavedDiscordDrops );
         var drops = await Task.Factory.StartNew( _serializer.Deserialize );
      
         splash.DisplayTask( LoadingTask.RegisteringSavedHotkeys );
         _hotkeyManager = new HotkeyManager();
         _hotkeyManager.HotkeyFired += OnHotkeyFired;
         _vm = new MainViewModel( _hotkeyManager );

         if ( drops != null )
         {
            foreach ( var drop in drops )
            {
               if ( _hotkeyManager.TryRegister( drop.HotKey, drop.Modifier, out int id ) )
               {
                  drop.HotkeyId = id;
                  _vm.DiscordDrop.Add( drop );
               }
            }
         }
         splash.Close();
      }

      public void ShowDialog()
      {
         if ( !_vm.DiscordDrop.Any() )
         {
            _vm.DiscordDrop.Add( new DiscordDropViewModel() );
         }

         _window = new MainWindow
         {
            DataContext = _vm
         };

         _window.ShowDialog();
      }

      private void OnHotkeyFired( object sender, HotkeyFiredEventArgs e )
      {
         var dropVm = _vm.DiscordDrop.FirstOrDefault( x => x.HotkeyId == e.HotkeyId );
         if ( dropVm != null )
         {
            _rateLimiter.EnqueueDrop( dropVm.DiscordDrop );
         }
      }

      private void FireDrop( string drop )
      {
         _messageBox.DoDefaultAction();
         var handle = _discord.Properties.NativeWindowHandle;
         using ( new WindowTemporaryForgrounder( handle ) )
         {
            SendKeys.SendWait( drop );
            SendKeys.SendWait( "{Enter}" );
         }
      }
   }
}
