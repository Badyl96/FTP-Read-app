using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPReadApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;
            label6.Visible = false;
            label5.Visible = false;
            label4.Visible = false;
            
        }


        public void Checkping()
        {
            
            Ping ping = new Ping();

            for (int i = 14; i < FtpValues.splitFile.Length; i = i + 5)
            {
                string tempIp = FtpValues.splitFile[i];
                PingReply reply = ping.Send(tempIp);

                if (reply.Status == IPStatus.Success)
                {

                }
                else if (reply.Status == IPStatus.TimedOut)
                {

                    MessageBox.Show("Minal czas polaczenia" + tempIp);
                }
                else
                {
                    MessageBox.Show("Błąd połączenia" + tempIp);
                    this.Close();
                }
               
            }
        }

        FtpValues FtpValues = new FtpValues();
        
    
        public bool ReadDataFromFile()
        {
            try
            {
                /*
                string testFolder = FtpValues.Folder;

                //var path = Path.Combine(Directory.GetCurrentDirectory(), "\\config.ini");
                string file = File.ReadAllText(testFolder + "config.ini");


                FtpValues.splitFile = file.Split(new char[] { '=', ';', '\r', '\n', ';' });

                FtpValues.login = FtpValues.splitFile[3];
                FtpValues.password = FtpValues.splitFile[6];
                */
                
                var path = Path.Combine(Directory.GetCurrentDirectory());
                string file = File.ReadAllText(path + "/" + "config.ini");
                FtpValues.splitFile = file.Split(new char[] { '=', ';', '\r', '\n', ';' });
           

                FtpValues.login = FtpValues.splitFile[3];

                FtpValues.password = FtpValues.splitFile[6];
                
                return true;
            }
            catch (IOException)
            {

                MessageBox.Show("Nie ma dostępu do pliku");
                return false;
                /*
                MessageBox.Show("Prosze podać scieżkę dostępu");
                string testFolder = FtpValues.Folder;


                Form3 form3 = new Form3();
                form3.Show();
                if (FtpValues.Folder.Equals(""))
                {
                        
                        return false;
                }
                else
                {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "\\config.ini");
                        string file = File.ReadAllText(testFolder + "config.ini");


                        FtpValues.splitFile = file.Split(new char[] { '=', ';', '\r', '\n', ';' });

                        FtpValues.login = FtpValues.splitFile[3];
                        FtpValues.password = FtpValues.splitFile[6];


                        return true;
                }
               
               */   
            }    

            
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
               
                backgroundWorker1.RunWorkerAsync();
                button2.Enabled = false;
                button3.Enabled = true;
                button5.Enabled = false;
            }
        }

        

        private void progressBar1_Click_1(object sender, EventArgs e)
        {

        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            label8.Invoke(new Action(delegate ()
            {
                label8.Visible = false;
            }));


            DateTime date = DateTime.Now;
            string tempdate = date.ToString();

            var path = Path.Combine(Directory.GetCurrentDirectory());
            StreamWriter fileStream = File.CreateText(path + "/" + "lol.txt");

            if (ReadDataFromFile() == true)
            {
                //Check ping from file
                Checkping();

                
                List<string> list = new List<string>();
                List<string> list1 = new List<string>();
                List<string> list2 = new List<string>();

                for (int i = 14; i < FtpValues.splitFile.Length; i = i + 5)
                {
                    

                    string tempLocalFile = FtpValues.splitFile[9];
                    //Read number of folder 
                    int directoryCount = System.IO.Directory.GetDirectories(tempLocalFile).Length;

                    for (int j = 16; j < FtpValues.splitFile.Length; j=j+5)
                    {
                        string tempFolder = FtpValues.splitFile[j];

                       
                        //Check exist folders
                        bool exists = System.IO.Directory.Exists(tempLocalFile + tempFolder);
                        if(!exists)
                        {
                            System.IO.Directory.CreateDirectory(tempLocalFile + tempFolder);
                        }

                        //split ip from file
                        string tempIp = FtpValues.splitFile[i];

                        //FTP method
                        FtpWebRequest request1 = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + tempIp + "/StorageCard/ND1/"));
                        request1.Method = WebRequestMethods.Ftp.ListDirectory;
                        request1.Credentials = new NetworkCredential(FtpValues.login, FtpValues.password);
                        FtpWebResponse response = (FtpWebResponse)request1.GetResponse();
                        Stream responseStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(responseStream);

                        //Read all list from ftp and split '\n', '\r'
                        string readerFromFile = reader.ReadToEnd();
                        string[] reade = readerFromFile.Split(new char[] { '\n', '\r' });
                        list = reade.ToList<string>();
                        //delete empty spaces
                        list = list.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                        
                        //destination path and equals data
                        string directioryFolderRead = tempLocalFile + tempFolder;

                        DirectoryInfo di = new DirectoryInfo(directioryFolderRead);
                        
                        foreach (FileInfo d in di.GetFiles())
                        {
                            list1.Add(d.ToString());

                        }

                        //equals list from folder to ftp
                        FtpValues.DifferencesList = list.Except(list1).ToList();

                        reader.Close();
                        response.Close();

                        int a = 0;
                        foreach (string element in FtpValues.DifferencesList)
                        {
                            //progressbar
                            int counter = FtpValues.DifferencesList.Count();
                            a++;
                            
                            worker.ReportProgress(a * 100 / counter);
                            //System.Threading.Thread.Sleep(500);


                            label2.Invoke(new Action(delegate ()
                            {
                                label6.Visible = true;
                                label2.Text = element;
                            }));


                            List<string> temp = new List<string>();

                            temp.Add(element);

                            //Download Data from FTP
                            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + tempIp + "/StorageCard/ND1/" + element));
                            request.Credentials = new NetworkCredential(FtpValues.login, FtpValues.password);
                            request.Method = WebRequestMethods.Ftp.DownloadFile;  //Download Method
                                                                                  //request.Timeout = 1000;
                           
                            fileStream.WriteLine(element);

                            label3.Invoke(new Action(delegate ()
                            {
                                label3.Text = tempIp.ToString();
                                label5.Visible = true;
                            }));

                            Stream ftpstream = request.GetResponse().GetResponseStream();

                            
                            FileStream fs = new FileStream(tempLocalFile + tempFolder + "\\" + element, FileMode.Create);

                            // Method to calculate and show the progress.


                            label7.Invoke(new Action(delegate ()
                            {
                                label7.Text = tempFolder;
                                label4.Visible = true;
                            }));

                            int Length = 2048;
                            Byte[] buffer = new Byte[Length];
                            int bytesRead = ftpstream.Read(buffer, 0, Length);

                            while (bytesRead > 0)
                            {
                                
                                fs.Write(buffer, 0, bytesRead);
                                bytesRead = ftpstream.Read(buffer, 0, Length);

                                //FtpValues.progressbyte = fs.Position.ToString();

                            }
                            
                            ftpstream.Close();
                            fs.Close();

                        }
                    }
                }
            }
            else {

            }
         
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label1.Text = ("Pobrano....." + e.ProgressPercentage.ToString() + "%");
            progressBar1.Value = e.ProgressPercentage;
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            progressBar1.Value = 0;
            MessageBox.Show("Koniec pobierania");
            this.Close();
            /*
            if (e.Cancelled)
            {
                MessageBox.Show("Anulowano");
            }
            else {
                MessageBox.Show("Pobrano " + e.Result);
            }
            */
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Anulowano");
            progressBar1.Value = 0;
            backgroundWorker1.CancelAsync();
            Application.Restart();
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

      
        

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Koniec Programu");
            this.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
