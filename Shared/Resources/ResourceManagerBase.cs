using System.Collections.Concurrent;
using System.Reflection;
using System.Resources;

namespace Shared.Resources
{
    public abstract class ResourceManagerBase : IResourceManager
    {
        private const string ResourceNameRequired = "Resources";
        private const string DefaultText = "default";
        private const string DefaultBrand = "stone";

        private readonly ConcurrentDictionary<(string, string), IEnumerable<ResourceManager>> _resourceDictionary = new();

        protected ResourceManagerBase() => LoadResources();

        public string GetResourceText(string key, string brand, string platform)
        {
            var value = TryGetResourceValue(key, brand.ToLower(), platform.ToLower());
            return !string.IsNullOrEmpty(value) ? value : DefaultText;
        }
        
        private void LoadResources()
        {
            var resourcesType = GetResources();
            var groupedTypes = resourcesType.GroupBy(x => GetBrandAndPlatformFromName(x.Name))
                .ToDictionary(
                    g => (g.Key.brand, g.Key.platform),
                    g => g.Select(x => new ResourceManager(x.FullName!, x.Assembly))
                );
            var brands = Enum.GetValues(typeof(Brand)).Cast<Brand>().ToList();
   
            foreach (var brand in brands)
            {
                BuildBrandResources(brand, groupedTypes);
            }
        }

        private void BuildBrandResources(Brand brand, Dictionary<(string, string), IEnumerable<ResourceManager>> resourcesByBrandAndPlatform)
        {
            BuildResourcesByPlatformAndBrand(brand, GeneralPlatForm.Web, resourcesByBrandAndPlatform);
            BuildResourcesByPlatformAndBrand(brand, GeneralPlatForm.Mobile, resourcesByBrandAndPlatform);
        }

        private void BuildResourcesByPlatformAndBrand(Brand brand, GeneralPlatForm platform, Dictionary<(string, string), IEnumerable<ResourceManager>> resourcesGroupedByBrandAndPlatform)
        {
            var brandString = brand.ToString().ToLower();
            var platformString = platform.ToString().ToLower();
            
            var hasKey = _resourceDictionary.ContainsKey((brandString, platformString));
            if (!hasKey)
            {
                _resourceDictionary.TryAdd((brandString, platformString), []);
            }
        
            _ = resourcesGroupedByBrandAndPlatform.TryGetValue((brandString, platformString), out var resourcesByBrandAndPlatform);
            _ = resourcesGroupedByBrandAndPlatform.TryGetValue((brandString, DefaultText), out var resourcesByBrandAndDefaultPlatform);
        
            if (brandString == DefaultBrand)
            {
                _resourceDictionary[(brandString, platformString)] = (resourcesByBrandAndPlatform ?? [])
                    .Concat(resourcesByBrandAndDefaultPlatform ?? [])
                    .ToArray();
                return;
            }
            
            _ = resourcesGroupedByBrandAndPlatform.TryGetValue((DefaultBrand, platformString), out var resourcesByDefaultBrandAndPlatform);
            _ = resourcesGroupedByBrandAndPlatform.TryGetValue((DefaultBrand, DefaultText), out var resourcesByDefaultBrandAndDefaultPlatform);
            
            _resourceDictionary[(brandString, platformString)] = (resourcesByBrandAndPlatform ?? [])
                .Concat(resourcesByBrandAndDefaultPlatform ?? [])
                .Concat(resourcesByDefaultBrandAndPlatform ?? [])
                .Concat(resourcesByDefaultBrandAndDefaultPlatform ?? [])
                .ToArray();
        }
        
        private IEnumerable<Type> GetResources()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            string[] resourcesNamespaces = [GetType().Namespace!, typeof(ResourceManagerBase).Namespace!];

            var resourceTypes = assemblies
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                })
                .Where(t => t?.Namespace != null && resourcesNamespaces.Contains(t.Namespace)
                                                && t.IsClass
                                                && t.Name.Contains(ResourceNameRequired,
                                                    StringComparison.InvariantCultureIgnoreCase));
            return (resourceTypes ?? [])!;
        }
        
        private static (string brand, string platform) GetBrandAndPlatformFromName(string resourceName)
        {
            var parts = resourceName.Split('_', StringSplitOptions.RemoveEmptyEntries)
                .Skip(2)
                .ToArray();

            return parts.Length == 1 ? 
                (parts[0].ToLower(), DefaultText) : 
                (parts[0].ToLower(), parts[1].ToLower());
        }
        
        private string? TryGetResourceValue(string key, string brand, string platform = DefaultText)
        {
            if (!_resourceDictionary.TryGetValue((brand.ToLower(), platform.ToLower()), out var resources))
                return null;
            
            foreach (var resource in resources)
            {
                var text = resource.GetString(key);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }
            return null;
        }
    }
}
