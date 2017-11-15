using System.Windows;

namespace DiscordAutoDrop.Windows
{
   public partial class UserTokenPromptDialog
   {
      public string UserToken { get; private set; }

      public UserTokenPromptDialog()
      {
         InitializeComponent();
      }

      private void OnOk( object sender, RoutedEventArgs e )
      {
         UserToken = TokenTextBlock.Text;
         DialogResult = true;
      }
   }
}
