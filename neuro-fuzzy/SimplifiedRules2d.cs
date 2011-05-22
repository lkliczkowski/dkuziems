using System;
using System.Collections.Generic;
using MyData;

namespace SimplifiedFuzzyRules
{
    /// <summary>
    /// struktura normy trojkatnej
    /// </summary>
    struct section
    {
        /// <summary>
        /// numer przedzialu
        /// </summary>
        public int index;
        
        /// <summary>
        /// przedzial trojkatny w postaci (a, b, c)
        /// </summary>
        public double a, b, c;
    }

    /* Uproszczone reguły dla norm trójkątnych
     * (a,b,c)
     *   b 
     *  /\
     * /__\
     * a  c
     * 
     * dla funkcji jednej zmiennej.
     * */
    class SimplifiedRules2d
    {
        /// <summary>
        /// Lista z przedziałami rozmytymi
        /// </summary>
        List<section> Sections;

        /// <summary>
        /// Obsługuje przepływ danych:
        /// - Pattern(int i) - dane WE dla wskazanego indeksu i
        /// - Target(int i) - docelowe WY dla wskazanego indeksu i
        /// Inicjajcja DataSet("ścieżkaDoPliku")
        /// </summary>
        private DataSet dataset;

        /// <summary>
        /// warstwa
        /// </summary>
        private double alpha;

        /// <summary>
        /// konkluzje generowanych reguł
        /// </summary>
        private double[] b;

        /// <summary>
        /// x_ile
        /// </summary>
        private int numberOfRules;

        /// <summary>
        /// Przedziały funkcji (dziedzina)
        /// </summary>
        private double domainFrom, domainTo;

        /// <summary>
        /// plik do którego zapisywane są wyniki
        /// </summary>
        private string resultFilename;

        /// <summary>
        /// konstruktor
        /// </summary>
        /// <param name="path">sciezka do pliku</param>
        /// <param name="result">plik wynikowy</param>
        /// <param name="alpha">alpha warstwa</param>
        /// <param name="dF">dziedzina, przedzial od</param>
        /// <param name="dT">dziedzina, przedzial do</param>
        /// <param name="howManySections"></param>
        public SimplifiedRules2d(string path, string result, double alpha, double dF, double dT, int howManySections)
        {

            dataset = new DataSet(path);
            resultFilename = result;
            this.alpha = alpha;

            domainFrom = dF;
            domainTo = dT;

            numberOfRules = howManySections;

            b = new double[numberOfRules];

            Sections = generateSections(numberOfRules, domainFrom, domainTo);

            foreach (section s in Sections)
            {
                Console.WriteLine("{0}\t({1:N2}, {2:N2}, {3:N2})",
                    s.index, s.a, s.b, s.c);
            }
            Console.WriteLine();
            
        }


        public void Run()
        {
            //faza identyfikacji
            identify();
            //dla wykresu funkcji liniowego
            SaveResultsAsLine(domainFrom, domainTo, dataset.Count, resultFilename);
            //porownanie z oryginalnymi punktami
            SaveResultsOriginalPoints(domainFrom, domainTo, dataset.Count, 
                resultFilename.Replace(".dat", "_pointByPoint.dat"));
        }

        private void identify()
        {
            //zmienne pomocnicze
            double w, sum;
            //x_ile //liczba rozmytych etykiet nadawanych wartosci wejsciowej
            for (int i = 0; i < numberOfRules; i++)
            { //odnosi się do reguł
                sum = 0;
                for (int k = 0; k < dataset.Count; k++) //biegniemy po wszystkich obserwacjach
                { 
                    // stopień zgodności obserwacji z rozmytym obszarem wyznaczonym przez wybraną rodzinę podzbiorów
                    w = 1;
                    for (int j = 0; j < dataset.LengthOfPattern ; j++) //dla kazdej zmiennej w obserwacji
                    {
                        w *= compatibility(dataset.Pattern(k)[j], i);
                    }
                    w = Math.Pow(w, alpha); //(4.45)

                    sum += w;

                    b[i] = b[i] + w * dataset.Target(k);
                }
                b[i] = b[i] / sum; //ważona suma i obserwacji zmiennej zależnej wzór(4.46)

                Console.WriteLine("{0}\t{1:N2}", i, b[i]);
            }
            Console.WriteLine("Koniec fazy identyfikacji");
        }

        /// <summary>
        /// Generuje wyjscie w oparciu o zadana wartosć wejsciowa r-nanie (8) [(4.44) w notatkach]
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double infer(double[] x)
        {
            double s = 0, y = 0, z;
            for (int i = 0; i < numberOfRules; i++)
            {
                z = 1;
                for (int j = 0; j < dataset.LengthOfPattern; j++)
                    z *= compatibility(x[j], i);

                s += z;
                y += z * b[i]; //ważenie, konkluzja i-tej reguły
            }
            return (y / s);
        }

        /// <summary>
        /// Obliczamy zgodosc liczby z podzbiorem
        /// </summary>
        /// <param name="x">wartosc obserwowana</param>
        /// <param name="what">dla ktorego podzbioru</param>
        /// <returns>stopien zgodnosci</returns>
        private double compatibility(double x, int what)
        {
            return triangle(x, Sections[what].a, Sections[what].b, Sections[what].c);
        }

        /// <summary>
        /// Trójkatna funkcja przynaleznosci
        /// </summary>
        /// <param name="x">obserwacja badana</param>
        /// <param name="a">(a,_,_)</param>
        /// <param name="b">(_,b,_)</param>
        /// <param name="c">(_,_,c)</param>
        /// <returns>stopień przynależności</returns>
        private static double triangle(double x, double a, double b, double c)
        {
            if (x <= a || x >= c)
                return 0.0;
            else
                if (a == Double.NegativeInfinity && x <= b)
                    return 1.0;
                else
                    if (c == Double.PositiveInfinity && x >= b)
                        return 1.0;
                    else
                        if (a <= x && x <= b)
                            return (x - a) / (b - a);
                        else
                            return (c - x) / (c - b);
        }

        /// <summary>
        /// Generuje trojkatne przedzialy przynaleznosci
        /// </summary>
        /// <param name="howMany">ile ma byc przedzialow</param>
        /// <param name="domainFrom">dziedzina, przedzial od</param>
        /// <param name="domainTo">dziedzina, przedzial do</param>
        /// <returns>lista przedzialow </returns>
        private static List<section> generateSections
            (int howMany, double domainFrom, double domainTo)
        {
            List<section> Sections = new List<section>();
            double step = Math.Abs(domainFrom - domainTo) / (howMany - 1) ;

            for (int i = 0; i < howMany; i++)
            {
                section tmp = new section();

                tmp.index = i;
                tmp.b = domainFrom + i * step;
                tmp.a = tmp.b - step;
                tmp.c = tmp.b + step;

                //dolny przedzial
                if (tmp.a < domainFrom)
                    tmp.a = Double.NegativeInfinity;
                //gorny przedzial
                if (tmp.c > domainTo)
                    tmp.c = Double.PositiveInfinity;

                Sections.Add(tmp);
            }

            return Sections;
        }

        /// <summary>
        /// wylicza wyniki działania sieci i zapisuje do pliku
        /// </summary>
        /// <param name="domainFrom">dziedzina od</param>
        /// <param name="domainTo">dziedzina do</param>
        /// <param name="howMany">ile punktów ma zostać zbadane na przedziale</param>
        /// <param name="path">ścieżka do pliku</param>
        /// <returns>"true" dla udanego zapisu do pliku</returns>
        public bool SaveResultsAsLine(double domainFrom, double domainTo, int howMany, string path)
        {
            double[] xs = new double[dataset.LengthOfPattern];

            List<double[]> results = new List<double[]>();
            List<double> singleRecord = new List<double>();

            double step = Math.Abs(domainFrom - domainTo) / howMany;

            for (int i = 0; i < howMany; i++)
            {
                for (int j = 0; j < dataset.LengthOfPattern; j++)
                    xs[j] = domainFrom + step * i;

                double target = infer(xs);

                foreach (double d in xs)
                {
                    singleRecord.Add(d);
                }

                singleRecord.Add(target);

                results.Add(singleRecord.ToArray());
                singleRecord.Clear();
            }

            //plik nagłówkowy umieszczony w komentarzu w pierwszej linii
            string header = String.Format("Wykres funkcji dla uproszczonych reguł rozmytych");
            Console.WriteLine(header);

            if (DataWrite.WriteData(path, results, header))
                Console.WriteLine("Zapisano wyniki w {0}", path);
            else return false;

            return true;
        }

        /// <summary>
        /// wylicza wyniki działania sieci i zapisuje do pliku
        /// </summary>
        /// <param name="domainFrom">dziedzina od</param>
        /// <param name="domainTo">dziedzina do</param>
        /// <param name="howMany">ile punktów ma zostać zbadane na przedziale</param>
        /// <param name="path">ścieżka do pliku</param>
        /// <returns>"true" dla udanego zapisu do pliku</returns>
        public bool SaveResultsOriginalPoints(double domainFrom, double domainTo, int howMany, string path)
        {
            double[] xs = new double[dataset.LengthOfPattern];

            List<double[]> results = new List<double[]>();
            List<double> singleRecord = new List<double>();

            double err = 0;

            for (int i = 0; i < howMany; i++)
            {
                xs = dataset.Pattern(i);

                double target = infer(xs);

                err += 0.5 * Math.Pow(dataset.Target(i) - target, 2);

                foreach (double d in xs)
                {
                    singleRecord.Add(d);
                }

                singleRecord.Add(target);

                results.Add(singleRecord.ToArray());
                singleRecord.Clear();
            }

            //plik nagłówkowy umieszczony w komentarzu w pierwszej linii
            string header = String.Format("Wyniki dla uproszczonych reguł rozmytych, błąd: {0}", err);
            Console.WriteLine(header);

            if (DataWrite.WriteData(path, results, header))
                Console.WriteLine("Zapisano wyniki w {0}", path);
            else return false;

            return true;
        }
    
    }
}
