using Stripe.Checkout;

namespace TFG_Salty.Server.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<Session> CreateCheckoutSessionAsync();
        Task<ServiceResponse<bool>> FulfillOrderAsync(HttpRequest request);
    }
}
