using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Socket_Data_Transfer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_start_server_Click(object sender, EventArgs e)
        {
            TCPServer obj_server = new TCPServer();
            System.Threading.Thread obj_thread = new System.Threading.Thread(obj_server.StartServer);
            obj_thread.Start();
        }

        private void Btn_send_Click(object sender, EventArgs e)
        {
            TcpClient tc = new TcpClient("127.0.0.1", 6868);
            NetworkStream ns = tc.GetStream();
            byte[] data_tosend = CreateDataPacket(Encoding.UTF8.GetBytes(tx_data_send.Text));
            ns.Write(data_tosend, 0, data_tosend.Length);
        }
        private byte[] CreateDataPacket(byte[] data)
        {
            byte[] initialize = new byte[1];
            initialize[0] = 2;
            byte[] separator = new byte[1];
            separator[0] = 4;
            byte[] datalength = Encoding.UTF8.GetBytes(Convert.ToString(data.Length));
            MemoryStream ms = new MemoryStream();
            ms.Write(initialize, 0, initialize.Length);
            ms.Write(datalength, 0, datalength.Length);
            ms.Write(separator, 0, separator.Length);
            ms.Write(data, 0, data.Length);
            return ms.ToArray();
        }
    }
    class TCPServer
    {
        TcpListener obj_server;
        public TCPServer()
        {
            obj_server = new TcpListener(IPAddress.Any, 6868);
        }

        public void StartServer()
        {
            obj_server.Start();
            while (true)
            {
                TcpClient tc = obj_server.AcceptTcpClient();
                NetworkStream ns = tc.GetStream();
                if (ns.ReadByte() == 2)
                {
                    byte[] recv_data = ReadStream(ns);
                    Form1.ActiveForm.Invoke(new MethodInvoker(delegate
                    {
                        ((TextBox)Form1.ActiveForm.Controls.Find("tx_recv_data", true)[0]).Text = Encoding.UTF8.GetString(recv_data);
                    }));
                }
            }
        }

        public byte[] ReadStream(NetworkStream ns)
        {
            byte[] data_buff = null;

            int b = 0;
            String buff_length = "";
            while ((b = ns.ReadByte()) != 4)
            {
                buff_length += (char)b;
            }
            int data_length = Convert.ToInt32(buff_length);
            data_buff = new byte[data_length];
            int byte_read = 0;
            int byte_offset = 0;
            while (byte_offset < data_length)
            {
                byte_read = ns.Read(data_buff, byte_offset, data_length - byte_offset);
                byte_offset += byte_read;
            }

            return data_buff;
        }
    }
    }
