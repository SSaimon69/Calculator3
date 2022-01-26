using System;
using System.Collections.Generic;

namespace Calculator3
{
    class Program
    {
        //Текущий массив
        static int[,] mas = new int[0,0];
        static Dictionary<string, Operation> allowedOper = new Dictionary<string, Operation>
            {
                {"-init", Operation.init},
                {"-max", Operation.max},
                {"-min", Operation.min}
            };

        enum Operation
        {
            init,
            max,
            min,
            sum,
            midAr,
            midGeo,
            matrixMulNum,
            matrixTransp,
            esc
        }

        static Func<string[], int[,]> Init = (vvod) =>
        {
            int count = 0, count2 = 0;
            try
            {
                //Если передали не 2 размерности, то вторую делаем равной 1
                bool get2Rank = vvod[1][0] == '$';
                int[,] curMas = new int[get2Rank ? int.Parse(vvod[1].Substring(1)) : 1, int.Parse(vvod[0].Substring(1))];

                for (int i = get2Rank ? 2 : 1; i < vvod.Length; i++)
                {
                    curMas[count++, count2] = int.Parse(vvod[i]);
                    if (count == curMas.GetLength(0))
                    {
                        count = 0;
                        count2++;
                    }
                }
                return curMas;
            }
            catch(Exception e)
            {
                throw new OperationException("Инициализация", "mas = {input}", e);
            }
        };


        static Func<int[,], int> Max = (mas) =>
        {
            try
            {
                int max = mas[0, 0];
                foreach (int elem in mas) max = elem > max ? elem : max;
                return max;
            }
            catch (Exception e)
            {
                throw new OperationException("Поиск максимума", "max = Max(mas)", e);
            }
        };

        static Func<int[,], int> Min = (mas) =>
        {
            try
            {
                int min = mas[0, 0];
                foreach (int elem in mas) min = elem < min ? elem : min;
                return min;
            }
            catch (Exception e)
            {
                throw new OperationException("Поиск минимума", "max = Min(mas)", e);
            }
        };

        static Func<int[,], int> Sum = (mas) =>
        {
            int sum = 0;
            foreach (int elem in mas) sum += elem;
            return sum;
        };

        static Func<int[,], double> midAr = (mas) =>
        {
            int sum = 0;
            foreach (int elem in mas) sum += elem;
            return sum / mas.Length;
        };

        static Func<int[,], double> midGeo = (mas) =>
        {
            float mul = 1;
            foreach (int elem in mas) mul *= elem;
            return Math.Pow(mul, 1.0 / mas.Length);
        };

        static Func<int[,], int, int[,]> matrixMulNum = (mas, num) =>
        {
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    mas[i, j] *= num;
                }
            }
            return mas;
        };

        static Func<int[,], int[,]> matrixTransp = (mas) =>
        {
            int[,] mas2 = new int[mas.GetLength(1), mas.GetLength(0)];
            int count = 0, count2 = 0;
            foreach (int elem in mas)
            {
                mas2[count++, count2] = elem;
                if (count == mas.GetLength(1))
                {
                    count = 0;
                    count2++;
                    if (count2 == mas.GetLength(0)) count2 = 0;
                }
            }
            return mas2;
        };

        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] != "-batch")
            {
                Operation oper;
                //Интерактивный режим
                while (true)
                {
                    oper = getOperation();

                    if (oper == Operation.esc) break;
                    if (oper != Operation.init && mas.Length == 0)
                    {
                        Console.WriteLine("Что ты собрался делать с пустым массивом ??? Введи массив сначала (для продожения нажми любую клавишу)");
                        Console.ReadKey();
                        continue;
                    }

                    switch (oper)
                    {
                        case Operation.init:
                            {
                                Console.Write("Введите размерность массива (для двумерного массива напишите размерность через пробел), и далее Введите массив указанной размерности через пробел в одну строку (двумерный массив вводить слева-направо сверху-вниз):  ");
                                mas = Init(Console.ReadLine().Split(' '));
                                printMas(mas);
                            }; break;
                        case Operation.max: Console.WriteLine("Максимальный элемент: " + Max(mas)); break;
                        case Operation.min: Console.WriteLine("Минимальный элемент: " + Min(mas)); break;
                        case Operation.sum: Console.WriteLine("Сумма элементов массива: " + Sum(mas)); break;
                        case Operation.midAr: Console.WriteLine("Среднее арифметическое массива: " + midAr(mas)); break;
                        case Operation.midGeo: Console.WriteLine("Среднее геометрическое массива: " + midGeo(mas)); break;
                        case Operation.matrixMulNum:
                            {
                                Console.Write("Введите число, на которое будет умножена матрица: ");
                                printMas(matrixMulNum(mas, int.Parse(Console.ReadLine())));
                            }
                            break;
                        case Operation.matrixTransp: printMas(matrixTransp(mas)); break;
                    }
                    Console.WriteLine("Нажмите любую кнопку чтобы продолжить");
                    Console.ReadKey(false);
                }
            }
            //Пакетный режим
            else
            {
                List<string> operands = new List<string>();
                int counterEx = 0, counterSucEx = 0, counterFailEx = 0;

                string curOper = "";
                int resTryOper = 0;

                for (int i = 1; i < args.Length; i++)
                {
                    //Нашли оператор
                    if (allowedOper.ContainsKey(args[i]))
                    {
                        counterEx++;
                        //До этого был оператор ?
                        if (curOper != "")
                        {
                            resTryOper = tryExecBatchOper(curOper, operands);
                            if (resTryOper == 0) counterSucEx++;
                            else counterFailEx++;
                            if (resTryOper == 2) break;
                        }

                        operands.Clear();
                        curOper = args[i];

                        //Предусматриваем вариант что массив закончился оператором
                        if (i == args.Length - 1)
                        {
                            resTryOper = tryExecBatchOper(curOper, operands);
                            if (resTryOper == 0) counterSucEx++;
                            else counterFailEx++;
                            if (resTryOper == 2) break;
                        }
                    }
                    else
                    {
                        operands.Add(args[i]);

                        //Предусматриваем вариант что массив закончился операндом
                        if (i == args.Length - 1)
                        {
                            resTryOper = tryExecBatchOper(curOper, operands);
                            if (resTryOper == 0) counterSucEx++;
                            else counterFailEx++;
                            if (resTryOper == 2) break;
                        }

                    }
                }
                
                Console.WriteLine("*********************");
                Console.WriteLine("Статистика пакетного режима: ");
                Console.WriteLine("Поступило заданий на вход: " + counterEx);
                Console.WriteLine("Успешно выполнено заданий: " + counterSucEx);
                Console.WriteLine("Заданий с ошибками: " + counterFailEx);
            }
            Console.WriteLine("ВВы вышли из калькулятора");
            Console.ReadKey();
        }

        static int tryExecBatchOper(string curOper, List<string> operands)
        {
            try
            {
                switch (allowedOper[curOper])
                {
                    case Operation.init:
                        {
                            mas = null;
                            Console.WriteLine("Пытаемся выполнить инициализацию...");
                            mas = Init(operands.ToArray());
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Успешно инициализировали. Результат: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            printMas(mas);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("*************");
                        }
                        break;
                    case Operation.max:
                        {
                            Console.WriteLine("Пытаемся выполнить поиск максимума...");
                            int res = Max(mas);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Максимум успешно найден. \nРезультат: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(res);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("*************");
                        }
                        break;
                    case Operation.min:
                        {
                            Console.WriteLine("Пытаемся выполнить поиск минимума...");
                            int res = Min(mas);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Миниимум успешно найден. \nРезультат: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(res);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("*************");
                        }
                        break;
                }
            }
            catch (OperationException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{" + e.Formula + "} ERROR ({" + e.OperationName + "}): {" + e.Message + "}");
                Console.ForegroundColor = ConsoleColor.White;
                //Мягкая ошибка
                return 1;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message + e.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
                //Грубая ошибка
                return 2;
            }

            //Нет ошибок
            return 0;
        }

        //Выводит меню и получает операцию от пользователя
        static Operation getOperation()
        {
            ConsoleKeyInfo oper;
            string operMas = "12345678" + (char)ConsoleKey.Escape;

            Console.Clear();
            Console.WriteLine("Список операций:");
            Console.WriteLine("Ввод массива: 1\n" +
                              "Поиск максимума: 2\n" +
                              "Поиск минимума: 3\n" +
                              "Расчёт суммы всех чисел: 4\n" +
                              "Расчёт среднеарифметического: 5\n" +
                              "Расчёт среднегеометрического: 6\n" +
                              "Произведение матрицы на число: 7\n" +
                              "Транспонирование матрицы: 8\n" +
                              "Выход: esc");
            Console.WriteLine();
            Console.Write("Введите операцию из перечисленных: ");
            oper = Console.ReadKey(false);
            Console.Clear();
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
                case '7': return Operation.matrixMulNum;
                case '8': return Operation.matrixTransp;
                case (char)ConsoleKey.Escape: return Operation.esc;
            }
            throw new Exception();
        }

        //Печатает массив в консоль
        static void printMas(int[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0);i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    Console.Write(mas[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public class OperationException : Exception
        {
            public string OperationName { get; set; }
            public string Formula { get; set; }

            public OperationException(string operationName, string formula, Exception innerException)
                :base(innerException?.Message,innerException)
            {
                OperationName = operationName;
                Formula = formula;
            }
        }
    }
}
