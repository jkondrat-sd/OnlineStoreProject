using Moq;
using PaymentService.Application.Queries.GetBalance;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;

namespace Tests;

public class GetBalanceQueryHandlerTests
{
    [Fact]
    public async Task Handle_AccountExists_ReturnsBalance()
    {
        var userId = Guid.NewGuid();
        var balanceAmount = 100m;
        var currency = "USD";

        var account = new Account(userId, balanceAmount, currency);

        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var handler = new GetBalanceQueryHandler(repoMock.Object);

        var query = new GetBalanceQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(balanceAmount, result.Amount);
        Assert.Equal(currency, result.Currency);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ThrowsApplicationException()
    {
        var userId = Guid.NewGuid();

        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        var handler = new GetBalanceQueryHandler(repoMock.Object);

        var query = new GetBalanceQuery(userId);

        await Assert.ThrowsAsync<ApplicationException>(() => handler.Handle(query, CancellationToken.None));
    }
}