using System.IO;
using System.Threading.Tasks;
using Application.Core.Services.Interfaces.OrderServices;
using Application.Dtos.BasketDtos;
using Domain.Models.BasketModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = Domain.Models.OrderModels.Order;

namespace API.Controllers.OrdersControllers
{
    [Produces("application/json")]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        //Key generated with Stripe CLI
        //Needs to be generated locally and placed in appsettings
        private readonly string _whSecret;
        private readonly string _publishableKey;
        private readonly ILogger<PaymentsController> _logger;
        public PaymentsController(
            IPaymentService paymentService,
            ILogger<PaymentsController> logger,
            IConfiguration config
            )
        {
            _logger = logger;
            _paymentService = paymentService;
            _whSecret = config.GetSection("StripeSettings:WhSecret").Value;
            _publishableKey = config.GetSection("StripeSettings:PublishableKey").Value;
        }

        /// <summary>
        /// Posts a new paymentIntent to Stripe
        /// </summary>
        /// <remarks>
        /// Listen locally in cmd with the following command:
        ///     
        ///     stripe listen -f https://localhost:5001/api/payments/webhook -e payment_intent.succeeded,payment_intent.payment_failed
        /// Sample basket:
        ///
        ///     basket1
        ///</remarks>
        /// <response code="200">Returns the basket with the added payment intent</response>
        /// <response code="400">Returns if the payment intent could not be added to basket</response>
        /// <response code="401">Returns if user is not logged in</response>
        [Authorize]
        [HttpPost("{basketId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest("Basket could not be updated.");

            return Ok(Mapper.Map<CustomerBasket, CustomerBasketDto>(basket));
        }

        /// <summary>
        /// Gets the publishable key fro Stripe
        /// </summary>
        /// <response code="200">Returns the publishable key</response>
        /// <response code="401">Returns if user is not logged in</response>
        /// <response code="404">Returns if publishable key is not found</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetPublishableKey()
        {
            if (_publishableKey != null)
            {
                return Ok(_publishableKey);
            }
            else
            {
                return NotFound("Publishable key for Stripe not found.");
            }
        }

        /// <summary>
        /// Creates a webhook to Stripe 
        /// </summary>
        /// <remarks>
        /// Listen locally in cmd with the following command:
        ///     
        ///     stripe listen -f https://localhost:5001/api/payments/webhook -e payment_intent.succeeded,payment_intent.payment_failed
        ///
        /// Can only be tested in the app.
        ///</remarks>
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: {Id}", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order updated to payment received: {Id}", order.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: {Id}", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    _logger.LogInformation("Payment Failed: {Id}", order.Id);
                    break;
            }

            return new EmptyResult();
        }

    }
}