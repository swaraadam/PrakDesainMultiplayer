using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Net;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;

public class ServerUDP : MonoBehaviour {

    ClientUDP.PlayerStat player1, player2, tempPlayer;
    UdpClient serverUDP;
    List<IPEndPoint> iPEndPoints = new List<IPEndPoint>();
    public Text debugger;


    byte[] ObjectToByte(object obj)
    {
        if (obj == null)
        {
            return null;
        }
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryFormatter.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }
    }

    System.Object ByteToObject(byte[] _byte)
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        memoryStream.Write(_byte, 0, _byte.Length);
        memoryStream.Seek(0, SeekOrigin.Begin);
        System.Object obj = binaryFormatter.Deserialize(memoryStream);
        return obj;
    }


    // Use this for initialization
    void Start () {
        iPEndPoints.Clear();
        serverUDP = new UdpClient(4048);
        Thread broadcastMessage = new Thread(broadcastToClient);
        broadcastMessage.Start();
        Thread addNewClient = new Thread(addClient);
        addNewClient.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void broadcastToClient()
    {
        while (iPEndPoints.Count < 2)
        {
            serverUDP.Send(Encoding.Default.GetBytes("Selamat Kamu Diterima"), 0, new IPEndPoint(IPAddress.Broadcast, 4048));
        }
    }

    void addClient()
    {
        while (iPEndPoints.Count < 2)
        {
            IPEndPoint tempIPEndPoint = new IPEndPoint(IPAddress.Any, 4048);
            byte[] serverRequestData = serverUDP.Receive(ref tempIPEndPoint);
            if(tempIPEndPoint.Address != IPAddress.Parse("192.168.8.101"))
            {
                if (iPEndPoints.Count != 0)
                {
                    if (iPEndPoints[0] != tempIPEndPoint)
                    {
                        iPEndPoints.Add(tempIPEndPoint);
                        //debugger.text = iPEndPoints.Count + " " + iPEndPoints[iPEndPoints.Count - 1];
                        Debug.Log(iPEndPoints.Count + " " + iPEndPoints[iPEndPoints.Count - 1]);
                    }
                }
                else
                {
                    iPEndPoints.Add(tempIPEndPoint);
                    //debugger.text = iPEndPoints.Count + " " + iPEndPoints[iPEndPoints.Count - 1];
                    Debug.Log(iPEndPoints.Count + " " + iPEndPoints[iPEndPoints.Count - 1]);
                }
            }
        }
        serverUDP.Close();
        Thread receive = new Thread(receiveFromClient);
        receive.Start();
    }

    void receiveFromClient()
    {
        serverUDP = new UdpClient(4050);
        IPEndPoint tempIPEndPoint = new IPEndPoint(IPAddress.Any, 4050);
        byte[] receiveData = serverUDP.Receive(ref tempIPEndPoint);
        tempPlayer = (ClientUDP.PlayerStat)ByteToObject(receiveData);
        debugger.text = "menerima pesan";
        StartCoroutine(sendToAnotherClient(receiveData, tempIPEndPoint));
    }

    IEnumerator sendToAnotherClient(byte[] packetData, IPEndPoint iPAnotherClient)
    {
        var client = new UdpClient();

        foreach (IPEndPoint ip in iPEndPoints)
        {
            if (ip != iPAnotherClient)
            {
                client.Send(packetData, packetData.Length, new IPEndPoint(IPAddress.Parse(ip.ToString()), 4050));
            }
        }
        debugger.text = "mengirim pesan";
        yield return new WaitForSeconds(0.5f);
    }
}
