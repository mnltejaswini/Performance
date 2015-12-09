// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Benchmarks.Framework;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.Compilation.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Xunit;

namespace Microbenchmarks.Tests.Razor
{
    public class RazorTests : BenchmarkTestBase
    {
        [Benchmark]
        [BenchmarkVariation("Runtime", false)]
        [BenchmarkVariation("Design Time", true)]
        public void TagHelperResolution(bool designTime)
        {
            var descriptorResolver = new TagHelperDescriptorResolver(designTime);
            var errorSink = new ErrorSink();
            var addTagHelperDirective = new TagHelperDirectiveDescriptor
            {
                DirectiveText = "*, Microsoft.AspNet.Mvc.TagHelpers",
                DirectiveType = TagHelperDirectiveType.AddTagHelper,
                Location = SourceLocation.Zero
            };
            var resolutionContext = new TagHelperDescriptorResolutionContext(
                new[] { addTagHelperDirective },
                errorSink);
            IEnumerable<TagHelperDescriptor> descriptors;

            using (Collector.StartCollection())
            {
                descriptors = descriptorResolver.Resolve(resolutionContext);
            }

            Assert.NotEmpty(descriptors);
            Assert.Empty(errorSink.Errors);
        }
    }
}
