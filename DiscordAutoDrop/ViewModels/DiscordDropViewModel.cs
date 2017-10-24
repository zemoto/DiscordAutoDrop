using System.Windows.Input;
using DiscordAutoDrop.MVVM;

namespace DiscordAutoDrop.ViewModels
{
   public sealed class DiscordDropViewModel : ViewModelBase
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

      private string _discordDrop;
      public string DiscordDrop
      {
         get => _discordDrop;
         set => SetProperty( ref _discordDrop, value );
      }
   }
}
