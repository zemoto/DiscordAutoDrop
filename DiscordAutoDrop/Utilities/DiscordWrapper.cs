using Discord;
using Discord.WebSocket;
using DiscordAutoDrop.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DiscordAutoDrop.Utilities
{
   internal sealed class DiscordWrapper : IDisposable
   {
      private readonly ulong _dropTargetChannelId;
      private readonly DiscordSocketClient _client = new DiscordSocketClient();

      public DiscordWrapper( ulong dropTargetChannelId )
      {
         _dropTargetChannelId = dropTargetChannelId;
      }

      public void Dispose()
      {
         _client.Dispose();
      }

      public async Task<bool> LoginAsync( Settings settings, Window owner )
      {
         while ( true )
         {
            var token = settings.UserToken;
            if ( string.IsNullOrEmpty( token ) )
            {
               var prompt = new UserTokenPromptDialog { Owner = owner };
               if ( prompt.ShowDialog() != true )
               {
                  return false;
               }
               token = prompt.UserToken;
            }
            try
            {
               await _client.LoginAsync( TokenType.User, token );
               await _client.StartAsync();
               settings.UserToken = token;
               return true;
            }
            catch { /* Try again */ }
         }
      }

      private SocketTextChannel GetTargetChannel()
      {
         foreach ( var guild in _client.Guilds )
         {
            if ( guild.Users.Any( x => x.Id == _client.CurrentUser.Id && x.VoiceChannel != null ) &&
                 guild.Channels.FirstOrDefault( x => x.Id == _dropTargetChannelId ) is SocketTextChannel targetChannel )
            {
               return targetChannel;
            }
         }
         return null;
      }

      public async void FireDrop( string drop )
      {
         Debug.WriteLine( $"Sending Drop: {drop}" );
         var channel = GetTargetChannel();
         if ( channel != null && !string.IsNullOrEmpty( drop ) )
         {
            await channel.SendMessageAsync( drop );
         }
      }
   }
}
