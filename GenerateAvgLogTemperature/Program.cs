using System;
using System.IO;

// See https://aka.ms/new-console-template for more information
class Program
{
    static void Main(string[] args)
    {

        if (args.Length < 1)
        {
            Console.WriteLine("Parameter erforderlich: Dateiname der Input-Datei [readingString writeStatString]");
            Console.WriteLine("");
            Console.WriteLine("z.B.  GenerateAvgLogTemperature.exe C:\\Temp\\logfile.txt \"temperature:\" \"eg.aussen.wettersensor statTemperature\"");
            Console.WriteLine("oder  GenerateAvgLogTemperature.exe C:\\Temp\\logfile.txt \"Temperaturabweichung:\" \"heizung.eg.whz.hk34 statTemperaturabweichung\"");
            return;
        }
        string inputFilePathOriginal = args[0];
        string inputFilePath = Path.ChangeExtension(inputFilePathOriginal,"oldlog");
        File.Copy(inputFilePathOriginal, inputFilePath, true);
        string outputFilePath = inputFilePathOriginal;
        string readingString = "temperature:"; // oder Temperaturabweichung:
        string writeStatString = "eg.aussen.wettersensor statTemperature"; // oder heizung.eg.whz.hk34 statTemperaturabweichung
        if (args.Length > 2)
        {
            readingString   = args[1];
            writeStatString = args[2];
        }

        try
        {
            using (StreamReader reader = new StreamReader(inputFilePath))
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                string      line;
                int         lastMonth               = -1;
                double      sumOfTempsInMonth       = 0;
                int         counterOfTempsInMonth   = 0;
                int         lastDay                 = -1;
                double      sumOfTempsInDay         = 0;
                int         counterOfTempsInDay     = 0;
                int         lastYear                = -1;
                double      sumOfTempsInYear        = 0;
                int         counterOfTempsInYear    = 0;
                DateTime    lastLineDateTime        = DateTime.MinValue;

                while ((line = reader.ReadLine()!) != null)
                {
                    if (line != null)
                    {
                        // Eine Zeile sieht so aus: "2024-01-01_00:10:03 eg.aussen.wettersensor temperature: 8.7"
                        string[] parts = line.Split(' ');
                        
                        // Wir wollen nur Zeilen parsen, die mit "temperature:" beginnen
                        if(readingString == parts[2])
                        {
                            string dateTimePart     = parts[0];
                            string temperaturePart  = parts[3];

                            DateTime    currentLineDateTime = DateTime.ParseExact(dateTimePart, "yyyy-MM-dd_HH:mm:ss", null);
                            double      currentLineTemp     = double.Parse(temperaturePart, System.Globalization.CultureInfo.InvariantCulture);

                            // Durchschnittliche Temperatur berechnen
                            if (currentLineDateTime.Month != lastMonth)
                            {
                                if (lastMonth != -1)
                                {
                                    double averageTemp = sumOfTempsInMonth / counterOfTempsInMonth;
                                    // Schreibe den Durchschnitt in die Ausgabedatei im Format "2024-12-21_23:59:58 eg.aussen.wettersensor statTemperatureDayAvgLast: 8.7"
                                    writer.WriteLine($"{lastLineDateTime.Year}-{lastLineDateTime.Month:D2}-{lastLineDateTime.Day:D2}_{lastLineDateTime.Hour:D2}:{lastLineDateTime.Minute:D2}:{lastLineDateTime.Second:D2} {writeStatString}MonthAvgLast: {averageTemp.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
                                }
                                lastMonth               = currentLineDateTime.Month;
                                sumOfTempsInMonth       = 0;
                                counterOfTempsInMonth   = 0;
                            }
                            if (currentLineDateTime.Day != lastDay)
                            {
                                if (lastDay != -1)
                                {
                                    double averageTemp = sumOfTempsInDay / counterOfTempsInDay;
                                    // Schreibe den Durchschnitt in die Ausgabedatei im Format "2024-12-21_23:59:58 eg.aussen.wettersensor statTemperatureDayAvgLast: 8.7"
                                    writer.WriteLine($"{lastLineDateTime.Year}-{lastLineDateTime.Month:D2}-{lastLineDateTime.Day:D2}_{lastLineDateTime.Hour:D2}:{lastLineDateTime.Minute:D2}:{lastLineDateTime.Second:D2} {writeStatString}DayAvgLast: {averageTemp.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
                                }
                                lastDay             = currentLineDateTime.Day;
                                sumOfTempsInDay     = 0;
                                counterOfTempsInDay = 0;
                            }
                            if (currentLineDateTime.Year != lastYear)
                            {
                                if (lastYear != -1)
                                {
                                    double averageTemp = sumOfTempsInYear / counterOfTempsInYear;
                                    // Schreibe den Durchschnitt in die Ausgabedatei im Format "2024-12-21_23:59:58 eg.aussen.wettersensor statTemperatureYearAvgLast: 8.7"
                                    writer.WriteLine($"{lastLineDateTime.Year}-{lastLineDateTime.Month:D2}-{lastLineDateTime.Day:D2}_{lastLineDateTime.Hour:D2}:{lastLineDateTime.Minute:D2}:{lastLineDateTime.Second:D2} {writeStatString}YearAvgLast: {averageTemp.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
                                }
                                lastYear                = currentLineDateTime.Year;
                                sumOfTempsInYear        = 0;
                                counterOfTempsInYear    = 0;
                            }
                           
                            sumOfTempsInYear += currentLineTemp;
                            counterOfTempsInYear++;
                            sumOfTempsInDay += currentLineTemp;
                            counterOfTempsInDay++;
                            sumOfTempsInMonth += currentLineTemp;
                            counterOfTempsInMonth++;
                            lastLineDateTime = currentLineDateTime;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                        // Wir wollen die Temperatur auslesen und in eine Dezimalzahl umwandeln                         
                        // Schreibe die verarbeitete Zeile in die Ausgabedatei
                        writer.WriteLine(line);
                    } // if (line != null)
             
                }
                {
                    // Am Ende der Datei noch das letzte Jahr ausgeben, da die Dateie jährlich organisiert ist 
                    double averageTemp = sumOfTempsInYear / counterOfTempsInYear;
                    // Schreibe den Durchschnitt in die Ausgabedatei im Format "2024-12-21_23:59:58 eg.aussen.wettersensor statTemperatureYearAvgLast: 8.7"
                    writer.WriteLine($"{lastLineDateTime.Year}-{lastLineDateTime.Month:D2}-{lastLineDateTime.Day:D2}_{lastLineDateTime.Hour:D2}:{lastLineDateTime.Minute:D2}:{lastLineDateTime.Second:D2} {writeStatString}YearAvgLast: {averageTemp.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
                }
            }

            Console.WriteLine("Datei wurde erfolgreich verarbeitet.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler: " + ex.Message);
        }
    }
}
