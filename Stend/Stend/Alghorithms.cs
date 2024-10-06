using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace Stend
{
    public abstract class Algorithm : IDisposable
    {
        protected double[] Times;
        protected double times_formule;
        protected List<int[]> Arr;
        protected string name;
        protected string time_select;
        protected int block_size;
        protected int repeats;
        protected Chart chart;
        protected ProgressBar progressBar;
        private bool disposed = false;


        public abstract void Run(int[] input);
        public abstract int[] Run(int[] input, int exponent);



        // реализация интерфейса IDisposable.
        public void Dispose()
        {
            // освобождаем неуправляемые ресурсы
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                Arr.Clear();
                Times = new double[0];
            }
            // освобождаем неуправляемые объекты
            disposed = true;
        }

        ~Algorithm() 
        { 
            Dispose(false); 
        }

        public static List<int[]> SplitArray(int[] array, int increment)
        {
            List<int[]> subarrays = new List<int[]>();
            int currentSize = increment;

            while (currentSize <= array.Length)
            {
                int[] subarray = new int[currentSize];
                Array.Copy(array, 0, subarray, 0, currentSize);
                subarrays.Add(subarray);
                currentSize += increment;
            }

            // Добавляем последний подмассив, если остались элементы
            if (currentSize - increment < array.Length)
            {
                int[] lastSubarray = new int[array.Length];
                Array.Copy(array, 0, lastSubarray, 0, array.Length);
                subarrays.Add(lastSubarray);
            }

            return subarrays;
        }

        public void PlotGraph()
        {
            Title tt = new Title();
            tt.Name = name;
            tt.Text = name;
            chart.Titles.Add(tt);
            //chart.Series[0].Label = name;
            chart.Series[0].Name = name;
            chart.ChartAreas[0].AxisX.Title = "Lenght";
            chart.ChartAreas[0].AxisY.Title = time_select;
            chart.Series[0].Points.Clear();
            int numbers_lenght = Arr.Count;
            for (var i = 0; i != numbers_lenght; i++)
            {
                progressBar.PerformStep();
                chart.Series[0].Points.AddXY(Arr[i].Length, Times[i]);
            }
        }

        public void PlotGraph_Pow(int[] times, int[] numbers)
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
        public BubbleSort(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Bubble Sort";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }
        private static void Swap(ref int e1, ref int e2)
        {
            var temp = e1;
            e1 = e2;
            e2 = temp;
        }
        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту ху*ту не трогать
            return numbers;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        bubblesort(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;

                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        private static void bubblesort(int[] numbers)
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
        public QuickSort(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Quick Sort";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public static void Sort(int[] arr, int left, int right)
        {
            if (left < right)
            {
                int pivot = Partition(arr, left, right);

                Sort(arr, left, pivot - 1);
                Sort(arr, pivot + 1, right);
            }
            System.Console.WriteLine();
        }

        private static int Partition(int[] arr, int left, int right)
        {
            // Используем медиану для выбора опорного элемента
            int middle = left + (right - left) / 2;
            int pivot = arr[middle];
            Swap(arr, middle, right);  // Перемещаем опорный элемент в конец

            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                if (arr[j] <= pivot)
                {
                    i++;
                    Swap(arr, i, j);
                }
            }

            Swap(arr, i + 1, right);  // Перемещаем опорный элемент на своё место
            return i + 1;
        }

        private static void Swap(int[] arr, int i, int j)
        {
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }


        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        Sort(array[i], 0, array[i].Length - 1);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;

                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }
    }

    public class SumCalculator : Algorithm
    {
        public SumCalculator(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Sum";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        Sum(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуе*у не трогать
            return numbers;
        }

        public static void Sum(int[] numbers)
        {
            int sum = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i];
            }
        }

    }

    public class PolynomialEvaluator : Algorithm
    {

        public PolynomialEvaluator(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Polynom";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуе*у не трогать
            return numbers;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        Polynomial(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public static void Polynomial(int[] coefficients)
        {
            double result = 0;
            double x = 1.5;  // Примерное значение x
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
        public ConstantFunction(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Constant";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту ху*ту не трогать
            return numbers;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        Constant(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public static void Constant(int[] vector)
        {
            var zero = 0;
            for (int i = 0; i < 10000; i++)
            {
                zero += i;
            }
        }
    }

    public class ProductCalculator : Algorithm
    {

        public ProductCalculator(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Multiply";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        Product(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public static void Product(int[] vector)
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

        public TimSort(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Tim Sort";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        private static void InsertionSort(int[] array, int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                int temp = array[i];
                int j = i - 1;
                while (j >= left && array[j] > temp)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = temp;
            }
        }

        private static void Merge(int[] array, int left, int mid, int right)
        {
            int len1 = mid - left + 1;
            int len2 = right - mid;
            int[] leftArray = new int[len1];
            int[] rightArray = new int[len2];

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

        public static void timsort(int[] array)
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

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        timsort(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }
    }

    public class MatrixMultiplication : Algorithm
    {

        public MatrixMultiplication(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Matrix Multiplication";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public static void matrixmult(int[] input)
        {
            // input будет содержать одномерный массив для представления матриц
            int size = (int)Math.Sqrt(input.Length / 2);  // Определяем размер матриц
            int[,] matrixA = new int[size, size];
            int[,] matrixB = new int[size, size];

            // Заполнение матриц значениями из входного массива
            Buffer.BlockCopy(input, 0, matrixA, 0, size * size * sizeof(int));
            Buffer.BlockCopy(input, size * size * sizeof(int), matrixB, 0, size * size * sizeof(int));

            int[,] result = new int[size, size];
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

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        matrixmult(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }
    }

    public class PowerNative : Algorithm
    {

        public PowerNative(int repeat, ref Chart ch, ref ProgressBar progressBar1)
        {
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        private static int power_naive(int baseNumber, int exponent)
        {
            int result = 1;
            int times = 0;
            for (int i = 0; i < exponent; i++)
            {
                result *= baseNumber;
                times++;
            }
            return times;
        }

        public override void Run(int[] input)
        {
            // Эту хуе*у не трогать
        }
        public override int[] Run(int[] numbers, int exponent)
        {
            var len = numbers.Length;
            Array.Sort(numbers);
            List<int[]> avg_steps = new List<int[]>();  // Список для хранения количества шагов для каждого повторения

            // Настраиваем прогресс-бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = len * repeats;  // Общий прогресс — количество чисел * количество повторений
                progressBar.Value = 0;
            }));

            // Многопоточный расчет с использованием Parallel.For
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                int[] steps_for_avg = new int[numbers.Length];

                Parallel.For(0, len, i =>
                {
                    steps_for_avg[i] = power_naive(exponent, numbers[i]);  // Подсчет шагов для каждого числа

                    // Обновляем прогресс-бар после выполнения каждого элемента
                    progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                });

                avg_steps.Add(steps_for_avg);  // Сохраняем количество шагов для текущего повторения
            }

            // Вычисляем среднее количество шагов для каждого числа
            int[] processingSteps = new int[numbers.Length];
            Parallel.For(0, len, j =>
            {
                int totalSteps = 0;
                for (int i = 0; i < repeats; i++)
                {
                    totalSteps += avg_steps[i][j];
                }

                int avg = totalSteps / repeats;  // Среднее количество шагов для текущего числа
                processingSteps[j] = avg;
            });

            // Строим график с количеством шагов
            PlotGraph_Pow(processingSteps, numbers);

            return numbers;
        }

    }

    public class PowerRecursive : Algorithm
    {

        public PowerRecursive(int repeat, ref Chart ch, ref ProgressBar progressBar1)
        {
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        private static double power_recursive(double baseNumber, double exponent, ref int actionCount)
        {
            actionCount++; // Увеличиваем количество действий

            if (exponent == 0)
                return 1;
            return baseNumber * power_recursive(baseNumber, exponent - 1, ref actionCount);
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            var len = numbers.Length;
            Array.Sort(numbers);
            List<int[]> avg_steps = new List<int[]>();  // Список для хранения количества шагов для каждого повторения

            // Настраиваем прогресс-бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = len * repeats;  // Общий прогресс — количество чисел * количество повторений
                progressBar.Value = 0;
            }));

            // Многопоточный расчет с использованием Parallel.For
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                int[] steps_for_avg = new int[numbers.Length];

                Parallel.For(0, len, i =>
                {
                    int actions = 0;
                    power_recursive(exponent, numbers[i], ref actions);  // Подсчет шагов для каждого числа
                    steps_for_avg[i] = actions;
                    actions = 0;

                    // Обновляем прогресс-бар после выполнения каждого элемента
                    progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                });

                avg_steps.Add(steps_for_avg);  // Сохраняем количество шагов для текущего повторения
            }

            // Вычисляем среднее количество шагов для каждого числа
            int[] processingSteps = new int[numbers.Length];
            for (int j = 0; j < numbers.Length; j++)
            {
                int totalSteps = 0;
                for (int i = 0; i < repeats; i++)
                {
                    totalSteps += avg_steps[i][j];
                }

                int avg = totalSteps / repeats;  // Среднее количество шагов для текущего числа
                processingSteps[j] = avg;
            }

            // Строим график с количеством шагов
            PlotGraph_Pow(processingSteps, numbers);

            return numbers;
        }

        public override void Run(int[] input)
        {
            // Эту хуе*у не трогать
        }
    }

    public class PowerQuickPow : Algorithm
    {
        public PowerQuickPow(int repeat, ref Chart ch, ref ProgressBar progressBar1)
        {
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        private static int Power_quick_pow(int baseValue, int exponent)
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

        public override int[] Run(int[] numbers, int exponent)
        {
            var len = numbers.Length;
            Array.Sort(numbers);
            List<int[]> avg_steps = new List<int[]>();  // Список для хранения количества шагов для каждого повторения

            // Настраиваем прогресс-бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = len * repeats;  // Общий прогресс — количество чисел * количество повторений
                progressBar.Value = 0;
            }));

            // Многопоточный расчет с использованием Parallel.For
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                int[] steps_for_avg = new int[numbers.Length];

                Parallel.For(0, len, i =>
                {
                    steps_for_avg[i] = Power_quick_pow(exponent, numbers[i]);  // Подсчет шагов для каждого числа

                    // Обновляем прогресс-бар после выполнения каждого элемента
                    progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                });

                avg_steps.Add(steps_for_avg);  // Сохраняем количество шагов для текущего повторения
            }

            // Вычисляем среднее количество шагов для каждого числа
            int[] processingSteps = new int[numbers.Length];
            Parallel.For(0, len, j =>
            {
                int totalSteps = 0;
                for (int i = 0; i < repeats; i++)
                {
                    totalSteps += avg_steps[i][j];
                }

                int avg = totalSteps / repeats;  // Среднее количество шагов для текущего числа
                processingSteps[j] = avg;
            });

            // Строим график с количеством шагов
            PlotGraph_Pow(processingSteps, numbers);

            return numbers;
        }

        public override void Run(int[] input)
        {
            // Эту хуе*у не трогать
        }

    }

    public class PowerQuickPowClassic : Algorithm
    {
        public PowerQuickPowClassic(int repeat, ref Chart ch, ref ProgressBar progressBar1)
        {
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            var len = numbers.Length;
            Array.Sort(numbers);
            List<int[]> avg_steps = new List<int[]>();  // Список для хранения количества шагов для каждого повторения

            // Настраиваем прогресс-бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = len * repeats;  // Общий прогресс — количество чисел * количество повторений
                progressBar.Value = 0;
            }));

            // Многопоточный расчет с использованием Parallel.For
            for (int avg_i = 0; avg_i < repeats; avg_i++)
            {
                int[] steps_for_avg = new int[numbers.Length];

                Parallel.For(0, len, i =>
                {
                    steps_for_avg[i] = Power_quickPowClassic(exponent, numbers[i]);  // Подсчет шагов для каждого числа

                    // Обновляем прогресс-бар после выполнения каждого элемента
                    progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                });

                avg_steps.Add(steps_for_avg);  // Сохраняем количество шагов для текущего повторения
            }

            // Вычисляем среднее количество шагов для каждого числа
            int[] processingSteps = new int[numbers.Length];

            Parallel.For(0, len, j =>
            {
                int totalSteps = 0;
                for (int i = 0; i < repeats; i++)
                {
                    totalSteps += avg_steps[i][j];
                }

                int avg = totalSteps / repeats;  // Среднее количество шагов для текущего числа
                processingSteps[j] = avg;
            });

            // Строим график с количеством шагов
            PlotGraph_Pow(processingSteps, numbers);

            return numbers;
        }

        public override void Run(int[] numbers)
        {
            // Эту хуету не трогать
        }

        private static int Power_quickPowClassic(int baseValue, int exponent)
        {
            double f = 1; // Изначальный результат
            double c = baseValue; // Текущая степень основания
            int k = (int)exponent; // Текущая степень
            int times = 0;

            while (k > 0)
            {

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

            return times;
        }
    }

    public class BucketSort : Algorithm
    {
        public BucketSort(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Bucket Sort";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        int[] test = bucketsort(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public static int[] bucketsort(int[] array)
        {
            if (array == null || array.Length <= 1)
            {
                return array;
            }
            int maxValue = array[0];
            int minValue = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > maxValue)
                {
                    maxValue = array[i];
                }
                if (array[i] < minValue)
                {
                    minValue = array[i];
                }
            }
            LinkedList<int>[] bucket = new LinkedList<int>[maxValue - minValue + 1];
            for (int i = 0; i < array.Length; i++)
            {
                if (bucket[array[i] - minValue] == null)
                {
                    bucket[array[i] - minValue] = new LinkedList<int>();
                }
                bucket[array[i] - minValue].AddLast(array[i]);
            }
            var index = 0;

            for (int i = 0; i < bucket.Length; i++)
            {
                if (bucket[i] != null)
                {
                    LinkedListNode<int> node = bucket[i].First;
                    while (node != null)
                    {
                        array[index] = node.Value;
                        node = node.Next;
                        index++;
                    }
                }
            }
            return array;
        }
    }

    public class SelectionSort : Algorithm
    {
        public SelectionSort(int repeat, int size_block, string time, double times, ref Chart ch, ref ProgressBar progressBar1)
        {
            name = "Selection Sort";
            times_formule = times;
            block_size = size_block;
            time_select = time;
            progressBar = progressBar1;
            repeats = repeat;
            chart = ch;
        }

        public override int[] Run(int[] numbers, int exponent)
        {
            // Эту хуету не трогать
            return numbers;
        }

        public override async void Run(int[] numbers)
        {
            var len = numbers.Length;
            List<int[]> array = SplitArray(numbers, block_size);

            // Настраиваем прогресс бар
            progressBar.Invoke((Action)(() =>
            {
                progressBar.Visible = true;
                progressBar.Maximum = (array.Count * repeats) * 8 + array.Count;  // Общий прогресс — количество подмассивов * количество повторений
                progressBar.Value = 0;
            }));

            Arr = array;
            List<double[]> avg_times = new List<double[]>(); // Храним время всех повторений

            await Task.Run(() =>
            {
                for (int avg_i = 0; avg_i < repeats; avg_i++)
                {
                    double[] times_for_avg = new double[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        selection(array[i]);
                        stopwatch.Stop();
                        times_for_avg[i] = (double)stopwatch.Elapsed.TotalMilliseconds * times_formule;
                        // Обновляем прогресс бар после каждой итерации сортировки массива
                        progressBar.Invoke((Action)(() => progressBar.PerformStep()));
                    }
                    avg_times.Add(times_for_avg);
                }

                // Вычисляем среднее время для каждого подмассива
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

                Times = processingTimes;

                // Отображаем график в основном потоке
                chart.Invoke((Action)(() => PlotGraph()));
            });
        }

        public static void selection(int[] input)
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
                int temp = input[minIndex];
                input[minIndex] = input[i];
                input[i] = temp;
            }
        }
    }

}
