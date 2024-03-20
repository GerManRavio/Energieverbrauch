using System;
using System.IO;

namespace Energie
{
    internal class Program
    {
        static void Main()
        {
            string filePath = "./verbrauch.csv";
            string csvData;

            using (StreamReader reader = new(filePath))
            {
                csvData = reader.ReadToEnd();
            }

            int[][] data = ReadCSV(csvData);
            Jahresstatistik j1 = Statistic(data, 20);

            Console.WriteLine($"MinVerbrauch: " + j1.GetMinVerbrauch());
            Console.WriteLine($"MaxVerbrauch: " + j1.GetMaxVerbrauch());
        }

        internal static readonly string[] separatorArray = ["\r\n", "\r", "\n"];

        static int[][] ReadCSV(string csvData)
        {
            string[] lines = csvData.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
            int rows = lines.Length;
            int cols = lines[0].Split(';').Length - 2;

            int[][] data = new int[rows][];

            for (int i = 0; i < rows; i++)
            {
                data[i] = new int[cols];
                string[] values = lines[i].Split(';');

                for (int j = 0; j < cols; j++)
                {
                    if (!string.IsNullOrEmpty(values[j + 2]))
                    {
                        data[i][j] = int.Parse(values[j + 2]);
                        //System.Console.WriteLine($"Data[{i}][{j}]: " + data[i][j]);
                    }
                }
            }

            return data;
        }

        public static Jahresstatistik Statistic(int[][] verbrauch, int limit)
        {
            int minVerbrauch = int.MaxValue;
            int maxVerbrauch = int.MinValue;
            List<Monatsverbrauch> limitVerbraucher = [];

            for (int i = 1; i < verbrauch.Length; i++)
            {
                int VerbrauchterNr = verbrauch[i][0];

                for (int j = 2; j < 12; j++)
                {
                    int UserverbrauchMonat = verbrauch[i][j] - verbrauch[i][j - 1];
                    //System.Console.WriteLine($"UserverbrauchMonat[{i}][{j}]: {UserverbrauchMonat}");
                    int Monatsnummer = j + 1;
                    //System.Console.WriteLine($"Monat: {Monatsnummer}");

                    if (UserverbrauchMonat < minVerbrauch)
                    {
                        minVerbrauch = UserverbrauchMonat;
                    }

                    if (UserverbrauchMonat > maxVerbrauch)
                    {
                        maxVerbrauch = UserverbrauchMonat;
                    }

                    if (limit >= UserverbrauchMonat)
                    {
                        limitVerbraucher.Add(new Monatsverbrauch(VerbrauchterNr, Monatsnummer, UserverbrauchMonat));
                    }
                }
            }
            return new Jahresstatistik(minVerbrauch, maxVerbrauch, limitVerbraucher);
        }
    }

    readonly struct Jahresstatistik(int min, int max, List<Monatsverbrauch> limit)
    {
        private readonly int minVerbrauch = min;
        private readonly int maxVerbrauch = max;
        private readonly List<Monatsverbrauch> limitVerbraucher = limit;

        public int GetMinVerbrauch()
        {
            return minVerbrauch;
        }

        public int GetMaxVerbrauch()
        {
            return maxVerbrauch;
        }

        public List<Monatsverbrauch> GetlimitVerbraucher()
        {
            return limitVerbraucher;
        }
    }

    readonly struct Monatsverbrauch(int verbraucherNr, int monatsnummer, int userverbrauchMonat)
    {
        private readonly int VerbrauchterNr = verbraucherNr;
        private readonly int Monatsnummer = monatsnummer;
        private readonly int UserverbrauchMonat = userverbrauchMonat;

        public int GetVerbrauchterNr()
        {
            return VerbrauchterNr;
        }

        public int GetMonatsnummer()
        {
            return Monatsnummer;
        }

        public int GetUserverbrauchMonat()
        {
            return UserverbrauchMonat;
        }
    }
}
