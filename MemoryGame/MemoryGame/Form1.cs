using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MemoryGame
{
    public partial class MemoryGame : Form
    {
        private int score = 0;
        private int tries = 0;
        private int remainingTime = 60;
        private PictureBox[] pictureBoxes = new PictureBox[16];
        private PictureBox firstPictureBox = null;
        private Random random = new Random();
        private int maxTries;

        public MemoryGame()
        {
            InitializeComponent();
            // Configurarea picture box-urilor

            for (int i = 0; i < 16; i++)
            {
                pictureBoxes[i] = new PictureBox();
                pictureBoxes[i].Size = new Size(258, 134);
                pictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxes[i].Enabled = true;
                pictureBoxes[i].BringToFront();
                pictureBoxes[i].Click += PictureBox_Click;
                tableLayoutPanel1.Controls.Add(pictureBoxes[i]);

                easyToolStripMenuItem.Click += (sender, e) => SetDifficulty(60, int.MaxValue);
                normalToolStripMenuItem.Click += (sender, e) => SetDifficulty(60, 20);
                hardToolStripMenuItem.Click += (sender, e) => SetDifficulty(40, 16);

                SetDifficulty(60, int.MaxValue); // Default difficulty: Easy
            }
            btnStart.Click += (sender, e) =>
            {
                StartGame();
            };

            // Configurarea timer-ului
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

        private void StartGame()
        {
            // Inițializarea jocului
            score = 0;
            tries = 0;
            txtBoxScore.Text = "0";
            txtBoxTime.Text = "0";
            firstPictureBox = null;
            timer.Start();

            // Configurarea imaginilor pentru picture box-uri
            List<int> numbers = Enumerable.Range(0, 8).ToList();
            numbers.AddRange(numbers);
            numbers = numbers.OrderBy(x => random.Next()).ToList();
            for (int i = 0; i < 16; i++)
            {
                pictureBoxes[i].Tag = new KeyValuePair<int, int>(numbers[i], -1); //-1 e tag-ul pictureBox.image
                pictureBoxes[i].Image = Properties.Resources.question_mark;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Actualizarea timpului rămas
            remainingTime--;
            txtBoxTime.Text = remainingTime.ToString();
            if (remainingTime == 0)
            {
                timer.Stop();
                MessageBox.Show("Jocul s-a încheiat! Scorul tău este: " + score.ToString() + " cu numarul de incercari: " + tries.ToString());
            }
        }

        private async void PictureBox_Click(object sender, EventArgs e)
        {
            // Verificarea dacă două picture box-uri sunt potrivite
            PictureBox pictureBox = sender as PictureBox;
            KeyValuePair<int, int> tagValuePair = (KeyValuePair<int, int>)pictureBox.Tag;
            int number = tagValuePair.Key;
            int imageState = tagValuePair.Value;
            Debug.WriteLine("Click pe picture box cu tag-ul: " + pictureBox.Tag.ToString());
            //pictureBox.Image == Properties.Resources.question_mark -aceasta conditie nu functioneaza in if, de aceea am folosit KeyValuePair
            if (imageState == -1) //-1 tag-ul question_mark.jpg
                {
                if (firstPictureBox == null)
                    {
                        pictureBox.Image = GetImage(number);
                        pictureBox.Tag = new KeyValuePair<int, int>(number, number);
                        firstPictureBox = pictureBox;
                }
                    else
                    {
                        if (firstPictureBox != pictureBox)
                        {
                            maxTries--;
                            if (maxTries < 0)
                            {
                                 timer.Stop();
                                MessageBox.Show("Ai depășit numărul maxim de încercări! Jocul s-a încheiat.");
                                return;
                            }
                        pictureBox.Image = GetImage(number);
                            pictureBox.Tag = new KeyValuePair<int, int>(number, number);
                            
                        KeyValuePair<int, int> firstPictureBoxTag = (KeyValuePair<int, int>)firstPictureBox.Tag;
                        KeyValuePair<int, int> pictureBoxTag = (KeyValuePair<int, int>)pictureBox.Tag;
                        if (firstPictureBoxTag.Key == pictureBoxTag.Key)
                            {
                                score++;
                                txtBoxScore.Text = score.ToString();
                                firstPictureBox.Enabled = false;
                                pictureBox.Enabled = false;
                            if (IsSolved())
                            {
                                timer.Stop();
                                MessageBox.Show("Felicitări! Ai câștigat jocul cu un scor de: " + score.ToString() + " si un numar de incercari: " + tries.ToString());
                            }
                        }
                            else
                            {
                            await Task.Delay(1000);
                            firstPictureBox.Image = Properties.Resources.question_mark;
                            pictureBox.Image = Properties.Resources.question_mark;
                      
                            firstPictureBox.Tag = new KeyValuePair<int, int>(firstPictureBoxTag.Key, -1);
                            pictureBox.Tag = new KeyValuePair<int, int>(pictureBoxTag.Key, -1);
                            tries++;
                            txtBoxTries.Text = tries.ToString();
                        }
                            firstPictureBox = null;
                        }
                    }

            }
        }

        private Image GetImage(int number)
        {
            switch (number)
            {
                case 0:
                    return Properties.Resources.a;
                case 1:
                    return Properties.Resources.b;
                case 2:
                    return Properties.Resources.c;
                case 3:
                    return Properties.Resources.d;
                case 4:
                    return Properties.Resources.e;
                case 5:
                    return Properties.Resources.f;
                case 6:
                    return Properties.Resources.g;
                case 7:
                    return Properties.Resources.h;
                default:
                    return null;
            }
        }
        private bool IsSolved()
        {
            return pictureBoxes.All(pictureBox => !pictureBox.Enabled);
        }
        private void SetDifficulty(int time, int tries)
        {
            remainingTime = time;
            maxTries = tries;
        }

    }
}
