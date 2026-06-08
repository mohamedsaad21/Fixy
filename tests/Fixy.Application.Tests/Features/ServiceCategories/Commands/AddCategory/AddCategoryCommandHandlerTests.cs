using AutoMapper;
using Fixy.Application.Features.ServiceCategories.Commands.AddCategory;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fixy.Application.Tests.Features.ServiceCategories.Commands;

public class AddCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<AddCategoryCommandHandler>> _loggerMock;
    private readonly AddCategoryCommandHandler _handler;

    public AddCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new AddCategoryCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryId_WhenCategoryIsAddedSuccessfully()
    {
        // Arrange
        var command = new AddCategoryCommand("Name", "اسم");
        ServiceCategory? category = new ServiceCategory { Id = Guid.NewGuid(), NameAr = command.NameAr, NameEn = command.NameEn };
        _mapperMock.Setup(m => m.Map<ServiceCategory>(command)).Returns(category);
        _unitOfWorkMock.Setup(u => u.ServiceCategories.AddAsync(category)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(category.Id);

        _unitOfWorkMock.Verify(u => u.ServiceCategories.AddAsync(category), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenCategoryIsAdded()
    {
        // Arrange
        var command = new AddCategoryCommand("Name", "اسم");
        var category = new ServiceCategory { Id = Guid.NewGuid(), NameAr = command.NameAr, NameEn = command.NameEn };
        _mapperMock.Setup(m => m.Map<ServiceCategory>(command)).Returns(category);
        _unitOfWorkMock.Setup(u => u.ServiceCategories.AddAsync(category)).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), command.NameAr, command.NameEn), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenAddAsyncFails()
    {
        // Arrange
        var command = new AddCategoryCommand("Name", "اسم");
        var category = new ServiceCategory { Id = Guid.NewGuid(), NameAr = command.NameAr, NameEn = command.NameEn };
        _mapperMock.Setup(m => m.Map<ServiceCategory>(command)).Returns(category);
        _unitOfWorkMock.Setup(u => u.ServiceCategories.AddAsync(category)).ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Theory]
    [InlineData(null, "Name")]
    [InlineData("اسم", null)]
    [InlineData("", "Name")]
    [InlineData("اسم", "")]
    public async Task Handle_ShouldThrowArgumentNullException_WhenInputIsNullOrEmpty(string nameAr, string nameEn)
    {
        // Arrange
        var command = new AddCategoryCommand(nameEn, nameAr);
        if (string.IsNullOrEmpty(command.NameEn))
        {
            throw new ArgumentNullException(nameof(command.NameEn), "English name cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(command.NameAr))
        {
            throw new ArgumentNullException(nameof(command.NameAr), "Arabic name cannot be null or empty.");
        }
        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
