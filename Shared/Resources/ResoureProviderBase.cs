namespace Shared.Resources;

public abstract class ResourceProviderBase<T>(T resourceTexts, IUser user) : IResoureProvider
    where T : IResourceManager
{
    private readonly T _resourceManagerFiles = resourceTexts;

    public string GetResourceString(string resourceName)
    {
        return _resourceManagerFiles.GetResourceText(resourceName, user.GetUserBrand().ToString(), user.GetUserPlatForm().ToString());
    }
}