using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using DiscordAutoDrop.Utilities;

namespace DiscordAutoDrop
{
   public sealed class HotkeyManager : Window
   {
      public event EventHandler<int> HotkeyFired;

      private readonly List<int> _registeredHotkeys = new List<int>();
      private IntPtr _hwnd;
      private HwndSource _source;

      public HotkeyManager()
      {
         var helper = new WindowInteropHelper( this );
         helper.EnsureHandle();
         Closing += OnClosing;
      }

      protected override void OnSourceInitialized( EventArgs e )
      {
         base.OnSourceInitialized( e );

         _hwnd = new WindowInteropHelper( this ).Handle;
         _source = HwndSource.FromHwnd( _hwnd );
         _source.AddHook( HwndHook );
      }

      private IntPtr HwndHook( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled )
      {
         if ( msg == NativeMethods.WM_HOTKEY )
         {
            HotkeyFired?.Invoke( this, wparam.ToInt32() );
         }
         return IntPtr.Zero;
      }

      public bool TryRegister( Key key, ModifierKeys modifier, out int id )
      {
         id = (int)key + (int)modifier * 0x10000;

         var interopModifier = (uint)modifier | NativeMethods.MOD_NOREPEAT;
         bool result = NativeMethods.RegisterHotKey( _hwnd, id, interopModifier, (uint)KeyInterop.VirtualKeyFromKey( key ) );
         if ( result )
         {
            _registeredHotkeys.Add( id );
         }

         return result;
      }

      public void Unregister( int id )
      {
         if ( _registeredHotkeys.Contains( id ) )
         {
            Debug.Assert( NativeMethods.UnregisterHotKey( _hwnd, id ) );
            _registeredHotkeys.Remove( id );
         }
      }

      private void OnClosing( object sender, CancelEventArgs args )
      {
         var registeredHotkeysFrozen = new List<int>( _registeredHotkeys );
         foreach ( var hotkeyId in registeredHotkeysFrozen )
         {
            Unregister( hotkeyId );
         }
         _source.RemoveHook( HwndHook );
      }
   }
}
