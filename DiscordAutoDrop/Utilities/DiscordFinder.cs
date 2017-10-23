using System;
using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using FlaUI.UIA3;
using Debug = System.Diagnostics.Debug;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class DiscordFinder
   {
      private bool _initialized;
      private readonly AutomationBase _automation = new UIA3Automation();

      ~DiscordFinder()
      {
         _automation?.Dispose();
      }

      public bool Inititialize()
      {
         if ( !_initialized )
         {
            using ( var launcher = new InspectLauncher() )
            {
               _initialized = launcher.EnsureInspectLaunched();
            }
         }
         return _initialized;
      }

      public (AutomationElement discord, IInvokePattern invokePattern) FindDiscord()
      {
         Debug.Assert( _initialized );

         var processes = Process.GetProcessesByName( "discord" );

         foreach ( var process in processes )
         {
            if ( process.MainWindowHandle == IntPtr.Zero )
            {
               continue;
            }

            var discord = _automation.FromHandle( process.MainWindowHandle );
            var messageBox = SearchDiscordForMessageBox( discord );
            if ( messageBox != null )
            {
               return (discord, messageBox);
            }
         }
         return (null, null);
      }

      private IInvokePattern SearchDiscordForMessageBox( AutomationElement discord )
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
