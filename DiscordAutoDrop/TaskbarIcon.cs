using System;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DiscordAutoDrop
{
   internal sealed class TaskbarIcon : IDisposable
   {
      private readonly NotifyIcon _notifyIcon;

      public TaskbarIcon( Action showHotkeyWindowAction )
      {
         _notifyIcon = new NotifyIcon();
         var menu = new ContextMenu();

         var showOptionsMenuItem = new MenuItem { Text = "Hotkeys && Drops..." };
         showOptionsMenuItem.Click += ( _, __ ) => showHotkeyWindowAction.Invoke();

         var exitMenuItem = new MenuItem { Text = "Exit" };
         exitMenuItem.Click += ( _, __ ) => Application.Current.Shutdown();

         menu.MenuItems.AddRange( new MenuItem[]
         {
            showOptionsMenuItem,
            exitMenuItem
         } );

         _notifyIcon.ContextMenu = menu;
         _notifyIcon.Icon = Properties.Resources.Icon;
         _notifyIcon.Text = "Discord Auto Drop";
         _notifyIcon.MouseDoubleClick += ( _, __ ) => showHotkeyWindowAction.Invoke();
      }

      public void Dispose()
      {
         _notifyIcon.Visible = false;
         _notifyIcon.Dispose();
      }

      public void Show()
      {
         _notifyIcon.Visible = true;
      }
   }
}
