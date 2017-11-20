using System;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DiscordAutoDrop
{
   internal sealed class TaskbarIcon : IDisposable
   {
      private readonly NotifyIcon _notifyIcon;

      public TaskbarIcon()
      {
         _notifyIcon = new NotifyIcon();
         var menu = new ContextMenu();

         var exitMenuItem = new MenuItem { Text = "Exit" };
         exitMenuItem.Click += OnExitMenuItemClicked;

         menu.MenuItems.Add( exitMenuItem );

         _notifyIcon.ContextMenu = menu;
         _notifyIcon.Icon = Properties.Resources.Icon;
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

      private void OnExitMenuItemClicked( object sender, EventArgs e )
      {
         Application.Current.Shutdown();
      }
   }
}
