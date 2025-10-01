using System.ComponentModel;
using EShop.Models;
using ModelContextProtocol.Server;

namespace EShop.Tools;

[McpServerToolType]
public class ShopTool
{
    [McpServerTool, Description("Gets available products.")]
    public List<Product> GetAvailableProducts() =>
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200 },
        new Product { Id = 2, Name = "Headphones", Price = 150 },
        new Product { Id = 3, Name = "Keyboard", Price = 80 }
    ];
}

