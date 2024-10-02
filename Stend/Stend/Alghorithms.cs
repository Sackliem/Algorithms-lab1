using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace Stend
{
    public abstract class Algorithm
    {
        protected double[] Times;
        protected List<double[]> Arr;
        protected int repeats;
        protected Chart chart;

        public abstract double[] Run(double[] input);
        public abstract double[] Run(double[] input, int exponent);

        public static List<double[]> SplitArray(double[] array, int increment)
        {
            List<double[]> subarrays = new List<double[]>();
            int currentSize = increment;

            while (currentSize <= array.Length)
            {
                double[] subarray = new double[currentSize];
                Array.Copy(array, 0, subarray, 0, currentSize);
                subarrays.Add(subarray);
                currentSize += increment;
            }

            // Добавляем последний подмассив, если остались элементы
            if (currentSize - increment < array.Length)
            {
                double[] lastSubarray = new double[array.Length];
                Array.Copy(array, 0, lastSubarray, 0, array.Length);
                subarrays.Add(lastSubarray);
            }

            return subarrays;
        }

        public void PlotGraph()
        {
            chart.Series[0].Points.Clear();
            int numbers_lenght = Arr.Count;
            for (var i = 0; i != numbers_lenght; i++)
            {
                chart.Series[0].Points.AddXY(Arr[i].Length, Times[i]);
            }
        }

        public void PlotGraph_Pow(double[] times, double[] numbers)
        {
            // надо переписать под возведение в степень
            chart.Series[0].Points.Clear();
            for (var i = 0; i != numbers.Length; i++)
            {
                chart.Series[0].Points.AddXY(i, times[i]);
            }
        }
    }

    public class BubbleSort : Algorithm
    {
        public BubbleSort(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }
        private static void Swap(ref double e1, ref double e2)
        {
            var temp = e1;
            e1 = e2;
            e2 = temp;
        }
        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override double[] Run(double[] numbers)
        {
            
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // храним время всех повторений алгритма

            //переменная repeats отвечающая за кол-во повторений
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    bubblesort(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }

            // вычисление среднего для повторений
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }

            // В Times хранится время для постройки графика
            Times = processingTimes;

            PlotGraph();
            return processingTimes;
        }
        private static void bubblesort(double[] numbers)
        {
            var len = numbers.Length;
            for (var i = 0; i < len - 1; i++)
            {
                for (var j = 0; j < len - 1 - i; j++)
                {
                    if (numbers[j] > numbers[j + 1])
                    {
                        Swap(ref numbers[j], ref numbers[j + 1]);
                    }
                }
            }
        }
    }

    public class QuickSort : Algorithm
    {
        public QuickSort(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        private static void Swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }

        private void QuickSortRecursive(double[] array, int left, int right)
        {
            var i = left;
            var j = right;
            var pivot = array[left];
            while (i <= j)
            {
                while (array[i] < pivot) i++;
                while (array[j] > pivot) j--;
                if (i <= j)
                {
                    Swap(ref array[i], ref array[j]);
                    i++;
                    j--;
                }
            }

            if (left < j) QuickSortRecursive(array, left, j);
            if (i < right) QuickSortRecursive(array, i, right);
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();

            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    QuickSortRecursive(array[i], 0, array[i].Length - 1);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }
    }

    public class SumCalculator : Algorithm
    {
        public SumCalculator(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Sum(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public static void Sum(double[] numbers)
        {
            double sum = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i];
            }
        }

    }

    public class PolynomialEvaluator : Algorithm
    {

        public PolynomialEvaluator(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Polynomial(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public static void Polynomial(double[] coefficients)
        {
            double result = 0;
            double x = 2.0;  // Примерное значение x
            int n = coefficients.Length;
            for (int i = 0; i < n; i++)
            {
                double term = coefficients[i];
                for (int j = 0; j < i; j++)
                {
                    term *= x;
                }
                result += term;
            }
        }
    }

    public class ConstantFunction : Algorithm
    {
        public ConstantFunction(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Constant(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public static void Constant(double[] vector)
        {
            var zero = 0;
            for (int i = 0; i < 10; i++)
            {
                zero += i;
            }
        }
    }

    public class ProductCalculator : Algorithm
    {

        public ProductCalculator(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Product(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public static void Product(double[] vector)
        {
            double product = 1;
            for (int i = 0; i < vector.Length; i++)
            {
                product *= vector[i];
            }
        }
    }

    public class TimSort : Algorithm
    {

        public TimSort(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        private static void InsertionSort(double[] array, int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                double temp = array[i];
                int j = i - 1;
                while (j >= left && array[j] > temp)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = temp;
            }
        }

        private static void Merge(double[] array, int left, int mid, int right)
        {
            int len1 = mid - left + 1;
            int len2 = right - mid;
            double[] leftArray = new double[len1];
            double[] rightArray = new double[len2];

            Array.Copy(array, left, leftArray, 0, len1);
            Array.Copy(array, mid + 1, rightArray, 0, len2);

            int i = 0, j = 0, k = left;
            while (i < len1 && j < len2)
            {
                if (leftArray[i] <= rightArray[j])
                {
                    array[k] = leftArray[i];
                    i++;
                }
                else
                {
                    array[k] = rightArray[j];
                    j++;
                }
                k++;
            }

            while (i < len1)
            {
                array[k] = leftArray[i];
                i++;
                k++;
            }

            while (j < len2)
            {
                array[k] = rightArray[j];
                j++;
                k++;
            }
        }

        public static void timsort(double[] array)
        {
            int minRun = 32; // Можно задать любой размер блока
            int n = array.Length;
            for (int i = 0; i < n; i += minRun)
            {
                InsertionSort(array, i, Math.Min(i + minRun - 1, n - 1));
            }

            for (int size = minRun; size < n; size = 2 * size)
            {
                for (int left = 0; left < n; left += 2 * size)
                {
                    int mid = left + size - 1;
                    int right = Math.Min(left + 2 * size - 1, n - 1);

                    if (mid < right)
                    {
                        Merge(array, left, mid, right);
                    }
                }
            };
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    timsort(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }
    }

    public class MatrixMultiplication : Algorithm
    {

        public MatrixMultiplication(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public static void matrixmult(double[] input)
        {
            // input будет содержать одномерный массив для представления матриц
            int size = (int)Math.Sqrt(input.Length / 2);  // Определяем размер матриц
            double[,] matrixA = new double[size, size];
            double[,] matrixB = new double[size, size];

            // Заполнение матриц значениями из входного массива
            Buffer.BlockCopy(input, 0, matrixA, 0, size * size * sizeof(double));
            Buffer.BlockCopy(input, size * size * sizeof(double), matrixB, 0, size * size * sizeof(double));

            double[,] result = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < size; k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }
        }

        public override double[] Run(double[] numbers)
        {

            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    matrixmult(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }
    }

    public class PowerNative : Algorithm
    {

        public PowerNative(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        private static double power_naive(double baseNumber, int exponent)
        {
            double result = 1;
            double times = 0;
            for (int i = 0; i < exponent; i++)
            {
                times++;
                result *= baseNumber;
                times++;
            }
            return times;
        }

        public override double[] Run(double[] input)
        {
            // Эту хуету не трогать
            return input;
        }
        public override double[] Run(double[] numbers, int exponent)
        {
            var len = numbers.Length;
            Array.Sort(numbers);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[numbers.Length];
                for (int i = 0; i < len; i++)
                {
                    times_for_avg[i] = power_naive(exponent, (int)numbers[i]);
                }
                avg_times.Add(times_for_avg);
            }
            double[] processingTimes = new double[numbers.Length];
            for (int j = 0; j < numbers.Length; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            PlotGraph_Pow(processingTimes, numbers);
            return processingTimes;
        }

    }

    public class PowerRecursive : Algorithm
    {

        public PowerRecursive(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        private static double power_recursive(double baseNumber, double exponent, ref int actionCount)
        {
            actionCount++; // Увеличиваем количество действий

            if (exponent == 0)
                return 1;
            actionCount += 2;
            return baseNumber * power_recursive(baseNumber, exponent - 1, ref actionCount);
        }

        public override double[] Run(double[] exponents, int number)
        {
            int actionCount = 0; // Переменная для подсчета действий
            var len = exponents.Length;
            Array.Sort(exponents);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[exponents.Length];
                for (int i = 0; i < len; i++)
                {
                    power_recursive(number, exponents[i], ref actionCount);
                    times_for_avg[i] = actionCount;
                    actionCount = 0;
                }
                avg_times.Add(times_for_avg);
            }
            double[] processingTimes = new double[exponents.Length];
            for (int j = 0; j < exponents.Length; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            PlotGraph_Pow(processingTimes, exponents);
            return processingTimes;
        }

        public override double[] Run(double[] input)
        {
            // Эту хуету не трогать
            return input; 
        }
    }

    public class PowerQuickPow : Algorithm
    {
        public PowerQuickPow(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        private static double Power_quick_pow(double baseValue, double exponent)
        {
            double f;
            double c = baseValue;
            int k = (int)exponent;
            int times = 0;

            // Инициализация значения f в зависимости от четности k
            times++;
            if (k % 2 == 1)
            {
                times++;
                f = c;
            }
            else
            {
                times++;
                f = 1;
            }

            // Основной цикл
            times++;
            k = k / 2;
            while (k > 0)
            {
                times++;
                times++;
                c = c * c; // Возведение текущей степени в квадрат
                times++;
                if (k % 2 == 1) // Если k нечётное
                {
                    times++;
                    f = f * c;
                }
                times++;
                k = k / 2; // Деление k на 2
            }

            return times;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            var len = numbers.Length;
            Array.Sort(numbers);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[numbers.Length];
                for (int i = 0; i < len; i++)
                {
                    times_for_avg[i] = Power_quick_pow(exponent, numbers[i]);
                }
                avg_times.Add(times_for_avg);
            }
            double[] processingTimes = new double[numbers.Length];
            for (int j = 0; j < numbers.Length; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            PlotGraph_Pow(processingTimes, numbers);
            return processingTimes;
        }

        public override double[] Run(double[] input)
        {
            // Эту хуету не трогать
            return input;
        }

    }

    public class PowerQuickPowClassic : Algorithm
    {
        public PowerQuickPowClassic(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            var len = numbers.Length;
            Array.Sort(numbers);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[numbers.Length];
                for (int i = 0; i < len; i++)
                {
                    times_for_avg[i] = Power_quickPowClassic(exponent, numbers[i]);
                }
                avg_times.Add(times_for_avg);
            }
            double[] processingTimes = new double[numbers.Length];
            for (int j = 0; j < numbers.Length; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            PlotGraph_Pow(processingTimes, numbers);
            return processingTimes;
        }

        public override double[] Run(double[] numbers)
        {
            // Эту хуету не трогать
            return numbers;
        }

        private static double Power_quickPowClassic(double baseValue, double exponent)
        {
            double f = 1; // Изначальный результат
            double c = baseValue; // Текущая степень основания
            int k = (int)exponent; // Текущая степень
            int times = 0;

            while (k > 0)
            {
                times++;
                times++;
                if (k % 2 != 0) // Если степень нечетная
                {
                    times++;
                    f *= c;
                    times++;
                    k -= 1;
                }
                else // Если степень четная
                {
                    times++;
                    c *= c;
                    times++;
                    k /= 2;
                }
            }

            return (double)times;
        }
    }

    public class BucketSort : Algorithm
    {
        public BucketSort(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    bucketsort(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public static void bucketsort(double[] input) 
        {
            int n = input.Length / 1000;  // Используем длину массива для количества корзин
            List<float>[] buckets = new List<float>[n];

            for (int i = 0; i < n; ++i)
                buckets[i] = new List<float>();

            // Распределение элементов по корзинам
            foreach (float num in input)
            {
                int bucketIndex = (int)(n * num);
                // out of index error 
                // TODO: FIX IT!!!!!
                buckets[bucketIndex].Add(num);
            }

            // Сортировка элементов в каждой корзине и их сборка
            int index = 0;
            for (int i = 0; i < n; ++i)
            {
                buckets[i].Sort();
                foreach (float num in buckets[i])
                {
                    input[index++] = num;
                }
            }
        }
    }

    public class SelectionSort : Algorithm
    {
        public SelectionSort(int repeat, ref Chart ch)
        {
            repeats = repeat;
            chart = ch;
        }

        public override double[] Run(double[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override double[] Run(double[] numbers)
        {
            var len = numbers.Length;
            List<double[]> array = SplitArray(numbers, 10000);
            List<double[]> avg_times = new List<double[]>();
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                double[] times_for_avg = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    selection(array[i]);
                    times_for_avg[i] = (double)stopwatch.ElapsedMilliseconds / 1000f;
                    stopwatch.Stop();
                }
                avg_times.Add(times_for_avg);
                array = SplitArray(numbers, 10000);
            }
            double[] processingTimes = new double[array.Count];
            for (int j = 0; j < array.Count; j++)
            {
                double tmp = 0;
                for (int i = 0; i < repeats; i++)
                {
                    tmp += avg_times[i][j];
                }
                double avg = tmp / repeats;
                processingTimes[j] = avg;
            }
            Arr = array;
            Times = processingTimes;
            PlotGraph();
            return processingTimes;
        }

        public static void selection(double[] input)
        {
            // Реализация сортировки выбором
            for (int i = 0; i < input.Length - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < input.Length; j++)
                {
                    if (input[j] < input[minIndex])
                    {
                        minIndex = j;
                    }
                }
                // Обмен элементов
                double temp = input[minIndex];
                input[minIndex] = input[i];
                input[i] = temp;
            }
        }
    }

}
