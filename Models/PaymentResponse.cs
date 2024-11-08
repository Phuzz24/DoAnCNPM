using Newtonsoft.Json;

namespace DoAnCNPM.Models
{
        public class PaymentResponse
        {
            [JsonProperty("payment_url")]
            public string PaymentUrl { get; set; }  // URL để điều hướng người dùng đến trang thanh toán

            [JsonProperty("transaction_id")]
            public string TransactionId { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        // Các thuộc tính khác tùy thuộc vào yêu cầu của API
    }
