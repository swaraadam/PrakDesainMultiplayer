using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Net;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class ClientUDP : MonoBehaviour {

    PlayerStat player1, player2, tempPlayer, Me;
    UdpClient udpClient;
    IPEndPoint iPEndPoint;


	// Use this for initialization
	void Start () {
        player1 = new PlayerStat(0, 0, false, false);
        player2 = new PlayerStat(0, 0, false, false);
        tempPlayer = new PlayerStat(0, 0, false, false);
        Me = new PlayerStat(0, 0, false, false);
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(player1.ToString());
        //Debug.Log(player2.ToString());
        //Debug.Log(tempPlayer.ToString());
	}


    byte[] ObjectToByte(object obj)
    {
        if (obj == null)
        {
            return null;
        }
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using(MemoryStream memoryStream = new MemoryStream())
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

    [Serializable]
    public class PlayerStat
    {
        public int id;
        public int wheel;
        public bool steering;
        public bool motor;
        public PlayerStat(int _id, int _wheel, bool _steering, bool _motor)
        {
            id = _id;
            wheel = _wheel;
            steering = _steering;
            motor = _motor;
        }
    }

    //Network

    public void joinConnectionWithUDP()
    {
        try
        {
            udpClient = new UdpClient(4048);
            iPEndPoint = new IPEndPoint(IPAddress.Any, 4048);
            byte[] serverRequestData = udpClient.Receive(ref iPEndPoint);
            string serverRequest = Encoding.ASCII.GetString(serverRequestData);
            Debug.Log(serverRequest);
            Thread threadReceive = new Thread(receivePacketFromServer);
            StartCoroutine(pingingServer());
            threadReceive.Start();
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }

    IEnumerator pingingServer()
    {
        var client = new UdpClient();
        byte[] sendPacket = new byte[1024];
        sendPacket = ObjectToByte(Me);
        client.Send(sendPacket, sendPacket.Length, new IPEndPoint(IPAddress.Parse(iPEndPoint.ToString()), 4048));
        yield return new WaitForSeconds(1f);
    }

    IEnumerator sendPacketToServer()
    {
        var client = new UdpClient();
        byte[] sendPacket = new byte[1024];
        sendPacket = ObjectToByte(Me);
        client.Send(sendPacket, sendPacket.Length, new IPEndPoint(IPAddress.Parse(iPEndPoint.ToString()), 4050));
        yield return new WaitForSeconds(0.5f);
    }

    void receivePacketFromServer()
    {
        udpClient = new UdpClient(4050);
        IPEndPoint tempIPEndPoint = new IPEndPoint(IPAddress.Any, 4050);
        byte[] receiveData = udpClient.Receive(ref tempIPEndPoint);
        tempPlayer = (PlayerStat)ByteToObject(receiveData);
        //filterring
        Me = tempPlayer;
        switch (tempPlayer.id)
        {
            case 1:
                {
                    player1 = tempPlayer;
                    break;
                }
            case 2:
                {
                    player2 = tempPlayer;
                    break;
                }
        }
    }

    public void buttonNgirim()
    {
        StartCoroutine(sendPacketToServer());
    }
}
