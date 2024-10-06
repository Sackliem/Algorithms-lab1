using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;

namespace Stend
{
  
    public partial class Form2 : Form
    {
        private string algorithm_select;
        public string time_select;
        private string data;
        private int select_replay;
        private int size;
        private int block_size;
        private Algorithm selectedAlgorithm;
        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            algorithm_select = mainForm.comboBox1.Text;
            time_select = mainForm.comboBox2.Text;
            try
            {
                string select_replay_text = mainForm.textBox2.Text;
                select_replay = int.Parse(select_replay_text);     
            }
            catch 
            {
                select_replay = 1;
            }
            try
            {
                string select_size_text = mainForm.textBox1.Text;
                string select_size_block_text = mainForm.textBox3.Text;
                size = int.Parse(select_size_text);
                block_size = int.Parse(select_size_block_text);
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так(");
                return;
            }
            graph.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            graph.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            graph.MouseWheel += graph_MouseWheel;
        }

        public int[] random_int(int lenght)
        {
            Random random = new Random();
            int[] input_ECP = new int[lenght];

            for (int i = 0; i < input_ECP.Length; ++i)
            {
                input_ECP[i] = random.Next();
            }
            return input_ECP;
        }
        
        private void graph_Click(object sender, EventArgs e)
        {
            
        }

        private void graph_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;
            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;
                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;
                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            /*
            seconds
            milliseconds
            microseconds
            nanoseconds
             */
            double time_formule = 1;
            switch (time_select)
            {
                case "seconds":
                    time_formule = 1/1000;
                    break;
                case "milliseconds":
                    time_formule = 1;
                    break;
                case "microseconds":
                    time_formule = 1000;
                    break;
                case "nanoseconds":
                    time_formule = 1000 * 1000;
                    break;
                default:
                    break;
            }
            Random random = new Random();
            int lenght = size;
            int[] arr = new int[lenght];
            switch (algorithm_select)
            {
                case "Babble sort":
                    arr = random_int(lenght);
                    selectedAlgorithm = new BubbleSort(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "StartQuickSort":
                    arr = random_int(lenght);
                    selectedAlgorithm = new QuickSort(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "Sum":
                    arr = random_int(lenght);
                    selectedAlgorithm = new SumCalculator(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "EvaluatePolynomiaNaive":
                    arr = random_int(lenght);
                    selectedAlgorithm = new PolynomialEvaluator(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "EvaluateConstantFunction":
                    arr = random_int(lenght);
                    selectedAlgorithm = new ConstantFunction(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "CalculateProduct":
                    arr = random_int(lenght);
                    selectedAlgorithm = new ProductCalculator(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "Timsort":
                    arr = random_int(lenght);
                    selectedAlgorithm = new TimSort(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "MatrixMultiplication":
                    arr = random_int(lenght);
                    selectedAlgorithm = new MatrixMultiplication(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "BucketSort":
                    arr = random_int(lenght);
                    selectedAlgorithm = new BucketSort(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    random = new Random();

                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(0, 99999);
                    }
                    selectedAlgorithm.Run(arr);
                    break;
                case "SelectionSort":
                    arr = random_int(lenght);
                    selectedAlgorithm = new SelectionSort(select_replay, block_size, time_select, time_formule, ref graph, ref progressBar1);
                    selectedAlgorithm.Run(arr);
                    break;
                case "PowerNative":
                    arr = random_int(lenght);
                    selectedAlgorithm = new PowerNative(select_replay, ref graph, ref progressBar1);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(1, 10);
                    }
                    selectedAlgorithm.Run(arr, 5);
                    break;
                case "PowerRecursive":
                    arr = random_int(lenght);
                    selectedAlgorithm = new PowerRecursive(select_replay, ref graph, ref progressBar1);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(1, 10);
                    }
                    selectedAlgorithm.Run(arr, 5);
                    break;
                case "PowerQuickPow":
                    arr = random_int(lenght);
                    selectedAlgorithm = new PowerQuickPow(select_replay, ref graph, ref progressBar1);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(1, 10);
                    }
                    selectedAlgorithm.Run(arr, 5);
                    break;
                case "PowerQuickPowClassic":
                    arr = random_int(lenght);
                    selectedAlgorithm = new PowerQuickPowClassic(select_replay, ref graph, ref progressBar1);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(1, 10);
                    }
                    selectedAlgorithm.Run(arr, 5);
                    break;
                default:
                    break;
            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            if (filename != "openFileDialog1")
            {
               data = File.ReadAllText(filename);
            }
        }
        class Check_times
        {
            public double numbers;
            public double times;
            public double replay;

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
