using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace LearningBPandLM
{

    public struct FileStatistics
    {
        public double TMse { get; set; }
        public double TAcc { get; set; }
        public double GMse { get; set; }
        public double GAcc { get; set; }
        public double BestTMse { get; set; }
        public double BestTMseGMse { get; set; }
        public int EpochWhenBTMse { get; set; }
        public double BestGMse { get; set; }
        public int EpochWhenBGMse { get; set; }
        public double BestGMseTMse { get; set; }
        public double BestTAcc { get; set; }
        public double BestGAcc { get; set; }
        public long TotalAverageTime { get; set; }
        public int EpochNum { get; set; }
    }

    public class CreateScript
    {
        private string header;
        private List<string> linestyles;
        private List<string> fileNames;
        private string scriptFileName;
        private string columnUsed;
        private string statistics;
        private bool testTolerance;

        public CreateScript(string path,
                             string keyNames,
                             string scriptFileName,
                             string output, string title,
                             string label, string columnUsed,
                             bool tT)
        {
            testTolerance = tT;

            Console.WriteLine("\n\n[{0}]", scriptFileName);

            createHeader(ref header, output, title, label);
            Console.WriteLine("tworzenie nagłówków...");

            createLineStyle(ref linestyles);
            Console.WriteLine("style linii...");

            fileNames = CreateListOfFiles(path, keyNames);
            Console.WriteLine("lista plików z wynikami... [{0}]", fileNames.Count);

            //statystyki danych
            statistics = CreateStatistics(fileNames, keyNames, false, testTolerance);

            this.scriptFileName = scriptFileName;
            this.columnUsed = columnUsed;
        }

        public static string CreateStatistics(List<System.String> fileNames,
                                               string key, bool shortForm, bool tOnly)
        {
            if (fileNames.Count == 0)
            {
                //Console.WriteLine("Nie znaleziono plików odpowiadających wzorcowi!");
                return null;
            }
            List<FileStatistics> statsList = new List<FileStatistics>();
            //jeżeli badany jest tOnly dodatkowa lista
            List<FileStatistics> leftStatsList = new List<FileStatistics>();

            char[] delimiter = new char[] { (char)9 };

            #region statystyki całych zestawów
            double bestFinalTMse = double.MaxValue,
                bestFinalGMse = double.MaxValue,
                bestFinalTAcc = double.MinValue,
                bestFinalGAcc = double.MinValue;

            double pbestFinalTMse = double.MaxValue,
                pbestFinalGMse = double.MaxValue,
                pbestFinalTAcc = double.MinValue,
                pbestFinalGAcc = double.MinValue;

            string bestFinalTMseFileName = null,
                bestFinalGMseFileName = null,
                bestFinalTAccFileName = null,
                bestFinalGAccFileName = null;

            string pbestFinalTMseFileName = null,
                pbestFinalGMseFileName = null,
                pbestFinalTAccFileName = null,
                pbestFinalGAccFileName = null;

            //dla TOnly
            double lbestFinalTMse = double.MaxValue,
                lbestFinalGMse = double.MaxValue,
                lbestFinalTAcc = double.MinValue,
                lbestFinalGAcc = double.MinValue;

            string lbestFinalTMseFileName = null,
                lbestFinalGMseFileName = null,
                lbestFinalTAccFileName = null,
                lbestFinalGAccFileName = null;

            bool TFoundGMse = false,
            TFoundGAcc = false,
            TFoundTMse = false,
            TFoundTAcc = false;

            long totalTIME = 0,
                pTotalTIME = 0;
            #endregion

            foreach (string file in fileNames)
            {
                using (StreamReader read = new StreamReader(file))
                {
                    List<string> lines = new List<string>();
                    string[] tmp;
                    int tmpi = new int();
                    string singleLine;

                    //pojedyncza struktura opisująca plik
                    FileStatistics singleFileStats = new FileStatistics()
                    {
                        BestGMse = Double.MaxValue,
                        BestTMse = Double.MaxValue,
                        BestTAcc = Double.MinValue,
                        BestGAcc = Double.MinValue,
                        EpochWhenBTMse = new int(),
                        EpochWhenBGMse = new int()
                    };

                    while ((singleLine = read.ReadLine()) != null)
                    {
                        lines.Add(singleLine);
                    }
                    read.Close();

                    //sumujemy czasy i wyniki końcowe
                    try
                    {
                        tmp = lines[lines.Count - 3].
                              Split(delimiter, StringSplitOptions.None);
                        if (tmp[0].Contains("total"))
                            totalTIME += Int64.Parse(tmp[1]);

                        if (tmp[0].Contains("ended") || tmp[0].Contains("total"))
                            tmp = lines[lines.Count - 4].
                               Split(delimiter, StringSplitOptions.None);

                        singleFileStats.EpochNum = Int32.Parse(tmp[0]);
                        singleFileStats.TMse = Double.
                            Parse(tmp[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                        singleFileStats.TAcc = Double.
                            Parse(tmp[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                        singleFileStats.GMse = Double.
                            Parse(tmp[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                        singleFileStats.GAcc = Double.
                            Parse(tmp[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                        tmp = lines[lines.Count - 2].
                              Split(delimiter, StringSplitOptions.None);
                        singleFileStats.TotalAverageTime = Int64.Parse(tmp[1]);
                        singleFileStats.TotalAverageTime *= singleFileStats.EpochNum;
                    }
                    catch
                    {
                        Console.WriteLine("[1] Błąd w parsowaniu:\t{0}", file);
                        break;
                    }

                    bool TFound = tOnly ? false : true;
                    if (tOnly)
                    {
                        pbestFinalTMse = bestFinalTMse;
                        pbestFinalGMse = bestFinalGMse;
                        pbestFinalTAcc = bestFinalTAcc;
                        pbestFinalGAcc = bestFinalGAcc;

                        pbestFinalTMseFileName = bestFinalTMseFileName;
                        pbestFinalGMseFileName = bestFinalGMseFileName;
                        pbestFinalTAccFileName = bestFinalTAccFileName;
                        pbestFinalGAccFileName = bestFinalGAccFileName;

                        TFoundGMse = false;
                        TFoundGAcc = false;
                        TFoundTMse = false;
                        TFoundTAcc = false;

                        pTotalTIME = totalTIME;
                    }

                    //szukamy najlepszych wyników
                    foreach (string l in lines)
                    {
                        if (l.Contains("#") || l == "" || l == null)
                            continue;

                        tmp = l.Split(delimiter, StringSplitOptions.None);

                        try
                        {
                            if (Double.Parse(tmp[1], NumberStyles.Float,
                                            CultureInfo.InvariantCulture) < singleFileStats.BestTMse)
                            {
                                singleFileStats.BestTMse = Double.Parse(tmp[1], NumberStyles.Float,
                                                             CultureInfo.InvariantCulture);

                                Int32.TryParse(tmp[0], out tmpi);
                                singleFileStats.EpochWhenBTMse = tmpi;
                                singleFileStats.BestTMseGMse = Double.Parse(tmp[3], NumberStyles.Float,
                                                             CultureInfo.InvariantCulture);
                            }
                            if (singleFileStats.BestTMse < bestFinalTMse)
                            {
                                bestFinalTMse = singleFileStats.BestTMse;
                                bestFinalTMseFileName = file;
                                TFoundTMse = true;
                            }


                            if (Double.Parse(tmp[2], NumberStyles.Float,
                                            CultureInfo.InvariantCulture) > singleFileStats.BestTAcc)
                                singleFileStats.BestTAcc = Double.Parse(tmp[2], NumberStyles.Float,
                                                             CultureInfo.InvariantCulture);
                            if (singleFileStats.BestTAcc > bestFinalTAcc)
                            {
                                bestFinalTAcc = singleFileStats.BestTAcc;
                                bestFinalTAccFileName = file;
                                TFoundTAcc = true;
                            }

                            if (Double.Parse(tmp[3], NumberStyles.Float,
                                            CultureInfo.InvariantCulture) < singleFileStats.BestGMse)
                            {
                                singleFileStats.BestGMse = Double.Parse(tmp[3], NumberStyles.Float,
                                                             CultureInfo.InvariantCulture);
                                Int32.TryParse(tmp[0], out tmpi);
                                singleFileStats.EpochWhenBGMse = tmpi;
                                singleFileStats.BestGMseTMse = Double.Parse(tmp[3], NumberStyles.Float,
                                                             CultureInfo.InvariantCulture);
                            }
                            if (singleFileStats.BestGMse < bestFinalGMse)
                            {
                                bestFinalGMse = singleFileStats.BestGMse;
                                bestFinalGMseFileName = file;
                                TFoundGMse = true;
                            }

                            if (Double.Parse(tmp[4], NumberStyles.Float,
                                            CultureInfo.InvariantCulture) > singleFileStats.BestGAcc)
                                singleFileStats.BestGAcc = Double.Parse(tmp[4], NumberStyles.Float,
                                                             CultureInfo.InvariantCulture);
                            if (singleFileStats.BestGAcc > bestFinalGAcc)
                            {
                                bestFinalGAcc = singleFileStats.BestGAcc;
                                bestFinalGAccFileName = file;
                                TFoundGAcc = true;
                            }

                            if (tOnly && tmp.Length == 7)
                            {
                                if (tmp[6].Contains("T"))
                                    TFound = true;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("[2] Błąd w parsowaniu:\t{0}", file);
                            break;
                        }
                    }

                    if (tOnly && (tOnly && !TFound))
                    {
                        if (lbestFinalTMse > bestFinalTMse && TFoundTMse)
                        {
                            lbestFinalTMse = bestFinalTMse;
                            lbestFinalTMseFileName = bestFinalTMseFileName;
                        }
                        if (lbestFinalGMse > bestFinalGMse && TFoundGMse)
                        {
                            lbestFinalGMse = bestFinalGMse;
                            lbestFinalGMseFileName = bestFinalGMseFileName;
                        }
                        if (lbestFinalTAcc < bestFinalTAcc && TFoundTAcc)
                        {
                            lbestFinalTAcc = bestFinalTAcc;
                            lbestFinalTAccFileName = bestFinalTAccFileName;
                        }
                        if (lbestFinalGAcc < bestFinalGAcc && TFoundGAcc)
                        {
                            lbestFinalGAcc = bestFinalGAcc;
                            lbestFinalGAccFileName = bestFinalGAccFileName;
                        }

                        bestFinalTMse = pbestFinalTMse;
                        bestFinalGMse = pbestFinalGMse;
                        bestFinalTAcc = pbestFinalTAcc;
                        bestFinalGAcc = pbestFinalGAcc;

                        bestFinalTMseFileName = pbestFinalTMseFileName;
                        bestFinalGMseFileName = pbestFinalGMseFileName;
                        bestFinalTAccFileName = pbestFinalTAccFileName;
                        bestFinalGAccFileName = pbestFinalGAccFileName;

                        totalTIME = pTotalTIME;
                    }

                    if (tOnly && (tOnly && TFound))
                    {
                        statsList.Add(singleFileStats);
                    }
                    else if (tOnly && (tOnly && !TFound))
                    {
                        leftStatsList.Add(singleFileStats);
                    }
                    else if (!tOnly)
                    {
                        statsList.Add(singleFileStats);
                    }
                }
            }


            #region normal stats
            long timeSum = 0;
            double gMseSumAve = 0, tMseSumAve = 0, gAccSumAve = 0, tAccSumAve = 0,
                gBestMseSumAve = 0, tBestMseSumAve = 0, gBestAccSumAve = 0, tBestAccSumAve = 0,
                differenceGMse = 0, differenceTMse = 0, epochBestTMSE = 0, epochBestGMSE = 0,
                epochNumAve = 0;

            List<double> listBGMse = new List<double>();
            List<double> listBTMse = new List<double>();
            List<double> listEGMse = new List<double>();
            List<double> listETMse = new List<double>();

            foreach (FileStatistics singleStat in statsList)
            {
                timeSum += singleStat.TotalAverageTime;
                gMseSumAve += singleStat.GMse;
                listEGMse.Add(singleStat.GMse);
                gAccSumAve += singleStat.GAcc;
                tMseSumAve += singleStat.TMse;
                listETMse.Add(singleStat.TMse);
                tAccSumAve += singleStat.TAcc;
                gBestMseSumAve += singleStat.BestGMse;
                listBGMse.Add(singleStat.BestGMse);
                tBestMseSumAve += singleStat.BestTMse;
                listBTMse.Add(singleStat.BestTMse);
                gBestAccSumAve += singleStat.BestGAcc;
                tBestAccSumAve += singleStat.BestTAcc;
                differenceGMse +=
                    Math.Abs(singleStat.BestTMseGMse - singleStat.BestGMse);
                differenceTMse +=
                    Math.Abs(singleStat.BestGMseTMse - singleStat.BestTMse);
                epochBestGMSE += singleStat.EpochWhenBGMse;
                epochBestTMSE += singleStat.EpochWhenBTMse;
                epochNumAve += singleStat.EpochNum;
            }

            gMseSumAve /= statsList.Count;
            gAccSumAve /= statsList.Count;
            tMseSumAve /= statsList.Count;
            tAccSumAve /= statsList.Count;
            gBestMseSumAve /= statsList.Count;
            tBestMseSumAve /= statsList.Count;
            gBestAccSumAve /= statsList.Count;
            tBestAccSumAve /= statsList.Count;
            epochNumAve /= statsList.Count;

            //differenceGMse *= 1;
            //differenceTMse *= 1;
            differenceGMse /= statsList.Count;
            differenceTMse /= statsList.Count;

            //gBetterThanAverageBestMseCount
            int gBetterTABMC = 0;

            foreach (FileStatistics singleStat in statsList)
            {
                if (gBestMseSumAve <= singleStat.BestGMse)
                {
                    gBetterTABMC++;
                }
            }
            #endregion

            #region stats left by TOnly

            long ltimeSum = 0;
            double lgMseSumAve = 0, ltMseSumAve = 0, lgAccSumAve = 0, ltAccSumAve = 0,
                lgBestMseSumAve = 0, ltBestMseSumAve = 0, lgBestAccSumAve = 0, ltBestAccSumAve = 0,
                ldifferenceGMse = 0, ldifferenceTMse = 0, lepochBestTMSE = 0, lepochBestGMSE = 0,
                lepochNumAve = 0;

            List<double> llistBGMse = new List<double>();
            List<double> llistBTMse = new List<double>();
            List<double> llistEGMse = new List<double>();
            List<double> llistETMse = new List<double>();

            int lgBetterTABMC = 0;

            if (tOnly)
            {
                foreach (FileStatistics singleStat in leftStatsList)
                {
                    ltimeSum += singleStat.TotalAverageTime;
                    lgMseSumAve += singleStat.GMse;
                    llistEGMse.Add(singleStat.GMse);
                    lgAccSumAve += singleStat.GAcc;
                    ltMseSumAve += singleStat.TMse;
                    llistETMse.Add(singleStat.TMse);
                    ltAccSumAve += singleStat.TAcc;
                    lgBestMseSumAve += singleStat.BestGMse;
                    llistBGMse.Add(singleStat.BestGMse);
                    ltBestMseSumAve += singleStat.BestTMse;
                    llistBTMse.Add(singleStat.BestTMse);
                    lgBestAccSumAve += singleStat.BestGAcc;
                    ltBestAccSumAve += singleStat.BestTAcc;
                    ldifferenceGMse +=
                        Math.Abs(singleStat.BestTMseGMse - singleStat.BestGMse);
                    ldifferenceTMse +=
                        Math.Abs(singleStat.BestGMseTMse - singleStat.BestTMse);
                    lepochBestGMSE += singleStat.EpochWhenBGMse;
                    lepochBestTMSE += singleStat.EpochWhenBTMse;
                    lepochNumAve += singleStat.EpochNum;

                    if (lbestFinalTMse > singleStat.BestTMse)
                        lbestFinalTMse = singleStat.BestTMse;
                    if (lbestFinalTAcc < singleStat.BestTAcc)
                        lbestFinalTAcc = singleStat.BestTAcc;
                }

                lgMseSumAve /= leftStatsList.Count;
                lgAccSumAve /= leftStatsList.Count;
                ltMseSumAve /= leftStatsList.Count;
                ltAccSumAve /= leftStatsList.Count;
                lgBestMseSumAve /= leftStatsList.Count;
                ltBestMseSumAve /= leftStatsList.Count;
                lgBestAccSumAve /= leftStatsList.Count;
                ltBestAccSumAve /= leftStatsList.Count;
                lepochNumAve /= leftStatsList.Count;



                ldifferenceGMse /= leftStatsList.Count;
                ldifferenceTMse /= leftStatsList.Count;

                foreach (FileStatistics singleStat in leftStatsList)
                {
                    if (gBestMseSumAve <= singleStat.BestGMse)
                    {
                        lgBetterTABMC++;
                    }
                }
            }
            #endregion


            Console.WriteLine("obliczanie statystyk...");

            string tableForm = String.Format("\n#================ STATYSTYKI LISTA =============================" +
                                             String.Format("\n#{0}", tOnly ? "toleranceTriggered\tnotTriggered" : null) +
                                             String.Format("\n#n\t{0}\t{1}", statsList.Count, tOnly ? leftStatsList.Count.ToString() : null) +
                                             String.Format("\n#t\t{0:N0}ms\t{1:N0}{2}", totalTIME != 0 ? totalTIME / statsList.Count : timeSum / statsList.Count,
                                                           tOnly ? (ltimeSum / leftStatsList.Count).ToString() : null, tOnly ? "ms" : null) +
                                             String.Format("\n#e\t{0:N1}\t{1}", epochNumAve, tOnly ? String.Format("{0:N1}", lepochNumAve) : null) +
                                             "\n#" +
                                             String.Format("\n#bGMse\t{0:N4}\t{1}", bestFinalGMse, tOnly ? String.Format("{0:N4}", lbestFinalGMse) : null) +
                                             String.Format("\n#mbGMse\t{0:N4}\t{1}", gBestMseSumAve, tOnly ? String.Format("{0:N4}", lgBestMseSumAve) : null) +
                                             String.Format("\n#sigmaBGMse\t{0:N4}\t{1}", stdDevContinuous(listBGMse.ToArray()), tOnly ? String.Format("{0:N4}", stdDevContinuous(llistBGMse.ToArray())) : null) +
                                             String.Format("\n#zmmBGMse\t{0:N4}\t{1}", stdDevContinuous(listBGMse.ToArray()) / gBestMseSumAve, tOnly ? String.Format("{0:N4}", (stdDevContinuous(llistBGMse.ToArray()) / lgBestMseSumAve)) : null) +
                                             String.Format("\n#percBmbGMse\t{0:N2}\\%\t{1}{2}", ((double)gBetterTABMC / statsList.Count) * 100, tOnly ? String.Format("{0:N2}", (((double)lgBetterTABMC / leftStatsList.Count) * 100)) : null, tOnly ? "\\%" : null) +
                                             String.Format("\n#meGMse\t{0:N4}\t{1}", gMseSumAve, tOnly ? String.Format("{0:N4}", lgMseSumAve) : null) +
                                             String.Format("\n#diffBGMse\t{0:N4}\t{1}", differenceGMse, tOnly ? String.Format("{0:N4}", ldifferenceGMse) : null) +
                                             String.Format("\n#EpochToBGMse\t{0:N1}\t{1}", (double)epochBestGMSE / statsList.Count, tOnly ? String.Format("{0:N1}", ((double)lepochBestGMSE / leftStatsList.Count)) : null) +
                                             String.Format("\n#TimeToBGMse\t{0:N0}ms\t{1}", (epochBestGMSE / statsList.Count) * (timeSum / statsList.Count) / epochNumAve,
                                                           tOnly ? String.Format("{0:N0}", ((lepochBestGMSE / leftStatsList.Count) * (ltimeSum / leftStatsList.Count) / lepochNumAve)) : null, tOnly ? "ms" : null) +
                                              "\n#" +
                                             String.Format("\n#bGAcc\t{0:N2}\\%\t{1}{2}", bestFinalGAcc, tOnly ? String.Format("{0:N2}", lbestFinalGAcc) : null, tOnly ? "\\%" : null) +
                                             String.Format("\n#mbGAcc\t{0:N2}\\%\t{1}{2}", gBestAccSumAve, tOnly ? String.Format("{0:N2}", lgBestAccSumAve) : null, tOnly ? "\\%" : null) +
                                             String.Format("\n#eGAcc\t{0:N2}\\%\t{1}{2}", gAccSumAve, tOnly ? String.Format("{0:N2}", lgAccSumAve) : null, tOnly ? "\\%" : null) +
                                             "\n#" +
                                             String.Format("\n#bTMSe\t{0:N4}\t{1}", bestFinalTMse, tOnly ? String.Format("{0:N4}", lbestFinalTMse) : null) +
                                             String.Format("\n#mbTMse\t{0:N4}\t{1}", tBestMseSumAve, tOnly ? String.Format("{0:N4}", ltBestMseSumAve) : null) +
                                             String.Format("\n#sigmabTMse\t{0:N4}\t{1}", stdDevContinuous(listBTMse.ToArray()), tOnly ? String.Format("{0:N4}", stdDevContinuous(llistBTMse.ToArray())) : null) +
                                             String.Format("\n#zmmBTMse\t{0:N4}\t{1}", stdDevContinuous(listBTMse.ToArray()) / tBestMseSumAve, tOnly ? String.Format("{0:N4}", (stdDevContinuous(llistBTMse.ToArray()) / ltBestMseSumAve)) : null) +
                                             String.Format("\n#meTMSE:\t{0:N4}\t{1}", tMseSumAve, tOnly ? String.Format("{0:N4}", ltMseSumAve) : null) +
                                             String.Format("\n#diffBTMse\t{0:N4}\t{1}", differenceTMse, tOnly ? String.Format("{0:N4}", ldifferenceTMse) : null) +
                                             String.Format("\n#EpochToBTMse\t{0:N1}\t{1}", (double)epochBestTMSE / statsList.Count, tOnly ? String.Format("{0:N1}", ((double)lepochBestTMSE / leftStatsList.Count)) : null) +
                                             String.Format("\n#TimeToBTMse\t{0:N0}ms\t{1}{2}", (epochBestTMSE / statsList.Count) * (timeSum / statsList.Count) / epochNumAve,
                                                           tOnly ? String.Format("{0:N0}", ((lepochBestTMSE / leftStatsList.Count) * (ltimeSum / leftStatsList.Count) / lepochNumAve)) : null, tOnly ? "ms" : null) +
                                              "\n#" +
                                             String.Format("\n#bTAcc\t{0:N2}\\%\t{1}{2}", bestFinalTAcc, tOnly ? String.Format("{0:N2}", lbestFinalTAcc) : null, tOnly ? "\\%" : null) +
                                             String.Format("\n#mbTAcc\t{0:N2}\\%\t{1}{2}", tBestAccSumAve, tOnly ? String.Format("{0:N2}", ltBestAccSumAve) : null, tOnly ? "\\%" : null) +
                                             String.Format("\n#meTAcc\t{0:N2}\\%\t{1}{2}", tAccSumAve, tOnly ? String.Format("{0:N2}", ltAccSumAve) : null, tOnly ? "\\%" : null));

            if (!shortForm)
                return String.Format("\n#=================== STATYSTYKI ================================" +
                                     "\n#" + "Wzorzec:\t" + key + "\n#" +
                                         "Liczba przypadków:\t" + statsList.Count + "\n#" +
                                         String.Format("Średni czas obliczania:\t{0:N0}",
                                     totalTIME != 0 ? totalTIME / statsList.Count : timeSum / statsList.Count) + "ms\n#" +
                                     String.Format("Średnia liczba epok:\t{0:N2}", epochNumAve) + "\n#" +
                                     "\n#=== GMSE ======================================================" +
                                     "\n#" + "\n#Best GMSE:\t" + bestFinalGMse +
                                     "\n#Best in:\t" + bestFinalGMseFileName +
                                     String.Format("\n#Średni najlepszy GMSE:\t{0:N4}",
                                                   gBestMseSumAve) +
                                     String.Format("\n#Odchylenie std. dla najlepszy GMSE:\t{0:N4}",
                                                   stdDevContinuous(listBGMse.ToArray())) +
                                     String.Format("\n#Współczynnik zmienności dla najlepszy GMSE:\t{0:N4}",
                                                   stdDevContinuous(listBGMse.ToArray()) / gBestMseSumAve) + "\n#" +

                                     String.Format("\n#Średni końcowy GMSE:\t{0:N4}",
                                                   gMseSumAve) +
                                     String.Format("\n#Odchylenie std. dla końcowy GMSE:\t{0:N4}",
                                                   stdDevContinuous(listEGMse.ToArray())) +
                                         String.Format("\n#Współczynnik zmienności dla końcowy GMSE:\t{0:N4}",
                                                   stdDevContinuous(listEGMse.ToArray()) / gMseSumAve) + "\n#" +

                                     String.Format("\n# % wyników lepszych niż średni najlepszy:\t{0:N2}%",
                                                   ((double)gBetterTABMC / statsList.Count) * 100) + "\n#" +
                                     String.Format("Średnia różnica w momencie BestGMse {0}:\t{1:N4}",
                                                   "(BestGMse - actualTMse)", differenceGMse) + "\n#" +
                                     String.Format("Średnia liczba epok dla osiągnięcia BestTMse:\t{0:N2}",
                                                   (double)epochBestGMSE / statsList.Count) + "\n#" +
                                     String.Format("Średni czas dla osiągnięcia BestTMse:\t{0:N0}ms",
                                                   (long)(epochBestGMSE / statsList.Count) *
                                                        (timeSum / statsList.Count) / epochNumAve) + "\n#" +
                                     "\n#=== GACC ======================================================" +
                                     "\n#" + "\n#Best GAcc:\t" + bestFinalGAcc + "%" +
                                     "\n#Best in:\t" + bestFinalGAccFileName +
                                     String.Format("\n#Średni najlepszy GAcc:\t{0:N2}%",
                                                   gBestAccSumAve) +
                                     String.Format("\n#Średni końcowy GAcc:\t{0:N2}%",
                                                   gAccSumAve) + "\n#" +
                                     "\n#=== TMSE ======================================================" +
                                     "\n#" + "\n#Best TMSE:\t" + bestFinalTMse +
                                     "\n#Best in:\t" + bestFinalTMseFileName +
                                     String.Format("\n#Średni najlepszy TMSE:\t{0:N4}",
                                                   tBestMseSumAve) +
                                     String.Format("\n#Odchylenie std. dla najlepszy TMSE:\t{0:N4}",
                                                   stdDevContinuous(listBTMse.ToArray())) +
                                     String.Format("\n#Współczynnik zmienności dla najlepszy TMSE:\t{0:N4}",
                                                   stdDevContinuous(listBTMse.ToArray()) / tBestMseSumAve) + "\n#" +

                                     String.Format("\n#Średni końcowy TMSE:\t{0:N4}",
                                                   tMseSumAve) +
                                     String.Format("\n#Odchylenie std. dla końcowy TMSE:\t{0:N4}",
                                                   stdDevContinuous(listETMse.ToArray())) +
                                     String.Format("\n#Współczynnik zmienności dla końcowy TMSE:\t{0:N4}",
                                                   stdDevContinuous(listETMse.ToArray()) / tMseSumAve) + "\n#" + "\n#" +

                                     String.Format("Średnia różnica w momencie BestTMse {0}:\t{1:N4}",
                                                   "(BestTMse - actualGMse)", differenceTMse) + "\n#" +
                                     String.Format("Średnia liczba epok dla osiągnięcia BestTMse:\t{0:N2}",
                                                  (double)epochBestTMSE / statsList.Count) + "\n#" +
                                     String.Format("Średni czas dla osiągnięcia BestTMse:\t{0:N0}ms",
                                                  (long)(epochBestTMSE / statsList.Count) *
                                                       (timeSum / statsList.Count) / epochNumAve) + "\n#" +
                                     "\n#=== TACC ======================================================" +
                                     "\n#" + "\n#Best TAcc:\t" + bestFinalTAcc + "%" +
                                     "\n#Best in:\t" + bestFinalTAccFileName +
                                     String.Format("\n#Średni najlepszy TAcc:\t{0:N2}%",
                                                   tBestAccSumAve) +
                                     String.Format("\n#Średni końcowy TAcc:\t{0:N2}%",
                                                   tAccSumAve) + "\n#" +
                                         tableForm +
                                     "\n#==================== Skrypt GP ================================").
                        Replace(",", ".");

            else return String.Format("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                                      totalTIME != 0 ? totalTIME / statsList.Count : timeSum / statsList.Count,
                                      (totalTIME != 0 ? totalTIME / statsList.Count : timeSum / statsList.Count / statsList.Count) / (long)epochNumAve,
                                      statsList.Count,
                                     String.Format("{0:N4}\t{1:N4}\t{2:N4}", bestFinalGMse,
                                                   gBestMseSumAve, gMseSumAve),
                                     String.Format("{0:N2}\\%\t{1:N2}\\%\t{2:N2}\\%", bestFinalGAcc,
                                                   gBestAccSumAve, gAccSumAve),
                                     String.Format("{0:N4}\t{1:N4}\t{2:N4}", bestFinalTMse,
                                                   tBestMseSumAve, tMseSumAve),
                                     String.Format("{0:N2}\\%\t{1:N2}\\%\t{2:N2}\\%", bestFinalTAcc,
                                                   tBestAccSumAve, tAccSumAve)).Replace(",", ".");
        }

        public bool Create()
        {
            if (fileNames.Count == 0)
            {
                Console.WriteLine("Nie znaleziono plików odpowiadających wzorcowi!");
                return false;
            }
            try
            {
                TextWriter scriptWriter = new StreamWriter(scriptFileName);


                scriptWriter.WriteLine(statistics);

                scriptWriter.WriteLine(header);
                scriptWriter.Flush();

                foreach (string s in linestyles)
                    scriptWriter.WriteLine(s);
                scriptWriter.Flush();

                int c;
                if (fileNames.Count > 150)
                    c = 150;
                else
                    c = fileNames.Count;

                scriptWriter.Write("plot ");
                for (int i = 0; i < c; i++)
                {
                    scriptWriter.Write("\"{0}\" using {1} with lines ls 1",
                                       fileNames[i], columnUsed);
                    if (!(i + 1 == fileNames.Count))
                        scriptWriter.Write(", ");
                    scriptWriter.Flush();
                }

                scriptWriter.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Nieudany zapis: ", e.Message);
                return false;
            }

            return true;
        }

        public static List<string> CreateListOfFiles(string path, string keyNames)
        {
            string[] filePaths = Directory.GetFiles(@path, keyNames);
            List<string> list = new List<string>();

            if (!System.Environment.OSVersion.VersionString.Contains("Windows"))
            {
                foreach (string s in filePaths)
                {
                    string[] tmp = s.Split('/');
                    list.Add(tmp[tmp.Length - 1]);
                }
            }
            else
            {
                return filePaths.ToList();
            }
            return list;
        }

        static void createHeader(ref string header, string filename, string title,
                                  string label)
        {
            header = String.Format("set term postscript eps enhanced");
            header = String.Format("{0}\nset output \"{1}\"", header, filename);
            header = String.Format("{0}\n{3}\nset title \"{1}\"\nset label \"{2}\"",
                                   header, title, label, "set encoding iso_8859_2");
            header = String.Format("{0}\nset xlabel \"Liczba epok\"\nset ylabel \"{1}\"",
                                   header, "Mean squared error");
            header = String.Format("{0}\nset nokey\nset grid",
                                   header);
        }

        static void createLineStyle(ref List<string> lineStyles)
        {
            lineStyles = new List<string>();
            lineStyles.Add("set style line 1 lc rgb \"black\" lw 1");
        }

        private static double stdDevContinuous(double[] doubleList)
        {
            double average = 0;
            double sumOfDerivation = 0;
            foreach (double val in doubleList)
            {
                average += val;
                sumOfDerivation += (val) * (val);
            }
            average /= doubleList.Length;
            double sumOfDerivationAverage = sumOfDerivation / doubleList.Length;
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }
    }
}

