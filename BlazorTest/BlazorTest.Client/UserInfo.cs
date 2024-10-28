using System.Security;
using System.Security.Claims;

namespace BlazorTest.Client;

public sealed class UserInfo
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public required string[] Roles { get; init; }
    public required string[] Groups { get; init; }
    public required string[] Wids { get; init; }

    public const string UserIdClaimType = "sub";
    public const string NameClaimType = "name";
    private const string RoleClaimType = "roles";
    private const string GroupsClaimType = "groups";
    private const string WidsClaimType = "wids";

    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
        new()
        {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            UserName = GetRequiredClaim(principal, NameClaimType),
            Roles = principal.FindAll(RoleClaimType).Select(claim => claim.Value).ToArray(),
            Groups = principal.FindAll(GroupsClaimType).Select(claim => claim.Value).ToArray(),
            Wids = principal.FindAll(WidsClaimType).Select(claim => claim.Value).ToArray(),
        };

    public ClaimsPrincipal ToClaimsPrincipal() =>
        new(new ClaimsIdentity(
            Roles.Select(role => new Claim(RoleClaimType, role))
                .Concat(Groups.Select(group => new Claim(GroupsClaimType, group)))
                .Concat(Wids.Select(wid => new Claim(WidsClaimType, wid)))
                .Concat([
                    new Claim(UserIdClaimType, UserId),
                    new Claim(NameClaimType, UserName)
                ]),
                authenticationType: nameof(UserInfo),
                nameType: NameClaimType,
                roleType: RoleClaimType));

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType)
        => principal.FindFirst(claimType)?.Value ?? throw new SecurityException($"Missing claim: {claimType}");
}