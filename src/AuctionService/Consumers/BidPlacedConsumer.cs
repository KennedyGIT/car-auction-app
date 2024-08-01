using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext dbContext;

        public BidPlacedConsumer(AuctionDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming auction finished");

            var auction = await dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

            if (auction.CurrentHighBid == null
                || context.Message.BidStatus.Contains("Accepted")
                && context.Message.Amount > auction.CurrentHighBid) 
            {
                auction.CurrentHighBid = context.Message.Amount;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
