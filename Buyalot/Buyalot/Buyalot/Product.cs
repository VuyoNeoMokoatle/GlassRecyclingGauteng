using SQLite;

namespace Buyalot
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int ProductId { get; set; }

        public int UserId { get; set; }

        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; } // Stock quantity
        public string ImageUrl { get; internal set; }


    }
}
