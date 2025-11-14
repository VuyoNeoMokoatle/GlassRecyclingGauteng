using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buyalot
{
    public class CartItem
    {
        [PrimaryKey, AutoIncrement]
        public int CartItemId { get; set; }
        public int UserId { get; set; }     
        public int ProductId { get; set; }  
        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal Price { get; set; }
    }
}
