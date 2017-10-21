using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class WindowTemporaryForgrounder : IDisposable
   {
      private readonly IntPtr _hwnd;
      private readonly IntPtr _oldForegroundWindow;
      private readonly IntPtr _inFrontWindow;

      public WindowTemporaryForgrounder( IntPtr hwnd, bool hideWhenForegrounded )
      {
         _hwnd = hwnd;
         _oldForegroundWindow = NativeMethods.GetForegroundWindow();
         _inFrontWindow = GetWindowInFrontOfWindowToForeground();
         ForegroundWindow( _hwnd, hideWhenForegrounded );
      }

      public void Dispose()
      {
         PutWindowBackInOriginalZOrder();
      }

      private static void ForegroundWindow( IntPtr hwnd, bool hideWhenForegrounded )
      {
         var windowThreadId = NativeMethods.GetWindowThreadProcessId( hwnd, IntPtr.Zero );
         var foregroundThreadId = NativeMethods.GetWindowThreadProcessId( NativeMethods.GetForegroundWindow(), IntPtr.Zero );

         var flags = NativeMethods.SetWindowPosFlags.IgnoreResize |
                     NativeMethods.SetWindowPosFlags.IgnoreMove;
         if ( hideWhenForegrounded )
         {
            NativeMethods.ShowWindow( hwnd, 0/*SW_HIDE*/ );
         }
         if ( foregroundThreadId != windowThreadId )
         {
            NativeMethods.AttachThreadInput( windowThreadId, foregroundThreadId, true );
            NativeMethods.SetWindowPos( hwnd, IntPtr.Zero, 0, 0, 0, 0, flags );
            NativeMethods.AttachThreadInput( windowThreadId, foregroundThreadId, false );
         }
         else
         {
            NativeMethods.SetWindowPos( hwnd, IntPtr.Zero, 0, 0, 0, 0, flags );
         }
      }

      private IntPtr GetWindowInFrontOfWindowToForeground()
      {
         var collection = new List<IntPtr>();

         bool DesktopWindowFilter( IntPtr hWnd, int lParam )
         {
            var strbTitle = new StringBuilder( 255 );
            NativeMethods.GetWindowText( hWnd, strbTitle, strbTitle.Capacity + 1 );
            string strTitle = strbTitle.ToString();

            if ( NativeMethods.IsWindowVisible( hWnd ) && string.IsNullOrEmpty( strTitle ) == false )
            {
               collection.Add( hWnd );
            }
            return true;
         }

         if ( NativeMethods.EnumDesktopWindows( IntPtr.Zero, DesktopWindowFilter, IntPtr.Zero ) )
         {
            var index = collection.FindLastIndex( x => x.Equals( _hwnd ) );
            if ( index > 0 )
            {
               return collection[index - 1];
            }
         }
         return IntPtr.Zero;
      }

      private void PutWindowBackInOriginalZOrder()
      {
         if ( _inFrontWindow != IntPtr.Zero )
         {
            NativeMethods.SetWindowPos( _hwnd, _inFrontWindow, 0, 0, 0, 0, NativeMethods.SetWindowPosFlags.IgnoreResize |
                                                                           NativeMethods.SetWindowPosFlags.IgnoreMove |
                                                                           NativeMethods.SetWindowPosFlags.DoNotActivate |
                                                                           NativeMethods.SetWindowPosFlags.ShowWindow );
         }
         if ( _oldForegroundWindow != IntPtr.Zero )
         {
            ForegroundWindow( _oldForegroundWindow, false );
         }
      }
   }
}
