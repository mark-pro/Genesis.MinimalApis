using FluentValidation;
using Genesis.DependencyInjection;

namespace Validation.Tests;

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

[TestClass]
public class ServiceCollectionValidationExtensionTests {

    [TestMethod]
    public void AddValidator_Should_Add_Validator_To_ServiceCollection_Default() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidator<SampleValidator>();

        // Assert
        services.Should().Contain(x => ServiceDescriptorIs<SampleValidator>(x));
    }
    
    [TestMethod]
    [DynamicData(nameof(ServiceLifetimes))]
    public void AddValidator_Should_Add_Validator_To_ServiceCollection(ServiceLifetime serviceLifetime) {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidator<SampleValidator>(serviceLifetime);

        // Assert
        services.Should().Contain(x => ServiceDescriptorIs<SampleValidator>(x) && x.Lifetime == serviceLifetime);
    }

    [TestMethod]
    public void AddValidatorsFromAssemblyContaining_Should_Add_All_Public_Validators_From_Assembly_To_ServiceCollection_Default() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidatorsFromAssemblyContaining<ServiceCollectionValidationExtensionTests>();

        // Assert
        services.Should().Contain(x => ServiceDescriptorIs<SampleValidator>(x));
    }
    
    [TestMethod]
    [DynamicData(nameof(ServiceLifetimes))]
    public void AddValidatorsFromAssemblyContaining_Should_Add_All_Public_Validators_From_Assembly_To_ServiceCollection(ServiceLifetime serviceLifetime) {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidatorsFromAssemblyContaining<ServiceCollectionValidationExtensionTests>(serviceLifetime);

        // Assert
        services.Should().Contain(x => ServiceDescriptorIs<SampleValidator>(x) && x.Lifetime == serviceLifetime);
    }

    [TestMethod]
    public void AddValidatorsFromAssemblyContaining_Should_Add_Only_Selected_Validators_From_Assembly_To_ServiceCollection_Default() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidatorsFromAssemblyContaining<ServiceCollectionValidationExtensionTests>(v => v is SampleValidator);

        // Assert
        services.Should().Contain(x => ServiceDescriptorIs<SampleValidator>(x));
        services.Should().NotContain(x => ServiceDescriptorIs<AnotherValidator>(x));
    }
    
    [TestMethod]
    [DynamicData(nameof(ServiceLifetimes))]
    public void AddValidatorsFromAssemblyContaining_Should_Add_Only_Selected_Validators_From_Assembly_To_ServiceCollection(ServiceLifetime serviceLifetime) {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidatorsFromAssemblyContaining<ServiceCollectionValidationExtensionTests>(v => v is SampleValidator, serviceLifetime);

        // Assert
        services.Should().Contain(x => ServiceDescriptorIs<SampleValidator>(x) && x.Lifetime == serviceLifetime);
        services.Should().NotContain(x => ServiceDescriptorIs<AnotherValidator>(x));
    }
    
    private static bool ServiceDescriptorIs<TValidator>(ServiceDescriptor serviceDescriptor) where TValidator : class, IValidator =>
        serviceDescriptor.ServiceType.IsAssignableTo(typeof(IValidator)) && serviceDescriptor.ImplementationType == typeof(TValidator);
    
    private static object[][] ServiceLifetimes => new object[][] {
        new object[] { ServiceLifetime.Scoped },
        new object[] { ServiceLifetime.Singleton },
        new object[] { ServiceLifetime.Transient }
    };
}

internal sealed class SampleValidator : AbstractValidator<object> { }
internal sealed class AnotherValidator : AbstractValidator<object> { }