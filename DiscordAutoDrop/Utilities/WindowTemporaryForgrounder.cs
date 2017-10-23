using System;
using System.Collections.Generic;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class WindowTemporaryForgrounder : IDisposable
   {
      private readonly IntPtr _hwnd;
      private readonly IntPtr _windowOriginallyBehind;

      public WindowTemporaryForgrounder( IntPtr hwnd )
      {
         _hwnd = hwnd;
         _windowOriginallyBehind = GetWindowBehindWindowToForeground();
         ForegroundWindow( _hwnd );
      }

      public void Dispose()
      {
         PutWindowBackInOriginalZOrder();
      }

      private static void ForegroundWindow( IntPtr hwnd )
      {
         var windowThreadId = NativeMethods.GetWindowThreadProcessId( hwnd, IntPtr.Zero );
         var foregroundThreadId = NativeMethods.GetWindowThreadProcessId( NativeMethods.GetForegroundWindow(), IntPtr.Zero );

         var flags = NativeMethods.SetWindowPosFlags.IgnoreResize |
                     NativeMethods.SetWindowPosFlags.IgnoreMove;
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

      private IntPtr GetWindowBehindWindowToForeground()
      {
         var collection = new List<IntPtr>();

         bool DesktopWindowFilter( IntPtr hWnd, int lParam )
         {
            if ( NativeMethods.IsWindowVisible( hWnd ) )
            {
               collection.Add( hWnd );
            }
            return true;
         }

         if ( NativeMethods.EnumDesktopWindows( IntPtr.Zero, DesktopWindowFilter, IntPtr.Zero ) )
         {
            var index = collection.FindLastIndex( x => x.Equals( _hwnd ) );
            if ( index < collection.Count - 1 )
            {
               return collection[index + 1];
            }
         }
         return IntPtr.Zero;
      }

      private void PutWindowBackInOriginalZOrder()
      {
         var flags = NativeMethods.SetWindowPosFlags.IgnoreResize |
                     NativeMethods.SetWindowPosFlags.IgnoreMove |
                     NativeMethods.SetWindowPosFlags.DoNotActivate;
         if ( _windowOriginallyBehind != IntPtr.Zero && _windowOriginallyBehind != _hwnd )
         {
            NativeMethods.SetWindowPos( _hwnd, new IntPtr( 1 ), 0, 0, 0, 0, flags );
            NativeMethods.SetWindowPos( _hwnd, _windowOriginallyBehind, 0, 0, 0, 0, flags );
         }
         else
         {
            NativeMethods.SetWindowPos( _hwnd, new IntPtr( 1 ), 0, 0, 0, 0, flags );
         }
      }
   }
}
