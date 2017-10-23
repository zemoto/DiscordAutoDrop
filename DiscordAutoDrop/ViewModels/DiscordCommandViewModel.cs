using System.Windows.Input;
using DiscordAutoDrop.MVVM;

namespace DiscordAutoDrop.ViewModels
{
   internal sealed class DiscordCommandViewModel : ViewModelBase
   {
      private int _hotkeyId;
      public int HotkeyId
      {
         get => _hotkeyId;
         set => SetProperty( ref _hotkeyId, value );
      }

      private Key _hotKey;
      public Key HotKey
      {
         get => _hotKey;
         set => SetProperty( ref _hotKey, value );
      }

      private ModifierKeys _modifier;
      public ModifierKeys Modifier
      {
         get => _modifier;
         set => SetProperty( ref _modifier, value );
      }

      private string _discordCommand;
      public string DiscordCommand
      {
         get => _discordCommand;
         set => SetProperty( ref _discordCommand, value );
      }
   }
}
