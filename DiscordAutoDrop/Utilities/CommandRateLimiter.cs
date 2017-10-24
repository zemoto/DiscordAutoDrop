using System;
using System.Timers;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class CommandRateLimiter
   {
      private readonly Action<string> _rateLimitedAction;
      private readonly Timer _timer;

      private string _queuedCommand;
      private readonly object _commandLock = new object();

      public CommandRateLimiter( Action<string> rateLimitedAction )
      {
         _rateLimitedAction = rateLimitedAction;

         _timer = new Timer( 500 );
         _timer.Elapsed += TimerTick;
      }

      public void QueueCommand( string command )
      {
         lock ( _commandLock )
         {
            _timer.Stop();
            _queuedCommand += $"!{command} ";
            _timer.Start();
         }
      }

      private void TimerTick( object sender, ElapsedEventArgs e )
      {
         lock ( _commandLock )
         {
            _rateLimitedAction( _queuedCommand );
            _queuedCommand = string.Empty;
            _timer.Stop();
         }
      }
   }
}
