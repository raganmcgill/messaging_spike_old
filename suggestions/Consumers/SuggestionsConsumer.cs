using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using common_models;
using message_types;
using MassTransit;

namespace suggestions
{
    public class SuggestionsConsumer : IConsumer<IGetSuggestions>
    {
        public async Task Consume(ConsumeContext<IGetSuggestions> context)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortDateString()}] Handling suggestions for {context.Message.Prefix}");

            //Note : you can bind same list from database  
            List<Column> ObjList = new List<Column>()
            {
            
                new Column {Name="Latur" },
                new Column {Name="Mumbai" },
                new Column {Name="Pune" },
                new Column {Name="Delhi" },
                new Column {Name="Dehradun" },
                new Column {Name="Noida" },
                new Column {Name="New Delhi" }
            
            };
            //Searching records from list using LINQ query  
            var list = (from N in ObjList where N.Name.Contains(context.Message.Prefix) select new { N.Name });


            Console.WriteLine($"[{DateTime.Now.ToShortDateString()}] {list.Count()} suggestions found");

            await context.RespondAsync<IReturnSuggestions>(
                new
                {
                    Timestamp = DateTime.Now,
                    StatusCode = 300,
                    StatusText = Guid.NewGuid().ToString(),
                    Suggestions = list
                }
            );
        }
    }
}