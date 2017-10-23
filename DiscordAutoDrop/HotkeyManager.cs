using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using DiscordAutoDrop.Utilities;

namespace DiscordAutoDrop
{
   public sealed class HotkeyFiredEventArgs : EventArgs
   {
      public HotkeyFiredEventArgs( int id )
      {
         HotkeyId = id;
      }

      public int HotkeyId { get; }
   }

   public sealed class HotkeyManager : Window, IDisposable
   {
      public event EventHandler<HotkeyFiredEventArgs> HotkeyFired;

      private readonly List<int> _registeredHotkeys = new List<int>();
      private IntPtr _hwnd;
      private HwndSource _source;

      public HotkeyManager()
      {
         var helper = new WindowInteropHelper( this );
         helper.EnsureHandle();
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
            HotkeyFired?.Invoke( this, new HotkeyFiredEventArgs( wparam.ToInt32() ) );
         }
         return IntPtr.Zero;
      }

      public bool TryRegister( Key key, ModifierKeys modifier, out int id )
      {
         id = (int)key + (int)modifier * 0x10000;

         bool result = NativeMethods.RegisterHotKey( _hwnd, id, (uint)modifier, (uint)KeyInterop.VirtualKeyFromKey( key ) );
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
            var result = NativeMethods.UnregisterHotKey( _hwnd, id );
            Debug.Assert( result );

            _registeredHotkeys.Remove( id );
         }
      }

      public void Dispose()
      {
         foreach ( var hotkeyId in _registeredHotkeys )
         {
            Unregister( hotkeyId );
         }
         _source.RemoveHook( HwndHook );
      }
   }
}
