using System.ComponentModel.DataAnnotations;

namespace FreakyTrader.Data.Entities
{
    public class Stock
    {
        public int StockId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public int MarketId { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }
        public int ExtId { get; set; }
    }
}
