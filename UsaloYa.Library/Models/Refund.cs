using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsaloYa.Library.Models
{
    public partial class Refund
    {
        public int SaleId { get; set; }
        public int UserId { get; set; }
        public DateTime RefundDate { get; set; }
        public string RefundMethod { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string Reason { get; set; }
        public int Measure { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPriceRefund { get; set; }
        public decimal RefundAmount { get; set; }
        [JsonIgnore]
        public virtual Sale Sale { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
