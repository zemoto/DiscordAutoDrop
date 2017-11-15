using System.Windows;
using System.Windows.Input;

namespace DiscordAutoDrop.Windows
{
   public partial class HotkeyControl
   {
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

      public static readonly DependencyProperty HotkeyProperty = DependencyProperty.Register(
         nameof(Hotkey),
         typeof(Key),
         typeof(HotkeyControl),
         new PropertyMetadata( Key.None ) );

      public Key Hotkey
      {
         get => (Key)GetValue( HotkeyProperty );
         set => SetValue( HotkeyProperty, value );
      }

      public static readonly DependencyProperty ModifierProperty = DependencyProperty.Register(
         nameof(Modifier),
         typeof(ModifierKeys),
         typeof(HotkeyControl),
         new PropertyMetadata( ModifierKeys.None ) );

      public ModifierKeys Modifier
      {
         get => (ModifierKeys)GetValue( ModifierProperty );
         set => SetValue( ModifierProperty, value );
      }

      public static readonly DependencyProperty HotkeyIdProperty = DependencyProperty.Register(
         nameof(HotkeyId),
         typeof(int),
         typeof(HotkeyControl),
         new PropertyMetadata( 0 ) );

      public int HotkeyId
      {
         get => (int)GetValue( HotkeyIdProperty );
         set => SetValue( HotkeyIdProperty, value );
      }

      public HotkeyControl()
      {
         InitializeComponent();

         Loaded += OnLoaded;
      }

      private void OnLoaded( object sender, RoutedEventArgs routedEventArgs )
      {
         UpdateText();
      }

      private void UpdateText()
      {
         if ( Hotkey == Key.None )
         {
            HintTextBlock.Text = "Select a hotkey";
         }
         else
         {
            HintTextBlock.Text = string.Empty;
            HotkeyTextBox.Text = $"{GetModifierString()}{Hotkey.ToString()}";
         }
      }

      private void OnHotkeyTextBoxGotKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
      {
         HintTextBlock.Text = "Press a hotkey";
         HotkeyTextBox.Text = string.Empty;
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
            if ( Hotkey != Key.None )
            {
               Manager.Unregister( HotkeyId );
            }

            Hotkey = key;
            Modifier = modifier;
            HotkeyId = id;
         }
         Keyboard.ClearFocus();
      }

      private void OnHotkeyTextBoxLostKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
      {
         HotkeyTextBox.PreviewKeyDown -= OnHotkeyTextBoxPreviewKeyDown;
         UpdateText();
      }

      private string GetModifierString()
      {
         var modifierString = string.Empty;
         if ( Modifier.HasFlag( ModifierKeys.Alt ) )
         {
            modifierString += "Alt + ";
         }
         if ( Modifier.HasFlag( ModifierKeys.Control ) )
         {
            modifierString += "Ctrl + ";
         }
         if ( Modifier.HasFlag( ModifierKeys.Shift ) )
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
