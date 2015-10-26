using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace NewsLetter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

      
        private void btnReload_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            HttpWebRequest request =
               (HttpWebRequest)WebRequest.Create("http://217.218.67.142/nl/getemails.aspx?kind=1");

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            stream.Dispose();
            reader.Dispose();
            Email Rv = JsonConvert.DeserializeObject<Email>(result);

            int indx = 1;
            foreach (var item in Rv.Emails)
            {
                dataGridView1.Rows.Add(indx, item);
                indx++;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            string Url = "http://hispantv.com/services/newsletter.aspx?ids=" + txtItem1.Text.Trim() + "," + txtItem2.Text.Trim() + "," + txtItem3.Text.Trim() + "," + txtItem4.Text.Trim() + "," + txtItem5.Text.Trim();
            webBrowser1.Navigate(Url);
            Process objProcess = Process.Start("IEXPLORE.EXE", "-nomerge "+ Url);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //btnReload_Click(null, null);
        }
        public void SendEmail(string to)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                NetworkCredential basicCredential = new NetworkCredential("noreply@hispantv.com", "%123456%");
                //NetworkCredential basicCredential = new NetworkCredential("emad81@presstv.com", "1qaz!QAZ");
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress("noreply@hispantv.com");
            //    MailAddress fromAddress = new MailAddress("emad81@presstv.com");

                smtpClient.Host = "mail.hispantv.com";
            //    smtpClient.Host = "mail.presstv.com";
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = basicCredential;
                smtpClient.Timeout = (60 * 5 * 1000);

                message.From = fromAddress;
                message.Subject = txtSubject.Text.Trim();
                message.IsBodyHtml = true;
                message.Body = webBrowser1.Document.Body.OuterHtml;
                message.To.Add(to);
                smtpClient.Send(message);
            }
            catch { }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                if (txtSubject.Text.Trim().Length > 0)
                {
                    timer1.Enabled = true;
                    timer1_Tick(null, null);
                }
                else
                {
                    MessageBox.Show("Insert subject, Please");
                }
            }
            else
            {
                MessageBox.Show("Reload list, Please");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                SendEmail(dataGridView1.Rows[0].Cells[1].Value.ToString());
                dataGridView1.Rows.RemoveAt(0);
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
            }
            if (dataGridView1.Rows.Count == 0)
            {
                timer1.Enabled = false;
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                groupBox4.Enabled = true;
            }
        }
    }
}
