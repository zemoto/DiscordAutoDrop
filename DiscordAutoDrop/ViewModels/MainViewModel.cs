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

      public ObservableCollection<DiscordDropViewModel> DiscordDrop { get; } = new ObservableCollection<DiscordDropViewModel>();
   }
}
