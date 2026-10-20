namespace OliveOilApi.Models;

public class OliveOilProduct
{
    public int Id { get ; set;}
    public string Name { get; set; }= string.Empty;
    public string Region { get; set; }= string.Empty;
    public decimal PricePerLiter { get; set; }
    public int QuantityInStock { get; set; }
    
}