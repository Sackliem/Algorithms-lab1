using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Stend
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //TODO:
            // переписать под многпоточность как нибудь
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Напишиете размер массива");
                return;
            }
            if (textBox3.Text.Length == 0)
            {
                MessageBox.Show("Напишиете размер подмассива");
                return;
            }
            if (comboBox1.Text.Length == 0)
            {
                MessageBox.Show("Выбери алгоритм");
                return;
            }
            if (comboBox2.Text.Length == 0)
            {
                MessageBox.Show("Выбери размерность времени");
                return;
            }
            Form2 form2 = new Form2(this);
            Task.Run(() => form2.ShowDialog());  
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
