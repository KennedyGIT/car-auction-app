

using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using System.Security.Principal;

namespace BiddingService.Services
{
    public class CheckAuctionFinished : BackgroundService
    {
        private readonly ILogger<CheckAuctionFinished> logger;
        private readonly IServiceProvider services;

        public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider services) 
        {
            this.logger = logger;
            this.services = services;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting check for finished auctions");

            stoppingToken.Register(() => logger.LogInformation("=> Auction check is stopping"));

            while(!stoppingToken.IsCancellationRequested) 
            {
                await CheckAuctions(stoppingToken);

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task CheckAuctions(CancellationToken stoppingToken)
        {
            var finishedAuctions = await DB.Find<Auction>()
                .Match(x => x.AuctionEnd <= DateTime.UtcNow)
                .Match(x => !x.Finished)
                .ExecuteAsync(stoppingToken);

            if (finishedAuctions.Count == 0) return;

            logger.LogInformation("==> Found {count} auctions that have completed", finishedAuctions.Count);

            using var scope = services.CreateScope();
            var endPoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            foreach (var auction in finishedAuctions) 
            {
                auction.Finished = true;
                await auction.SaveAsync(null, stoppingToken);

                var winningBid = await DB.Find<Bid>()
                    .Match(a => a.AuctionId == auction.ID)
                    .Match(b => b.BidStatus == BidStatus.Accepted)
                    .Sort(x => x.Descending(s => s.Amount))
                    .ExecuteFirstAsync(stoppingToken);

                await endPoint.Publish(new AuctionFinished
                {
                    ItemSold = winningBid != null,
                    AuctionId = auction.ID,
                    Winner = winningBid?.Bidder,
                    Amount = winningBid?.Amount,
                    Seller = auction.Seller
                }, stoppingToken);

            }
        }
    }
}
