using System;
using MyData;

namespace FunahashiNeuralNetwork
{
	partial class BackpropagationTest
	{
		public static void BackpropagationLearningMain ()
		{
			int option;
			while(!selectFunction(out option)){ }
			
			string inputFilename, resultFilename;
			int hiddenRatio, pointNumber;
			double learningRate;
			bool useBatch = false;
			ulong maxEpoch = 100000;
			
			double domainFrom, domainTo;
			
			switch(option)
			{
			case 1:
			default:
				inputFilename = "funkcja1.dat";
				resultFilename = "wyniki1-backprop.dat";
				hiddenRatio = 9;
				learningRate = 0.001;
				domainFrom = 0;
				domainTo = 2 * Math.PI;
                //można wygenerować wartości lub wczytać dane - tutaj generowane do pliku, potem wczytane
                if (generateSet())
                {
                    pointNumber = 100;
                    setPointsNumber(ref pointNumber);
					DataGenerator.Function1a(inputFilename, pointNumber); //dla \eps [-0.05, 0.05]
                    //DataGenerator.Function1b(inputFilename, pointNumber); //dla \eps [-0.1, 0.1]
                }
                else
                    Console.WriteLine("Nazwa pliku z danymi powinna się nazywać: \"{0}\"", inputFilename);
				break;
			case 2:
				inputFilename = "funkcja2.dat";
                resultFilename = "wyniki2-backprop.dat";
				learningRate = 0.01;
				hiddenRatio = 24;
				domainFrom = 0;
				domainTo = 1;
                //można wygenerować wartości lub wczytać dane - tutaj generowane do pliku, potem wczytane
                if (generateSet())
                {
                    pointNumber = 100;
                    setPointsNumber(ref pointNumber);
                    DataGenerator.Function2(inputFilename, pointNumber);
                }
                else
                    Console.WriteLine("Nazwa pliku z danymi powinna się nazywać: \"{0}\"", inputFilename);
				break;
			case 3:
				inputFilename = "funkcja3.dat";
                resultFilename = "wyniki3-backprop.dat";
				learningRate = 0.01;
				hiddenRatio = 120;
				domainFrom = -5;
				domainTo = 5;
                //można wygenerować wartości lub wczytać dane - tutaj generowane do pliku, potem wczytane
                if (generateSet())
                {
                    pointNumber = 1000;
                    setPointsNumber(ref pointNumber);
                    DataGenerator.Function3(inputFilename, pointNumber);
                }
                else
                    Console.WriteLine("Nazwa pliku z danymi powinna się nazywać: \"{0}\"", inputFilename);
				break;
			}
						
			setHiddenRatio(ref hiddenRatio);
			setLearningRate(ref learningRate);
            setMaxEpoch(ref maxEpoch);
            setUseBatch(ref useBatch);

            new Backpropagation(hiddenRatio, learningRate, maxEpoch, 
                inputFilename, resultFilename, domainFrom, domainTo).Train();
            Console.WriteLine("Zakończono nauczanie sieci! (Naciśnij dowolny przycisk aby powrócić do Menu)");
            Console.ReadKey();
		}
    }
}

