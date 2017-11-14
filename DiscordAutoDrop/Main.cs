using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Discord;
using Discord.WebSocket;
using DiscordAutoDrop.MVVM;
using DiscordAutoDrop.Splash;
using DiscordAutoDrop.Utilities;
using DiscordAutoDrop.ViewModels;

namespace DiscordAutoDrop
{
   internal sealed class Main
   {
      private const string XmlFileName = "DiscordAutoDropSettings.xml";

      private readonly XmlSerializer<Settings> _serializer;
      private readonly DiscordDropRateLimiter _rateLimiter;

      private MainWindow _window;
      private MainViewModel _vm;
      private HotkeyManager _hotkeyManager;
      private Settings _settings;

      private DiscordSocketClient _client;

      public Main()
      {
         var xmlFilePath = Path.Combine( Directory.GetCurrentDirectory(), XmlFileName );
         _serializer = new XmlSerializer<Settings>( xmlFilePath );

         _rateLimiter = new DiscordDropRateLimiter( FireDrop );
      }

      ~Main()
      {
         _settings.DiscordDrops = _vm.DiscordDrops.Where( x => x.HotKey != Key.None && !string.IsNullOrWhiteSpace( x.DiscordDrop ) ).ToList();
         _serializer.Serialize( _settings );
      }

      public async Task<bool> StartupAsync()
      {
         var splash = new SplashScreen();
         splash.Show();

         splash.DisplayTask( LoadingTask.LoadingSettings );

         _vm = new MainViewModel( _hotkeyManager )
         {
            AddDropCommand = new RelayCommand( () => _vm.DiscordDrops.Add( new DiscordDropViewModel() ) ),
            RemoveDropCommand = new RelayCommand<DiscordDropViewModel>( drop => _vm.DiscordDrops.Remove( drop ) )
         };

         _settings = await Task.Factory.StartNew( _serializer.Deserialize );
         if ( _settings != null )
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
         else
         {
            _settings = new Settings();
         }

         splash.DisplayTask( LoadingTask.LoggingIn );
         _client = new DiscordSocketClient();
         _client.Connected += OnConnected;
         await _client.LoginAsync( TokenType.User, "MjE5Mjg1MjcxOTE1NjU5MjY0.DOkyvA.r8DNTAGvmtEreK84O5heJKUh98c" );
         await _client.StartAsync();

         splash.DisplayTask( LoadingTask.RegisteringSavedHotkeys );
         _hotkeyManager = new HotkeyManager();
         _hotkeyManager.HotkeyFired += OnHotkeyFired;

         splash.Close();
         return true;
      }

      private async Task OnConnected()
      {
         var user = _client.CurrentUser;
         try
         {
            var channel = GetCurrentChannel();
            var chan = _client.GetChannel( channel.Id ) as SocketTextChannel;
            await chan.SendMessageAsync( "PANIC" );

         }
         catch ( Exception e )
         {
            Console.WriteLine( e );
            throw;
         }
      }

      private SocketGuildChannel GetCurrentChannel()
      {
         var currentUser = _client.CurrentUser;
         
         foreach ( var guild in _client.Guilds )
         {
            foreach ( var channel in guild.Channels )
            {
               foreach ( var user in channel.Users )
               {
                  if ( user.Id == currentUser.Id && user.VoiceChannel != null )
                  {
                     return channel;
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
            _rateLimiter.EnqueueDrop( dropVm.DiscordDrop );
         }
      }

      private void FireDrop( string drop )
      {
         throw new NotImplementedException();
      }
   }
}
