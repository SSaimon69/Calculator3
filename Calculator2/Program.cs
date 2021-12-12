using System;

namespace Calculator2
{
    class Program
    {
        enum Operation
        {
            init,
            max,
            min,
            sum,
            midAr,
            midGeo,
            esc
        }

        static void Main(string[] args)
        {
            Operation oper;

            while (true)
            {
                oper = getOperation();
                int[] mas = new int[0];

                if (oper == Operation.esc) break;
                if (oper != Operation.init && mas.Length == 0)
                {
                    Console.WriteLine("Что ты собрался делать с пустым массивом ??? Введи массив сначала");
                    continue;
                }

                switch (oper)
                {
                    case Operation.init:
                        {
                            int count;
                            Console.Write("Введите размерность массива: ");
                            while (!int.TryParse(Console.ReadLine(), out count) && count <= 0)
                            {
                                Console.Clear();
                                Console.WriteLine("Вы ввели не число, либо число меньше или равно 0");
                                Console.Write("Введите размерность массива: ");
                            }
                            mas = new int[count];

                            Console.Write("Введите массив указанной размерности через пробел: ");
                            count = 0;
                            foreach (string num in Console.ReadLine().Split(' '))
                            {
                                mas[count++] = int.Parse(num);
                            }
                        }; break;
                    case Operation.max:
                        {
                            int max = mas[0];
                            foreach(int elem in mas) if (elem > max) max = elem;
                            Console.WriteLine("Максимальный элемент: " + max);
                        }; break;
                    case Operation.min:
                        {
                            int min = mas[0];
                            foreach (int elem in mas) if (elem < min) min = elem;
                            Console.WriteLine("Минимальный элемент: " + min);
                        }; break;
                    case Operation.sum:
                        {
                            int sum = 0;
                            foreach (int elem in mas) sum += elem;
                            Console.WriteLine("Сумма элементов массива: " + sum);
                        }
                        break;
                    case Operation.midAr:
                        {
                            int sum = 0;
                            foreach (int elem in mas) sum += elem;
                            Console.WriteLine("Среднеарифметическое массива: " + sum/mas.Length);
                        }; break;
                    case Operation.midGeo:
                        {
                        }break;
                }
            }
            Console.Clear();
            Console.WriteLine("Вы вышли из калькулятора");
            Console.ReadKey();
        }

        static Operation getOperation()
        {
            ConsoleKeyInfo oper;
            string operMas = "123456" + (char)ConsoleKey.Escape;

            Console.Clear();
            Console.WriteLine("Список операций:");
            Console.WriteLine("Ввод массива: 1\n" +
                              "Поиск максимума: 2\n" +
                              "Поиск минимума: 3\n" +
                              "Расчёт суммы всех чисел: 4\n" +
                              "Расчёт среднеарифметического: 5\n" +
                              "Расчёт среднегеометрического: 6\n" +
                              "Выход: esc");
            Console.WriteLine();
            Console.Write("Введите операцию из перечисленных: ");
            oper = Console.ReadKey();

            while (!operMas.Contains(oper.KeyChar))
            {
                Console.Clear();
                Console.WriteLine("Вы ввели не валидную операцию!");
                Console.Write("Введите операцию из перечисленных: ");
                oper = Console.ReadKey();
            }
            switch (oper.KeyChar)
            {
                case '1': return Operation.init;
                case '2': return Operation.max;
                case '3': return Operation.min;
                case '4': return Operation.sum;
                case '5': return Operation.midAr;
                case '6': return Operation.midGeo;
                case (char)ConsoleKey.Escape: return Operation.esc;
            }
            throw new Exception();
        }
    }
}
