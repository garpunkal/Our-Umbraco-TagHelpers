using System.Linq;
using System.Security.Claims;
using Umbraco.Cms.Core;

namespace Our.Umbraco.TagHelpers.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static bool IsAllowedToSeeEditLink(this ClaimsIdentity identity)
        {
            return IsLoggedIntoUmbraco(identity) && HasAccessToContentSection(identity);
        }

        public static bool IsLoggedIntoUmbraco(this ClaimsIdentity identity)
        {
            return identity?.AuthenticationType != null 
                && identity.AuthenticationType == Constants.Security.BackOfficeAuthenticationType;
        }

        public static bool HasAccessToContentSection(this ClaimsIdentity identity)
        {
            return identity?.Claims != null && identity.Claims.Any(x =>
#if NET10_0_OR_GREATER
#pragma warning disable CS0618
                    x.Type == Constants.Security.AllowedApplicationsClaimType
#pragma warning restore CS0618
#else
                    x.Type == Constants.Security.AllowedApplicationsClaimType
#endif
                    && x.Value == Constants.Conventions.PermissionCategories.ContentCategory);
        }
    }
}
