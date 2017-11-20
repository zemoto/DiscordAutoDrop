using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DiscordAutoDrop.MVVM;
using DiscordAutoDrop.Windows;
using DiscordAutoDrop.Utilities;
using DiscordAutoDrop.ViewModels;
using System;

namespace DiscordAutoDrop
{
   internal sealed class Main : IDisposable
   {
      private readonly SettingsSerializer _serializer = new SettingsSerializer();
      private readonly DropLimiter _dropLimiter = new DropLimiter();

      private DiscordWrapper _discordWrapper;
      private TaskbarIcon _taskBarIcon;
      private HotkeyWindow _window;
      private MainViewModel _vm;
      private Settings _settings;

      ~Main()
      {
         _settings.DiscordDrops = _vm.DiscordDrops.Where( x => x.HotKey != Key.None && !string.IsNullOrWhiteSpace( x.DiscordDrop ) ).ToList();
         _serializer.Serialize( _settings );
      }

      public void Dispose()
      {
         _dropLimiter.Dispose();
         _discordWrapper?.Dispose();
         _taskBarIcon?.Dispose();
      }

      public async Task<bool> StartupAsync()
      {
         var splash = new SplashScreen();
         splash.Show();

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
         _discordWrapper = new DiscordWrapper( _settings.TargetChannelId );
         _dropLimiter.FireDrop += ( _, drop ) => _discordWrapper.FireDrop( drop );
         if ( !await _discordWrapper.LoginAsync( _settings, splash ) )
         {
            return false;
         }

         splash.DisplayTask( LoadingTask.LoadingTaskbarIcon );
         _taskBarIcon = new TaskbarIcon( ShowHotkeyWindow );
         _taskBarIcon.Show();

         splash.Close();
         return true;
      }

      private void ShowHotkeyWindow()
      {
         if ( _window?.IsLoaded == true )
         {
            _window.Activate();
            return;
         }

         _window = new HotkeyWindow
         {
            DataContext = _vm
         };
         _window.Show();
      }

      private void OnHotkeyFired( object sender, int hotkeyId )
      {
         var dropVm = _vm.DiscordDrops.FirstOrDefault( x => x.HotkeyId == hotkeyId );
         if ( dropVm != null )
         {
            _dropLimiter.EnqueueDrop( dropVm.DiscordDrop );
         }
      }
   }
}
