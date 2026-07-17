using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.UserSubscriptions;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing user subscriptions
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/user-subscriptions")]
    [Produces("application/json")]
    public class UserSubscriptionsController : ControllerBase
    {
        private readonly IUserSubscriptionService _service;
        private readonly ILogger<UserSubscriptionsController> _logger;

        /// <summary>
        /// Initializes a new instance of the UserSubscriptionsController class
        /// </summary>
        /// <param name="service">The user subscription service dependency</param>
        /// <param name="logger">The logger dependency</param>
        public UserSubscriptionsController(IUserSubscriptionService service, ILogger<UserSubscriptionsController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new user subscription
        /// </summary>
        /// <param name="dto">The subscription data to create</param>
        /// <returns>The ID of the created subscription</returns>
        /// <response code="201">Subscription created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserSubscriptionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { subscriptionId = result.Value }, new { SubscriptionId = result.Value });
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves a subscription by its ID
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <returns>The subscription information</returns>
        /// <response code="200">Subscription found and returned</response>
        /// <response code="404">Subscription not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{subscriptionId:guid}", Name = "GetUserSubscriptionById")]
        [ProducesResponseType(typeof(UserSubscriptionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserSubscriptionResponseDto>> GetById(Guid subscriptionId)
        {
            var result = await _service.GetByIdAsync(subscriptionId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all subscriptions for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of subscriptions belonging to the user</returns>
        /// <response code="200">Subscriptions found and returned</response>
        /// <response code="404">No subscriptions found for the user</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-user/{userId:guid}")]
        [ProducesResponseType(typeof(List<UserSubscriptionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserSubscriptionResponseDto>>> GetByUserId(Guid userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all subscriptions for a specific plan
        /// </summary>
        /// <param name="planId">The plan ID</param>
        /// <param name="status">Optional status filter (Trial, Active, Expired, Cancelled, PastDue)</param>
        /// <returns>List of subscriptions for the given plan</returns>
        /// <response code="200">Subscriptions found and returned</response>
        /// <response code="404">No subscriptions found for the plan</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-plan/{planId:guid}")]
        [ProducesResponseType(typeof(List<UserSubscriptionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserSubscriptionResponseDto>>> GetByPlanId(Guid planId, [FromQuery] string? status)
        {
            var result = await _service.GetByPlanIdAsync(planId, status);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all subscriptions
        /// </summary>
        /// <param name="status">Optional status filter (Trial, Active, Expired, Cancelled, PastDue)</param>
        /// <returns>List of all subscriptions</returns>
        /// <response code="200">Subscriptions retrieved successfully</response>
        /// <response code="404">No subscriptions found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserSubscriptionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserSubscriptionResponseDto>>> GetAll([FromQuery] string? status)
        {
            var result = await _service.GetAllAsync(status);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Performs a general-purpose partial update on a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <param name="dto">Fields to update; null fields are left unchanged</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{subscriptionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(Guid subscriptionId, [FromBody] UpdateUserSubscriptionDto dto)
        {
            var result = await _service.UpdateAsync(subscriptionId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Hard-deletes a subscription record (GDPR / data-removal use only)
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription deleted successfully</response>
        /// <response code="404">Subscription not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{subscriptionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(Guid subscriptionId)
        {
            var result = await _service.DeleteAsync(subscriptionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        

        /// <summary>
        /// Activates a subscription (moves Trial / Expired / PastDue → Active)
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <param name="dto">NextBillingDate and optional EndDate</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription activated successfully</response>
        /// <response code="400">Subscription is not eligible for activation</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{subscriptionId:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Activate(Guid subscriptionId, [FromBody] ActivateUserSubscriptionDto dto)
        {
            var result = await _service.ActivateAsync(subscriptionId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Cancels a subscription immediately
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription cancelled successfully</response>
        /// <response code="400">Subscription is already cancelled</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{subscriptionId:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Cancel(Guid subscriptionId)
        {
            var result = await _service.CancelAsync(subscriptionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Upgrades or downgrades the plan of a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <param name="dto">New plan details</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Plan changed successfully</response>
        /// <response code="400">Invalid request or subscription is cancelled</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{subscriptionId:guid}/change-plan")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePlan(Guid subscriptionId, [FromBody] ChangePlanDto dto)
        {
            var result = await _service.ChangePlanAsync(subscriptionId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Renews a subscription after a successful payment
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <param name="dto">New billing dates and optional updated amount</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription renewed successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{subscriptionId:guid}/renew")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Renew(Guid subscriptionId, [FromBody] RenewUserSubscriptionDto dto)
        {
            var result = await _service.RenewAsync(subscriptionId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Toggles the AutoRenew flag on a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <param name="dto">The desired AutoRenew state</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">AutoRenew toggled successfully</response>
        /// <response code="400">Subscription is cancelled</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{subscriptionId:guid}/auto-renew")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ToggleAutoRenew(Guid subscriptionId, [FromBody] ToggleAutoRenewDto dto)
        {
            var result = await _service.ToggleAutoRenewAsync(subscriptionId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Marks a subscription as Expired (nightly job endpoint)
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription marked as expired</response>
        /// <response code="400">Subscription is not eligible to be marked expired</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{subscriptionId:guid}/mark-expired")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MarkExpired(Guid subscriptionId)
        {
            var result = await _service.MarkExpiredAsync(subscriptionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Marks a subscription as PastDue after a failed billing attempt
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription marked as past due</response>
        /// <response code="400">Subscription must be in Active status</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{subscriptionId:guid}/mark-past-due")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MarkPastDue(Guid subscriptionId)
        {
            var result = await _service.MarkPastDueAsync(subscriptionId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }


        /// <summary>
        /// Checks whether a user has an active or trial subscription
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Boolean indicating subscription activity</returns>
        /// <response code="200">Check completed</response>
        /// <response code="400">Invalid user ID</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("is-active/{userId:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> IsActive(Guid userId)
        {
            var result = await _service.IsActiveAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Returns active auto-renew subscriptions due for billing on or before the given date
        /// </summary>
        /// <param name="asOfDate">Cut-off date (defaults to UTC now when omitted)</param>
        /// <returns>List of subscriptions due for billing</returns>
        /// <response code="200">Due-billing list returned</response>
        /// <response code="404">No subscriptions due for billing</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("due-billing")]
        [ProducesResponseType(typeof(List<DueBillingResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DueBillingResponseDto>>> GetDueBilling([FromQuery] DateTime? asOfDate)
        {
            var result = await _service.GetDueBillingAsync(asOfDate);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Returns subscriptions whose EndDate has passed but are not yet marked Expired
        /// </summary>
        /// <param name="asOfDate">Cut-off date (defaults to UTC now when omitted)</param>
        /// <returns>List of expired subscriptions awaiting cleanup</returns>
        /// <response code="200">Expired list returned</response>
        /// <response code="404">No expired subscriptions found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("expired")]
        [ProducesResponseType(typeof(List<ExpiredSubscriptionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ExpiredSubscriptionResponseDto>>> GetExpired([FromQuery] DateTime? asOfDate)
        {
            var result = await _service.GetExpiredAsync(asOfDate);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Returns trials whose TrialEndDate has passed and are still in Trial status
        /// </summary>
        /// <param name="asOfDate">Cut-off date (defaults to UTC now when omitted)</param>
        /// <returns>List of expired trial subscriptions</returns>
        /// <response code="200">Expired trials returned</response>
        /// <response code="404">No expired trials found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("expired-trials")]
        [ProducesResponseType(typeof(List<ExpiredTrialResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ExpiredTrialResponseDto>>> GetExpiredTrials([FromQuery] DateTime? asOfDate)
        {
            var result = await _service.GetExpiredTrialsAsync(asOfDate);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Returns aggregate subscription counts and revenue grouped by status
        /// </summary>
        /// <returns>List of stats rows, one per subscription status</returns>
        /// <response code="200">Stats returned successfully</response>
        /// <response code="404">No subscription data available</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(List<SubscriptionStatResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<SubscriptionStatResponseDto>>> GetStats()
        {
            var result = await _service.GetStatsAsync();
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Returns active auto-renew subscriptions billing within the next N days
        /// </summary>
        /// <param name="daysAhead">Look-ahead window in days (default 7)</param>
        /// <returns>List of upcoming billings enriched with user and plan info</returns>
        /// <response code="200">Upcoming billings returned</response>
        /// <response code="404">No upcoming billings found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("upcoming-billings")]
        [ProducesResponseType(typeof(List<UpcomingBillingResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UpcomingBillingResponseDto>>> GetUpcomingBillings([FromQuery] int daysAhead = 7)
        {
            var result = await _service.GetUpcomingBillingsAsync(daysAhead);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }
    }
}
