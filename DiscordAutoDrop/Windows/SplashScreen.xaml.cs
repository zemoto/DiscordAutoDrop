using System.Collections.Generic;

namespace DiscordAutoDrop.Windows
{
   public enum LoadingTask
   {
      Initializing,
      LoadingSettings,
      RegisteringSavedHotkeys,
      LaunchingSelfBot
   }

   public partial class SplashScreen
   {
      private readonly Dictionary<LoadingTask, string> _stepToTaskDictionary = new Dictionary<LoadingTask, string>
      {
         [LoadingTask.Initializing] = "Initializing...",
         [LoadingTask.LoadingSettings] = "Loading Settings...",
         [LoadingTask.RegisteringSavedHotkeys] = "Registering saved hotkeys...",
         [LoadingTask.LaunchingSelfBot] = "Launching Self-Bot..."
      };

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
