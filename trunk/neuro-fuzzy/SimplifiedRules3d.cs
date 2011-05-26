using System;
using System.Collections.Generic;
using MyData;

namespace SimplifiedFuzzyRules
{
    /*  Zmodyfikowana wersja "SimplifiedRules2d.class",
     * zamist trójkątów, stożki. Zrezygnowałem z możliwości
     * utorzenia wszystkiego w jednym pliku aby uniknąć niepotrzebnego 
     * chaosu (np.: struktura Point dla funkcji jednej zmiennej) 
     * - Brzytwa Ockhama, no i tak jest prościej i bardziej zrozumiale.
     * 
     * ==============================================================
     * Pozwala na identyfikacje funkcji dwóch zmiennych dzięki
     * założeniu, że punkty A,B,D,E mają wspólne punkty x,y wg
     * schematu:
     * 
     * (każdy punkt to P(x,y))
     * 
     * D_______E
     * |       |
     * |   *   |
     * |_______|
     * A       B
     * 
     * gdzie * to punkt C (wierzchołek),
     * 
     * A.x == D.x
     * B.x == E.x
     * A.y == B.y
     * D.y == E.y
     * 
     * dzięki czemu możemy przepisać stożki jako trójkąty.
     * 
     * ==============================================================
     * 
     * Funkcje sumujące wzory dla wielu zmiennych były już w 
     * "SimplifiedRules2d.class", przykład:
     * {
     *  w = 1;
     *  for (int j = 0; j < dataset.LengthOfPattern ; j++) //dla kazdej zmiennej w obserwacji
     *  {
     *      w *= compatibility(dataset.Pattern(k)[j], i, j); 
     *  }
     * }
     * 
     * można było róznie dobrze krótko napisać jako:
     * {
     *  w = compatibility(dataset.Pattern(k)[0], i, j);
     * }
     * 
     * mało elegancko.
     * */
    class SimplifiedRules3d
    {
        /// <summary>
        /// punkt w przestrzeni
        /// </summary>
        private struct point
        {
            public double X, Y;
        }
        /// <summary>
        /// struktura normy trojkatnej
        /// </summary>
        private struct section3d
        {
            /// <summary>
            /// numer przedzialu
            /// </summary>
            public int Index;

            /// <summary>
            /// stożek z wierzchołkiem c
            /// </summary>
            public point A, B, C, D, E;
        }

        /// <summary>
        /// Lista z przedziałami rozmytymi
        /// </summary>
        private List<section3d> sections;

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
        /// Numeryczne konkluzje generowanych reguł
        /// </summary>
        private double[] b;

        /// <summary>
        /// liczba regul
        /// </summary>
        private int numberOfRules;

        /// <summary>
        /// Przedziały funkcji (dziedzina)
        /// </summary>
        private double domainXFrom, domainXTo, domainYFrom, domainYTo;

        /// <summary>
        /// plik do którego zapisywane są wyniki
        /// </summary>
        private string resultFilename;

        /// <summary>
        /// konstruktor
        /// </summary>
        /// <param name="path">ścieżka do pliku</param>
        /// <param name="alpha">warstwa alpha</param>
        /// <param name="dxF">dziedzina x</param>
        /// <param name="dxT">dziedzina x</param>
        /// <param name="dyF">dziedzina y</param>
        /// <param name="dyT">dziedzina y</param>
        /// <param name="howManySections">ilosc zbiorow</param>
        public SimplifiedRules3d(string path, string result, double alpha, double dxF, 
            double dxT, double dyF, double dyT, int howManySections)
        {

            dataset = new DataSet(path);
            this.alpha = alpha;
            resultFilename = result;

            domainXFrom = dxF;
            domainYFrom= dyF;
            domainXTo = dxT;
            domainYTo = dyT;

            numberOfRules = howManySections;

            b = new double[numberOfRules];

            sections = generateSections(numberOfRules, domainXFrom, domainXTo, domainYFrom, domainYTo);

            foreach (section3d s in sections)
            {
                Console.WriteLine("({0:N1}, {1:N1}), ({2:N1}, {3:N1}), (({4:N1}, {5:N1})), ({6:N1}, {7:N1}), ({8:N1}, {9:N1})",
                    s.A.X, s.A.Y, s.B.X, s.B.Y, s.C.X, s.C.Y, s.D.X, s.D.Y, s.E.X, s.E.Y);
            }
            Console.WriteLine();
            
        }

        /// <summary>
        /// funkcja głowna
        /// </summary>
        public void Run()
        {
            //faza identykifacji
            identify();
            //test i wyniki
            SaveResults(domainXFrom, domainXTo, dataset.Count, resultFilename);
        }

        /// <summary>
        /// Identyfikacja prostych reguł rozmytych
        /// </summary>
        private void identify()
        {
            //zmienna sumujaca dla stopnie zgodnosci obserwacji x
            double w;
            //zmianna sumujaca w (mianownik w 4.46)
            double sum;
			
			Console.WriteLine("{0}\t{1:N2}", "nr", "reguła");

            //liczba rozmytych etykiet nadawanych wartosci wejsciowej
            for (int i = 0; i < numberOfRules; i++)
            { //odnosi się do reguł
                sum = 0;
                for (int k = 0; k < dataset.Count; k++) //biegniemy po wszystkich obserwacjach
                { 
                    // stopień zgodności obserwacji z rozmytym obszarem wyznaczonym przez wybraną rodzinę podzbiorów
                    w = 1;
                    for (int j = 0; j < dataset.LengthOfPattern ; j++) //dla kazdej zmiennej w obserwacji
                    {
                        w *= compatibility(dataset.Pattern(k)[j], i, j); ;
                    }
                    w = Math.Pow(w, alpha); //(4.45)

                    sum += w;

                    b[i] = b[i] + w * dataset.Target(k);
                }
                b[i] = b[i] / sum; //ważona suma i obserwacji zmiennej zależnej wzór(4.46)

                Console.WriteLine("{0}\t{1:N2}", i+1, b[i]);
            }
            Console.WriteLine("Koniec fazy identyfikacji");
        }

        /*
         * Generuje wyjscie w oparciu o zadana wartosć wejsciowa
         * r-nanie (8) [(4.44) w notatkach]
         */
        private double infer(double[] x)
        {
            double sum = 0, y = 0, z;
            for (int i = 0; i < numberOfRules; i++)
            {
                z = 1;
                for (int j = 0; j < dataset.LengthOfPattern; j++ )
                    z *= compatibility(x[j], i, j);

                sum += z;
                y += z * b[i]; //ważenie, konkluzja i-tej reguły
            }
            return (y / sum);
        }

        /// <summary>
        /// Obliczamy zgodosc liczby z podzbiorem
        /// </summary>
        /// <param name="x">wartosc obserwowana</param>
        /// <param name="what">dla ktorego podzbioru</param>
        /// <param name="dim">który wymiar</param>
        /// <returns>stopien zgodnosci</returns>
        private double compatibility(double x, int what, int dim)
        {
            switch(dim)
            {
                case 0:
                    return triangle(x, sections[what].A.X, sections[what].C.X, sections[what].B.X);
                case 1:
                    return triangle(x, sections[what].A.Y, sections[what].C.Y, sections[what].D.Y);
                default:
                    Console.WriteLine("[!] Funkcja niedostosowana do podanej liczby wymiarów...");
                    return 0;
            }
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
        /// Generuje "ostroslupowe" przedzialy przynaleznosci
        /// </summary>
        /// <param name="howMany">ile ma byc przedzialow</param>
        /// <param name="domainXFrom">dziedzina X, przedzial od</param>
        /// <param name="domainXTo">dziedzina X, przedzial do</param>
        /// <param name="domainYFrom">dziedzina Y, przedzial od</param>
        /// <param name="domainYTo">dziedzina Y, przedzial do</param>
        /// <returns>lista przedzialow </returns>
        private static List<section3d> generateSections
            (int howMany, double domainXFrom, double domainXTo,
            double domainYFrom, double domainYTo)
        {
            List<section3d> Sections = new List<section3d>();
            int inEachRow = (int)Math.Sqrt(howMany);
            double stepX = Math.Abs(domainXFrom - domainXTo) / (inEachRow - 1);
            double stepY = Math.Abs(domainYFrom - domainYTo) / (inEachRow - 1);

            for (int x = 0; x < inEachRow; x++)
            {
                for (int y = 0; y < inEachRow; y++)
                {
                    section3d tmp = new section3d();

                    tmp.Index = x + y;

                    tmp.C.X = domainXFrom + x * stepX;
                    tmp.C.Y = domainYFrom + y * stepY;

                    tmp.A.X = tmp.C.X - stepX;
                    tmp.A.Y = tmp.C.Y - stepY;

                    tmp.B.X = tmp.C.X + stepX;
                    tmp.B.Y = tmp.C.Y - stepY;

                    tmp.D.X = tmp.C.X - stepX;
                    tmp.D.Y = tmp.C.Y + stepY;

                    tmp.E.X = tmp.C.X + stepX;
                    tmp.E.Y = tmp.C.Y + stepY;

                    if (tmp.A.X < domainXFrom) tmp.A.X = Double.NegativeInfinity;
                    if (tmp.A.X > domainXTo)   tmp.A.X = Double.PositiveInfinity;
                    if (tmp.A.Y < domainYFrom) tmp.A.Y = Double.NegativeInfinity;
                    if (tmp.A.Y > domainYTo)   tmp.A.Y = Double.PositiveInfinity;

                    if (tmp.B.X < domainXFrom) tmp.B.X = Double.NegativeInfinity;
                    if (tmp.B.X > domainXTo)   tmp.B.X = Double.PositiveInfinity;
                    if (tmp.B.Y < domainYFrom) tmp.B.Y = Double.NegativeInfinity;
                    if (tmp.B.Y > domainYTo)   tmp.B.Y = Double.PositiveInfinity;

                    if (tmp.D.X < domainXFrom) tmp.D.X = Double.NegativeInfinity;
                    if (tmp.D.X > domainXTo)   tmp.D.X = Double.PositiveInfinity;
                    if (tmp.D.Y < domainYFrom) tmp.D.Y = Double.NegativeInfinity;
                    if (tmp.D.Y > domainYTo)   tmp.D.Y = Double.PositiveInfinity;

                    if (tmp.E.X < domainXFrom) tmp.E.X = Double.NegativeInfinity;
                    if (tmp.E.X > domainXTo)   tmp.E.X = Double.PositiveInfinity;
                    if (tmp.E.Y < domainYFrom) tmp.E.Y = Double.NegativeInfinity;
                    if (tmp.E.Y > domainYTo)   tmp.E.Y = Double.PositiveInfinity;

                    Sections.Add(tmp);
                }
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
        public bool SaveResults(double domainFrom, double domainTo, int howMany, string path)
        {
            double[] xs = new double[dataset.LengthOfPattern];

            List<double[]> results = new List<double[]>();
            List<double> singleRecord = new List<double>();

            double err = 0;

            for (int i = 0; i < howMany; i++)
            {
                xs = dataset.Pattern(i);

                double target = infer(xs);

                err += Math.Pow(dataset.Target(i) - target, 2);

                foreach (double d in xs)
                {
                    singleRecord.Add(d);
                }

                singleRecord.Add(target);

                results.Add(singleRecord.ToArray());
                singleRecord.Clear();
            }
			err /= howMany;

            //plik nagłówkowy umieszczony w komentarzu w pierwszej linii
            string header = String.Format("Wyniki dla uproszczonych reguł rozmytych, błąd: {0}, obszarów: {1}",
			                              err, numberOfRules);
            Console.WriteLine(header);

            if (DataWrite.WriteData(path, results, header))
                Console.WriteLine("Zapisano wyniki w {0}", path);
            else return false;

            return true;
        }    
    }
}
