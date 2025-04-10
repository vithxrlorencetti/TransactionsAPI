using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TransactionsAPI.DTOs;
using TransactionsAPI.Helpers;
using TransactionsAPI.Interfaces;

namespace TransactionsAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("user/{userId:int}")]
    [SwaggerOperation(Summary = "List a user's transactions with pagination", Description = "Returns user information and all his transactions with pagination")]
    [ProducesResponseType(typeof(PagedResult<UserTransactionsDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserTransactions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _transactionService.GetUserTransactionsAsync(userId, page, pageSize);
        return Ok(result);
    }

    [HttpPost("deposit")]
    [SwaggerOperation(Summary = "Create a deposit", Description = "Creates a deposit for a Receiver user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Deposit(DepositDTO dto)
    {
        var result = await _transactionService.CreateDepositAsync(dto.UserId, dto.Amount, dto.Description);

        return Ok(new
        {
            message = $"Deposit of {dto.Amount:C} for user {result} was successfully completed."
        });
    }

    [HttpPost("transfer")]
    [SwaggerOperation(Summary = "Create a transfer between users", Description = "Creates a transfer transaction between a Sender user and a Receiver user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Transfer(TransferDTO dto)
    {
        var result = await _transactionService.CreateTransferAsync(dto.SenderId, dto.ReceiverId, dto.Amount, dto.Description);

        return Ok(new
        {
            message = $"Transfer of {dto.Amount:C} from user {result.Item1} to user {result.Item2} was successfully completed."
        });
    }

    [HttpPost("revert/{transactionId:int}")]
    [SwaggerOperation(Summary = "Revert a transfer", Description = "Reverts a transfer made between two users before")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Revert(int transactionId)
    {
        await _transactionService.RevertTransferAsync(transactionId);

        return Ok(new
        {
            message = $"Transfer with transaction ID {transactionId} has been successfully reverted."
        });
    }

    [HttpGet("export/{userId:int}")]
    [SwaggerOperation(Summary = "Export filtered transactions to CSV", Description = "Exports transactions to CSV with filter (All, ByMonth, Last30days, Reversed)")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportCsv(int userId, [FromQuery] string filterType, [FromQuery] int? month = null, [FromQuery] int? year = null)
    {
        var csvBytes = await _transactionService.ExportTransactionsToCsvAsync(userId, filterType, month, year);
        return File(csvBytes, "text/csv", $"transactions_{userId}_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
