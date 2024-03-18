using Blazored.SessionStorage;
using CourseEnrollBlaze.Client.Extentions;
using CourseEnrollBlaze.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CourseEnrollBlaze.Client.Authentication
{
    public class CustomAuthenticationState:AuthenticationStateProvider
    {
        public readonly ISessionStorageService _sessionStorageService;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        public CustomAuthenticationState(  ISessionStorageService sessionStorageService)
        { 
            _sessionStorageService = sessionStorageService;
        }
         
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSession = await _sessionStorageService.ReadEncryptedItemAsync<UserSession>("UserSession");
                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                }
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                     new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Name,userSession.FirstName),
                    new Claim(ClaimTypes.Surname, userSession.LastName) 
               }, "JwtAuth"));
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception)
            {

                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }
        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;
                if (userSession == null)
                {
                    claimsPrincipal = _anonymous;
                    await _sessionStorageService.RemoveItemAsync("UserSession");
                }
                else
                {
                    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                     new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Name,userSession.FirstName),
                    new Claim(ClaimTypes.Surname, userSession.LastName)
               }));
                    userSession.ExpiresInTimeStamp = DateTime.Now.AddSeconds(userSession.ExpiresIn);
                    await _sessionStorageService.SaveItemEncryptedAsync("UserSession", userSession);
                }

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                Console.WriteLine($"Error updating authentication state: {ex.Message}");
            }
        }
        public async Task<string> GetTokenAsync()
        {
            var result = string.Empty;
            try
            {
                var userSession = await _sessionStorageService.ReadEncryptedItemAsync<UserSession>("UserSession");
                if (userSession != null && DateTime.Now<userSession.ExpiresInTimeStamp)
                {
                    result = userSession.Token;

                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }
    }
}
