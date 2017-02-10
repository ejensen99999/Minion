using System;
using System.Collections.Generic;

namespace Minion.TestConsole.Presentation
{
    public class PresentationTest
    {

        public static void Run(int iterations)
        {
            var start = DateTime.Now;




            Console.WriteLine("Total time: " + DateTime.Now.Subtract(start));
            Console.ReadLine();
        }

        public static void Compare<TData>(List<TData> samples, PresentationPerspective cube, List<TData> list)
        {
            //Cube Test
            Timer.Go("Dimension 1 perspective", samples, x =>
            {


                return 1;
            });

            //List Test
            Timer.Go("Dimension 1 list search", samples, x =>
            {


                return 1;
            });

        }
    }
}
