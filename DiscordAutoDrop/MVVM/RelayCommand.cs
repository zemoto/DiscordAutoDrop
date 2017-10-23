using System;
using System.Windows.Input;

namespace DiscordAutoDrop.MVVM
{
   public class RelayCommand : ICommand
   {
      readonly Action _execute;
      readonly Func<bool> _canExecute;

      public RelayCommand( Action execute, Func<bool> canExecute = null )
      {
         _execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
         _canExecute = canExecute;
      }

      public bool CanExecute( object parameter )
      {
         return _canExecute?.Invoke() ?? true;
      }

      public event EventHandler CanExecuteChanged
      {
         add => CommandManager.RequerySuggested += value;
         remove => CommandManager.RequerySuggested -= value;
      }

      public void Execute( object _ )
      {
         _execute();
      }
   }

   public class RelayCommand<T> : ICommand
   {
      readonly Action<T> _execute;
      readonly Predicate<T> _canExecute;

      public RelayCommand( Action<T> execute, Predicate<T> canExecute = null )
      {
         _execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
         _canExecute = canExecute;
      }

      public bool CanExecute( object parameter )
      {
         if ( _canExecute == null )
         {
            return true;
         }
         if ( parameter == null && typeof( T ).IsValueType )
         {
            return _canExecute( default( T ) );
         }
         return _canExecute( (T)parameter );
      }

      public event EventHandler CanExecuteChanged
      {
         add => CommandManager.RequerySuggested += value;
         remove => CommandManager.RequerySuggested -= value;
      }

      public void Execute( object parameter )
      {
         if ( parameter == null )
         {
            parameter = default( T );
         }
         _execute( (T)parameter );
      }
   }
}
