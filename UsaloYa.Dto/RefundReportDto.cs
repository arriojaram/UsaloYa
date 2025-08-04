using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Dto
{
    public class RefundReportDto
    {
        public int SaleId { get; set; }
        public int? Folio { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime RefundDate { get; set; }
        public string RefundMethod { get; set; }
        public decimal RefundAmountTotal { get; set; }

        public List<RefundProductDto>? Products { get; set; }

    }

}
