using System;
using System.IO;

namespace helpers
{
    public static class ConsoleAppHelper
    {
        public static void PrintHeader(string file)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(file);

                //Read the first line of text
                line = sr.ReadLine();

                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the lie to console window
                    Console.WriteLine(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                Console.WriteLine(string.Empty);

                //close the file
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException());
            }
            finally
            {
                ///Console.WriteLine("Executing finally block.");
            }
        }
    }
}
