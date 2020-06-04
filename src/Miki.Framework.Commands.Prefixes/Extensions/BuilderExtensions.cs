using System;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Commands.Prefixes.Triggers;
using Miki.Framework.Hosting;
using Miki.Logging;

namespace Miki.Framework.Commands.Prefixes
{
    public static class BuilderExtensions
    {
        public static PrefixCollectionBuilder AddDynamic(this PrefixCollectionBuilder builder, string prefix, bool isDefault = false)
        {
            return builder.Add(new DynamicPrefixTrigger(prefix), isDefault);
        }
        
        public static PrefixCollectionBuilder Add(this PrefixCollectionBuilder builder, string prefix, bool isDefault = false)
        {
            return builder.Add(new PrefixTrigger(prefix), isDefault);
        }
        
        public static PrefixCollectionBuilder AddMention(this PrefixCollectionBuilder builder, bool isDefault = false)
        {
            return builder.Add(new MentionTrigger(), isDefault);
        }

        public static PrefixCollectionBuilder Add(this PrefixCollectionBuilder builder, ITrigger trigger, bool isDefault)
        {
            return isDefault ? builder.AddAsDefault(trigger) : builder.Add(trigger);
        }
    }
}