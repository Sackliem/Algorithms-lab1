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

namespace Stend
{
  
    public partial class Form2 : Form
    {
        private string algorithm_select;
        private string data;
        private int select_replay;
        private Algorithm selectedAlgorithm;
        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            algorithm_select = mainForm.comboBox1.Text;
            string select_replay_text = mainForm.textBox2.Text;
            select_replay = int.Parse(select_replay_text);
        }

        public double[] random_double(int lenght)
        {
            Random random = new Random();
            double[] input_ECP = new double[lenght];

            for (int i = 0; i < input_ECP.Length; ++i)
            {
                input_ECP[i] = (double)random.Next();
            }
            return input_ECP;
        }
        
        private void graph_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int lenght = 500000;
            double[] arr = new double[lenght];
            switch (algorithm_select)
            {
                case "Babble sort":
                    arr = random_double(lenght);
                    selectedAlgorithm = new BubbleSort(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "StartQuickSort":
                    arr = random_double(lenght);
                    selectedAlgorithm = new QuickSort(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "Sum":
                    arr = random_double(lenght);
                    selectedAlgorithm = new SumCalculator(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "EvaluatePolynomiaNaive":
                    arr = random_double(lenght);
                    selectedAlgorithm = new PolynomialEvaluator(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "EvaluateConstantFunction":
                    arr = random_double(lenght);
                    selectedAlgorithm = new ConstantFunction(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "CalculateProduct":
                    arr = random_double(lenght);
                    selectedAlgorithm = new ProductCalculator(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "Timsort":
                    arr = random_double(lenght);
                    selectedAlgorithm = new TimSort(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "MatrixMultiplication":
                    arr = random_double(lenght);
                    selectedAlgorithm = new MatrixMultiplication(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "BucketSort":
                    arr = random_double(lenght);
                    selectedAlgorithm = new BucketSort(select_replay, ref graph);
                    random = new Random();

                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.NextDouble() * 0.99;
                    }
                    selectedAlgorithm.Run(arr);
                    break;
                case "SelectionSort":
                    arr = random_double(lenght);
                    selectedAlgorithm = new SelectionSort(select_replay, ref graph);
                    selectedAlgorithm.Run(arr);
                    break;
                case "PowerNative":
                    arr = random_double(lenght);
                    selectedAlgorithm = new PowerNative(select_replay, ref graph);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(1, 10);
                    }
                    selectedAlgorithm.Run(arr, 5);
                    break;
                case "PowerRecursive":
                    arr = random_double(lenght);
                    selectedAlgorithm = new PowerRecursive(select_replay, ref graph);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(1, 10);
                    }
                    selectedAlgorithm.Run(arr, 5);
                    break;
                case "PowerQuickPow":
                    arr = random_double(lenght);
                    selectedAlgorithm = new PowerQuickPow(select_replay, ref graph);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = random.Next(1, 10);
                    }
                    selectedAlgorithm.Run(arr, 5);
                    break;
                case "PowerQuickPowClassic":
                    arr = random_double(lenght);
                    selectedAlgorithm = new PowerQuickPowClassic(select_replay, ref graph);
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
    }
}
