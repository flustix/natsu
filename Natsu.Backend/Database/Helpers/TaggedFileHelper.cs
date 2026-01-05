using MongoDB.Bson;
using MongoDB.Driver;
using Natsu.Backend.Models;

namespace Natsu.Backend.Database.Helpers;

public static class TaggedFileHelper
{
    private static IMongoCollection<TaggedFile> collection => MongoDatabase.GetCollection<TaggedFile>("files");
    private static IMongoCollection<ImageComparison> comparisons => MongoDatabase.GetCollection<ImageComparison>("image-comparisons");

    public static List<TaggedFile> All => collection.Find(_ => true).ToList();

    public static void Add(TaggedFile file) => collection.InsertOne(file);

    public static TaggedFile? Get(string id) => !ObjectId.TryParse(id, out var obj) ? null : Get(obj);
    public static TaggedFile? Get(ObjectId id) => collection.Find(x => x.ID == id).FirstOrDefault();

    public static List<TaggedFile> OwnedBy(ObjectId owner) => collection.Find(x => x.Owner == owner).ToList();

    public static bool Update(TaggedFile file) => collection.ReplaceOne(f => f.ID == file.ID, file).IsAcknowledged;

    public static bool Delete(string id) => ObjectId.TryParse(id, out var obj) && Delete(obj);

    public static bool Delete(ObjectId id)
    {
        var result = collection.DeleteOne(f => f.ID == id);
        return result.DeletedCount > 0;
    }

    public static TaggedFile? GetByPath(string id, ObjectId owner) => collection.Find(x => x.FilePath == id && x.Owner == owner).FirstOrDefault();

    public static ImageComparison? GetComparison(ObjectId one, ObjectId two) => comparisons.Find(x => x.File1 == one && x.File2 == two).FirstOrDefault();
    public static List<ImageComparison> GetComparisons(float percent) => comparisons.Find(x => x.Similarity >= percent).ToList();
    public static void SaveComparison(ObjectId one, ObjectId two, float similarity) => comparisons.InsertOne(new ImageComparison { File1 = one, File2 = two, Similarity = similarity });
}
