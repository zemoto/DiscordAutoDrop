using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using DiscordAutoDrop.Utilities;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Patterns;

namespace DiscordAutoDrop
{
   public partial class MainWindow
   {
      private readonly AutomationElement _discord;
      private readonly IInvokePattern _messageBoxLegacyPattern;

      public MainWindow()
      {
         InitializeComponent();

         using ( var discordFinder = new DiscordFinder() )
         {
            Debug.Assert( discordFinder.Inititialize() );
            (_discord, _messageBoxLegacyPattern) = discordFinder.FindDiscord();
         }
      }

      private void OnButtonClick( object sender, RoutedEventArgs e )
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
