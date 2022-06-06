using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Xor_Processing_WritingIntoFile
{

    public partial class Form1 : Form
    {
        private static ManualResetEvent mre = new ManualResetEvent(false);

        string filename = null;
        public int nWorkerThreads;
        public int nCompletionThreads;

        public Thread Thread = null;
        public ThreadStart Thread_start = null;
        public Form1()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
            //textBox2.PasswordChar = '\0';
            textBox2.MaxLength = 12;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox2.PasswordChar = '\0';
            }
            else if (checkBox1.Checked != true) textBox2.PasswordChar = '*';
        }
        public void Enc()
        {
            filename = textBox1.Text;
            StreamReader reader = new StreamReader(filename);
            //progressBar1.Maximum = Invoke
            this.Invoke(new Action(() => { progressBar1.Value = 0; }));
            this.Invoke(new Action(() => { progressBar1.Maximum = System.IO.File.ReadAllText(filename).Length; }));

            string encryptedMessageByPass = Encrypt(System.IO.File.ReadAllText(filename), textBox2.Text);
            reader?.Close();
            StreamWriter writer = new StreamWriter(filename);
            writer.Write(encryptedMessageByPass);
            writer.Close();
        }
        public void Desc()
        {
            filename = textBox1.Text;
            StreamReader reader = new StreamReader(filename);
            //progressBar1.Maximum = System.IO.File.ReadAllText(filename).Length;
            this.Invoke(new Action(() => { progressBar1.Value = 0; }));
            this.Invoke(new Action(() => { progressBar1.Maximum = System.IO.File.ReadAllText(filename).Length; }));

            var DecryptMessageByPass = Decrypt(System.IO.File.ReadAllText(filename), textBox2.Text);
            reader?.Close();
            StreamWriter writer = new StreamWriter(filename);
            writer.Write(DecryptMessageByPass);
            writer.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            mre.Set();
            //ThreadPool.QueueUserWorkItem(JobForAThread);
            //var x = new XORCipher();

            progressBar1.Value = 0;
            //filename = textBox1.Text;
            //StreamReader reader = new StreamReader(filename);
            //progressBar1.Maximum = reader.ReadToEnd().Length;
            if (radioButton1.Checked == true)
            {
                Thread_start = new ThreadStart(Enc);
                Thread = new Thread(Thread_start);
                Thread.Start();
            }
            else if (radioButton2.Checked == true)
            {
                Thread_start = new ThreadStart(Desc);
                Thread = new Thread(Thread_start);
                Thread.Start();
            }
        }
        private string GetRepeatKey(string s, int n)
        {
            var r = s;
            while (r.Length < n)
            {
                r += r;
            }
            return r.Substring(0, n);
        }
        private string Cipher(string text, string secretKey)
        {
            progressBar1.Value = 0;
            var currentKey = GetRepeatKey(secretKey, text.Length);
            var res = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                res += ((char)(text[i] ^ currentKey[i])).ToString();
                Thread.Sleep(100);
                mre.WaitOne();
                this.Invoke(new Action(() => { progressBar1.Value += 1; }));
            }
            MessageBox.Show(res);
            return res;
        }
        public string Encrypt(string plainText, string password) => Cipher(plainText, password);
        public string Decrypt(string encryptedText, string password) => Cipher(encryptedText, password);
        //public void JobForAThread(object state)
        //{
        //    StreamReader reader = new StreamReader(filename);
        //    string length = reader.ReadToEnd();
        //    reader.Close();
        //    StreamWriter writer = new StreamWriter(filename);
        //    for (int i = 0; i < progressBar1.Maximum; i++)
        //    {
        //        progressBar1.Value += Thread.CurrentThread.ManagedThreadId;
        //        Thread.Sleep(200);
        //    }
        //    if(radioButton2.Checked == true)
        //    {
        //        writer.WriteLine(textBox2);
        //        writer?.Close();
        //    }
        //    else if(radioButton1.Checked == true)
        //    {
        //        var currentKey = GetRepeatKey(textBox2.Text, length.Length);
        //        var res = string.Empty;
        //        for (int i = 0; i < length.Length; i++)
        //        {
        //            res += ((char)(textBox2.Text[i] ^ currentKey[i])).ToString();
        //        }
        //        writer.WriteLine(res);
        //        writer?.Close();
        //    }

        //}

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            filename = null;
            filename = openFileDialog1.FileName;



            string fileText = System.IO.File.ReadAllText(filename);
            textBox1.Text = filename;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mre.Reset(); //so after you use mre.Set() it opens the gate threw and doesn't stop there where you have called mre.WaitOne()
            //and after you reset it it stops there where is mre.WaitOne
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mre.Set(); //so here we use 
        }
    }
    //public class XORCipher
    //{
    //private string GetRepeatKey(string s, int n)
    //{
    //    var r = s;
    //    while (r.Length < n)
    //    {
    //        r += r;
    //    }
    //    return r.Substring(0, n);
    //}
    //private string Cipher(string text, string secretKey)
    //{

    //    var currentKey = GetRepeatKey(secretKey, text.Length);
    //    var res = string.Empty;
    //    for (int i = 0; i < text.Length; i++)
    //    {
    //        res += ((char)(text[i] ^ currentKey[i])).ToString();
    //    }
    //    return res;
    //}
    //public string Encrypt(string plainText, string password) => Cipher(plainText, password);
    //public string Decrypt(string encryptedText, string password) => Cipher(encryptedText, password);
    //}
}
