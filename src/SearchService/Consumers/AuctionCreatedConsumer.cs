using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming Auction created: " + context.Message.Id);

            var item = mapper.Map<Item>(context.Message);

            if (item.Model == "Foo") throw new ArgumentException("Cannot Sell Cars with name of Foo");

            await item.SaveAsync();
        }
    }
}
