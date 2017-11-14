using System;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DiscordAutoDrop
{
   internal sealed class AuthenticationManager
   {
      private const string ClientId = "379073407829934080";
      private const string ClientSecret = "GM2Z2DaoDtvyNacfDymq_pgkseQ4Zni-";
      private const string RedirectUrl = "http://localhost/";
      private const string TokenUrl = "https://discordapp.com/api/oauth2/token";

      private readonly string _authorizationCodeUrl = $"https://discordapp.com/api/oauth2/authorize?response_type=code&client_id={ClientId}&scope=bot";

      public async Task<bool> SignInAsync( Settings settings )
      {
         if ( !EnsureRunningAsAdmin() )
         {
            return false;
         }

         if ( string.IsNullOrEmpty( settings.AccessToken ) )
         {
            await Login( settings );
         }
         else if ( DateTime.Now > settings.ExpirationTime )
         {
            await RefreshLogin( settings );
         }

         return !string.IsNullOrEmpty( settings.AccessToken );
      }

      private async Task Login( Settings settings )
      {
         var listener = new HttpListener();
         listener.Prefixes.Add( RedirectUrl );
         listener.Start();

         Process.Start( _authorizationCodeUrl );

         var context = await listener.GetContextAsync();
         var request = context.Request;

         var codeUrl = request.Url.ToString();
         if ( codeUrl.Contains( "code=" ) )
         {
            var authCode = codeUrl.Replace( RedirectUrl + "?code=", string.Empty );

            using ( var client = new HttpClient() )
            {
               var parameters = new Dictionary<string, string>
               {
                  ["client_id"] = ClientId,
                  ["client_secret"] = ClientSecret,
                  ["grant_type"] = "authorization_code",
                  ["code"] = authCode,
                  ["redirect_uri"] = RedirectUrl
               };
               var rawResponse = await client.PostAsync( TokenUrl, new FormUrlEncodedContent( parameters ) );
               var response = await rawResponse.Content.ReadAsStringAsync();
               var json = JObject.Parse( response );

               settings.AccessToken = json["access_token"].ToString();
               settings.RefreshToken = json["refresh_token"].ToString();
               settings.ExpirationTime = DateTime.Now + TimeSpan.FromSeconds( int.Parse( json["expires_in"].ToString() ) - 60 );
            }
         }
         listener.Stop();
      }

      private static async Task RefreshLogin( Settings settings )
      {
         using ( var client = new HttpClient() )
         {
            var parameters = new Dictionary<string, string>
            {
               ["client_id"] = ClientId,
               ["client_secret"] = ClientSecret,
               ["grant_type"] = "refresh_token",
               ["refresh_token"] = settings.RefreshToken,
               ["redirect_uri"] = RedirectUrl
            };

            var rawResponse = await client.PostAsync( TokenUrl, new FormUrlEncodedContent( parameters ) );
            var response = await rawResponse.Content.ReadAsStringAsync();
            var json = JObject.Parse( response );

            settings.AccessToken = json["access_token"].ToString();
            settings.RefreshToken = json["refresh_token"].ToString();
            settings.ExpirationTime = DateTime.Now + TimeSpan.FromSeconds( int.Parse( json["expires_in"].ToString() ) - 60 );
         }
      }

      private static bool EnsureRunningAsAdmin()
      {
         var identity = WindowsIdentity.GetCurrent();
         var principal = new WindowsPrincipal( identity );
         if ( !principal.IsInRole( WindowsBuiltInRole.Administrator ) )
         {
            var startInfo = new ProcessStartInfo( Process.GetCurrentProcess().MainModule.FileName )
            {
               Verb = "runas"
            };
            Process.Start( startInfo );
            return false;
         }
         return true;
      }
   }
}