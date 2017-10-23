using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiscordAutoDrop.MVVM
{
   public abstract class ViewModelBase : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      private void OnPropertyChanged( [CallerMemberName] string propertyName = null )
      {
         PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
      }

      protected bool SetProperty<T>( ref T property, T newValue, [CallerMemberName] string propertyName = null )
      {
         if ( Equals( property, newValue ) )
         {
            return false;
         }

         property = newValue;
         OnPropertyChanged( propertyName );
         return true;
      }
   }
}
