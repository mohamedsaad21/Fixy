using Fixy.Application.Abstracts;
using Fixy.Application.Common.DTOs.Payment;
using Fixy.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class PaymobService : IPaymobService
{
    private readonly HttpClient _httpClient;
    private readonly PaymobSettings _paymobSetings;
    private readonly ILogger<PaymobService> _logger;
    private string _cachedToken;
    private DateTime _tokenExpiry;

    public PaymobService(HttpClient httpClient, PaymobSettings paymobSetings, ILogger<PaymobService> logger)
    {
        _httpClient = httpClient;
        _paymobSetings = paymobSetings;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(_paymobSetings.BaseUrl);
    }

    public async Task<string> GetAuthTokenAsync()
    {
        // Check cache
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _cachedToken;
        }

        try
        {
            _logger.LogInformation("Requesting Paymob auth token");
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://accept.paymob.com/api/auth/tokens");
            request.Method = HttpMethod.Post;
            
            var requestBody = new { api_key = _paymobSetings.ApiKey };

            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);

            _cachedToken = result["token"].GetString();
            _tokenExpiry = DateTime.UtcNow.AddMinutes(50); // Token valid for 1 hour, cache for 50 min

            _logger.LogInformation("Paymob auth token obtained successfully");

            return _cachedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Paymob auth token");
            throw;
        }
    }

    public async Task<PaymentUrlResult> CreatePaymentUrlAsync(decimal amount, Guid bookingId, string customerName, string customerEmail, string customerPhone)
    {
        try
        {
            var authToken = await GetAuthTokenAsync();
            var amountCents = (int)(amount * 100);
            //var merchantOrderId = $"BK-{bookingId}";
            var merchantOrderId = $"BK-{bookingId}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            _logger.LogInformation($"Creating Paymob payment for booking {bookingId}, amount: {amount} EGP");
            //Exception
            // Step 1: Create Order
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://accept.paymob.com/api/ecommerce/orders");
            request.Method = HttpMethod.Post;
            var orderRequestBody = new
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = amountCents,
                currency = "EGP",
                merchant_order_id = merchantOrderId
            };
            var orderJson = JsonSerializer.Serialize(orderRequestBody);
            _logger.LogDebug($"Order request: {orderJson}");
            request.Content = new StringContent(orderJson, Encoding.UTF8, "application/json");
            var orderResponse = await _httpClient.SendAsync(request);

            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            _logger.LogDebug($"Order response ({orderResponse.StatusCode}): {orderContent}");
            if (!orderResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Order creation failed: {orderContent}");
                throw new Exception($"Paymob order failed: {orderContent}");
            }
            var order = JsonSerializer.Deserialize<JsonElement>(orderContent);
            var orderId = order.GetProperty("id").GetInt32();

            _logger.LogInformation($"Order created: {orderId}");

            // Step 2: Generate Payment Key
            var names = customerName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = names.FirstOrDefault() ?? customerName;
            var lastName = names.Length > 1 ? names.Last() : firstName;

            HttpRequestMessage keyRequest = new HttpRequestMessage();
            keyRequest.RequestUri = new Uri("https://accept.paymob.com/api/acceptance/payment_keys");
            keyRequest.Method = HttpMethod.Post;
            keyRequest.Content = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        auth_token = authToken,
                        amount_cents = amountCents,
                        expiration = 3600, // 1 hour
                        order_id = orderId,
                        billing_data = new
                        {
                            apartment = "NA",
                            email = customerEmail,
                            floor = "NA",
                            first_name = firstName,
                            last_name = lastName,
                            street = "NA",
                            building = "NA",
                            phone_number = customerPhone,
                            shipping_method = "NA",
                            postal_code = "NA",
                            city = "Cairo",
                            country = "EG",
                            state = "Cairo"
                        },
                        currency = "EGP",
                        integration_id = _paymobSetings.IntegrationIdCard
                    }),
                    Encoding.UTF8,
                    "application/json");
            var keyResponse = await _httpClient.SendAsync(keyRequest);

            keyResponse.EnsureSuccessStatusCode();

            var keyContent = await keyResponse.Content.ReadAsStringAsync();
            var key = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(keyContent);
            var paymentToken = key["token"].GetString();

            var paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_paymobSetings.IframeId}?payment_token={paymentToken}";
            _logger.LogInformation($"Payment URL generated for booking {bookingId}");

            return new PaymentUrlResult
            {
                PaymentUrl = paymentUrl,
                PaymobOrderId = orderId,            
                MerchantOrderId = merchantOrderId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating payment URL for booking {bookingId}");
            throw;
        }
    }


    public bool VerifyHmacSignature(PaymobCallbackDto callback)
    {
        try
        {
            var transaction = callback.obj;
            var order = transaction.order;

            var concatenatedString = string.Concat(
                transaction.amount_Cents.ToString(),
                transaction.created_At,
                transaction.currency,
                transaction.id.ToString(),
                order.merchant_Order_Id,
                order.id.ToString(),
                transaction.success.ToString().ToLower(),
                _paymobSetings.HmacSecret
            );

            _logger.LogDebug($"HMAC Concatenated String: {concatenatedString}");

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_paymobSetings.HmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
            var calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToUpper();

            _logger.LogInformation($"Calculated HMAC: {calculatedHmac}");
            _logger.LogInformation($"Received HMAC:   {callback.hmac}");

            var isValid = calculatedHmac.Equals(callback.hmac, StringComparison.OrdinalIgnoreCase);

            if (!isValid)
            {
                _logger.LogWarning("HMAC MISMATCH!");
                _logger.LogWarning($"Expected: {calculatedHmac}");
                _logger.LogWarning($"Received: {callback.hmac}");
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying HMAC");
            return false;
        }
    }
}