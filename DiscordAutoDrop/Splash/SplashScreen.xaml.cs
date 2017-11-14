using System.Collections.Generic;
using System.Collections.Immutable;

namespace DiscordAutoDrop.Splash
{
   public enum LoadingTask
   {
      Initializing,
      LoadingSettings,
      LoggingIn,
      RegisteringSavedHotkeys
   }

   public partial class SplashScreen
   {
      private readonly ImmutableDictionary<LoadingTask, string> _stepToTaskDictionary = new Dictionary<LoadingTask, string>
      {
         [LoadingTask.Initializing] = "Initializing...",
         [LoadingTask.LoadingSettings] = "Loading Settings...",
         [LoadingTask.LoggingIn] = "Logging in...",
         [LoadingTask.RegisteringSavedHotkeys] = "Registering saved hotkeys..."
      }.ToImmutableDictionary();

      public SplashScreen()
      {
         InitializeComponent();

         DisplayTask( LoadingTask.Initializing );
      }

      public void DisplayTask( LoadingTask task )
      {
         SplashLabel.Text = _stepToTaskDictionary[task];
      }
   }
}
