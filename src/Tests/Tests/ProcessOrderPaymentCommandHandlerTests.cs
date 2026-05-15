using Microsoft.Extensions.Logging;
using Moq;
using PaymentService.Application.Commands.ProcessOrderPayment;
using PaymentService.Application.Events.IntegrationEvents;
using PaymentService.Application.Services;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Domain.ValueObjects;

namespace Tests;

public class ProcessOrderPaymentCommandHandlerTests
{
    [Fact]
    public async Task Handle_SufficientFunds_PublishesSuccessEventAndSavesChanges()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var amount = 50m;
        var currency = "USD";
        var accountId = Guid.NewGuid();
        var initialBalance = 100m;

        var account = new Account(userId, initialBalance, currency)
        {
            Id = new AccountId(accountId)
        };

        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var publisherMock = new Mock<IIntegrationEventPublisher>();

        var loggerMock = new Mock<ILogger<ProcessOrderPaymentCommandHandler>>();

        var handler = new ProcessOrderPaymentCommandHandler(
            repoMock.Object,
            uowMock.Object,
            publisherMock.Object,
            loggerMock.Object
        );

        var command = new ProcessOrderPaymentCommand(userId, orderId, amount, currency);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(initialBalance - amount, account.Balance.Amount);
        repoMock.Verify(r => r.UpdateAsync(account, It.IsAny<CancellationToken>()), Times.Once);
        publisherMock.Verify(p => p.PublishAsync(It.Is<PaymentSucceededIntegrationEvent>(e =>
            e.OrderId == orderId &&
            e.AccountId == accountId &&
            e.Amount == amount &&
            e.Currency == currency
        ), It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientFunds_PublishesFailEvent()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var amount = 150m;
        var currency = "USD";
        var accountId = Guid.NewGuid();
        var initialBalance = 100m;

        var account = new Account(userId, initialBalance, currency);
        account.Id = new AccountId(accountId);

        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var publisherMock = new Mock<IIntegrationEventPublisher>();

        var loggerMock = new Mock<ILogger<ProcessOrderPaymentCommandHandler>>();

        var handler = new ProcessOrderPaymentCommandHandler(
            repoMock.Object,
            uowMock.Object,
            publisherMock.Object,
            loggerMock.Object
        );

        var command = new ProcessOrderPaymentCommand(userId, orderId, amount, currency);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(initialBalance, account.Balance.Amount);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        publisherMock.Verify(p => p.PublishAsync(It.Is<PaymentFailedIntegrationEvent>(e =>
            e.OrderId == orderId &&
            e.AccountId == accountId &&
            e.Amount == amount &&
            e.Currency == currency &&
            e.Reason == "Not enough money"
        ), It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ThrowsApplicationException()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var amount = 50m;
        var currency = "USD";

        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        var uowMock = new Mock<IUnitOfWork>();
        var publisherMock = new Mock<IIntegrationEventPublisher>();
        var loggerMock = new Mock<ILogger<ProcessOrderPaymentCommandHandler>>();

        var handler = new ProcessOrderPaymentCommandHandler(
            repoMock.Object,
            uowMock.Object,
            publisherMock.Object,
            loggerMock.Object
        );

        var command = new ProcessOrderPaymentCommand(userId, orderId, amount, currency);

        await Assert.ThrowsAsync<ApplicationException>(() => handler.Handle(command, CancellationToken.None));
    }
}