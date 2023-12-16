using System;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;

namespace SAMSUNG_Easy_read_info
{
    internal class Program
    {
        static void Main(string[] args)
        {
			foreach (ManagementBaseObject managementBaseObject in new ManagementClass("Win32_SerialPort").GetInstances())
			{
				ManagementObject managementObject = (ManagementObject)managementBaseObject;
				if (managementObject["name"].ToString().Contains("SAMSUNG Mobile USB Modem"))
				{
					try
					{
						Thread.Sleep(1000);
						SerialPort serialPort = new SerialPort();
						serialPort.PortName = managementObject["deviceid"].ToString();
						serialPort.DataBits = 8;
						serialPort.BaudRate = 19200;
						Thread.Sleep(100);
						if (!serialPort.IsOpen)
						{
							serialPort.Open();
						}
						Thread.Sleep(100);
						serialPort.WriteLine("AT+DEVCONINFO\r");
						Thread.Sleep(100);
						string text = serialPort.ReadExisting();
						serialPort.Close();
						if (text.Contains("BUSY"))
						{
							serialPort.Dispose();
						}
						else
						{
							var groups = new Regex("MN\\((.*?)\\);BASE\\((.*?)\\);VER\\((.*?)/(.*?)/(.*?)/(.*?)\\);HIDVER\\(.*?\\);MNC\\(.*?\\);MCC\\(.*?\\);PRD\\((.*?)\\);.*?SN\\((.*?)\\);IMEI\\((.*?)\\);UN\\((.*?)\\);").Match(text).Groups;
							if (groups.Count > 1)
							{
								var dev = new DeviceInfo
								{
									Model = groups[1].Value,
									DeviceName = groups[1].Value,
									PDAVersion = groups[3].Value,
									CSCVersion = groups[4].Value,
									MODEMVersion = groups[5].Value,
									Region = groups[7].Value.Substring(groups[7].Value.Length - 3),
									SN = groups[8].Value,
									IMEI = groups[9].Value,
									UN = groups[10].Value
								};
								Console.WriteLine("Model : " + dev.Model);
								Console.WriteLine("DeviceName : " + dev.DeviceName);
								Console.WriteLine("PDA Version : " + dev.PDAVersion);
								Console.WriteLine("CSC Version : " + dev.CSCVersion);
								Console.WriteLine("MODEM Version : " + dev.MODEMVersion);
								Console.WriteLine("Region : " + dev.Region);
								Console.WriteLine("SN : " + dev.SN);
								Console.WriteLine("IMEI : " + dev.IMEI);
								Console.WriteLine("UN : " + dev.UN);
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}
	}
}
