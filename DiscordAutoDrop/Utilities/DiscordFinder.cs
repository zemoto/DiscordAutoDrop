using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using FlaUI.UIA3;
using Debug = System.Diagnostics.Debug;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class DiscordFinder : IDisposable
   {
      private bool _initialized;
      private readonly AutomationBase _automation = new UIA3Automation();
      private readonly InspectLauncher _inspectLauncher = new InspectLauncher();

      public bool Inititialize()
      {
         if ( !_initialized )
         {
            _initialized = _inspectLauncher.EnsureInspectLaunched();
         }
         return _initialized;
      }

      public void Dispose()
      {
         _automation?.Dispose();
         _inspectLauncher?.Dispose();
      }

      public (AutomationElement discord, IInvokePattern invokePattern) FindDiscord()
      {
         Debug.Assert( _initialized );

         var rootElement = _automation.GetDesktop();
         var children = rootElement.FindAll( TreeScope.Children, _automation.ConditionFactory.ByClassName( "Chrome_WidgetWin_1" ) );
         foreach ( var child in children )
         {
            if ( child.Name.Contains( "- Discord" ) )
            {
               var messageBox = SearchDiscordForMessageBox( child );
               if ( messageBox != null )
               {
                  return (child, messageBox);
               }
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
