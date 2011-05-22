using System;

namespace FunahashiNeuralNetwork
{
    partial class BackpropagationTest
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
        /// ustala liczbę neuronów  warstwie ukrytej
        /// </summary>
        /// <param name="numHidden">ref ilość</param>
        private static void setHiddenRatio(ref int numHidden)
        {
            Console.WriteLine("Podaj liczbę neuronów w warstwie ukrytej, obecna: {0}", numHidden);
            try
            {
                int hr = Int32.Parse(Console.ReadLine());
                if (hr > 0)
                {
                    numHidden = hr;
                }
                else
                {
                    throw new System.ArgumentException("Liczba neuronów powinna być większa od zera!");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednia ({0})",
                                  e.Message);
            }
            Console.WriteLine("Obecna liczba neuronów wynosi: {0}\n", numHidden);
        }

        /// <summary>
        /// ustala współczynnik uczenia
        /// </summary>
        /// <param name="lr">ref wartość</param>
        private static void setLearningRate(ref double lr)
        {
            Console.WriteLine("Podaj współczynnik uczenia, obecny: {0}", lr);
            string tmp = Console.ReadLine();
            try
            {
                double tmpLR = Double.Parse(tmp.Replace(".", ","));
                if (tmpLR <= 0)
                    throw new System.ArgumentException("Współczynnik musi być większy od zera!");
                else
                    lr = tmpLR;
            }
            catch (Exception e)
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią ({0})",
                                  e.Message);
            }
            Console.WriteLine("Obecny współczynnik uczenia wynosi: {0}\n", lr);
        }

        /// <summary>
        /// ustal maksymalną liczbę epok
        /// </summary>
        /// <param name="maxEpochsPar">ref ilość</param>
        private static void setMaxEpoch(ref ulong maxEpochsPar)
        {
            Console.WriteLine("Podaj maksymalną ilość epok, obecna: {0}", maxEpochsPar);
            Console.WriteLine("Zero dla uczenia w \"nieskończoność\" (wyniki są zapisywane cyklicznie)");
            try
            {
                maxEpochsPar = ulong.Parse(Console.ReadLine());
                if (maxEpochsPar == 0)
                {
                    maxEpochsPar = ulong.MaxValue;
                }
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią...");
            }
            Console.WriteLine("Obecna maksymalna ilość epok: {0}\n", maxEpochsPar);
        }


        /// <summary>
        /// czy ma stosować BatchLearning
        /// </summary>
        /// <param name="useBatch">ref boolVal, true dla tak, false wpp.</param>
        private static void setUseBatch(ref bool useBatch)
        {
            char useGenSetToo = 'n';
            Console.WriteLine("Stosować BatchLearning? (t/N) Obecnie: {0}\n", useBatch ? "True" : "False");
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
                useBatch = true;
            }
            else
            {
                Console.WriteLine("Opcja ustawiona na False\n");
                useBatch = false;
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
        #endregion

    }
}
