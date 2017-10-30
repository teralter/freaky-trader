using System;

namespace FreakyTrader.Data.DTO
{
    public class CandleDTO
    {
        public DateTime RepDate { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
    }
}
