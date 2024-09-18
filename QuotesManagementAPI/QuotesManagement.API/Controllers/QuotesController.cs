using Microsoft.AspNetCore.Mvc;
using QuotesManagement.Domain.DTO;
using QuotesManagement.Domain.ResultSet;
using QuotesManagement.Domain.Services;

namespace QuotesManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuoteResult>>> GetAllQuotes()
        {
            try
            {
                var quotes = await _quoteService.GetAllQuotesAsync();
                return Ok(quotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuoteResult>> GetQuoteById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                var quote = await _quoteService.GetQuoteByIdAsync(id);
                if (quote == null)
                    return NotFound($"No quote found with ID = {id}");

                return Ok(quote);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddQuotes([FromBody] IEnumerable<CreateQuoteDTO> createQuoteDTOs)
        {
            if (createQuoteDTOs == null || !createQuoteDTOs.Any())
            {
                return BadRequest("Quote data cannot be null or empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _quoteService.AddQuotesAsync(createQuoteDTOs);
                return CreatedAtAction(nameof(GetAllQuotes), null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuote([FromBody] UpdateQuoteDTO updateQuoteDTO)
        {
            if (updateQuoteDTO == null || updateQuoteDTO.Id <= 0)
            {
                return BadRequest("Invalid data provided.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var quote = await _quoteService.GetQuoteByIdAsync(updateQuoteDTO.Id);
                if (quote == null)
                {
                    return NotFound($"Unable to Update, No quote found with ID = {updateQuoteDTO.Id}");
                }

                await _quoteService.UpdateQuoteAsync(updateQuoteDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            try
            {
                var quote = await _quoteService.GetQuoteByIdAsync(id);
                if (quote == null)
                {
                    return NotFound($"No quote found with ID = {id}");
                }

                await _quoteService.DeleteQuoteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<QuoteResult>>> Search([FromQuery] string? author, [FromQuery] List<string>? tags, [FromQuery] string? quoteContent)
        {
            if (string.IsNullOrEmpty(author) && (tags == null || !tags.Any()) && string.IsNullOrEmpty(quoteContent))
            {
                return BadRequest("At least one search parameter must be provided.");
            }

            try
            {
                var quotes = await _quoteService.SearchQuotesAsync(author, tags, quoteContent);
                return Ok(quotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
