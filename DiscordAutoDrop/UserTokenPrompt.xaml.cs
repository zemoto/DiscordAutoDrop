using System.Windows;

namespace DiscordAutoDrop
{
   public partial class UserTokenPrompt
   {
      public string UserToken { get; private set; }

      public UserTokenPrompt()
      {
         InitializeComponent();
      }

      private void OnCancel( object sender, RoutedEventArgs e )
      {
         DialogResult = false;
      }

      private void OnOk( object sender, RoutedEventArgs e )
      {
         UserToken = TokenTextBlock.Text;
         DialogResult = true;
      }
   }
}
