using AutoMapper.Implementations;
using FluentAssertions;
using Xunit;

namespace AutoMapper;

public class AutoMapperWithExpressionTests
{
    [Fact]
    public void ShouldMapObjectProperly()
    {
        // Arrange
        IMapper mapper = new MyExpressionImplementation();
        var myRequest = new MyRequest()
        {
            Id = 42,
            Name = "Ank"
        };

        // Act
        var viewModel = mapper.Map<MyRequest, MyRequestViewModel>(myRequest);

        // Assert
        viewModel.Should().BeOfType<MyRequestViewModel>();
        viewModel.Id.Should().Be(42);
        viewModel.Name.Should().Be("Ank");
    }

    [Fact]
    public void ShouldMapObjectsProperly()
    {
        // Arrange
        IMapper mapper = new MyExpressionImplementation();
        var myRequest = new MyRequest()
        {
            Id = 42,
            Name = "Ank"
        };

        // Act
        var viewModel = mapper.Map<MyRequest, MyRequestViewModel>(myRequest);

        var viewModel1 = mapper.Map<MyRequest, MyRequestViewModel>(myRequest);

        // Assert
        viewModel.Should().BeOfType<MyRequestViewModel>();
        viewModel.Id.Should().Be(42);
        viewModel.Name.Should().Be("Ank");

        viewModel1.Should().BeOfType<MyRequestViewModel>();
        viewModel1.Id.Should().Be(42);
        viewModel1.Name.Should().Be("Ank");

        viewModel.GetHashCode().Should().NotBe(viewModel1.GetHashCode());
    }
}
