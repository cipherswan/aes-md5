﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AES_test
{
    public partial class Form1 : Form
    {

        private Thread thread;
        private Thread thread2;
        private Thread threadProg;
        private ManualResetEvent mre = new ManualResetEvent(false);


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }


        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public void EncryptFile(string a, string b)
        {
            string file = a;
            string password = b;

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            
            passwordBytes = MD5.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string fileEncrypted = @"C:\Users\Mindaugas\Desktop\decrypted_files\" + Path.GetFileName(file); 

            File.WriteAllBytes(fileEncrypted, bytesEncrypted);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < passwordBytes.Length; i++)

            {

                sb.Append(passwordBytes[i].ToString("X2"));

            }

            md5Label.Text = sb.ToString();

            string createText = sb.ToString();
           File.WriteAllText(@"C:\Users\Mindaugas\Desktop\hash.txt", createText);
        }


        public void DecryptFile(string a, string b)
        {
            string fileEncrypted = a;
            string password = b;


            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = MD5.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string file = @"C:\Users\Mindaugas\Desktop\decrypted_files\" + Path.GetFileName(fileEncrypted);
            File.WriteAllBytes(file, bytesDecrypted);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < passwordBytes.Length; i++)

            {

                sb.Append(passwordBytes[i].ToString("X2"));

            }

            md5LabelD.Text = sb.ToString();

            if (md5Label.Text == md5LabelD.Text)
            {
                label2.Text = "lygūs";
            }
            else
            {
                label2.Text = "nelygūs";
            }
        }


        void dirMethod(object obj)
        {
            // string psw = Convert.ToString(obj);

            //string psw = pswLabel.Text;

            string psw = pswTextBox.Text;




            //    pswLabel.Text = psw;
            string directory = dirTextBox.Text;
            string fileName = "*";
            int i = 1;

            

            string[] paths = Directory.GetFiles(directory, fileName, SearchOption.AllDirectories);

            foreach (string path in paths)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    richTextBox1.AppendText(Environment.NewLine + path);
                    richTextBox1.ScrollToCaret();
                    EncryptFile(path, psw);

                    i++;
                });
                Thread.Sleep(500);
                mre.WaitOne();
            }
        }
 

        void dirMethodDcrpt(object obj)
        {
            string directory = @"C:\Users\Mindaugas\Desktop\decrypted_files";
            string fileName = "*";
            string psw = pswTextBox.Text;
            int i = 1;
            




            string[] paths = Directory.GetFiles(directory, fileName, SearchOption.AllDirectories);

            foreach (string path in paths)
            {

                this.BeginInvoke((MethodInvoker)delegate
                {

                    richTextBox1.AppendText(Environment.NewLine + path);
                richTextBox1.ScrollToCaret();
                DecryptFile(path, psw);

                i++;
                });
                Thread.Sleep(500);
                mre.WaitOne();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {

                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                string realDirectory = folderBrowserDialog1.SelectedPath;
                dirTextBox.Text = realDirectory;

            }
        }

        void progressMethod(object obj)
        {
            int n = Convert.ToInt32(obj);
            
            

            for (int i = 0; i <= n; i++)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    progressBar1.Value = i;
                    progressBar1.Maximum = n;

                });

                Thread.Sleep(500);
                mre.WaitOne();
            }
        }

        private void EncryptBtn_Click(object sender, EventArgs e)
        {

            string psw = pswTextBox.Text;
            //pswLabel.Text = psw;
            string directory = dirTextBox.Text;
            string fileName = "*"; 

            int n = Directory.GetFiles(directory, fileName, SearchOption.AllDirectories).Length;

            if (thread == null || thread?.IsAlive == false && thread2 == null || thread2?.IsAlive == false)
            {
                thread = new Thread(new ParameterizedThreadStart(dirMethod));
                thread.Start();
                thread.IsBackground = true;

                threadProg = new Thread(new ParameterizedThreadStart(progressMethod));
                threadProg.Start(n);
                threadProg.IsBackground = true;

                mre = new ManualResetEvent(true);
                
            }
            else
            {
                MessageBox.Show("A thread is already running");
            }

            //Thread thread1 = new Thread(new ParameterizedThreadStart(dirMethod));
            //thread1.Start();

            // dirMethod();
        }

        private void DecryptBtn_Click(object sender, EventArgs e)
        {
            string directory = dirTextBox.Text;
            string fileName = "*";
            int n = Directory.GetFiles(directory, fileName, SearchOption.AllDirectories).Length;

            if (thread2 == null || thread2?.IsAlive == false)
            {
                thread2 = new Thread(new ParameterizedThreadStart(dirMethodDcrpt));
                thread2.Start();
                thread2.IsBackground = true;

                threadProg = new Thread(new ParameterizedThreadStart(progressMethod));
                threadProg.Start(n);
                threadProg.IsBackground = true;

                mre = new ManualResetEvent(true);

            }
            else
            {
                MessageBox.Show("A thread is already running");
            }


            //Thread thread2 = new Thread(new ParameterizedThreadStart(dirMethodDcrpt));
            //thread2.Start();

            // dirMethodDcrpt();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //pause

            mre.Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //continue

            mre.Set();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // cancel WIP

            if (thread2 != null || thread2?.IsAlive == true)
            {
                MessageBox.Show("vyksta decrypt");
            }
            else
            {
                MessageBox.Show("Nevyksta");
            }
          
        }
    }
}
