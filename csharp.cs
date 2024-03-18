using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YourNamespace
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            // Assuming buffer is defined somewhere in your code
            byte[] buffer = { 0x12, 0x34, 0xAB, 0xCD, 0xEF };

            // Define a list to store the hexadecimal strings
            List<string> hexStrings = new List<string>();

            // Convert each byte array to a hexadecimal string and add it to the list
            foreach (byte b in buffer)
            {
                hexStrings.Add(b.ToString("X2")); // "X2" formats the byte as a hexadecimal string
            }

            // Display the hexadecimal strings in a list box or any other control
            listBox1.Items.Clear();
            listBox1.Items.AddRange(hexStrings.ToArray());
        }
    }
}


/////adding to list is in above 






using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

namespace YourNamespace
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();

            // Initialize the serial port
            serialPort = new SerialPort("COM1", 9600); // Change "COM1" to your Arduino's COM port
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Read the data from the serial port
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

            // Define a list to store the hexadecimal strings
            List<string> hexStrings = new List<string>();

            // Convert each byte array to a hexadecimal string and add it to the list
            foreach (byte b in buffer)
            {
                hexStrings.Add(b.ToString("X2")); // "X2" formats the byte as a hexadecimal string
            }

            // Update the UI with the hexadecimal strings
            Invoke(new Action(() =>
            {
                listBox1.Items.Clear();
                listBox1.Items.AddRange(hexStrings.ToArray());
            }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the serial port when the form is closing
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}






///batching is in below
///

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace YourNamespace
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        List<byte> receivedData = new List<byte>();

        public Form1()
        {
            InitializeComponent();

            // Initialize the serial port
            serialPort = new SerialPort("COM1", 9600); // Change "COM1" to your Arduino's COM port
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Read the data from the serial port
            int bytesToRead = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);

            // Add the received data to the list
            receivedData.AddRange(buffer);

            // Process the data in batches of 25 bytes
            while (receivedData.Count >= 25)
            {
                // Get a batch of 25 bytes
                byte[] batch = receivedData.Take(25).ToArray();
                receivedData.RemoveRange(0, 25);

                // Check the header and footer IDs
                if (batch[0] == 0x05 && batch[24] == 0x0A)
                {
                    // Header and footer IDs are correct
                    // Process the batch further if needed
                    List<string> hexStrings = batch.Select(b => b.ToString("X2")).ToList();

                    // Update the UI with the hexadecimal strings
                    Invoke(new Action(() =>
                    {
                        listBox1.Items.AddRange(hexStrings.ToArray());
                    }));
                }
                else
                {
                    // Header or footer IDs are incorrect
                    // You may want to handle this case accordingly
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the serial port when the form is closing
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}




////In this modified version:

//We maintain a List<byte> called receivedData to accumulate the received bytes.
//When data is received, we add it to the receivedData list.
//We then process the data in batches of 25 bytes each. If there's enough data in receivedData to form a batch, we take the first 25 bytes and process them.
//We check if the first byte (header) of the batch is 0x05 and if the last byte (footer) is 0x0A. If both conditions are met, we process the batch further (in this case, converting it to hexadecimal strings and updating the UI).
//If the header or footer IDs are incorrect, you may want to handle this case accordingly. For simplicity, the code here just skips processing such batches.
//Make sure to replace "COM1" with the appropriate COM port your Arduino is connected to. Also, handle the FormClosing event to ensure the serial port is closed properly when the form is closed.