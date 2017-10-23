using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using FlaUI.UIA3;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class DiscordFinder : IDisposable
   {
      private AutomationBase _automation;

      public void Initialize()
      {
         if ( _automation == null )
         {
            _automation = new UIA3Automation();
         }
      }

      public void Dispose()
      {
         _automation?.Dispose();
      }

      public async Task<AutomationElement> FindDiscordAsync()
      {
         return await Task.Factory.StartNew( FindDiscord );
      }

      private AutomationElement FindDiscord()
      {
         var processes = Process.GetProcessesByName( "discord" );
         foreach ( var process in processes )
         {
            if ( process.MainWindowHandle == IntPtr.Zero )
            {
               continue;
            }

            return _automation.FromHandle( process.MainWindowHandle );
         }
         return null;
      }

      public async Task<IInvokePattern> FindDiscordMessageBoxAsync( AutomationElement discord )
      {
         return await Task.Factory.StartNew( () => FindDiscordMessageBox( discord ) );
      }

      private IInvokePattern FindDiscordMessageBox( AutomationElement discord )
      {
         var editControls = discord.FindAll( TreeScope.Descendants, _automation.ConditionFactory.ByControlType( ControlType.Edit ) );
         foreach ( var editControl in editControls )
         {
            if ( editControl.Name.Contains( "Message" ) && editControl.Patterns.Invoke.IsSupported )
            {
               return editControl.Patterns.Invoke.Pattern;
            }
         }
         return null;
      }
   }
}
