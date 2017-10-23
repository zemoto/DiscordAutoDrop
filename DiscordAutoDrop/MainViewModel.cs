using System.Diagnostics;
using System.Windows.Forms;
using DiscordAutoDrop.MVVM;
using DiscordAutoDrop.Utilities;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Patterns;

namespace DiscordAutoDrop
{
   internal sealed class MainViewModel : ViewModelBase
   {
      private readonly AutomationElement _discord;
      private readonly IInvokePattern _messageBoxLegacyPattern;

      public MainViewModel()
      {
         using ( var discordFinder = new DiscordFinder() )
         {
            Debug.Assert( discordFinder.Inititialize() );
            (_discord, _messageBoxLegacyPattern) = discordFinder.FindDiscord();
         }
      }

      private RelayCommand _buttonPressedCommand;
      public RelayCommand ButtonPressedCommand => _buttonPressedCommand ?? ( _buttonPressedCommand = new RelayCommand( () =>
      {
         _messageBoxLegacyPattern.Invoke();
         var handle = _discord.Properties.NativeWindowHandle;
         using ( new WindowTemporaryForgrounder( handle, true ) )
         {
            SendKeys.SendWait( "!toot" );
            SendKeys.SendWait( "{Enter}" );
         }
      } ) );
   }
}
