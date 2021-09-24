using System;
using System.Collections.Generic;
using System.Linq;

namespace PromotionEngine
{
    class Program
    {
        public static void Main(string[] args)
        {
            var dict1 = new Dictionary<string,int>();
            dict1.Add("A", 3);
            var dict2 = new Dictionary<string,int>();
            dict2.Add("B", 2);
            var dict3 = new Dictionary<string,int>();
            dict3.Add("C", 1);
            dict3.Add("D", 1);
            List<Promotion> promList = new List<Promotion>() 
            {
                new Promotion(1, dict1, 130),
                new Promotion(2, dict2, 45),
                new Promotion(3, dict3, 30)
            };

            List<Order> orders = new List<Order>();
            Order o1 = new Order(1, new List<Product>(){ new Product("A"),new Product("B"),new Product("C") });
            Order o2 = new Order(2, new List<Product>(){ new Product("A"),new Product("A"),new Product("A"),new Product("A"),new Product("A"),
                        new Product("B"),new Product("B"),new Product("B"),new Product("B"),new Product("B"), new Product("C")});
            Order o3 = new Order(3, new List<Product>(){ new Product("A"),new Product("A"),new Product("A"),
                        new Product("B"),new Product("B"),new Product("B"),new Product("B"),new Product("B"), new Product("C"), new Product("D")});
            orders.Add(o1);
            orders.Add(o2);
            orders.Add(o3);
            foreach (var o in orders)
            {
                List<decimal> promoprices = promList.Select(prom => PromotionChecker.GetTotalPrice(o,prom)).ToList();
                decimal promoPrice = promoprices.Sum();
                Console.WriteLine($"OrderId: {o.OrderId} | Final price: {(promoPrice)}");
            }
        }
    }
    public class Promotion 
    {
        public int PromotionId { get; set; }
        public Dictionary<string,int> ProductInformation { get; set; }
        public decimal PromotionPrice { get; set; }
        public Promotion(int promoId, Dictionary<string,int> prodInfo, decimal promoprice)
        {
            PromotionId = promoId;
            ProductInformation = prodInfo;
            PromotionPrice = promoprice;
        }
    }
    public class Order
    {
        public int OrderId { get; set; }
        public List<Product> Products { get; set; }
        public Order(int orderId, List<Product> products)
        {
            OrderId = orderId;
            Products = products;
        }
    }
    public class Product
    {
        public string ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public Product(string id)
        {
            this.ProductId = id;
            switch (id)
            {
                case "A":
                this.ProductPrice = 50;
                break;
                case "B":
                this.ProductPrice = 30;
                break;
                case "C":
                this.ProductPrice = 20;
                break;
                case "D":
                this.ProductPrice = 15;
                break;
            }
        }
    }
    public static class PromotionChecker
    {
        public static decimal GetTotalPrice(Order ord, Promotion prom)
        {
            decimal d = 0;
            var countPP = ord.Products
                .GroupBy(x => x.ProductId)
                .Where(grp => prom.ProductInformation.Any(z => grp.Key == z.Key && grp.Count() >= z.Value))
                .Select(grp => grp.Count())
                .Sum();
            int productpc = prom.ProductInformation.Sum(kvp => kvp.Value);
            if(countPP >= productpc)
            {
                d += ((countPP / productpc) * prom.PromotionPrice) + (countPP%productpc) * ord.Products.First(p => p.ProductId == prom.ProductInformation.First().Key).ProductPrice;
            }
            else
            {
                d +=  ord.Products.First(p => p.ProductId == prom.ProductInformation.First().Key).ProductPrice;
            }
            return d;
        }
    }
}
