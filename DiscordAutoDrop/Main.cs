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

namespace DiscordAutoDrop
{
   internal sealed class Main : IDisposable
   {
      private readonly SettingsSerializer _serializer = new SettingsSerializer();
      private readonly DiscordDropRateLimiter _dropLimiter = new DiscordDropRateLimiter();
      private readonly DiscordSocketClient _client = new DiscordSocketClient();
      private readonly TaskbarIcon _taskBarIcon = new TaskbarIcon();

      private HotkeyWindow _window;
      private MainViewModel _vm;
      private Settings _settings;

      public Main()
      {
         _dropLimiter.FireDrop += FireDrop;
         _taskBarIcon = new TaskbarIcon();
      }

      ~Main()
      {
         _settings.DiscordDrops = _vm.DiscordDrops.Where( x => x.HotKey != Key.None && !string.IsNullOrWhiteSpace( x.DiscordDrop ) ).ToList();
         _serializer.Serialize( _settings );
      }

      public void Dispose()
      {
         _client.Dispose();
         _dropLimiter.Dispose();
         _taskBarIcon.Dispose();
      }

      public async Task<bool> StartupAsync()
      {
         var splash = new Windows.SplashScreen();
         splash.Show();
         _taskBarIcon.Show();

         _vm = new MainViewModel()
         {
            AddDropCommand = new RelayCommand( () => _vm.DiscordDrops.Insert( 0, new DiscordDropViewModel() ) ),
            RemoveDropCommand = new RelayCommand<DiscordDropViewModel>( drop => _vm.DiscordDrops.Remove( drop ) )
         };
         _vm.Manager.HotkeyFired += OnHotkeyFired;

         splash.DisplayTask( LoadingTask.LoadingSettings );
         _settings = await Task.Factory.StartNew( _serializer.Deserialize ) ?? new Settings();

         splash.DisplayTask( LoadingTask.RegisteringHotkeys );
         if ( _settings?.DiscordDrops != null )
         {
            foreach ( var drop in _settings.DiscordDrops )
            {
               if ( _vm.Manager.TryRegister( drop.HotKey, drop.Modifier, out int id ) )
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
               break;
            }
            catch
            {
               _settings.UserToken = string.Empty;
               MessageBox.Show( "Could not start self-bot with given token" );
            }
         }
         splash.Close();
         return true;
      }

      private SocketTextChannel GetCurrentChannel()
      {
         foreach ( var guild in _client.Guilds )
         {
            if ( guild.Users.Any( x => x.Id == _client.CurrentUser.Id && x.VoiceChannel != null ) &&
                 guild.Channels.FirstOrDefault( x => x.Id == _settings.TargetChannelId ) is SocketTextChannel targetChannel )
            {
               return targetChannel;
            }
         }
         return null;
      }

      public void ShowDialog()
      {
         _window = new HotkeyWindow
         {
            DataContext = _vm
         };

         _window.ShowDialog();
      }

      private void OnHotkeyFired( object sender, int hotkeyId )
      {
         var dropVm = _vm.DiscordDrops.FirstOrDefault( x => x.HotkeyId == hotkeyId );
         if ( dropVm != null )
         {
            _dropLimiter.EnqueueDrop( dropVm.DiscordDrop );
         }
      }

      private async void FireDrop( object sender, string drop )
      {
         Debug.WriteLine( $"Sending Drop: {drop}" );
         var channel = GetCurrentChannel();
         if ( channel != null && !string.IsNullOrEmpty( drop ) )
         {
            await channel.SendMessageAsync( drop );
         }
      }
   }
}
