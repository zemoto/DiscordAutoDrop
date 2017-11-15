using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Discord;
using Discord.WebSocket;
using DiscordAutoDrop.MVVM;
using DiscordAutoDrop.Windows;
using DiscordAutoDrop.Utilities;
using DiscordAutoDrop.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Web;

namespace DiscordAutoDrop
{
   internal sealed class Main : IDisposable
   {
      private const string XmlFileName = "DiscordAutoDropSettings.xml";

      private readonly XmlSerializer<Settings> _serializer;
      private readonly DiscordDropRateLimiter _dropLimiter;
      private readonly DiscordSocketClient _client = new DiscordSocketClient();

      private MainWindow _window;
      private MainViewModel _vm;
      private HotkeyManager _hotkeyManager;
      private Settings _settings;

      public Main()
      {
         var xmlFilePath = Path.Combine( Directory.GetCurrentDirectory(), XmlFileName );
         _serializer = new XmlSerializer<Settings>( xmlFilePath );

         _dropLimiter = new DiscordDropRateLimiter( FireDrop );
      }

      ~Main()
      {
         _hotkeyManager.HotkeyFired -= OnHotkeyFired;
         _settings.DiscordDrops = _vm.DiscordDrops.Where( x => x.HotKey != Key.None && !string.IsNullOrWhiteSpace( x.DiscordDrop ) ).ToList();
         _serializer.Serialize( _settings );
      }

      public void Dispose()
      {
         _client.Dispose();
         _dropLimiter.Dispose();
      }

      public async Task<bool> StartupAsync()
      {
         var splash = new Windows.SplashScreen();
         splash.Show();

         _hotkeyManager = new HotkeyManager();
         _hotkeyManager.HotkeyFired += OnHotkeyFired;
         _vm = new MainViewModel( _hotkeyManager )
         {
            AddDropCommand = new RelayCommand( () => _vm.DiscordDrops.Add( new DiscordDropViewModel() ) ),
            RemoveDropCommand = new RelayCommand<DiscordDropViewModel>( drop => _vm.DiscordDrops.Remove( drop ) )
         };

         splash.DisplayTask( LoadingTask.LoadingSettings );
         _settings = await Task.Factory.StartNew( _serializer.Deserialize ) ?? new Settings();

         splash.DisplayTask( LoadingTask.RegisteringSavedHotkeys );
         if ( _settings?.DiscordDrops != null )
         {
            foreach ( var drop in _settings.DiscordDrops )
            {
               if ( _hotkeyManager.TryRegister( drop.HotKey, drop.Modifier, out int id ) )
               {
                  drop.HotkeyId = id;
                  _vm.DiscordDrops.Add( drop );
               }
            }
         }

         splash.DisplayTask( LoadingTask.LaunchingSelfBot );
         while ( true )
         {
            if ( string.IsNullOrEmpty( _settings.UserToken ) )
            {
               var prompt = new UserTokenPromptDialog { Owner = splash };
               if ( prompt.ShowDialog() != true )
               {
                  return false;
               }
               _settings.UserToken = prompt.UserToken;
            }
            try
            {
               await _client.LoginAsync( TokenType.User, _settings.UserToken );
               await _client.StartAsync();
               _settings.UserToken = _settings.UserToken;
               break;
            }
            catch ( Exception ex )
            {
               _settings.UserToken = string.Empty;
               switch ( ex )
               {
                  case HttpException _:
                     MessageBox.Show( "Could not start self-bot with given token" );
                     break;
                  case FormatException _:
                     MessageBox.Show( "Token format is invalid" );
                     break;
               }
            }
         }

         splash.Close();
         return true;
      }

      private SocketTextChannel GetCurrentChannel()
      {
         var currentUser = _client.CurrentUser;

         foreach ( var guild in _client.Guilds )
         {
            if ( guild.Users.Any( x => x.Id == currentUser.Id && x.VoiceChannel != null ) )
            {
               foreach ( var channel in guild.Channels )
               {
                  if ( channel is SocketTextChannel && channel.Users.Any( x => x.DiscriminatorValue == 8428 && x.Username == "buster" ) )
                  {
                     return channel as SocketTextChannel;
                  }
               }
            }
            
         }
         return null;
      }

      public void ShowDialog()
      {
         _window = new MainWindow
         {
            DataContext = _vm
         };

         _window.ShowDialog();
      }

      private void OnHotkeyFired( object sender, HotkeyFiredEventArgs e )
      {
         var dropVm = _vm.DiscordDrops.FirstOrDefault( x => x.HotkeyId == e.HotkeyId );
         if ( dropVm != null )
         {
            _dropLimiter.EnqueueDrop( dropVm.DiscordDrop );
         }
      }

      private async void FireDrop( string drop )
      {
         Debug.WriteLine( $"Sending Drop: {drop}" );
         var channel = GetCurrentChannel();
         if ( channel != null )
         {
            await channel.SendMessageAsync( drop );
         }
      }
   }
}
