﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace _220425
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpListener Server; //伺服網路監聽器
        Socket Client; //給客戶用的連線物件
        Thread Th_Svr; //伺服器監聽用執行緒
        Thread Th_Clt; //客戶用的通話執行緒
        Hashtable HT = new Hashtable();

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Th_Svr = new Thread(ServerSub);
            Th_Svr.IsBackground = true;
            Th_Svr.Start();
            button1.Enabled = false;
        }

        //接受客戶連線要求的程式，針對每一個客戶建立一個連線，以及獨立執行緒
        private void ServerSub()
        {
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(textBox1.Text),int.Parse(textBox2.Text));

            Server = new TcpListener(EP); //建立Server端監聽器
            Server.Start(100); //最多連線數100人

            while (true)
            {
                Client = Server.AcceptSocket(); //建立客戶的連線物件
                Th_Clt = new Thread(Listen); //建立客戶連線獨立的執行緒
                Th_Clt.IsBackground = true;
                Th_Clt.Start();
            }
        }

        private void Listen()
        {
            Socket Sck = Client;
            Thread TH = Th_Clt;

            while (true)
            {
                try
                {
                    byte[] B = new byte[1023];
                    int inLen = Sck.Receive(B);
                    string Msg = Encoding.Default.GetString(B, 0, inLen);

                    string Cmd = Msg.Substring(0, 1);
                    String Str = Msg.Substring(1);
                    switch (Cmd)
                    {
                        case "0":
                            HT.Add(Str, Sck);
                            listBox1.Items.Add(Str);
                            break;
                        case "9":
                            HT.Remove(Str);
                            listBox1.Items.Remove(Str);
                            TH.Abort();
                            break;
                    }

                }
                catch(Exception)
                {
                    //忽略錯誤
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread(); //關閉所有執行緒
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
