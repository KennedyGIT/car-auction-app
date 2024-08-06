using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionRepository auctionRepository;
    private readonly IMapper mapper;
    private readonly IPublishEndpoint publishEndpoint;

    public AuctionsController(
        IAuctionRepository auctionRepository,
        IMapper mapper, 
        IPublishEndpoint publishEndpoint)
    {
        this.auctionRepository = auctionRepository;
        this.mapper = mapper;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
       return await auctionRepository.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await auctionRepository.GetAuctionEntityById(id);

        if(auction == null) return NotFound();

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);

        auction.Seller = User.Identity.Name;

        auctionRepository.AddAuction(auction);

        var newAuction = mapper.Map<AuctionDto>(auction);

        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

        var result = await auctionRepository.SaveChangesAsync();

        if(!result) return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, newAuction);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await auctionRepository.GetAuctionEntityById(id); 

        if(auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

        var result = await auctionRepository.SaveChangesAsync();

        if (result) return Ok();

        return BadRequest("Problem solving Changes");
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await auctionRepository.GetAuctionEntityById(id);

        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        auctionRepository.RemoveAuction(auction);

        await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString()});

        var result = await auctionRepository.SaveChangesAsync();

        if (result) return Ok();

        return BadRequest("Could not update the database");
    }
}