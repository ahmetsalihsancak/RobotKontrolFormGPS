using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;

namespace RobotKontrolFormGPS
{
    public partial class Form1 : Form
    {
        private string data;
        byte[,] a = new byte[160, 120];
        private byte b;
        private int baudrate = 1000000; //default baudrate
        public Form1()
        {
            InitializeComponent();
            //this.Load += Form1_Load;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ImageSetter();
            textBox1.ReadOnly = true; //textbox1 okuma modunda
            label6.Text = baudrate.ToString();
            label7.Text = "Default Baudrate: " + baudrate.ToString();
            string[] ports = SerialPort.GetPortNames(); //seri portlar diziye ekleniyor
            int[] baudrates = { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 31250, 38400, 57600, 115200,250000,1000000,2000000 };
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port); //seri portlar combobox'a ekleniyor
            }
            for (int i = 0; i < baudrates.Length; i++)
            {
                comboBox2.Items.Add(baudrates[i]);
            }
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived); //DataReceived eventini oluşturma
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gMapControl1.Position = new PointLatLng(39.940985403793995, 32.81912902229168);
            textBox4.Text = "39.940985403793995" + ", " + "32.81912902229168";
        }
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                //data = serialPort1.BytesToRead.ToString(); //Veriyi al
                data = serialPort1.ReadExisting();
                //b = serialPort1.ReadLine();
                this.Invoke(new EventHandler(displayData_event));
            }
        }
        private void displayData_event(object sender, EventArgs e)
        {
            textBox1.Text += DateTime.Now.ToString() + "  " + data + "\n"; //Gelen veriyi textBox içine güncel zaman ile ekle
            //pictureBox1.Image = Base64ToImage(data);
            //Console.WriteLine(data);
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret(); //textbox'ı kaydır
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) { 
                
            } //serialPort1.Write("1"); //ileri
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Write("2"); //sola
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Write("3"); //sağa
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Write("4"); //geri
        }

        private void button5_Click(object sender, EventArgs e) //BAGLAN BUTONU
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = comboBox1.Text; //ComboBox1'de seçili nesneyi port ismine ata
                    serialPort1.BaudRate = baudrate; //BaudRate ayarla
                    serialPort1.Parity = Parity.None;
                    serialPort1.DataBits = 8;
                    serialPort1.StopBits = StopBits.One;
                    serialPort1.Open(); //Seri portu aç
                    label2.Text = "BAĞLANTI VAR";
                    label2.ForeColor = Color.Green;
                    button5.Text = "BAĞLANTIYI KES";  //Buton5 yazısını değiştir
                }
                else
                {
                    label2.Text = "BAĞLANTI KESİLDİ";
                    label2.ForeColor = Color.Red;
                    button5.Text = "BAĞLAN";              //Buton5 yazısını değiştir
                    serialPort1.Close();                   //Seri portu kapa
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata"); //Hata mesajı göster
                throw;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //textBox1.ResetText(); //textBox1'i sıfırla
            textBox1.Clear();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();    //Seri port açıksa kapat
        }

        private void button6_Click(object sender, EventArgs e) //MAP LOAD BUTTON
        {
            if (textBox2.Text != "" || textBox3.Text != "") {
                float lat = float.Parse(textBox2.Text);
                float lng = float.Parse(textBox3.Text);
                gMapControl1.Position = new PointLatLng(lat, lng);  //lat, long bilgisini alır map'te gösterir
                textBox4.Text = textBox2.Text + ", " + textBox3.Text;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) //sayı fakat küsüratlı sayılar için, gps lat
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e) //sayı fakat küsüratlı sayılar için, gps long
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            baudrate = int.Parse(comboBox2.Text);
            serialPort1.BaudRate = baudrate;
            label6.Text = baudrate.ToString(); //baudrate ayarlama
        }
        private void ImageSetter()
        {
            int max_w = 160, max_h = 120;
            Image image = Image.FromFile(@"C:\Users\ahmet\OneDrive\Resimler\1.png");
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var bytes = ms.ToArray();
            Console.WriteLine(" BYTES TO ARRAY LENGTH " + bytes.Length); 
            Console.WriteLine(ImageToBase64(image, System.Drawing.Imaging.ImageFormat.Png));
            var imageMemoryStream = new MemoryStream(bytes);
            Image imgFromStream = Image.FromStream(imageMemoryStream);
            pictureBox1.Image = imgFromStream;
        }
        public string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to base 64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        public Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }
        public void dddddd()
        {
            
        }

    }
}
