using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace FloorFinder
{
    class Program
    {
        //Метод для вывода значений если этаж не был найден
        public static void FloorNotFound(string s, string newCsvPath)
        {
            using (var sw = new StreamWriter(newCsvPath, true))
                if (s != "ID,description,floor_level")
                    sw.WriteLine(s + "not_found");
                else
                    sw.WriteLine(s);
        }

        //Метод для записи значения найденного этажа в файл
        public static void FloorWriter(string typeOfFloorEntry, Match match, StreamWriter sw, string s)
        {
            if (match.ToString().Contains(typeOfFloorEntry))
            {
                int index = match.ToString().IndexOf(typeOfFloorEntry, 0, match.ToString().Length, StringComparison.CurrentCultureIgnoreCase);
                sw.WriteLine(s + "\"" + match.ToString().Remove(index) + "\"");
            }
        }

        //Метод определения типа записи этажа если он был найден(n/n, n-й, n, "number")
        public static void FloorFound(string s, MatchCollection matches, string newCsvPath)
        {
            using (var sw = new StreamWriter(newCsvPath, true))
                foreach (Match match in matches)
                {
                    string typeOfFloorEntry = "/";
                    FloorWriter(typeOfFloorEntry, match, sw, s);
                    typeOfFloorEntry = "-";
                    FloorWriter(typeOfFloorEntry, match, sw, s);
                    typeOfFloorEntry = "этаж";
                    if (!match.ToString().Contains("/") && !match.ToString().Contains("-") && !match.ToString().Contains("этажа"))
                        FloorWriter(typeOfFloorEntry, match, sw, s);
                    if (!match.ToString().Contains("/") && !match.ToString().Contains("-") && match.ToString().Contains("этажа"))
                        sw.WriteLine(s + "not_found");
                    if (!match.ToString().Contains("/") && !match.ToString().Contains("-") && match.ToString().Contains("этажей"))
                        sw.WriteLine(s + "not_found");
                }
        }

        //Метод для поиска этажей
        public static void RegularExspression(string line, string newCsvPath)
        {
            string s = line;
            string pattern = @"(\w*)й.этаж(\w*)|(\w*)м.этаж(\w*)| \d*.этаж(\w*)|\d*\/\d*.этаж(\w*)|\d*\S*й.этаж(\w*)|\d*\S*м.этаж(\w*)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(s);
            if (matches.Count == 1 && File.Exists(newCsvPath))
                FloorFound(s, matches, newCsvPath);
            else
                FloorNotFound(s, newCsvPath);
        }

        static void Main()
        {
            int pathCheck = 1;
            while (pathCheck == 1)
            {
                //Запрос для ввода пути к исходному файлу
                Console.WriteLine("Введите путь исходного csv файла: ");
                Console.WriteLine(@"Пример ввода: C:\Users\out_processed_floor");
                var csvPath = Console.ReadLine() + ".csv";
                //Проверка существования файла по указанному пути
                if (File.Exists(csvPath))
                {
                    //Запрос для ввода пути к новому файлу
                    pathCheck++;
                    Console.WriteLine("\nНовый файл будет создан в той же папке где лежит исходный файл");
                    Console.WriteLine("Введите название для нового csv файла: ");
                    var newCsvPath = Path.GetDirectoryName(csvPath) + @"\" + Console.ReadLine() + @".csv";
                    //Объявление переменных
                    var lineCounter = 0;
                    var numberOfLinesToProcess = 10;
                    string fields = null;
                    //Очистка файла в который будут записываться новые значения
                    File.WriteAllText(newCsvPath, String.Empty);
                    //Буфер представлен в виде списка
                    List<string> List = new List<string>();
                    //Чтение из старого файла
                    using (TextFieldParser tfp = new TextFieldParser(csvPath))
                    {
                        tfp.TextFieldType = FieldType.Delimited;
                        tfp.SetDelimiters(",");
                        //Цикл который обеспечивает обработку из старого файла 10 строк за каждую итерацию
                        while ((lineCounter < numberOfLinesToProcess) && (!tfp.EndOfData))
                        {
                            //Цикл для записи из старого файла 10 строк в буфер
                            for (; lineCounter < numberOfLinesToProcess && !tfp.EndOfData; lineCounter++)
                                //Запись 10 строк из старого файла в буфер
                                List.Add(fields = tfp.ReadLine());
                            //Обработка 10 строк и запись их в новый файл
                            if (lineCounter <= numberOfLinesToProcess)
                            {
                                foreach (var line in List)
                                    RegularExspression(line, newCsvPath);
                                //Очистка буфера
                                List.Clear();
                                //
                                if (lineCounter == numberOfLinesToProcess)
                                    numberOfLinesToProcess = numberOfLinesToProcess + 10;
                            }
                        }
                    }
                    Console.Clear();
                    Console.WriteLine("Значения этажей добавлены в новый файл");
                }
                //Вывод программы в случае неправильного ввода пути 
                else
                {
                    Console.WriteLine("Файл не был найден!");
                    Console.WriteLine("Ввести путь файла заново? (1 - да, 2 - нет)");
                    pathCheck = Convert.ToInt32(Console.ReadLine());
                    while (pathCheck != 1 && pathCheck != 2)
                    {
                        Console.WriteLine("неправильный ввод");
                        Console.WriteLine("Ввести путь файла заново? (1 - да, 2 - нет)");
                        pathCheck = Convert.ToInt32(Console.ReadLine());
                    }
                    Console.Clear();
                }
            }
        }
    }
}   
