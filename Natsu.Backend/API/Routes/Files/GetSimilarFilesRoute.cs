using Midori.API.Components.Interfaces;
using Midori.Networking;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Files;

public class GetSimilarFilesRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files/similar";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var percent = interaction.GetFloatQuery("percent") ?? 0.98f;
        var results = TaggedFileHelper.GetComparisons(percent);

        var files = new List<SimilarFile>();

        foreach (var result in results)
        {
            var a = TaggedFileHelper.Get(result.File1);
            var b = TaggedFileHelper.Get(result.File2);

            if (a?.Owner != interaction.UserID || b?.Owner != interaction.UserID)
                continue;

            files.Add(new SimilarFile(a, b, result.Similarity));
        }

        await interaction.Reply(HttpStatusCode.OK, files);
    }

    public class SimilarFile
    {
        [JsonProperty("one")]
        public TaggedFile File1 { get; set; }

        [JsonProperty("two")]
        public TaggedFile File2 { get; set; }

        [JsonProperty("similarity")]
        public float Similarity { get; set; }

        public SimilarFile(TaggedFile file1, TaggedFile file2, float similarity)
        {
            File1 = file1;
            File2 = file2;
            Similarity = similarity;
        }
    }
}
