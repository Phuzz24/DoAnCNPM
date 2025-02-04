namespace DoAnCNPM.Models
{
    public class PaymentCallbackRequest
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public PaymentCallbackData Data { get; set; }
        public string Signature { get; set; }
    }

    public class PaymentCallbackData
    {
        public string OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string Code { get; set; }
    }

}
