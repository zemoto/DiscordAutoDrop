﻿using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace DiscordAutoDrop.Utilities
{
   internal static class NativeMethods
   {
      public const int WM_HOTKEY = 0x0312;
      public const int MOD_NOREPEAT = 0x4000;

      [DllImport( "user32.dll" )]
      public static extern bool RegisterHotKey( IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc );

      [DllImport( "user32.dll" )]
      public static extern bool UnregisterHotKey( IntPtr hWnd, int id );

      [DllImport( "user32.dll" )]
      [return: MarshalAs( UnmanagedType.Bool )]
      public static extern bool SetForegroundWindow( IntPtr hWnd );
   }
}
