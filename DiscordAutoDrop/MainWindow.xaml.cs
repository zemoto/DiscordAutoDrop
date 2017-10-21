using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using DiscordAutoDrop.Utilities;
using UIAutomationClient;

namespace DiscordAutoDrop
{
   public partial class MainWindow
   {
      private readonly IUIAutomationElement _discord;
      private readonly IUIAutomationLegacyIAccessiblePattern _messageBoxLegacyPattern;

      public MainWindow()
      {
         InitializeComponent();

         var discordFinder = new DiscordFinder();
         _discord = discordFinder.FindDiscord();
         if ( _discord != null )
         {
            _messageBoxLegacyPattern = discordFinder.FindDiscordMessageBoxLegacyPattern( _discord );
         }
      }

      private void OnButtonClick( object sender, RoutedEventArgs e )
      {
         _messageBoxLegacyPattern.DoDefaultAction();
         var handle = _discord.CurrentNativeWindowHandle;
         using ( new WindowTemporaryForgrounder( handle, true ) )
         {
            SendKeys.SendWait( "!toot" );
            SendKeys.SendWait( "{Enter}" );
         }
      }
   }
}
