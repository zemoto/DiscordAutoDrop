using System;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable InconsistentNaming

namespace DiscordAutoDrop.Utilities
{
   internal static class NativeMethods
   {
      [DllImport( "user32.dll", EntryPoint = "AttachThreadInput", CharSet = CharSet.Unicode )]
      public static extern bool AttachThreadInput( IntPtr idAttach, IntPtr idAttachTo, bool fAttach );

      [DllImport( "user32.dll" )]
      public static extern IntPtr GetWindowThreadProcessId( IntPtr hWnd, IntPtr ProcessId );

      [DllImport( "user32.dll" )]
      public static extern IntPtr GetForegroundWindow();

      [DllImport( "user32.dll" )]
      [return: MarshalAs( UnmanagedType.Bool )]
      public static extern bool IsWindowVisible( IntPtr hWnd );

      [DllImport( "user32.dll", EntryPoint = "GetWindowText", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true )]
      public static extern int GetWindowText( IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount );

      [DllImport( "user32.dll" )]
      public static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

      [DllImport( "user32.dll" )]
      public static extern bool EnumDesktopWindows( IntPtr hDesktop, EnumDesktopWindowsDelegate lpfn, IntPtr lParam );

      public delegate bool EnumDesktopWindowsDelegate( IntPtr hWnd, int lParam );

      [Flags]
      public enum SetWindowPosFlags : uint
      {
         DoNotActivate = 0x0010,
         IgnoreMove = 0x0002,
         IgnoreResize = 0x0001,
         ShowWindow = 0x0040,
      }

      [DllImport( "user32.dll", SetLastError = true )]
      public static extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags );
   }
}
