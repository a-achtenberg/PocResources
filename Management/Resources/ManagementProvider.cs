using Shared;
using Shared.Resources;

namespace Management.Resources;

public sealed class ManagementProvider(ManagementResourceManager resourceTexts, IUser user) : ResourceProviderBase<ManagementResourceManager>(resourceTexts, user), IManagementResourceProvider;