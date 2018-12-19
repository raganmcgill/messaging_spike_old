using System;
using System.Collections.Generic;
using System.IO;
using SqlFormatter;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TestHarness
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //var script = "SELECT * FROM TEST";
            var script = "SELECT p.Name AS ProductName, NonDiscountSales = (OrderQty * UnitPrice),Discounts = ((OrderQty * UnitPrice) * UnitPriceDiscount) FROM Production.Product AS p INNER JOIN Sales.SalesOrderDetail AS sod ON p.ProductID = sod.ProductID ORDER BY ProductName DESC; ";

            Console.WriteLine(script);

            Console.WriteLine("");
            Console.WriteLine("--FORMATTING--");
            Console.WriteLine("");
            
            var formattedSQL = NSQLFormatter.Formatter.Format(script);

            Console.WriteLine(formattedSQL);






//            string sql = "select col from (select a as col from something /*else*/) a";
//            var p = new TSql100Parser(true);
//            IList<ParseError> errors;
//
//            p.Parse(new StringReader(sql), out errors);
//
//
//            if (errors.Count == 0)
//                Console.Write("No Errors");
//            else
//                foreach (ParseError parseError in errors)
//                    Console.Write(parseError.Message);

            Console.ReadLine();
        }
    }
    
}
