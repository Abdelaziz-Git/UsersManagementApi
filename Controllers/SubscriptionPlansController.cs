using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.SubscriptionPlans;
using TailorSoftAPI.Interfaces.Services;

namespace TailorSoftAPI.Controllers
{
    /// <summary>
    /// API Controller for managing subscription plans
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/subscription-plans")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status403Forbidden)]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ILogger<SubscriptionPlansController> _logger;

        /// <summary>
        /// Initializes a new instance of the SubscriptionPlansController class
        /// </summary>
        /// <param name="subscriptionPlanService">The subscription plan service dependency</param>
        /// <param name="logger">The logger dependency</param>
        public SubscriptionPlansController(ISubscriptionPlanService subscriptionPlanService, ILogger<SubscriptionPlansController> logger)
        {
            _subscriptionPlanService = subscriptionPlanService ?? throw new ArgumentNullException(nameof(subscriptionPlanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new subscription plan
        /// </summary>
        /// <param name="dto">The subscription plan data to create</param>
        /// <returns>The ID of the created subscription plan</returns>
        /// <response code="201">Subscription plan created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateSubscriptionPlanDto dto)
        {
            var result = await _subscriptionPlanService.CreateAsync(dto);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { planId = result.Value }, new { PlanId = result.Value });
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Retrieves a subscription plan by its ID
        /// </summary>
        /// <param name="planId">The plan ID</param>
        /// <returns>The subscription plan information</returns>
        /// <response code="200">Subscription plan found and returned</response>
        /// <response code="404">Subscription plan not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{planId:guid}", Name = "GetSubscriptionPlanById")]
        [ProducesResponseType(typeof(SubscriptionPlanResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> GetById(Guid planId)
        {
            var result = await _subscriptionPlanService.GetByIdAsync(planId);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves a subscription plan by its name
        /// </summary>
        /// <param name="planName">The plan name</param>
        /// <returns>The subscription plan information</returns>
        /// <response code="200">Subscription plan found and returned</response>
        /// <response code="404">Subscription plan not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("by-name/{planName}")]
        [ProducesResponseType(typeof(SubscriptionPlanResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> GetByName(string planName)
        {
            var result = await _subscriptionPlanService.GetByNameAsync(planName);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Retrieves all subscription plans
        /// </summary>
        /// <param name="activeOnly">When true, returns only active plans (default: true)</param>
        /// <returns>List of subscription plans</returns>
        /// <response code="200">Subscription plans retrieved successfully</response>
        /// <response code="404">No subscription plans found</response>
        /// <response code="500">Internal server error</response>
        [EnableRateLimiting("public-endpoints")]
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(List<SubscriptionPlanResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<SubscriptionPlanResponseDto>>> GetAll([FromQuery] bool activeOnly = true)
        {
            var result = await _subscriptionPlanService.GetAllAsync(activeOnly);
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Updates a subscription plan
        /// </summary>
        /// <param name="planId">The plan ID</param>
        /// <param name="dto">The subscription plan data to update</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription plan updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{planId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(Guid planId, [FromBody] UpdateSubscriptionPlanDto dto)
        {
            var result = await _subscriptionPlanService.UpdateAsync(planId, dto);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Deletes a subscription plan
        /// </summary>
        /// <param name="planId">The plan ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Subscription plan deleted successfully</response>
        /// <response code="400">Delete operation failed</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{planId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(Guid planId)
        {
            var result = await _subscriptionPlanService.DeleteAsync(planId);
            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}