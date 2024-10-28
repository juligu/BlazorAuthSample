using System.Security.Claims;
using BlazorTest.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorTest.Client;

internal sealed class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultAuthenticationState =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultAuthenticationState;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return;
        }

        authenticationStateTask = Task.FromResult(new AuthenticationState(userInfo.ToClaimsPrincipal()));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        authenticationStateTask;
}