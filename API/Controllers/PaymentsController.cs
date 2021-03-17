using System.IO;
using System.Threading.Tasks;
using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;

namespace API.Controllers
{
    [Produces("application/json")]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        //Key generated with Stripe CLI
        //Needs to be generated locally and replaced
        private readonly string _whSecret;
        private readonly ILogger<PaymentsController> _logger;
        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger, IConfiguration config)
        {
            _logger = logger;
            _paymentService = paymentService;
            _whSecret = config.GetSection("StripeSettings:WhSecret").Value;
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
        /// <response code="400">Returns if user is not logged in</response>
        [Authorize]
        [HttpPost("{basketId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null)
            {
                return BadRequest(new ApiResponse(400, "Problem with your basket"));
            }

            return basket;
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