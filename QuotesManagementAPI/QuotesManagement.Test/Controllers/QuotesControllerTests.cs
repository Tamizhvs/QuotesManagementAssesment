using Microsoft.AspNetCore.Mvc;
using Moq;
using QuotesManagement.API.Controllers;
using QuotesManagement.Domain.DTO;
using QuotesManagement.Domain.ResultSet;
using QuotesManagement.Domain.Services;
using Xunit;

namespace QuotesManagement.Test.Controllers
{
    public class QuotesControllerTests
    {
        private readonly Mock<IQuoteService> _mockQuoteService;
        private readonly QuotesController _controller;

        public QuotesControllerTests()
        {
            _mockQuoteService = new Mock<IQuoteService>();
            _controller = new QuotesController(_mockQuoteService.Object);
        }

        #region GetAllQuotes Tests

        [Fact]
        public async Task GetAllQuotes_ReturnsOk_WithQuotes()
        {
            // Arrange
            var quotes = new List<QuoteResult>
            {
                new QuoteResult { Id = 1, Author = "Author1", Quote = "Test Quote 1" },
                new QuoteResult { Id = 2, Author = "Author2", Quote = "Test Quote 2" }
            };
            _mockQuoteService.Setup(service => service.GetAllQuotesAsync()).ReturnsAsync(quotes);

            // Act
            var result = await _controller.GetAllQuotes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<QuoteResult>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAllQuotes_Returns500_WhenExceptionThrown()
        {
            // Arrange
            _mockQuoteService.Setup(service => service.GetAllQuotesAsync()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAllQuotes();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Database error", statusCodeResult.Value);
        }

        #endregion

        #region GetQuoteById Tests

        [Fact]
        public async Task GetQuoteById_ReturnsOk_WhenQuoteExists()
        {
            // Arrange
            var quote = new QuoteResult { Id = 1, Author = "Author1", Quote = "Test Quote" };
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync(quote);

            // Act
            var result = await _controller.GetQuoteById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<QuoteResult>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetQuoteById_ReturnsNotFound_WhenQuoteDoesNotExist()
        {
            // Arrange
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync((QuoteResult)null);

            // Act
            var result = await _controller.GetQuoteById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No quote found with ID = 1", notFoundResult.Value);
        }

        [Fact]
        public async Task GetQuoteById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.GetQuoteById(0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid ID", badRequestResult.Value);
        }

        #endregion

        #region AddQuotes Tests

        [Fact]
        public async Task AddQuotes_ReturnsCreatedAtAction_WhenValid()
        {
            // Arrange
            var quotes = new List<CreateQuoteDTO>
            {
                new CreateQuoteDTO { Author = "Author1", Quote = "Test Quote 1", Tags = new List<string> { "tag1" } }
            };

            // Act
            var result = await _controller.AddQuotes(quotes);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetAllQuotes), createdResult.ActionName);
        }

        [Fact]
        public async Task AddQuotes_ReturnsBadRequest_WhenNull()
        {
            // Act
            var result = await _controller.AddQuotes(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quote data cannot be null or empty.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddQuotes_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Author", "Author is required");

            // Act
            var result = await _controller.AddQuotes(new List<CreateQuoteDTO>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // If the error is based on the ModelState, expect SerializableError
            if (badRequestResult.Value is SerializableError modelStateErrors)
            {
                Assert.True(modelStateErrors.ContainsKey("Author"));
            }
            // If the error is a string, assert accordingly
            else if (badRequestResult.Value is string errorMessage)
            {
                Assert.Equal("Quote data cannot be null or empty.", errorMessage);
            }
        }

        [Fact]
        public async Task AddQuotes_Returns500_WhenExceptionThrown()
        {
            // Arrange
            _mockQuoteService.Setup(service => service.AddQuotesAsync(It.IsAny<IEnumerable<CreateQuoteDTO>>()))
                             .ThrowsAsync(new System.Exception("Database error"));

            var quotes = new List<CreateQuoteDTO> { new CreateQuoteDTO { Author = "Author", Quote = "Content" } };

            // Act
            var result = await _controller.AddQuotes(quotes);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Database error", statusCodeResult.Value);
        }

        #endregion

        #region UpdateQuote Tests

        [Fact]
        public async Task UpdateQuote_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var updateQuoteDTO = new UpdateQuoteDTO { Id = 1, Author = "Updated Author", Quote = "Updated Content" };
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync(new QuoteResult { Id = 1 });
            _mockQuoteService.Setup(service => service.UpdateQuoteAsync(updateQuoteDTO)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateQuote(updateQuoteDTO);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateQuote_ReturnsBadRequest_WhenDataIsNull()
        {
            // Act
            var result = await _controller.UpdateQuote(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid data provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateQuote_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var updateQuoteDTO = new UpdateQuoteDTO { Id = 0, Author = "Invalid", Quote = "Invalid" };

            // Act
            var result = await _controller.UpdateQuote(updateQuoteDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid data provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateQuote_ReturnsNotFound_WhenQuoteDoesNotExist()
        {
            // Arrange
            var updateQuoteDTO = new UpdateQuoteDTO { Id = 1, Author = "Updated Author", Quote = "Updated Content" };
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync((QuoteResult)null);

            // Act
            var result = await _controller.UpdateQuote(updateQuoteDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Unable to Update, No quote found with ID = {updateQuoteDTO.Id}", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateQuote_Returns500_WhenExceptionThrown()
        {
            // Arrange
            var updateQuoteDTO = new UpdateQuoteDTO { Id = 1, Author = "Updated Author", Quote = "Updated Content" };
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync(new QuoteResult { Id = 1 });
            _mockQuoteService.Setup(service => service.UpdateQuoteAsync(updateQuoteDTO)).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.UpdateQuote(updateQuoteDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Database error", statusCodeResult.Value);
        }

        #endregion
        #region Delete Tests

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync(new QuoteResult { Id = 1 });
            _mockQuoteService.Setup(service => service.DeleteQuoteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.Delete(0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid ID", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenQuoteDoesNotExist()
        {
            // Arrange
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync((QuoteResult)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No quote found with ID = 1", notFoundResult.Value);
        }

        [Fact]
        public async Task Delete_Returns500_WhenExceptionThrown()
        {
            // Arrange
            _mockQuoteService.Setup(service => service.GetQuoteByIdAsync(1)).ReturnsAsync(new QuoteResult { Id = 1 });
            _mockQuoteService.Setup(service => service.DeleteQuoteAsync(1)).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Database error", statusCodeResult.Value);
        }

        #endregion

        #region Search Tests

        [Fact]
        public async Task Search_ReturnsOk_WithMatchingQuotes()
        {
            // Arrange
            var searchResults = new List<QuoteResult>
            {
                new QuoteResult { Id = 1, Author = "Author1", Quote = "Content1" },
                new QuoteResult { Id = 2, Author = "Author2", Quote = "Content2" }
            };
            _mockQuoteService.Setup(service => service.SearchQuotesAsync("Author1", null, null)).ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search("Author1", null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<QuoteResult>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task Search_ReturnsBadRequest_WhenNoSearchParametersProvided()
        {
            // Act
            var result = await _controller.Search(null, null, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("At least one search parameter must be provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task Search_Returns500_WhenExceptionThrown()
        {
            // Arrange
            _mockQuoteService.Setup(service => service.SearchQuotesAsync(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()))
                             .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.Search("Author1", null, null);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Database error", statusCodeResult.Value);
        }

        #endregion

    }
}
