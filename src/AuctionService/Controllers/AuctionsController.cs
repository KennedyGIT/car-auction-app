using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext context;
    private readonly IMapper mapper;
    private readonly IPublishEndpoint publishEndpoint;

    public AuctionsController(AuctionDbContext context, IMapper mapper, 
        IPublishEndpoint publishEndpoint)
    {
        this.context = context;
        this.mapper = mapper;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        var query = context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date)) 
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }
        
        return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);

        //TODO : add current user as seller

        auction.Seller = "test";

        context.Auctions.Add(auction);

        var newAuction = mapper.Map<AuctionDto>(auction);

        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

        var result = await context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, newAuction);
    }




    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

        var result = await context.SaveChangesAsync() > 0;

        if(result) return Ok();

        return BadRequest("Problem solving Changes");
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.Auctions.FindAsync(id);

        if (auction == null) return NotFound();

        context.Auctions.Remove(auction);

        await publishEndpoint.Publish(mapper.Map<AuctionDeleted>(auction));

        var result = await context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Could not update the database");
    }
}