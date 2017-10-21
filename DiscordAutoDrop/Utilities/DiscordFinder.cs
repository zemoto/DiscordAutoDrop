using UIAutomationClient;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class DiscordFinder
   {
      private readonly CUIAutomation _automation = new CUIAutomation();

      public IUIAutomationElement FindDiscord()
      {
         var classNameCondition = _automation.CreatePropertyCondition( UIA_PropertyIds.UIA_ClassNamePropertyId, "Chrome_WidgetWin_1" );
         var rootElement = _automation.GetRootElement();
         var children = rootElement.FindAll( TreeScope.TreeScope_Children, classNameCondition );
         for ( int i = 0; i < children.Length; i++ )
         {
            var child = children.GetElement( i );
            if ( child.CurrentName.Contains( "- Discord" ) )
            {
               return child;
            }
         }
         return null;
      }

      public IUIAutomationLegacyIAccessiblePattern FindDiscordMessageBoxLegacyPattern( IUIAutomationElement discord )
      {
         var controlTypeCondition = _automation.CreatePropertyCondition( UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_EditControlTypeId );
         var editControls = discord.FindAll( TreeScope.TreeScope_Descendants, controlTypeCondition );
         for ( int j = 0; j < editControls.Length; j++ )
         {
            var editControl = editControls.GetElement( j );
            if ( !editControl.CurrentName.Contains( "Message" ) )
            {
               continue;
            }

            int patternId = 10018/*IUIAutomationLegacyIAccessiblePattern*/;
            if ( editControl.GetCurrentPattern( patternId ) is IUIAutomationLegacyIAccessiblePattern legacyPattern )
            {
               return legacyPattern;
            }
         }
         return null;
      }
   }
}
