using System.Windows.Input;
using DiscordAutoDrop.MVVM;

namespace DiscordAutoDrop.ViewModels
{
   internal sealed class MainViewModel : ViewModelBase
   {
      private ICommand _mainButtonCommand;
      public ICommand MainButtonCommand
      {
         get => _mainButtonCommand;
         set => SetProperty( ref _mainButtonCommand, value );
      }
   }
}
