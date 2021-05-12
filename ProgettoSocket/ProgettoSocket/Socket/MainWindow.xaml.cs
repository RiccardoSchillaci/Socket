using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//Aggiunta delle seguenti librerie
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace socket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int contErrori = 0;
        IPAddress provaip;
        int provaint;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {

            if(contErrori == 0)
            {
                if (IPAddress.TryParse(txtIPTuo.Text, out provaip) && int.TryParse(txtPortTuo.Text, out provaint) && provaint > 0 && provaint < 65536)
                {
                    IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse(txtIPTuo.Text), int.Parse(txtPortTuo.Text));
                    Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));
                    ricezione.Start(sourceSocket);
                    contErrori++;
                }
                else
                {
                    MessageBox.Show("Riscrivi ip e/o porta tuoi");
                }
                
                
            }
            btnInvia.IsEnabled = true;

        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //aggiunre controlli sul contenuto delle textbox
            
            //controllo del input delle textbox
            if (IPAddress.TryParse(txtIP.Text, out provaip) && int.TryParse(txtPort.Text, out provaint) && provaint > 0 && provaint < 65536)
            {
                SocketSend(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text), txtMsg.Text);
            }
            else
            {
                MessageBox.Show("Riscrivi ip e/o porta del destinatario");
            }
        }

        public async void SocketReceive(object socksource)
        {
            IPEndPoint ipendp = (IPEndPoint)socksource;

            Socket t = new Socket(ipendp.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            t.Bind(ipendp);

            Byte[] bytesRicevuti = new Byte[256];

            string message;

            int contaCaratteri = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if(t.Available >0)
                    {
                        message = "";

                        contaCaratteri = t.Receive(bytesRicevuti, bytesRicevuti.Length,0);
                        message = message + Encoding.ASCII.GetString(bytesRicevuti, 0, contaCaratteri);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {

                            lblRicevi.Content = message;
                        }));

                    }

                }

            });

        }

        public void SocketSend(IPAddress dest, int destport, string message)
        {
            Byte[] byteInviati = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destport);

            s.SendTo(byteInviati, remote_endpoint);
        }
    
    }
}
