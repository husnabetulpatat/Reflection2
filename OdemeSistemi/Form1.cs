using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OdemeSistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Reflection ile ödeme yöntemlerini ComboBox'a ekle
            var odemeYontemleri = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IOdemeYontemi).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var tip in odemeYontemleri)
            {
                comboBox1.Items.Add(tip.Name);
            }
        }

        private void btnKlasorSec_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                string path = dialog.SelectedPath;
                KlasorVeClasslariListele(path);
            }
        }

        private void KlasorVeClasslariListele(string path)
        {
            string[] klasorler = Directory.GetDirectories(path);
            foreach (var klasor in klasorler)
            {
                listBox1.Items.Add($"📁 {Path.GetFileName(klasor)}");
                var csDosyalari = Directory.GetFiles(klasor, "*.cs");

                foreach (var csDosyasi in csDosyalari)
                {
                    var classIsimleri = File.ReadLines(csDosyasi)
                        .Where(line => line.Contains("class"))
                        .Select(line => line.Split(' ')[2].Trim())
                        .ToList();

                    foreach (var classIsmi in classIsimleri)
                    {
                        listBox1.Items.Add($"   📄 {classIsmi}");
                    }
                }
            }
        }

        private void btnOde_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || !decimal.TryParse(textBox1.Text, out decimal tutar))
            {
                MessageBox.Show("Lütfen geçerli bir tutar ve ödeme yöntemi giriniz.");
                return;
            }

            string secilenSinif = comboBox1.SelectedItem.ToString();
            var tip = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == secilenSinif);

            if (tip != null)
            {
                IOdemeYontemi yontem = (IOdemeYontemi)Activator.CreateInstance(tip);
                labelSonuc.Text = yontem.Ode(tutar);
            }
            else
            {
                MessageBox.Show("Geçersiz ödeme yöntemi seçildi.");
            }
        }
    }
}
