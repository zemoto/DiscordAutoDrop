using System;
using System.Windows;
using System.Windows.Input;

namespace DiscordAutoDrop
{
   public sealed class HotkeyRegisteredEventArgs : EventArgs
   {
      public HotkeyRegisteredEventArgs( int id )
      {
         HotkeyId = id;
      }

      public int HotkeyId { get; }
   }

   public partial class HotkeyControl
   {
      private Key _hotkey = Key.None;
      private ModifierKeys _modifier = ModifierKeys.None;

      public static readonly DependencyProperty ManagerProperty = DependencyProperty.Register(
         nameof(Manager),
         typeof(HotkeyManager),
         typeof(HotkeyControl),
         new PropertyMetadata( null ) );

      public HotkeyManager Manager
      {
         get => (HotkeyManager)GetValue( ManagerProperty );
         set => SetValue( ManagerProperty, value );
      }

      public event EventHandler<HotkeyRegisteredEventArgs> HotkeyRegistered; 

      public HotkeyControl()
      {
         InitializeComponent();
      }

      private void OnHotkeyTextBoxGotKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
      {
         HintTextBlock.Text = "Press a hotkey";
         HotkeyTextBox.PreviewKeyDown += OnHotkeyTextBoxPreviewKeyDown;
      }

      private void OnHotkeyTextBoxPreviewKeyDown( object sender, KeyEventArgs args )
      {
         var modifier = args.KeyboardDevice.Modifiers;
         var key = args.Key;

         if ( IsModifierKey( key ) || key == Key.Tab )
         {
            return;
         }

         if ( Manager != null && Manager.TryRegister( key, modifier, out int id ) )
         {
            _hotkey = key;
            _modifier = modifier;
            Keyboard.ClearFocus();

            HotkeyRegistered?.Invoke( this, new HotkeyRegisteredEventArgs( id ) );
         }
      }

      private void OnHotkeyTextBoxLostKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
      {
         HotkeyTextBox.PreviewKeyDown -= OnHotkeyTextBoxPreviewKeyDown;
         if ( _hotkey == Key.None )
         {
            HintTextBlock.Text = "Select a hotkey";
         }
         else
         {
            HotkeyTextBox.Text = $"{GetModifierString()}{_hotkey.ToString()}";
         }
      }

      private string GetModifierString()
      {
         var modifierString = string.Empty;
         if ( _modifier.HasFlag( ModifierKeys.Alt ) )
         {
            modifierString += "Alt + ";
         }
         if ( _modifier.HasFlag( ModifierKeys.Control ) )
         {
            modifierString += "Ctrl + ";
         }
         if ( _modifier.HasFlag( ModifierKeys.Shift ) )
         {
            modifierString += "Shift + ";
         }
         return modifierString;
      }

      private static bool IsModifierKey( Key key )
      {
         return key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt;
      }
   }
}
