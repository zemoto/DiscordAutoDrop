using System;
using System.Collections.Generic;
using DiscordAutoDrop.ViewModels;

namespace DiscordAutoDrop
{
   [Serializable]
   public sealed class Settings
   {
      public List<DiscordDropViewModel> DiscordDrops { get; set; }
      public string AccessToken { get; set; }
      public string RefreshToken { get; set; }
      public DateTime ExpirationTime { get; set; }
   }
}
