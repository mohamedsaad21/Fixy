using Fixy.Application.Abstracts;
using Fixy.Application.Common.DTOs.Payment;
using Fixy.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// Verify HMAC signature according to Paymob documentation
    /// </summary>
    public bool VerifyHmac(IQueryCollection query, string receivedHmac)
    {
        try
        {
            // Build concatenated string in EXACT order per Paymob docs
            var concatenated =
                query["amount_cents"].ToString() +
                query["created_at"].ToString() +
                query["currency"].ToString() +
                query["error_occured"].ToString().ToLower() +
                query["has_parent_transaction"].ToString().ToLower() +
                query["id"].ToString() +
                query["integration_id"].ToString() +
                query["is_3d_secure"].ToString().ToLower() +
                query["is_auth"].ToString().ToLower() +
                query["is_capture"].ToString().ToLower() +
                query["is_refunded"].ToString().ToLower() +
                query["is_standalone_payment"].ToString().ToLower() +
                query["is_voided"].ToString().ToLower() +
                query["order"].ToString() +
                query["owner"].ToString() +
                query["pending"].ToString().ToLower() +
                query["source_data.pan"].ToString() +
                query["source_data.sub_type"].ToString() +
                query["source_data.type"].ToString() +
                query["success"].ToString().ToLower() +
                _paymobSetings.HMAC;

            _logger.LogDebug($"Concatenated string: {concatenated}");

            // Calculate HMAC SHA512
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_paymobSetings.HMAC));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

            _logger.LogInformation($"Calculated: {calculatedHmac}");
            _logger.LogInformation($"Received:   {receivedHmac.ToLower()}");

            var isValid = calculatedHmac.Equals(receivedHmac, StringComparison.OrdinalIgnoreCase);

            _logger.LogInformation(isValid ? "✅ HMAC VALID" : "❌ HMAC INVALID");
            _logger.LogInformation("======================================");

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HMAC error");
            return false;
        }
    }
}