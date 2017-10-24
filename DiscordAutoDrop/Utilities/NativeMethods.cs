using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace DiscordAutoDrop.Utilities
{
   internal static class NativeMethods
   {
      [Flags]
      public enum SetWindowPosFlags : uint
      {
         DoNotActivate = 0x0010,
         IgnoreMove = 0x0002,
         IgnoreResize = 0x0001,
      }

      public const int WM_HOTKEY = 0x0312;

      public delegate bool EnumDesktopWindowsDelegate( IntPtr hWnd, int lParam );

      [DllImport( "user32.dll", EntryPoint = "AttachThreadInput", CharSet = CharSet.Unicode )]
      public static extern bool AttachThreadInput( IntPtr idAttach, IntPtr idAttachTo, bool fAttach );

      [DllImport( "user32.dll" )]
      public static extern IntPtr GetWindowThreadProcessId( IntPtr hWnd, IntPtr ProcessId );

      [DllImport( "user32.dll" )]
      public static extern IntPtr GetForegroundWindow();

      [DllImport( "user32.dll" )]
      [return: MarshalAs( UnmanagedType.Bool )]
      public static extern bool IsWindowVisible( IntPtr hWnd );

      [DllImport( "user32.dll" )]
      public static extern bool EnumDesktopWindows( IntPtr hDesktop, EnumDesktopWindowsDelegate lpfn, IntPtr lParam );

      [DllImport( "user32.dll", SetLastError = true )]
      public static extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags );

      [DllImport( "user32.dll" )]
      public static extern bool RegisterHotKey( IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc );

      [DllImport( "user32.dll" )]
      public static extern bool UnregisterHotKey( IntPtr hWnd, int id );
   }
}
