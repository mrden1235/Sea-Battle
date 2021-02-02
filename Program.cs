using System;
namespace seaBattle
{
   public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Чемпионаты по 1000 игр");
            Console.WriteLine();
            var r = new Random();
            Console.WriteLine("Cтратегия 1 против стратегии 1");
            int v1 = 0, v11 = 0, v12 = 0;
            for (var i = 0; i < 1000; ++i)
            {
                var pf1 = new PlayingField(10, 10);
                var pf2 = new PlayingField(10, 10);
                pf1.StartRandomField(r);
                pf2.StartRandomField(r);
                while (pf1.Live() && pf2.Live())
                {
                    pf1.RandomShot(r);
                    pf2.RandomShot(r);
                }

                if (!pf1.Live() && !pf2.Live()) ++v1;
                if (pf1.Live() && !pf2.Live()) ++v11;
                if (!pf1.Live() && pf2.Live()) ++v12;
            }
            Console.WriteLine("Ничьих: " + v1);
            Console.WriteLine("Выиграл первый: " + v11);
            Console.WriteLine("Выиграл второй: " + v12);
            Console.WriteLine();
            Console.WriteLine("Cтратегия 2 против стратегии 2");
            int v2 = 0, v21 = 0, v22 = 0;
            for (var i = 0; i < 1000; ++i)
            {
                var pf1 = new PlayingField(10, 10);
                var pf2 = new PlayingField(10, 10);
                pf1.StartRandomField(r);
                pf2.StartRandomField(r);
                while (pf1.Live() && pf2.Live())
                {
                    pf1.SmartRandomShot(r);
                    pf2.SmartRandomShot(r);
                }
                if (!pf1.Live() && !pf2.Live()) ++v2;
                if (pf1.Live() && !pf2.Live()) ++v21;
                if (!pf1.Live() && pf2.Live()) ++v22;
            }
            Console.WriteLine("Ничьих: " + v2);
            Console.WriteLine("Выиграл первый: " + v21);
            Console.WriteLine("Выиграл второй: " + v22);
            Console.WriteLine();
            Console.WriteLine("Cтратегия 1 против стратегии 2");
                int v3 = 0, v31 = 0, v32 = 0;
                for (var i = 0; i < 1000; ++i)
                {
                    var pf1 = new PlayingField(10, 10);
                    var pf2 = new PlayingField(10, 10);
                    pf1.StartRandomField(r);
                    pf2.StartRandomField(r);
                    while (pf1.Live() && pf2.Live())
                    {
                        pf1.SmartRandomShot(r);
                        pf2.RandomShot(r);
                    }

                    if (!pf1.Live() && !pf2.Live()) ++v3;
                    if (pf1.Live() && !pf2.Live()) ++v31;
                    if (!pf1.Live() && pf2.Live()) ++v32;
                }
                Console.WriteLine("Ничьих: " + v3);
                Console.WriteLine("Выиграл первый: " + v31);
                Console.WriteLine("Выиграл второй: " + v32);
        }
    }
}