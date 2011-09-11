using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LearningBPandLM
{
    class StatisticsConfig
    {
        static bool endStatFlag = false, dtSelected = false,
            aSelectedLM = false;

        static EnumDatasetStructures dtS;

        private static string dtPar, hPar;

        public static string Path = Directory.GetCurrentDirectory();

        public static void StatisticsMenu()
        {
            Program.PrintInfo("Moduł statystyczny");
            statInfo();

            while (!endStatFlag)
            {
                Menu statMenu = new Menu();

                statMenu.Add("Zobacz listę gotowych przykładowych wzorców", readyStats);
                statMenu.Add("Wybierz zbiór, algorytm i wygeneruj statystyki i plik *gp", statCreate);

                statMenu.Add("Informacja", statInfo);
                statMenu.Add("Zakończ", statEnd);

                Program.PrintLongLine();
                statMenu.Show();
            }
        }

        private static void statCreate()
        {
            if (!dtSelected)
            {
                Console.WriteLine("Najpierw musisz wybrać wzorzec zbioru!");
                return;
            }
            else
            {
                selectAlgorithm();
                selectDataset();
            }
        }

        private static void selectDataset()
        {
            Program.PrintLongLine();
            Menu selectDataMenu = new Menu();
            selectDataMenu.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
            selectDataMenu.Add("[wybor danych] Ustaw Opcje dla GermanCreditData", SetToGermanCreditData);
            selectDataMenu.Add("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
            selectDataMenu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
            selectDataMenu.Show();
        }

        private static void SetToHeartDisease() { SetFunc(1); }
        private static void SetToGermanCreditData() { SetFunc(2); }
        private static void SetToCreditRisk() { SetFunc(3); }
        private static void SetToLetterRecognition() { SetFunc(4); }

        private static void SetFunc(int opt)
        {
            if (!aSelectedLM)
            {
                switch (opt)
                {
                    case 1:
                        BP_Heart_gMSE(dtPar, hPar, Path);
                        break;
                    case 2:
                        BP_GermanCreditData_gMSE(dtPar, hPar, Path);
                        break;
                    case 3:
                        BP_CreditRisk_gMSE(dtPar, hPar, Path);
                        break;
                    case 4:
                        BP_LetterRecognition_gMSE(dtPar, hPar, Path);
                        break;
                    default:
                        return;
                }
            }
            else
            {
                if (dtPar.Equals("D"))
                {
                    Console.WriteLine("Wybrano jako typ zbioru \"D\", czyli zbiór additional dla którego potrzebny jest dodatkowy parametr 't', podaj jego wartość:");
                    {
                        double dOpt;
                        try
                        {
                            dOpt = Double.Parse(Console.ReadLine().Replace(".", ","));
                        }
                        catch
                        {
                            Console.WriteLine("Niepowodzenie w parsowaniu wejścia, powrót do menu...");
                            return;
                        }
                        switch (opt)
                        {
                            case 1:
                                LM_Heart_gMSETTolerance(dtPar, hPar, Path, dOpt.ToString());
                                break;
                            case 2:
                                LM_GermanCreditData_gMSERegTTolerance(dtPar, hPar, Path, dOpt.ToString());
                                break;
                            case 3:
                                LM_CreditRisk_gMSERegTTolerance(dtPar, hPar, Path, dOpt.ToString());
                                break;
                            case 4:
                                LM_LetterRecognitionA_gMSERegTTolerance(dtPar, hPar, Path, dOpt.ToString());
                                break;
                            default:
                                return;
                        }
                    }
                }
                else
                {
                    switch (opt)
                    {
                        case 1:
                            LM_Heart_gMSEReg(dtPar, hPar, Path);
                            break;
                        case 2:
                            LM_GermanCreditData_gMSEReg(dtPar, hPar, Path);
                            break;
                        case 3:
                            LM_CreditRisk_gMSEReg(dtPar, hPar, Path);
                            break;
                        case 4:
                            LM_LetterRecognitionA_gMSEReg(dtPar, hPar, Path);
                            break;
                        default:
                            return;
                    }
                }
            }
        }

        private static void selectAlgorithm()
        {
            Program.PrintLongLine();
            Console.WriteLine("Wybierz algorytm dla którego wyniki mają być analizowane (1/2): \n\t{0}\n\t{1}",
                "[1] BP, propagacja wsteczna błędu", "[2] LM, levenberg-marquardt");
            try
            {
                int opt = Int32.Parse(Console.ReadLine());

                switch (opt)
                {
                    case 1:
                        aSelectedLM = false;
                        break;
                    case 2:
                        aSelectedLM = true;
                        break;
                    default:
                        throw new Exception();
                }
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, zostanie zastosowany parametr domyślny (1 - BP)");
            }
        }

        private static void printDatasetTypes()
        {
            Console.WriteLine("Oznaczenia:");
            Console.WriteLine("\"W\" - Windowed (okienkowy)");
            Console.WriteLine("\"V\" - WindowedNoRandom (okienkowy bez random()");
            Console.WriteLine("\"G\" - Growing (rosnący)");
            Console.WriteLine("\"S\" - Simple (prosty, brak podziałów)");
            Console.WriteLine("\"D\" - Additional (dodatkowy podział specjalny dla LM)");
        }

        private static void readyStats()
        {
            Program.PrintLongLine();
            printDatasetTypes();
            Console.WriteLine("Gotowe wzorce:");
            Console.WriteLine("\t[1] Sg50");
            Console.WriteLine("\t[2] Vg50");
            Console.WriteLine("\t[3] Dg50");
            Console.WriteLine("\t[4] Sg80");
            Console.WriteLine("\t[5] Vg80");
            Console.WriteLine("\t[6] Dg50");
            Console.WriteLine("\t[7] Zdefiniuj własny wzorzec");

            Console.WriteLine("\n\t[8] Powrót");

            if (dtSelected)
                Console.WriteLine("Aktualny wzorzec zbioru: {0}g{1}", dtPar, hPar);


            try
            {
                int opt = Int32.Parse(Console.ReadLine());

                switch (opt)
                {
                    case 1:
                        dtPar = "S";
                        hPar = "g50";
                        break;
                    case 2:
                        dtPar = "V";
                        hPar = "g50";
                        break;
                    case 3:
                        dtPar = "D";
                        hPar = "g50";
                        break;
                    case 4:
                        dtPar = "S";
                        hPar = "g80";
                        break;
                    case 5:
                        dtPar = "V";
                        hPar = "g80";
                        break;
                    case 6:
                        dtPar = "D";
                        hPar = "g80";
                        break;
                    case 7:
                        if (statDtCreator())
                        {
                            Console.WriteLine("Wzorzec został zdefiniowany");
                        }
                        else
                        {
                            Console.WriteLine("Niepowodzenie w definiowaniu wzorca");
                            return;
                        }
                        break;
                    case 8:
                        return;
                    default:
                        throw new Exception("");
                }
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość");
                return;
            }

            dtSelected = true;
            Console.WriteLine("\nWybrano wzorzec zbioru: {0}g{1}", dtPar, hPar);
        }

        private static bool statDtCreator()
        {
            printDatasetTypes();
            Console.WriteLine("Podaj oznaczenie typu zbioru:");
            string sOpt = Console.ReadLine();
            if (!(sOpt.Equals("W") || sOpt.Equals("V") || sOpt.Equals("G") || sOpt.Equals("S") || sOpt.Equals("D")))
            {
                Console.WriteLine("Nieznane oznaczenie zbioru, powrót do menu...");
                return false;
            }
            else
            {
                dtPar = sOpt;
            }
            int iOpt = new int();
            Console.WriteLine("Dla jakiej wielkości zbioru (0-100), obecnie: g{0}", hPar);
            try
            {
                iOpt = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            if (iOpt > 101 || iOpt < 0)
            {
                Console.WriteLine("Niepoprawna wartość, powinna wynosić pomiędzy (0-100)%");
                return false;

            }
            hPar = iOpt.ToString();
            Console.WriteLine("Utworzono wzorzec zbioru: {0}g{1}", dtPar, hPar);

            return true;
        }

        private static void statInfo()
        {
            Console.WriteLine("Można wybrać jeden z gotowych wzorców lub zdefiniować własny.\n{0}{1}{2}",
                "info: wzorzec jest potrzebny do filtrowania nazw plików z wynikami (wyrażenie regularne), ",
                "najpierw wybierany jest wzorzec zbioru (typ, rozmiar), a następnie wybierany ",
                "jest algorytm i zbiór, dla którego zbioru ma zostać zastosowany.");
        }

        private static void statEnd()
        {
            Console.WriteLine("Moduł zakończy działanie, powrót do menu...\n");
            endStatFlag = true;
        }

        /// <summary>
        /// Funkcja do tworzenia wielu statystyk - nalezy definiować je manualnie
        /// </summary>
        private static void manualStatistics()
        {

            string[] dataSetTypes = { "V", "S", "D"/**/};
            string[] holdouts = {/**/"95", "80", "50", "85" };

            foreach (string dt in dataSetTypes)
            {
                foreach (string h in holdouts)
                {
                    BP_Heart_gMSE(dt, h, Path);
                    LM_Heart_gMSEReg(dt, h, Path);

                    BP_GermanCreditData_gMSE(dt, h, Path);
                    LM_GermanCreditData_gMSEReg(dt, h, Path);

                    BP_CreditRisk_gMSE(dt, h, Path);
                    LM_CreditRisk_gMSEReg(dt, h, Path);
                    LM_CreditRisk_gMSERegT(dt, h, Path);

                    LM_CreditRisk_gMSERegTTolerance(dt, h, Path, 0.1.ToString());
                    LM_Heart_gMSETTolerance(dt, h, Path, 0.1.ToString());
                    LM_Heart_gMSETTolerance(dt, h, Path, 0.1.ToString());

                    BP_LetterRecognition_gMSE(dt, h, Path);
                    LM_LetterRecognitionA_gMSEReg(dt, h, Path);

                    LM_CreditRisk_gMSE_SVD(dt, h, Path);
                    LM_CreditRisk_gMSE_PINV(dt, h, Path);

                }
            }

            /*string[,] configForHeartDiseaseS85 = new string [2,2] 
                {{"*HeartDisease*BP*Sg85*", "dataSummary_S85_HeartDiseaseBP.dat"},
                {"*HeartDisease*LM*Sg85*", "dataSummary_S85_HeartDiseaseLMReg.dat"}};
            createStatsBySize(configForHeartDiseaseS85, path, 0, 4);
			
            string[,] configForHeartDiseaseD85 = new string [2,2] 
                {{"*HeartDisease*LM*Dg85*", "dataSummary_D85_HeartDiseaseLMReg.dat"},
                {"*HeartDisease*LM*Sg85*", "dataSummary_S85_HeartDiseaseLMReg.dat"}};
            createStatsBySize(configForHeartDiseaseD85, path, 0, 4);*/
            /*			
                        string[,] configForCreditRiskSV85 = new string [2,2] 
                            {{"*CreditRisk*BP*Sg85*", "dataSummary_S85_CreditRiskBP.dat"},
                            {"*CreditRisk*BP*Vg85*", "dataSummary_V85_CreditRiskBP.dat"}};
                        createStatsBySize(configForCreditRiskSV85, path, 0, 6);
				
                        string[,] configForCreditRiskSD85 = new string [2,2] 
                            {{"*CreditRisk*LM*Sg85*", "dataSummary_S85_CreditRiskLM.dat"},
                            {"*CreditRisk*LM*Dg85*", "dataSummary_D85_CreditRiskLM.dat"}};
                        createStatsBySize(configForCreditRiskSD85, path, 0, 6);
				
                        string[,] configForHeartDiseaseVD85 = new string [2,2] 
                            {{"*HeartDisease*BP*Vg85", "dataSummary_V85_HeartDiseaseBP.dat"},
                            {"*HeartDisease*LM*Dg85*", "dataSummary_D85_HeartDiseaseLMReg.dat"}};
                        createStatsBySize(configForHeartDiseaseVD85, path, 0, 6);
            */
            string[,] configForHeartDiseaseSD85 = new string[2, 2] 
				{{"*HeartDisease*LM*Sg85*", "dataSummary_S85_HeartDiseaseLMReg.dat"},
				{"*HeartDisease*LM*Dg85*", "dataSummary_D85_HeartDiseaseLMReg.dat"}};
            createStatsBySize(configForHeartDiseaseSD85, Path, 0, 6);


            /*
            string[,] configForHeartDisease = new string [2,2] 
                {{"*HeartDisease*BP*", "dataSummaryHeartDiseaseBP.dat"},
                {"*HeartDisease*LM*", "dataSummaryHeartDiseaseLMReg.dat"}};
            createStatsBySize(configForHeartDisease, path, 0, 32);
			
            string[,] configForGermanCreditData = new string [2,2] 
                {{"*GermanCreditData*BP*", "dataSummaryGermanCreditDataBP.dat"},
                {"*GermanCreditData*LM*", "dataSummaryGermanCreditDataLMReg.dat"}};
            createStatsBySize(configForGermanCreditData, path, 0, 32);
			
            string[,] configForCreditRiskV = new string [2,2] 
                {{"*CreditRisk*BP*V*", "dataSummaryCreditRiskBP_V.dat"},
                {"*CreditRisk*LM*V*", "dataSummaryCreditRiskLMReg_V.dat"}};
            createStatsBySize(configForCreditRisk, path, 0, 32);
			
            string[,] configForLetterRecognition = new string [2,2] 
                {{"*LetterRecognition*BP*", "dataSummaryLetterRecognitionBP.dat"},
                {"*LetterRecognition*LM*", "dataSummaryLetterRecognitionLMReg.dat"}};
            createStatsBySize(configForLetterRecognition, path, 0, 32);
            */

        }

        static void createStatsBySize(string[,] configs, string path, int fromN, int toN)
        {
            string headerData = String.Format("#{0}\t{1}\t{2}\t{3}\t{4}",
                                          String.Format("{0}\t{1}\t{2}\t{3}",
                                          "hNum", "mTime", "meTime", "numN"),
                                          String.Format("{0}\t{1}\t{2}",
                                          "bGMSE", "mbGMSE", "meGMSE"),
                                          String.Format("{0}\t{1}\t{2}",
                                          "bGAcc", "mbGAcc", "meGAcc"),
                                          String.Format("{0}\t{1}\t{2}",
                                          "bTMSE", "mbTMSE", "meTMSE"),
                                          String.Format("{0}\t{1}\t{2}",
                                          "bTAcc", "mbTAcc", "meTAcc"));

            for (int i = 0; i < configs.GetLongLength(0); i++)
            {
                Console.WriteLine("\nOdliczanie statystyk według wielkości\n\t[{0}]",
                                  configs[i, 0]);
                List<string> statsBySize = new List<string>();
                statsBySize.Add(headerData);
                List<string> list;
                for (int num = fromN; num < toN; num++)
                {

                    string key = String.Format("{0}-{1}-*.txt", configs[i, 0], num);
                    list = CreateScript.CreateListOfFiles(path, key);
                    for (int p = list.Count; p != 0; p--)
                    {
                        if (list[p - 1].Contains(String.Format("-{0}-Reg", num)) ||
                           list[p - 1].Contains(String.Format("-{0}-SVD", num)) ||
                           list[p - 1].Contains(String.Format("-{0}-PIN", num)) ||
                           list[p - 1].Contains(String.Format("-{0}-Non", num)))
                        {
                            string[] tmp = list[p - 1].Split('-');
                            int c = 0;
                            foreach (string s in tmp)
                            {
                                if (s.Equals(num.ToString()))
                                    c++;
                            }
                            if (c == 1)
                                list.RemoveAt(p - 1);
                        }

                    }
                    if (list.Count != 0)
                    {
                        if (configs[i, 0].Contains("Dg"))
                        {
                            statsBySize.Add(String.Format("{0}{1}", num,
                                    CreateScript.CreateStatistics(list, key, true, true)));
                        }
                        else
                            statsBySize.Add(String.Format("{0}{1}", num,
                                        CreateScript.CreateStatistics(list, key, true, false)));
                    }
                }

                if (SaveDataStats(configs[i, 1], statsBySize))
                {
                    Console.WriteLine("Utworzono [{0}]!", configs[i, 1]);
                }
            }

            int columnDrawn = 6;
            string[] h = headerData.Split((char)9);

            string headerScript =
                String.Format("set term postscript eps enhanced\n" +
                              String.Format("set output \"{0}\"", configs[0, 1].
                                            Replace(".dat", ".eps")) +
                              "\nset grid" +
                              "\n\nset data style boxes\nset boxwidth 0.35 relative\n\n" +
                              String.Format("\nset ylabel \"{0}\"", h[columnDrawn - 1]) +
                              String.Format("\nset xlabel \"Liczba jednostek ukrytych\"") +
                              String.Format("\nset title \"{0}\"",
                                            configs[1, 1].Replace(".dat", "")));
            List<string> scriptBySize = new List<string>();
            scriptBySize.Add(headerScript);



            for (int i = 0; i < configs.GetLongLength(0); i++)
            {
                if (i != 0)
                    scriptBySize.Add(String.Format("\"{0}\" using ($1+0.35):(${2}) title \"{1}\" fs solid 0.25 lw 3",
                                                   configs[i, 1], configs[i, 0], columnDrawn));
                else
                    scriptBySize.Add(String.Format("\nplot \"{0}\" using 1:{3} title \"{1}\" fs solid 0.75{2}",
                                               configs[i, 1], configs[i, 0],
                                                   configs.GetLength(0) < i + 1 ? "" : ",\\", columnDrawn));
            }

            if (scriptBySize.Count == 1)
                return;

            if (SaveDataStats(configs[1, 1].Replace(".dat", ".gp"), scriptBySize))
            {
                Console.WriteLine("Utworzono [{0}]!", configs[0, 1].
                              Replace(".dat", ".gp"));
            }
        }

        public static bool SaveDataStats(string dataName, List<string> toWrite)
        {
            try
            {
                using (TextWriter writeFile = new StreamWriter(dataName))
                {
                    foreach (string s in toWrite)
                    {
                        writeFile.WriteLine(s);
                        writeFile.Flush();
                    }

                    writeFile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        #region >>>> BP
        public static void BP_LetterRecognition_gMSE(string dt, string holdout, string path)
        {

            string keyName = String.Format("*LetterRecognition*BP*{0}g{1}*.txt", dt, holdout),
                scriptFileName = String.Format("letterrecognitionaBP_{0}gMSE{1}.gp", dt, holdout),
                output = String.Format("LetterRecognition_BP_{0}gMSE{1}.eps", dt, holdout),
                title = String.Format("gMSE{0} BP LetterRecognition-data", holdout),
                label = String.Format("gMSE{0}", holdout),
                columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
            else
            {
                Console.WriteLine("Tworzenie skryptu nie powiodło się!");
            }
        }


        public static void BP_GermanCreditData_gMSE(string dt, string holdout, string path)
        {

            string keyName = String.Format("*GermanCreditData*BP*{0}g{1}*.txt", dt, holdout),
            scriptFileName = String.Format("germancreditdataBP_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("GermanCreditData_BP_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} BP GermanCreditData-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
            else
            {
                Console.WriteLine("Tworzenie skryptu nie powiodło się!");
            }
        }


        public static void BP_CreditRisk_gMSE(string dt, string holdout, string path)
        {

            string keyName = String.Format("*CreditRisk*BP*{0}g{1}*.txt", dt, holdout),
            scriptFileName = String.Format("creditriskBP_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("CreditRisk_BP_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} BP CreditRisk-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
            else
            {
                Console.WriteLine("Tworzenie skryptu nie powiodło się!");
            }
        }


        public static void BP_Heart_gMSE(string dt, string holdout, string path)
        {

            string keyName = String.Format("*HeartDisease*BP*{0}g{1}*.txt", dt, holdout),
            scriptFileName = String.Format("heartdiseaseBP_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("HeartDisease_BP_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} BP HeartDisease-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
            else
            {
                Console.WriteLine("Tworzenie skryptu nie powiodło się!");
            }
        }

        #endregion

        #region >>>> LM
        public static void LM_LetterRecognitionA_gMSEReg(string dt, string holdout, string path)
        {

            string keyName = String.Format("*LetterRecognition*LM*{0}g{1}*Reg*F.txt", dt, holdout),
            scriptFileName = String.Format("letterrecognitionaLMReg_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("LetterRecognitionA_LM_Reg_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} LM LetterRecognitionA-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_GermanCreditData_gMSEReg(string dt, string holdout, string path)
        {

            string keyName = String.Format("*GermanCreditData*LM*{0}g{1}*Reg*F.txt", dt, holdout),
            scriptFileName = String.Format("germancreditdataLMReg_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("GermanCreditData_LM_Reg_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} LM GermanCreditData-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_CreditRisk_gMSEReg(string dt, string holdout, string path)
        {

            string keyName = String.Format("*CreditRisk*LM*{0}g{1}*Reg*F.txt", dt, holdout),
            scriptFileName = String.Format("creditriskLMReg_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("CreditRisk_LM_Reg_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} LM CreditRisk-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_CreditRisk_gMSERegT(string dt, string holdout, string path)
        {

            string keyName = String.Format("*CreditRisk*LM*{0}g{1}*Reg*T.txt", dt, holdout),
            scriptFileName = String.Format("creditriskLMReg_{0}gMSE{1}T.gp", dt, holdout),
            output = String.Format("CreditRisk_LM_Reg_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} LM CreditRisk-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, true).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_Heart_gMSEReg(string dt, string holdout, string path)
        {

            string keyName = String.Format("*Heart*LM*{0}g{1}*Reg*F.txt", dt, holdout),
            scriptFileName = String.Format("heartdiseaseLMReg_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("HeartDisease_LM_Reg_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} LM HeartDisease-data", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }
        #endregion

        #region special
        public static void LM_CreditRisk_gMSE_PINV(string dt, string holdout, string path)
        {

            string keyName = String.Format("*CreditRisk*LM*{0}g{1}*PINV*.txt", dt, holdout),
            scriptFileName = String.Format("creditriskLM_PINV_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("CreditRisk_LM_PINV_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} LM CreditRisk-data PINV", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_CreditRisk_gMSE_SVD(string dt, string holdout, string path)
        {

            string keyName = String.Format("*CreditRisk*LM*{0}g{1}*SVD*.txt", dt, holdout),
            scriptFileName = String.Format("creditriskLM_SVD_{0}gMSE{1}.gp", dt, holdout),
            output = String.Format("CreditRisk_LM_SVD_{0}gMSE{1}.eps", dt, holdout),
            title = String.Format("gMSE{0} LM CreditRisk-data SVD", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_Heart_gMSETTolerance(string dt, string holdout, string path, string tol)
        {
            string keyName = String.Format("*Heart*LM*{0}g{1}*T{2}.txt", dt, holdout, tol),
            scriptFileName = String.Format("heartdiseaseLMReg_{0}gMSE{1}T{2}.gp", dt, holdout),
            output = String.Format("HeartDisease_LM_Reg_{0}gMSE{1}T010.eps", dt, holdout),
            title = String.Format("gMSE{0} LM HeartDisease-data T010", holdout),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, true).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_CreditRisk_gMSERegTTolerance(string dt, string holdout, string path, string tol)
        {
            string keyName = String.Format("*CreditRisk*LM*{0}g{1}*Reg*T{2}.txt", dt, holdout, tol),
            scriptFileName = String.Format("creditriskLMReg_{0}gMSE{1}T{2}.gp", dt, holdout, tol),
            output = String.Format("CreditRisk_LM_Reg_{0}gMSE{1}T{2}.eps", dt, holdout, tol),
            title = String.Format("gMSE{0} LM CreditRisk-data T{1}", holdout, tol),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, true).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_LetterRecognitionA_gMSERegTTolerance(string dt, string holdout, string path, string tol)
        {

            string keyName = String.Format("*LetterRecognition*LM*{0}g{1}*Reg*T{2}.txt", dt, holdout, tol),
            scriptFileName = String.Format("letterrecognitionaLMReg_{0}gMSE{1}T{2}.gp", dt, holdout, tol),
            output = String.Format("LetterRecognitionA_LM_Reg_{0}gMSE{1}T{2}.eps", dt, holdout, tol),
            title = String.Format("gMSE{0} LM LetterRecognitionA-data T{1}", holdout, tol),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, true).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }

        public static void LM_GermanCreditData_gMSERegTTolerance(string dt, string holdout, string path, string tol)
        {

            string keyName = String.Format("*GermanCreditData*LM*{0}g{1}*Reg*T{2}.txt", dt, holdout, tol),
            scriptFileName = String.Format("germancreditdataLMReg_{0}gMSE{1}T{2}.gp", dt, holdout, tol),
            output = String.Format("GermanCreditData_LM_Reg_{0}gMSE{1}T{2}.eps", dt, holdout, tol),
            title = String.Format("gMSE{0} LM GermanCreditData-data T{1}", holdout, tol),
            label = String.Format("gMSE{0}", holdout),
            columnUsed = "1:4";

            if (new CreateScript(path,
                                keyName,
                                scriptFileName,
                                output, title,
                                label, columnUsed, false).Create())
            {
                Console.WriteLine("Tworzenie skryptu zakończone sukcesem!\n");
            }
        }
        #endregion

    }
}
