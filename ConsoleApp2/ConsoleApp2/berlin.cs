using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TSP
{
    class Program
    {
        static void Main(string[] args)
        {
            int size;
            int population = 40;
            int min = 4000000;
            int[,] distance;
            int[,] populatonarray;
            int[] gradesarray;
            int[,] tournamentpop;
            int[] shotrouletteid = new int[population];
            //int[,] roulettepop;
            int[,] crossarray;
            string[] substrings;
            Random rnd = new Random();
            string line;

            //WCZYTYWANIE Z PLIKU
            using (StreamReader sr = new StreamReader(@"C:\Users\karko\source\repos\ConsoleApp2\zawody2018gr1\berlin52.txt"))
            {
                size = Convert.ToInt32(sr.ReadLine());
                distance = new int[size, size];
                for (int i = 0; i < size; i++)
                {
                    line = sr.ReadLine().Trim();
                    substrings = line.Split(' ');

                    for (int j = 0; j < substrings.Length; j++)
                    {
                        distance[i, j] = Convert.ToInt32(substrings[j]);
                        distance[j, i] = Convert.ToInt32(substrings[j]);
                    }

                }

                //WYSWIETLANIE LOSOWYCH
                populatonarray = Generator(size, population);
                //for(int i = 0; i < population; i++)
                //{
                //    for(int j = 0; j < size; j++)
                //    {
                //        Console.Write(populatonarray[i, j]+ " ");

                //    }
                //    Console.WriteLine();
                //}

                for (int iteration = 0; iteration < 500000; iteration++)
                {

                    //WYSWIETLANIE OCEN
                    gradesarray = Evaluation(populatonarray, distance, population, size);
                    for (int i = 0; i < population; i++)
                    {
                        //Console.WriteLine(gradesarray[i]);
                        if (min > gradesarray[i])
                        {
                            min = gradesarray[i];
                            Console.WriteLine("Iteration num: " + iteration + " best score: " + min);
                            for (int j = 0; j < size; j++)
                            {
                                Console.Write(populatonarray[i, j] + "-");
                            }
                            Console.WriteLine();
                        }
                    }


                    //WYSWIETLANIE WYGRANYCH Z RULETKI
                    int[] section = new int[gradesarray.Length];
                    section = Roulette(gradesarray, population);

                    //for (int i = 0; i < population; i++)
                    //{
                    //    //Console.WriteLine(section[i]);
                    //}
                    Random rng = new Random();
                    int auxid = 0;
                    for (int x = 0; x < population; x++)
                    {
                        int shot = rng.Next(0, section[population - 1]);
                        for (int z = 0; z < population; z++)
                        {
                            if (shot < section[z])
                            {
                                //Console.WriteLine("Wylosowana liczba: " + shot + " nalezy do przedzialu: " + z);
                                shotrouletteid[auxid] = z;
                                auxid++;

                                break;
                            }

                        }
                    }
                    //for (int i = 0; i < population; i++)
                    //{
                    //    Console.WriteLine(shotrouletteid[i]);
                    //}

                    //WYSWIETLANIE RULETKI
                    //roulettepop = Winnersarray(populatonarray, shotrouletteid, size, population);
                    //for (int i = 0; i < m; i++)
                    //{
                    //    for (int j = 0; j < size; j++)
                    //    {
                    //        Console.Write(roulettepop[i, j] + " ");

                    //    }
                    //    Console.WriteLine();
                    //}

                    //WYSWIETLANIE WYGRANYCH Z TURNIEJU
                    int[] winnersid = new int[gradesarray.Length];
                    winnersid = Tournament(gradesarray, population);
                    //for (int i = 0; i < m; i++)
                    //{
                    //    Console.WriteLine(winnersid[i]);
                    //}

                    //WYSWIETLANIE NOWEJ POPULACJI
                    tournamentpop = Winnersarray(populatonarray, winnersid, size, population);
                    //for (int i = 0; i < m; i++)
                    //{
                    //    for (int j = 0; j < size; j++)
                    //    {
                    //        Console.Write(tournamentpop[i, j] + " ");

                    //    }
                    //    Console.WriteLine();
                    //}

                    //Console.WriteLine("After crossover:");

                    //WYŚWIETLANIE KRZYŻOWANIA
                    crossarray = Crossover(tournamentpop, population, size, 1000);
                    //for (int i = 0; i < m; i++)
                    //{
                    //    for (int j = 0; j < size; j++)
                    //    {
                    //        Console.Write(crossarray[i, j] + " ");

                    //    }
                    //    Console.WriteLine();
                    //}
                    //Console.WriteLine("After mutation:");

                    //WYSWIETLANIE MUTACJI
                    populatonarray = Mutation(crossarray, size, population, 9950);
                    
                    //for (int i = 0; i < m; i++)
                    //{
                    //    for (int j = 0; j < size; j++)
                    //    {
                    //        Console.Write(populatonarray[i, j] + " ");

                    //    }
                    //    Console.WriteLine();
                    //}
                }
            }
        }

        //GENEROWANIE LOSOWYCH
        public static int[,] Generator(int size, int population)
        {
            int[,] randomarray = new int[population, size];
            Random rnd = new Random();
            int p;
             
            for(int i = 0; i < population; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    randomarray[i, j] = -1;
                }
            }
            for (int i = 0; i < population; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    p = rnd.Next(0, size);

                    while(Contain(i, p, size, randomarray) == true)
                    {
                        p = rnd.Next(0, size);

                    }
                    randomarray[i, j] = p;
                }
            }
            return randomarray;
        }

        //METODA POTRZEBNA DO NIEPOWTARZANIA 
        public static bool Contain(int rownum, int numvalue, int size, int[,] randomarray)
        {
            for(int i = 0; i < size; i++)
            {
                if(numvalue == randomarray[rownum, i])
                {
                    return true;
                }
                
            }
            return false;
        }

        //OCENIANIE
        public static int[] Evaluation(int[,] array, int[,] distance, int population, int size)
        {
            int[] grades = new int[population];
            int sum = 0;

            for (int i = 0; i < population; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (j != size - 1)
                    {
                        sum = sum + distance[array[i, j], array[i, j + 1]];
                    }
                    else
                    {
                        sum = sum + distance[array[i, j], array[i, 0]];
                    }
                }
                grades[i] = sum;
                sum = 0;
            }
            return grades;
        }

        //TURNIEJ
        public static int[] Tournament(int[] grades, int population)
        {
            int participants = 2;
            int min = 0;
            int id = -1;
            int[] idarray = new int[grades.Length];
            Random rnd = new Random();
            for (int i = 0; i < population; i++)
            {
                for (int e = 0; e < participants; e++)
                {
                    int draw = rnd.Next(0, grades.Length);
                    if (e == 0)
                    {
                        min = grades[draw];
                        id = draw;
                    }
                    if (min > grades[draw])
                    {
                        min = grades[draw];
                        id = draw;
                    }

                }
                idarray[i] = id;
            }
            return idarray;

        }

        //RULETKA
        public static int[] Roulette(int[] grades, int population)
        {
            int max = 0;
            int[] section = new int[population];
            for (int i = 0; i < population; i++)
            {
                int value = grades[i];
                if (value > max)
                {
                    max = value;
                }

            }
            max = max + 1;

            for (int k = 0; k < population; k++)
            {
                if (k == 0)
                {
                    section[k] = max - grades[k];
                }
                else
                {
                    section[k] = max - grades[k] + section[k - 1];
                }
            }
            return section;
        }

        //POPULACJA WYGRANYCH
        private static int[,] Winnersarray(int[,] populationarray, int[] winnersid, int size, int population)
        {
            int[,] winnerroute = new int[population, size];
            for (int i = 0; i < population; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    winnerroute[i, j] = populationarray[winnersid[i], j];
                }
                
            }
            return winnerroute;
        }


        //KRZYŻOWANIE
        private static int[,] Crossover(int[,] populationarray, int population, int size, int ratio)
        {
            int[,] crossoverarray = new int[population, size];
            Random rnd = new Random();
            int partition1 = 0;
            int partition2 = 0;
            int item1;
            int item2;
            int[] row1 = new int[size];
            int[] row2 = new int[size];
            int k;

            for (int i = 0; i < population; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    crossoverarray[i, j] = -1;
                }
            }

            for (int i = 0; i < population; i = i + 2) //wymiana środków
            {
                k = rnd.Next(0, 10000);
                if (k > ratio)
                {
                    partition1 = rnd.Next(0, size);
                    partition2 = rnd.Next(0, size);

                    while (partition1 == partition2)
                    {
                        partition2 = rnd.Next(0, size);
                    }

                    if (partition1 > partition2)
                    {
                        int aux = partition1;
                        partition1 = partition2;
                        partition2 = aux;
                    }

                    for (int j = partition1; j <= partition2; j++)
                    {
                        crossoverarray[i, j] = populationarray[i + 1, j];
                        crossoverarray[i + 1, j] = populationarray[i, j];
                    }
                }
                else
                {
                    for (int a = 0; a < size; a++)
                    {
                        crossoverarray[i, a] = populationarray[i, a];
                        crossoverarray[i + 1, a] = populationarray[i + 1, a];
                    }
                    //Console.WriteLine("Wiersz: " + i + " oraz " + (i + 1) + " nie krzyzuja sie");
                }
            }

            for (int i = 0; i < population; i = i + 2) //wymienianie reszty
            {
                for (int j = 0; j < size; j++)
                {
                    row1[j] = crossoverarray[i, j];
                    row2[j] = crossoverarray[i + 1, j];
                }
                for (int j = 0; j < size; j++)
                {

                    if (row1[j] == -1)
                    {
                        item1 = populationarray[i, j];
                        item2 = populationarray[i + 1, j];
                        while (Array.IndexOf(row1, item1) != -1) //array.indexof zwraca -1 gdy danej liczby nie ma w całej tablicy
                        {
                            item1 = populationarray[i, Array.IndexOf(row1, item1)]; // zamieniłem i+1 na i
                        }

                        crossoverarray[i, j] = item1; // wpisuje liczbę do tablicy

                        while (Array.IndexOf(row2, item2) != -1)
                        {
                            item2 = populationarray[i + 1, Array.IndexOf(row2, item2)]; //zamieniłem i na i+1
                        }

                        crossoverarray[i + 1, j] = item2;
                    }

                }
            }
            return crossoverarray;
        }

        //MUTACJA
        private static int[,] Mutation(int[,] mutationarray, int size, int population, int ratio)
        {
            Random rnd = new Random();
            int aux;
            int swap;
            int v; //element z którym zamieniam zmutowaną liczbę

            for (int i = 0; i < population; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    aux = rnd.Next(0, 10000);
                    if (aux > ratio)
                    {
                        swap = rnd.Next(0, size);
                        v = mutationarray[i, j];
                        mutationarray[i, j] = mutationarray[i, swap];
                        mutationarray[i, swap] = v;
                        //Console.WriteLine("Zmutowanio wiersz: " + i + " numer " + j + " z numerem " + swap);
                    }
                }
            }
            return mutationarray;
        }
    }
}

