using System.Collections.ObjectModel;
using DiscordAutoDrop.MVVM;

namespace DiscordAutoDrop.ViewModels
{
   internal sealed class MainViewModel : ViewModelBase
   {
      public MainViewModel( HotkeyManager manager )
      {
         Manager = manager;
      }

      public HotkeyManager Manager { get; }

      public ObservableCollection<DiscordCommandViewModel> DiscordCommands { get; } = new ObservableCollection<DiscordCommandViewModel>();
   }
}
