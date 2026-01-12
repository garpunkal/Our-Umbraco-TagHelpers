using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
#if NET10_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Manifest;
#endif

namespace Our.Umbraco.TagHelpers.Composing
{
    public class PackageManifestComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
#if NET10_0_OR_GREATER
            builder.Services.AddSingleton<IPackageManifestReader, TagHelperPackageManifestReader>();
#else
            builder.ManifestFilters().Append<TagHelperManifestFilter>();
#endif
        }
    }

#if NET10_0_OR_GREATER
    public class TagHelperPackageManifestReader : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
             var version = typeof(TagHelperPackageManifestReader).Assembly.GetName()?.Version?.ToString() ?? "Unknown";
             
             return Task.FromResult<IEnumerable<PackageManifest>>(new[] {
                 new PackageManifest
                 {
                     Name = "Our Umbraco TagHelpers",
                     Version = version,
                     AllowTelemetry = true,
                     Extensions = new object[0] // Adding empty extensions just in case
                 }
             });
        }
    }
#else
    public class TagHelperManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            var version = typeof(TagHelperManifestFilter).Assembly.GetName()?.Version?.ToString() ?? "Unknown";

            manifests.Add(new PackageManifest
            {
                PackageName = "Our Umbraco TagHelpers",
                AllowPackageTelemetry = true,
                Version = version
            });
        }
    }
#endif
}

