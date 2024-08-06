using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests
{
    public class AuctionControllerTests
    {
        private readonly Mock<IAuctionRepository> auctionRepository;
        private readonly Mock<IPublishEndpoint> publishEndpoint;
        private readonly Fixture fixture;
        private readonly AuctionsController controller;
        private readonly IMapper mapper;

        public AuctionControllerTests()
        {
            fixture = new Fixture();
            auctionRepository = new Mock<IAuctionRepository>();
            publishEndpoint = new Mock<IPublishEndpoint>();

            var mockMapper = new MapperConfiguration(mc =>
            {
                mc.AddMaps(typeof(MappingProfiles).Assembly);

            }).CreateMapper().ConfigurationProvider;

            var mapper = new Mapper(mockMapper);

            controller = new AuctionsController(auctionRepository.Object, mapper, publishEndpoint.Object) 
            {
                ControllerContext = new ControllerContext() 
                {
                    HttpContext = new DefaultHttpContext 
                    {
                        User = Helpers.GetClaimsPrincipal()
                    }
                }
            };

        }

        [Fact]
        public async Task GetAuctions_WithNoParams_Return10Auctions() 
        {
            //Arrange
            var auctions = fixture.CreateMany<AuctionDto>(10).ToList();
            auctionRepository.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

            //Act
            var result = await controller.GetAllAuctions(null);

            //Assert
            Assert.Equal(10, result.Value.Count);
            Assert.IsType<ActionResult<List<AuctionDto>>>(result);
        }


        [Fact]
        public async Task GetAuctionById_WithValidGuid_ReturnAuctionDto()
        {
            //Arrange
            var auctionDto = fixture.Create<AuctionDto>();
            auctionRepository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auctionDto);

            //Act
            var result = await controller.GetAuctionById(auctionDto.Id);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtAction()
        {
            //Arrange
            var createAuctionDto = fixture.Create<CreateAuctionDto>();

            auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
            auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            //Act
            var result = await controller.CreateAuction(createAuctionDto);

            var createdResult = result.Result as CreatedAtActionResult;


            //Assert
            Assert.NotNull(createdResult);
            Assert.Equal("GetAuctionById", createdResult.ActionName);
            Assert.IsType<AuctionDto>(createdResult.Value);
        }



        [Fact]
        public async Task CreateAuction_FailedCreate_Returns400BadRequest() 
        {
            //Arrange
            var createAuctionDto = fixture.Create<CreateAuctionDto>();

            auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
            auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

            //Act
            var result = await controller.CreateAuction(createAuctionDto);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public async Task UpdateAuction_WithUpdateAuctionDto_ReturnsOkResponse() 
        {
            //Arrange
            var updateAuctionDto = fixture.Create<UpdateAuctionDto>();  
            var auction = fixture.Build<Auction>().Without(x => x.Item).Create();
            auction.Item = fixture.Build<Item>().Without(x => x.Auction).Create();
            auction.Seller = Helpers.GetClaimsPrincipal().Identity.Name;
            

            auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>())).ReturnsAsync(auction);
            auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            //Act
            var result = await controller.UpdateAuction(auction.Id, updateAuctionDto);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateAuction_WithInvalidUser_Returns403Forbid()
        {
            //Arrange
            var updateAuctionDto = fixture.Create<UpdateAuctionDto>();
            var auction = fixture.Build<Auction>().Without(x => x.Item).Create();
            auction.Item = fixture.Build<Item>().Without(x => x.Auction).Create();


            auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>())).ReturnsAsync(auction);
            auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            //Act
            var result = await controller.UpdateAuction(auction.Id, updateAuctionDto);

            //Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
        {
            //Arrange
            var updateAuctionDto = fixture.Create<UpdateAuctionDto>();
            Auction auction = null;

            auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>())).ReturnsAsync(auction);
            
            //Act
            var result = await controller.UpdateAuction(It.IsAny<Guid>(), updateAuctionDto);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAuction_WithValidUser_ReturnsOkResponse()
        {
            //Arrange
            var auction = fixture.Build<Auction>().Without(x => x.Item).Create();
            auction.Item = fixture.Build<Item>().Without(x => x.Auction).Create();
            auction.Seller = Helpers.GetClaimsPrincipal().Identity.Name;


            auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>())).ReturnsAsync(auction);
            auctionRepository.Setup(repo => repo.RemoveAuction(auction));
            auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            //Act
            var result = await controller.DeleteAuction(It.IsAny<Guid>());

            //Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
