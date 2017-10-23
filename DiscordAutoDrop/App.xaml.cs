namespace DiscordAutoDrop
{
   public partial class App
   {
      public App()
      {
         var main = new Main();
         main.Startup();
         main.ShowDialog();
      }
   }
}
