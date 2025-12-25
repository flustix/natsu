using Midori.Networking;
using Natsu.Backend.API.Components;

namespace Natsu.Backend.API;

public class NotFoundRoute : INatsuAPIRoute
{
    public string RoutePath => "/404";
    public HttpMethod Method => HttpMethod.Get;

    public Task Handle(NatsuAPIInteraction interaction)
        => interaction.ReplyMessage(HttpStatusCode.NotFound, "The requested route does not exist.");
}
