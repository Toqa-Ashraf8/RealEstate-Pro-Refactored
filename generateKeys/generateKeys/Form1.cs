using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace generateKeys
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
          


        private void button1_Click(object sender, EventArgs e)
        {
            //Advanced Encryption Standard
            using (Aes aes = Aes.Create())
            { 
             
                aes.KeySize = 256; // تعيين حجم المفتاح إلى 256 بت  
                aes.GenerateKey(); // توليد المفتاح  

                byte[] key = aes.Key;
                textBox1.Text=Convert.ToBase64String(key).ToString();
            }    //Console.WriteLine("مفتاح 256 بت:");
               
            
           

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
