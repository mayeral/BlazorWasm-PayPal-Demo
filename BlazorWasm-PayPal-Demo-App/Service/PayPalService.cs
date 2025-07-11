using System.Net.Http.Json;

namespace BlazorWasm_PayPal_Demo_App.Service
{
    public class PayPalService
    {
        private readonly HttpClient _httpClient;

        public PayPalService(HttpClient httpClient)
        {
            _httpClient = httpClient;

#if DEBUG
            // if debugging, use http://localhost:7013
            _httpClient.BaseAddress = new Uri("http://localhost:7013/");
#endif
        }

        /// <summary>
        /// Creates a PayPal order
        /// </summary>
        /// <returns>Order creation response with order ID</returns>
        public async Task<OrderResponse> CreateOrderAsync(PaymentOrderRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/paypal/create-order", request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderResponse>();
        }

        /// <summary>
        /// Captures a PayPal order after approval
        /// </summary>
        /// <param name="orderId">The PayPal order ID to capture</param>
        /// <returns>Order capture response</returns>
        public async Task<CaptureResponse> CaptureOrderAsync(string orderId)
        {
            var request = new
            {
                OrderId = orderId
            };

            var response = await _httpClient.PostAsJsonAsync("api/paypal/capture-order", request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CaptureResponse>();
        }
    }

    // Response models
    public class OrderResponse
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
    }

    public class CaptureResponse
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
    }

    // Request models
    public class PaymentOrderRequest
    {
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }

    public class CaptureRequest
    {
        public string? OrderId { get; set; }
    }
}