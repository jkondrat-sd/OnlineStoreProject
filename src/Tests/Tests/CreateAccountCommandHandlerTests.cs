using Moq;
using PaymentService.Application.Commands.CreateAccount;
using PaymentService.Application.Events.IntegrationEvents;
using PaymentService.Application.Services;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;

namespace Tests;

public class CreateAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_AccountDoesNotExist_CreatesAccountAndPublishesEvent()
    {
        var userId = Guid.NewGuid();
        var initialBalance = 0m;
        var currency = "USD";

        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var publisherMock = new Mock<IIntegrationEventPublisher>();

        var handler = new CreateAccountCommandHandler(repoMock.Object, uowMock.Object, publisherMock.Object);

        var command = new CreateAccountCommand(userId, initialBalance, currency);

        var result = await handler.Handle(command, CancellationToken.None);

        repoMock.Verify(r => r.AddAsync(It.Is<Account>(a => a.UserId == userId && a.Balance.Amount == initialBalance && a.Balance.Currency == currency), It.IsAny<CancellationToken>()), Times.Once);
        publisherMock.Verify(p => p.PublishAsync(It.Is<AccountCreatedIntegrationEvent>(e => e.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(initialBalance, result.Balance);
        Assert.Equal(currency, result.Currency);
    }

    [Fact]
    public async Task Handle_AccountAlreadyExists_ThrowsApplicationException()
    {
        var userId = Guid.NewGuid();
        var initialBalance = 0m;
        var currency = "USD";

        var existingAccount = new Account(userId, 100m, "USD");

        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAccount);

        var uowMock = new Mock<IUnitOfWork>();
        var publisherMock = new Mock<IIntegrationEventPublisher>();

        var handler = new CreateAccountCommandHandler(repoMock.Object, uowMock.Object, publisherMock.Object);

        var command = new CreateAccountCommand(userId, initialBalance, currency);

        await Assert.ThrowsAsync<ApplicationException>(() => handler.Handle(command, CancellationToken.None));
    }
}