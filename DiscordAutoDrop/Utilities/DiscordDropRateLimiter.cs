using System;
using System.Timers;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class DiscordDropRateLimiter
   {
      private readonly Action<string> _rateLimitedAction;
      private readonly Timer _timer;

      private string _queuedDrop;
      private readonly object _threadLock = new object();

      public DiscordDropRateLimiter( Action<string> rateLimitedAction )
      {
         _rateLimitedAction = rateLimitedAction;

         _timer = new Timer( 500/*ms*/ );
         _timer.Elapsed += TimerTick;
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
            _rateLimitedAction( _queuedDrop );
            _queuedDrop = string.Empty;
            _timer.Stop();
         }
      }
   }
}
