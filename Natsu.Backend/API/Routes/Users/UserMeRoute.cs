using Midori.Networking;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;

namespace Natsu.Backend.API.Routes.Users;

public class UserMeRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/users/@me";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
        => await interaction.Reply(HttpStatusCode.OK, interaction.User);
}
