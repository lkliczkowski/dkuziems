using System;
using MyData;

namespace SimplifiedFuzzyRules
{
    partial class SimplifiedFuzzyRules
    {
        public static void SimplifiedFuzzyRulesTest()
        {
            int option;
            while (!selectFunction(out option)) { }

            string inputFilename, resultFilename;
            int numberOfSections, pointNumber;
            /// <summary>
            /// Przedziały funkcji (dziedzina)
            /// </summary>
            double domainXFrom, domainXTo, domainYFrom, domainYTo;
            double alpha;

            switch (option)
            {
                case 1:
                default:
                    inputFilename = "funkcja1.dat";
                    resultFilename = "wyniki1_SimpleRules.dat";
                    
                    numberOfSections = 9;
                    setAmountOfSections(ref numberOfSections, option);

                    domainXFrom = 0;
                    domainXTo = 2 * Math.PI;

                    alpha = 10;
                    setAlpha(ref alpha);

                    //można wygenerować wartości lub wczytać dane - tutaj generowane do pliku, potem wczytane
                    if (generateSet())
                    {
                        pointNumber = 100;
                        setPointsNumber(ref pointNumber);
                        DataGenerator.Function1(inputFilename, pointNumber);
                    }
                    else
                        Console.WriteLine("Nazwa pliku z danymi powinna się nazywać: \"{0}\"", inputFilename);

                    Console.WriteLine("Kontynuuj... [dowolny przycisk]");
                    Console.ReadKey();

                    new SimplifiedRules2d(inputFilename, resultFilename, alpha, domainXFrom, domainXTo, numberOfSections).Run();

                    break;
                case 2:
                    inputFilename = "funkcja2.dat";
                    resultFilename = "wyniki2_SimpleRules.dat";
                    
                    numberOfSections = 22;
                    setAmountOfSections(ref numberOfSections, option);

                    domainXFrom = 0;
                    domainXTo = 1;

                    alpha = 10;
                    setAlpha(ref alpha);

                    //można wygenerować wartości lub wczytać dane - tutaj generowane do pliku, potem wczytane
                    if (generateSet())
                    {
                        pointNumber = 100;
                        setPointsNumber(ref pointNumber);
                        DataGenerator.Function2(inputFilename, pointNumber);
                    }
                    else
                        Console.WriteLine("Nazwa pliku z danymi powinna się nazywać: \"{0}\"", inputFilename);

                    Console.WriteLine("Kontynuuj... [dowolny przycisk]");
                    Console.ReadKey();

                    new SimplifiedRules2d(inputFilename, resultFilename, alpha, domainXFrom, domainXTo, numberOfSections).Run();
                    break;
                case 3:
                    inputFilename = "funkcja3.dat";
                    resultFilename = "wyniki3_SimpleRules.dat";
                    
                    numberOfSections = 64;
                    setAmountOfSections(ref numberOfSections, option);

                    domainXFrom = domainYFrom = -5;
                    domainXTo = domainYTo = 5;

                    alpha = 10;
                    setAlpha(ref alpha);

                    //można wygenerować wartości lub wczytać dane - tutaj generowane do pliku, potem wczytane
                    if (generateSet())
                    {
                        pointNumber = 1000;
                        setPointsNumber(ref pointNumber);
                        DataGenerator.Function3(inputFilename, pointNumber);
                    }
                    else
                        Console.WriteLine("Nazwa pliku z danymi powinna się nazywać: \"{0}\"", inputFilename);

                    Console.WriteLine("Kontynuuj... [dowolny przycisk]");
                    Console.ReadKey();

                    new SimplifiedRules3d(inputFilename, resultFilename, alpha, domainXFrom, domainXTo, domainYFrom, domainYTo, numberOfSections).Run();
                    break;
            }
            Console.WriteLine("Zakończono nauczanie sieci! (Naciśnij dowolny przycisk aby powrócić do Menu)");
            Console.ReadKey();
        }
    }
}
