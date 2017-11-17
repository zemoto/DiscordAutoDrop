using System;
using System.Timers;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class DiscordDropRateLimiter : IDisposable
   {
      public event EventHandler<string> FireDrop;

      private readonly Timer _timer;

      private string _queuedDrop;
      private readonly object _threadLock = new object();

      public DiscordDropRateLimiter()
      {
         _timer = new Timer( 400/*ms*/ );
         _timer.Elapsed += TimerTick;
      }

      public void Dispose()
      {
         _timer.Dispose();
      }

      public void EnqueueDrop( string drop )
      {
         lock ( _threadLock )
         {
            _timer.Stop();
            _queuedDrop += $"!{drop} ";
            _timer.Start();
         }
      }

      private void TimerTick( object sender, ElapsedEventArgs e )
      {
         lock ( _threadLock )
         {
            FireDrop?.Invoke( this, _queuedDrop );
            _queuedDrop = string.Empty;
            _timer.Stop();
         }
      }
   }
}
