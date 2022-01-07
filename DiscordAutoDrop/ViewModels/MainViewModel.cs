﻿using System.Collections.ObjectModel;
using ZemotoUI;

namespace DiscordAutoDrop.ViewModels
{
   internal sealed class MainViewModel : ViewModelBase
   {
      public ObservableCollection<DiscordDropViewModel> DiscordDrops { get; } = new ObservableCollection<DiscordDropViewModel>();

      public RelayCommand AddDropCommand { get; set; }
      public RelayCommand<DiscordDropViewModel> RemoveDropCommand { get; set; }
   }
}
