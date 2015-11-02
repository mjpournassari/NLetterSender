﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
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
        string _Lastemail = string.Empty;

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
            Process objProcess = Process.Start("IEXPLORE.EXE", "-nomerge " + Url);
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
                message.Body = webBrowser1.Document.Body.OuterHtml.Replace("[[UN]]", "http://217.218.64.54/nl/unsubscribe.aspx?email=" + Encrypt(to, true));
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
 
        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 2 && e.RowIndex != -1)
            {
                ((DataGridView)sender).Cursor = Cursors.Hand;
            }
            else
            {
                ((DataGridView)sender).Cursor = Cursors.Default;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            btnReload_Click(null, null);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _Lastemail = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            if (((DataGridView)sender).CurrentCell.ColumnIndex.Equals(2))
            {
                //Save:
                if (!_Lastemail.Equals(string.Empty))
                {
                    if (_Lastemail != dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString().Trim())
                    {
                        HttpWebRequest request =
                       (HttpWebRequest)WebRequest.Create("http://217.218.67.142/nl/updateemail.aspx?email=" + _Lastemail + "&newemail=" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString().Trim() + "");

                        WebResponse response = request.GetResponse();
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        var result = reader.ReadToEnd();
                        stream.Dispose();
                        reader.Dispose();
                        _Lastemail = string.Empty;
                        MessageBox.Show("Email Changed to:" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    }
                    else
                    {
                        MessageBox.Show("New email and old are the same");
                    }
                }
                else
                {
                    MessageBox.Show("Please Change Email first");
                }
            }
            if (((DataGridView)sender).CurrentCell.ColumnIndex.Equals(3))
            {
               DialogResult dr= MessageBox.Show("Are you sure to delete:"+ dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + "???","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if(dr==DialogResult.Yes)
                {
                    HttpWebRequest request =
                      (HttpWebRequest)WebRequest.Create("http://217.218.64.54/nl/unsubscribe.aspx?email=" + Encrypt(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), true));

                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    var result = reader.ReadToEnd();
                    stream.Dispose();
                    reader.Dispose();
                    _Lastemail = string.Empty;
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    MessageBox.Show("Email has been removed");
                }
            }

        }
        public string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            string key = "ez.WWtG-6YEu(Rm+";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}
