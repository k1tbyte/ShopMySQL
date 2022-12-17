using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDB.Models
{
    public enum OrderStatus { paid, canceled, done };
    public enum DeliveryType { none, mail };
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Name { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public float TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public string OrderDeliveryType { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
