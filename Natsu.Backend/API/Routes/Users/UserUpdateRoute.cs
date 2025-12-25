using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Users;

public class UserUpdateRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/users/@me";
    public HttpMethod Method => HttpMethod.Patch;

    public IEnumerable<(string, string)> Validate(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryParseBody<Payload>(out var payload))
            yield return ("_form", "Invalid body JSON.");

        if (payload is not null)
            interaction.AddCache("payload", payload);
    }

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetCache<Payload>("payload", out var payload))
            throw new CacheMissingException("payload");

        var user = UserHelper.Get(interaction.UserID)!;

        if (payload.Avatar != null)
            user.AvatarID = payload.Avatar;

        UserHelper.Update(user);
        await interaction.Reply(Midori.Networking.HttpStatusCode.OK, user);
    }

    public class Payload
    {
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }
    }
}

