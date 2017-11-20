using System.Collections.Generic;

namespace DiscordAutoDrop.Windows
{
   public enum LoadingTask
   {
      Initializing,
      LoadingSettings,
      RegisteringHotkeys,
      LaunchingSelfBot,
      LoadingTaskbarIcon
   }

   public partial class SplashScreen
   {
      private readonly Dictionary<LoadingTask, string> _stepToTaskDictionary = new Dictionary<LoadingTask, string>
      {
         [LoadingTask.Initializing] = "Initializing...",
         [LoadingTask.LoadingSettings] = "Loading Settings...",
         [LoadingTask.RegisteringHotkeys] = "Registering Hotkeys...",
         [LoadingTask.LaunchingSelfBot] = "Hooking up Discord Self-Bot...",
         [LoadingTask.LoadingTaskbarIcon] = "Launching Taskbar Icon..."
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
