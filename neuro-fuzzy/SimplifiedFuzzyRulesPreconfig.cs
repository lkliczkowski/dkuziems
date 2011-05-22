using System;

namespace SimplifiedFuzzyRules
{
    partial class SimplifiedFuzzyRules
    {
        # region funkcje konfiguracji

        /// <summary>
        /// wybór funkcji
        /// </summary>
        /// <param name="option">out wartość</param>
        /// <returns>true jeżeli wartość została podana, false wpp.</returns>
        private static bool selectFunction(out int option)
        {
            Console.WriteLine("Która funkcja?\n1) sin(x)+ε\n");
            Console.WriteLine("2) (-2*Log10(2)*((x-0.08)/0.854)^2) * "
                              + "((Sin(5*PI*((x^{3/4}) - 0.05)))^6)\n");
            Console.WriteLine("3) 200 - (x^2 + y - 11)^2-(x+ y^2 -7))\n");
            try
            {
                int op = Int32.Parse(Console.ReadLine());
                if (op > 0 && op < 4)
                {
                    option = op;
                    return true;
                }
                else
                {
                    throw new System.ArgumentException("Opcje od 1-3");
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine("Niepoprawna wartość ({0})", e.Message);
            }
            option = 0;
            return false;
        }

        /// <summary>
        /// ustal liczbę zbiorów rozmytych
        /// </summary>
        /// <param name="numberOfSections">ref ilość</param>
        /// <param name="selectedFunc">numer funkcji</param>
        private static void setAmountOfSections(ref int numberOfSections, int selectedFunc)
        {
            Console.WriteLine("Podaj liczbę zbiorów rozmytych, obecna: {0}", numberOfSections);
            if (selectedFunc == 3)
                Console.WriteLine("info: 4,9,16,25,36,49,64,81,100,121,169(etc.) zadziała najlepiej dla funkcji 3D");
            Console.WriteLine("Im więcej obszarów tym większa dokładność aproksymacji funkcji\n{0}",
                "Uwaga! Zbyt duża ilość dla funkcji jednej zmiennej prowadzi także do błędnych wyników!");
            try
            {
                int num = Int32.Parse(Console.ReadLine());
                if (num > 0)
                {
                    numberOfSections = num;
                }
                else
                {
                    throw new System.ArgumentException("Liczba obszarów powinna być większa od zera!");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednia ({0})",
                                  e.Message);
            }
            Console.WriteLine("Obecna liczba obszarów wynosi: {0}\n", numberOfSections);
        }

        /// <summary>
        /// ustal współczynnik alpha
        /// </summary>
        /// <param name="alpha">ref wartość</param>
        private static void setAlpha(ref double alpha)
        {
            Console.WriteLine("Podaj współczynnik warstwy (α alpha), obecny: {0}", alpha);
            string tmp = Console.ReadLine();
            try
            {
                double tmpAlpha = Double.Parse(tmp.Replace(".", ","));
                if (tmpAlpha <= 0)
                    throw new System.ArgumentException("Współczynnik musi być większy od zera!");
                else
                    alpha = tmpAlpha;
            }
            catch (Exception e)
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią ({0})",
                                  e.Message);
            }
            Console.WriteLine("Obecny współczynnik uczenia wynosi: {0}\n", alpha);
        }

        /// <summary>
        /// pyta czy wygenerować zbiór
        /// </summary>
        /// <returns>true dla tak (domyślne), false wpp.</returns>
        private static bool generateSet()
        {
            char useGenSetToo = 'T';
            Console.WriteLine("Wygenerować zbiór danych dla tej funkcji (T/n) Obecnie: {0}\n", "Tak");
            try
            {
                useGenSetToo = Char.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość/lub [Enter]");
            }
            if (useGenSetToo == 't' || useGenSetToo == 'T')
            {
                Console.WriteLine("Opcja ustawiona na True\n");
                return true;
            }
            else
            {
                Console.WriteLine("Opcja ustawiona na False\n");
                Console.WriteLine("Uwaga! Program nie wygeneruje sobie pliku z danymi - musisz sam o niego zadbać!");
                return false;
            }
        }

        /// <summary>
        /// ustal liczbę punktów, która ma zostać wygenerowana
        /// </summary>
        /// <param name="pNumber"> ref ilość</param>
        private static void setPointsNumber(ref int pNumber)
        {
            Console.WriteLine("Zbiór danych zostanie wygenerowany.");
            Console.WriteLine("Ilość punktów, domyślnie: {0}", pNumber);
            Console.WriteLine("([Enter] by kontynuować bez wybierania)");
            try
            {
                int num = Int32.Parse(Console.ReadLine());
                if (num > 0)
                {
                    pNumber = num;
                }
                else
                {
                    throw new System.ArgumentException("Punktów musi być więcej niż zero!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Niepoprawna wartość/lub [Enter] ({0})", e.Message);
            }
            Console.WriteLine("Obecna ilość punktów: {0}\n", pNumber);
        }

        #endregion
    }
}
