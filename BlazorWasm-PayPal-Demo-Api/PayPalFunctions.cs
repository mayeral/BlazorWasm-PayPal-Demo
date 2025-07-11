using BlazorWasm_PayPal_Demo_App.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Net;


namespace BlazorWasm_PayPal_Demo_Api;

public class PayPalFunctions
{
    private readonly ILogger<PayPalFunctions> _logger;

    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly bool _useSandbox;

    public PayPalFunctions(ILogger<PayPalFunctions> logger, IConfiguration configuration)
    {
        _logger = logger;
        _clientId = configuration["PayPal:ClientId"] ?? throw new ArgumentNullException("PayPal:ClientId");
        _clientSecret = configuration["PayPal:ClientSecret"] ?? throw new ArgumentNullException("PayPal:ClientSecret");
        _useSandbox = bool.Parse(configuration["PayPal:UseSandbox"] ?? "true");
    }


    /// <summary>
    /// Create order function to start the payment
    /// </summary>
    /// <param name="req"></param>
    [Function("CreateOrder")]
    public async Task<HttpResponseData> CreateOrderAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paypal/create-order")] HttpRequestData req)
    {
        _logger.LogInformation("Create Order");

        // Read request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
        var orderRequest = JsonConvert.DeserializeObject<PaymentOrderRequest>(requestBody);

        // Create the order request
        var paypalOrderRequest = new OrdersCreateRequest();
        paypalOrderRequest.Prefer("return=representation");

        // set request body
        paypalOrderRequest.RequestBody(
            new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = orderRequest?.Currency,
                            Value = orderRequest?.Amount.ToString()
                        },
                        Description = orderRequest?.Description
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = orderRequest?.ReturnUrl,
                    CancelUrl = orderRequest?.CancelUrl
                }
            });

        // get paypal client
        PayPalHttpClient client = GetPayPalClient();    

        try
        {
            // execute the request
            var response = await client.Execute(paypalOrderRequest).ConfigureAwait(false);
            var paypalOrder = response.Result<Order>();

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);

            // return the order ID
            await httpResponse.WriteAsJsonAsync(new { id = paypalOrder.Id }).ConfigureAwait(false);
            return httpResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating PayPal order");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = ex.Message }).ConfigureAwait(false);
            return errorResponse;
        }
    }


    /// <summary>
    /// Capture order function to complete the payment
    /// </summary>
    [Function("CaptureOrder")]
public async Task<HttpResponseData> CaptureOrderAsync(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paypal/capture-order")] HttpRequestData req)
{
    _logger.LogInformation("Capture Order");

    // Read request body
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
    var captureRequest = JsonConvert.DeserializeObject<CaptureRequest>(requestBody);

    // Validate order ID
    if (string.IsNullOrEmpty(captureRequest?.OrderId))
    {
        var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
        await errorResponse.WriteStringAsync("Order ID is required").ConfigureAwait(false);
        return errorResponse;
    }

    // Create capture request
    var request = new OrdersCaptureRequest(captureRequest.OrderId);
        // Set the prefer header for response representation
        request.Prefer("return=representation");
        request.RequestBody(new OrderActionRequest());

        // Get PayPal client
        PayPalHttpClient client = GetPayPalClient();

    try
    {
        var response = await client.Execute(request).ConfigureAwait(false);
        var capturedOrder = response.Result<Order>();

        var httpResponse = req.CreateResponse(HttpStatusCode.OK);
        httpResponse.Headers.Add("Content-Type", "application/json");
        
        await httpResponse.WriteStringAsync(JsonConvert.SerializeObject(new
        {
            id = capturedOrder.Id,
            status = capturedOrder.Status
        })).ConfigureAwait(false);

        return httpResponse;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error capturing PayPal order");
        var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
        errorResponse.Headers.Add("Content-Type", "application/json");
        
        await errorResponse.WriteStringAsync(JsonConvert.SerializeObject(new { 
            error = ex.Message,
            details = ex.ToString()
        })).ConfigureAwait(false);
        
        return errorResponse;
    }
}


    private PayPalHttpClient GetPayPalClient()
    {
        PayPalEnvironment environment = _useSandbox ? new SandboxEnvironment(_clientId, _clientSecret) : new LiveEnvironment(_clientId, _clientSecret);
        var payPalHttpClient = new PayPalHttpClient(environment);
        return payPalHttpClient;
    }
}