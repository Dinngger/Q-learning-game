using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Q_learning
{
    public partial class Form1 : Form
    {
        Bot mybot;
        public Form1()
        {
            mybot = new Bot();
            InitializeComponent();
        }

        private void LabelPaint()
        {
            switch(mybot.Last)
            {
                case 0: label3.Text = "机器：石头";break;
                case 1: label3.Text = "机器：剪刀";break;
                case 2: label3.Text = "机器： 布 ";break;
                default:label3.Text = "机器：";break;
            }
            label4.Text = string.Format("五局三胜制 总比分： {0}:{1}:{2}", mybot.Win, mybot.Even, mybot.Lose);
            label1.Text = string.Format("机器：{0}", mybot.Thiswin);
            label5.Text = string.Format("平：{0}", mybot.Thiseven);
            label2.Text = string.Format("你：{0}", mybot.Thislose);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mybot.NextS(0);
            LabelPaint();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mybot.NextS(1);
            LabelPaint();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mybot.NextS(2);
            LabelPaint();
        }
    }

    public class Bot
    {
        int win;
        int lose;
        int even;
        int thiswin;
        int thiseven;
        int thislose;
        int[] lasthree = new int[3] { 5, 5, 5 };
        int counter = 0;
        double[] Q0 = new double[3];
        double[,] Q1 = new double[3, 3];
        double[,,] Q2 = new double[3, 3, 3];
        double[,,,] Q3 = new double[3, 3, 3, 3];
        double[,,,,] Q4 = new double[3, 3, 3, 3, 3];
        int step;
        int[] state = new int[5];
        int laststep = -1;
        int thisstep;
        Random rd = new Random();

        public int Win => win;
        public int Lose => lose;
        public int Even => even;
        public int Thiswin => thiswin;
        public int Thiseven => thiseven;
        public int Thislose => thislose;
        public int Last => laststep;

        private int Is_win(int a, int b)
        {
            if (a == b)
                return 2;
            else if (a == b - 1 || (a == 2 && b == 0))
                return 3;
            else
                return 1;
        }

        private void Init()
        {
            step = 0;
            thiseven = 0;
            thislose = 0;
            thiswin = 0;
        }

        private void Count(int w)
        {
            lasthree[counter] = w;
            counter++;
            if (counter > 2)
                counter = 0;
        }

        private void ReQ(ref double L, double N, double rate)
        {
            if (L == 0)
                L = N;
            else
                L = (L + rate * N) / (1 + rate);
        }

        public void NextS(int input)
        {
            state[step] = input;
            laststep = thisstep;
            int iswin = Is_win(laststep, input);
            if (iswin == 1)
                thislose++;
            else if (iswin == 3)
                thiswin++;
            else
                thiseven++;
            double rate = (lasthree[0] + lasthree[1] + lasthree[2]) / 15.0;
            switch (step)
            {
                case 0: ReQ(ref Q0[laststep], iswin, rate); break;
                case 1: ReQ(ref Q1[state[0], laststep], iswin, rate); break;
                case 2: ReQ(ref Q2[state[0], state[1], laststep], iswin, rate); break;
                case 3: ReQ(ref Q3[state[0], state[1], state[2], laststep], iswin, rate); break;
                case 4: ReQ(ref Q4[state[0], state[1], state[2], state[3], laststep], iswin, rate);break;
                default:break;
            }

            step++;
            if (step > 4)
                step = 0;

            if (thiswin > 2 || (step == 0 && thiswin > thislose))
            {
                MessageBox.Show("你输了");
                win++;
                Init();
                Count(5);
            }
            else if (thislose > 2 || (step == 0 && thiswin < thislose))
            {
                MessageBox.Show("你赢了");
                lose++;
                Init();
                Count(3);
            }
            else if (step == 0 && thiswin == thislose)
            {
                MessageBox.Show("平局");
                even++;
                Init();
                Count(4);
            }
            double[] theQ = new double[3];
            for (int i=0; i<3; i++)
            {
                double thisQ;
                switch (step)
                {
                    case 0: thisQ = Q0[i]; break;
                    case 1: thisQ = Q1[state[0], i]; break;
                    case 2: thisQ = Q2[state[0], state[1], i]; break;
                    case 3: thisQ = Q3[state[0], state[1], state[2], i]; break;
                    case 4: thisQ = Q4[state[0], state[1], state[2], state[3], i]; break;
                    default: thisQ = 0; break;
                }
                theQ[i] = thisQ;
            }
            int explore = 0;
            for (int i=0; i<3; i++)
            {
                if (theQ[i] == 0)
                    explore++;
            }
            if (explore == 3)
                thisstep = rd.Next(0, 3);
            else if (explore == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (theQ[i] != 0)
                    {
                        if (rd.Next(0, 2) == 0)
                        {
                            thisstep = i;
                        }
                        else
                        {
                            thisstep = rd.Next(0, 3);
                            while (thisstep == i)
                            {
                                thisstep = rd.Next(0, 3);
                            }
                        }
                        break;
                    }
                }
            }
            else if (explore == 1)
            {
                for (int i=0; i<3; i++)
                {
                    if (theQ[i] == 0)
                    {
                        if (rd.Next(0, 3) == 0)
                            thisstep = i;
                        else
                        {
                            for (int j=0; j<3; j++)
                            {
                                if (theQ[j] == theQ.Max())
                                    thisstep = j;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                for (int i=0; i<3; i++)
                {
                    if (theQ[i] == theQ.Max())
                        thisstep = i;
                }
            }
        }
        public Bot()
        {
            Q0.Initialize();
            Q1.Initialize();
            Q2.Initialize();
            Q3.Initialize();
            Q4.Initialize();
            thisstep = rd.Next(0, 3);
            Init();
            win = 0;
            lose = 0;
            even = 0;
        }
    }
}
