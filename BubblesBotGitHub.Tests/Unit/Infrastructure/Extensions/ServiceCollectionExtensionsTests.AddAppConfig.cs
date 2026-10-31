using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Extensions;

[Trait(Traits.Names.Category, Traits.Categories.Names.ServiceCollectionExtensionsTests)]
public sealed class AddAppConfig(ServiceCollectionExtensionsTestFixture classFixture) : IClassFixture<ServiceCollectionExtensionsTestFixture>
{
    [Theory]
    [MemberData(
        nameof(ServiceCollectionExtensionsTestFixture.TheoryData),
        MemberType = typeof(ServiceCollectionExtensionsTestFixture))]
    public void MissingEnv_FailsValidation(string configVar)
    {
        IServiceProvider provider = classFixture.BuildServiceProviderWithOptionMissing(configVar);
        
        Action action = () => _ = provider.GetRequiredService<IOptions<Config>>().Value;
        
        action.Should().Throw<OptionsValidationException>();
    }
    
}