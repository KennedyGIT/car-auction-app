using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Entities;

namespace BiddingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly GrpcAuctionClient grpcAuctionClient;

        public BidsController(IMapper mapper, IPublishEndpoint publishEndpoint, GrpcAuctionClient grpcAuctionClient)
        {
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
            this.grpcAuctionClient = grpcAuctionClient;
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount) 
        {
            var auction = await DB.Find<Auction>().OneAsync(auctionId);

            if(auction == null) 
            {
                auction = grpcAuctionClient.GetAuction(auctionId);

                if (auction == null) return BadRequest("Cannot bids on this auction at this time");
            }

            if(auction.Seller == User.Identity.Name) 
            {
                return BadRequest("You cannot bid on your own auction");
            }

            var bid = new Bid
            {
                Amount = amount,
                AuctionId = auctionId,
                Bidder = User.Identity.Name
            };

            if (auction.AuctionEnd < DateTime.UtcNow)
            {
                bid.BidStatus = BidStatus.Finished;
            }
            else 
            {
                var highestBid = await DB.Find<Bid>()
                    .Match(a => a.AuctionId == auctionId)
                    .Sort(b => b.Descending(x => x.Amount))
                    .ExecuteFirstAsync();

                if (highestBid != null && amount > highestBid.Amount || highestBid == null) 
                {
                    bid.BidStatus = amount > auction.ReservePrice
                        ? BidStatus.Accepted
                        : BidStatus.AcceptedBelowReserve;
                }

                if(highestBid != null && bid.Amount <= highestBid.Amount) 
                {
                    bid.BidStatus = BidStatus.TooLow;
                }
            }

            await DB.SaveAsync(bid);

            await publishEndpoint.Publish(mapper.Map<BidPlaced>(bid));

            return Ok(mapper.Map<BidDto>(bid));
        }


        [HttpGet("{auctionId}")]
        public async Task<ActionResult<List<BidDto>>> GetBidsForAuction(string auctionId) 
        {
            var bids = await DB.Find<Bid>()
                .Match(a => a.AuctionId == auctionId)
                .Sort(b => b.Descending(a => a.BidTime))
                .ExecuteAsync();

            return bids.Select(mapper.Map<BidDto>).ToList();
        }
    }
}
