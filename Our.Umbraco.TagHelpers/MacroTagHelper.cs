using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Templates;
using Umbraco.Cms.Core.Web;

namespace Our.Umbraco.TagHelpers
{
    /// <summary>
    /// Render Umbraco Macros in your views
    /// </summary>
    [HtmlTargetElement("our-macro")]
#if NET10_0_OR_GREATER
    [Obsolete("Macros are removed in Umbraco 14+. This TagHelper does nothing in this version.")]
#endif
    public class MacroTagHelper : TagHelper
    {
        private readonly IUmbracoComponentRenderer _umbracoComponentRenderer;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        /// <summary>
        /// The alias of the Macro to execute
        /// </summary>
        [HtmlAttributeName("alias")]
        public string? MacroAlias { get; set; }

        /// <summary>
        /// An optional attribute to set the context of the Content that the Macro will use
        /// Without it set it will use the current page
        /// </summary>
        [HtmlAttributeName("content")]
        public IPublishedContent? ContentNode { get; set; }

        /// <summary>
        /// An optional list of attributes that will map to Macro Parameters
        /// bind:myMacroParam="" bind:startNodeId="" etc...
        /// </summary>
        [HtmlAttributeName(DictionaryAttributePrefix = "bind:")]
        public IDictionary<string, string>? MacroParameters { get; set; } = new Dictionary<string, string>();

        public MacroTagHelper(IUmbracoComponentRenderer umbracoComponentRenderer, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoComponentRenderer = umbracoComponentRenderer;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = ""; // Remove the outer <umb-macro> tag

#if NET10_0_OR_GREATER
            // Macros are not supported in Umbraco 17
            // We could log a warning but for now we just suppress output
            await Task.CompletedTask;
#else
            if (ContentNode is null)
            {
                // Get the current page/published request IPublishedContent for the content/context to run the Macro in
                if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                {
                    ContentNode = umbracoContext.PublishedRequest.PublishedContent;
                }
            }

            // TagHelpers dont seem to like HTMLAttributes as <string, object> dictionary
            // Hence us trying to convert it here
            IDictionary<string, object> macroParams = new Dictionary<string, object>();
            if (MacroParameters != null && MacroParameters.Any())
            {
                macroParams = MacroParameters.ToDictionary(p => p.Key, p => (object)p.Value);
            }

            var macroResult = await _umbracoComponentRenderer.RenderMacroForContent(ContentNode, MacroAlias, macroParams);
            output.Content.SetHtmlContent(macroResult.ToHtmlString());
#endif
        }
    }
}
