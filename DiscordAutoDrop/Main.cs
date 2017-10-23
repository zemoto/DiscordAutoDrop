using System.Diagnostics;
using System.Windows.Forms;
using DiscordAutoDrop.MVVM;
using DiscordAutoDrop.Utilities;
using DiscordAutoDrop.ViewModels;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Patterns;

namespace DiscordAutoDrop
{
   internal sealed class Main
   {
      private AutomationElement _discord;
      private IInvokePattern _messageBoxLegacyPattern;

      private readonly MainWindow _window = new MainWindow();
      private readonly MainViewModel _vm = new MainViewModel();

      public void Startup()
      {
         using ( var discordFinder = new DiscordFinder() )
         {
            Debug.Assert( discordFinder.Inititialize() );
            (_discord, _messageBoxLegacyPattern) = discordFinder.FindDiscord();
         }
      }

      public void ShowDialog()
      {
         _vm.MainButtonCommand = new RelayCommand( OnMainButtonPressed );
         _window.DataContext = _vm;
         _window.ShowDialog();
      }

      private void OnMainButtonPressed()
      {
         _messageBoxLegacyPattern.Invoke();
         var handle = _discord.Properties.NativeWindowHandle;
         using ( new WindowTemporaryForgrounder( handle, true ) )
         {
            SendKeys.SendWait( "!toot" );
            SendKeys.SendWait( "{Enter}" );
         }
      }
   }
}
