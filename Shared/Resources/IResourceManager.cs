namespace Shared.Resources;

public interface IResourceManager
{
    string GetResourceText(string key, string brand, string platform);
}