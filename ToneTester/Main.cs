using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToneTester_AI;

namespace ToneTester
{
    public partial class Main : Form
    {

        static Random rnd = new Random();
        public Dictionary<string, Color> moodColors = new Dictionary<string, Color>() { ["joy"] = Color.Green, ["fear"] = Color.BlueViolet, ["surprise"] = Color.Orange, ["anger"] = Color.Red, ["sadness"] = Color.Blue, ["love"] = Color.Pink };
        public static bool evaluated = false;
        public static Dictionary<string, float> moods = new Dictionary<string, float>();

        public Main()
        {
            InitializeComponent();
            CenterToScreen(); // Just makes the UI appear in the center of the screen. 
            pictureBox2.Paint += pictureBox2_Paint;
            pictureBox2.Resize += pictureBox2_Resize;
        }

        private void pictureBox2_Resize(object sender, EventArgs pe) // If the screen changes size we have to redraw the pie chart
        {
            pictureBox2.Invalidate();
        }
            
        private void pictureBox2_Paint(object sender, System.Windows.Forms.PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.Clear(pictureBox2.BackColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p = new Pen(Color.Black, 2);

            Rectangle rec = new Rectangle(0, 0, pictureBox2.Width-2, pictureBox2.Height-2);

            if (evaluated)
            {
                float total = 0;
                foreach (KeyValuePair<string, float> pair in moods)
                {
                    float degree = pair.Value * 360; // To get the size of the pie slice

                    Brush b1 = new SolidBrush(moodColors[pair.Key.ToLower()]);
                    g.FillPie(b1, rec, total, degree);
                    total += degree;
                    b1.Dispose();
                }
            }
            p.Dispose();
        }


        
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
            pictureBox2.Resize += new EventHandler(this.pictureBox2_Resize);
            this.DoubleBuffered = true;
        }

        private void UpdateUI(object sender, EventArgs e)
        {
            SentenceData result = Interpreter.GetMostProminent(textBox1.Text.ToLower(), true);
            evaluated = true;
            int i = 0;
            foreach (float val in result.score)
            {
                moods[moodColors.Keys.ElementAt(i)] = val;
                i++;
            }
            label1.Text = "Mood: " + result.type.FirstCharToUpper();
            richTextBox1.Text = textBox1.Text.ToLower().CapitalizeFirst();
            SetBoxText(richTextBox1);
            pictureBox2.Invalidate(); // To force the pie chart to redraw and display
        }

        public void SetBoxText(RichTextBox box)
        {
            int index = 0;
            foreach (string word in box.Text.Split(' '))
            {
                string mood = Interpreter.GetMostProminent(word).type;
                int selectStart = box.SelectionStart;
                if (moodColors.ContainsKey(mood.ToLower()))
                {
                    box.Select(index, word.Length);
                    box.SelectionColor = moodColors[mood.ToLower()];
                    box.Select(selectStart, 0);
                    box.SelectionColor = box.ForeColor;
                }
                index += word.Length + 1;
            }
        }

        // Quit function
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Just an about me menu

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ToneTester 0.2\nBuilt by Austin Phillips\n2022-2025");
        }
    }
}
