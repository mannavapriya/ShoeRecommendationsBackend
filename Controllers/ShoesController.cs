using ChoETL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using System.Collections.Specialized;

namespace ShoeRecommendationsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]

    public class ShoesController : ControllerBase
    {
        public ShoesController()
        {
            
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<List<ShoeModel>> GetShoes(SearchModel model)
        {
            List<ShoeModel> shoes = new List<ShoeModel>();

            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = ","
            };

            using var streamReader = new StreamReader("Assets/dataset.csv");
            using var csvReader = new CsvReader(streamReader, csvConfig);


            int count = 1;
            while (csvReader.Read() && !String.IsNullOrEmpty(csvReader.GetField(10)))
            {
               if(count > 1) 
               {
                    //remove duplicates
                    if(!shoes.Select(p => p.id).ToList().Contains(csvReader.GetField(0))) 
                    {
                        ShoeModel shoe = new ShoeModel()
                        {
                            id = csvReader.GetField(0),
                            brand = csvReader.GetField(4),
                            categories = csvReader.GetField(5),
                            color = csvReader.GetField(7),
                            imageURL = csvReader.GetField(10)
                        };
                        shoes.Add(shoe);
                    }                    
               }
               count++;
            }

            if(!String.IsNullOrEmpty(model.color))
                shoes = shoes.Where(p => p.color.ToLower().Contains(model.color.ToLower())).ToList();

            if (model.heels)
                shoes = shoes.Where(p => p.categories.ToLower().Contains("heels")).ToList();
            if (model.sandals)
                shoes = shoes.Where(p => p.categories.ToLower().Contains("sandals")).ToList();
            if (model.shoes)
                shoes = shoes.Where(p => p.categories.ToLower().Contains("shoes")).ToList();
            if (model.slippers)
                shoes = shoes.Where(p => p.categories.ToLower().Contains("slippers")).ToList();
            if (model.boots)
                shoes = shoes.Where(p => p.categories.ToLower().Contains("boots")).ToList();

            return shoes;
        }
    }

    public class ShoeModel 
    {
        public string id { get; set; }
        public string brand { get;set; }
        public string categories { get; set; }
        public string color { get; set; }
        public string imageURL { get; set; }   
        
        public ShoeModel()
        {

        }
    }

    public class SearchModel 
    {
        public string color { get; set; }
        public bool heels { get;set; }
        public bool sandals { get;set; }
        public bool shoes { get;set; }
        public bool slippers { get;set; }
        public bool boots { get;set; }
    }
}
