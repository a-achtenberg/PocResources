using Shared;
using Shared.Resources;

namespace Revit;

public class RevitResourceProvider(RevitResourceManager resourceTexts, IUser user) : ResourceProviderBase<RevitResourceManager>(resourceTexts, user), IRevitResourceProvider;