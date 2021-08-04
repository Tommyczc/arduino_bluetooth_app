using System;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;
using UnityEngine.SceneManagement;

public class Bluetooth_control : MonoBehaviour
{
    private BluetoothHelper helper;
    private string device_name;
    public static bool hex_or_string = true; //hex:true, string false
    public Text recive;
    public Text device;
    public InputField send_message;
    public Button send;
    public Button up;
    public Button down;
    public Button right;
    public Button left;
    public Button one;
    public Button two;
    public Button three;
    public Button four;
    public Button five;
    public byte[] up_hex = { 0x00, 0x01 };
    public byte[] down_hex= { 0x00,0x02 };
    public byte[] right_hex = { 0x00, 0x03 };
    public byte[] left_hex = { 0x00, 0x04 };
    public byte[] break_hex = { 0x00, 0x05 };
    public byte[] one_hex = { 0x01, 0x01 };
    public byte[] two_hex = { 0x01, 0x02 };
    public byte[] three_hex = { 0x01, 0x03 };
    public byte[] four_hex = { 0x01, 0x04 };
    public byte[] five_hex = { 0x01, 0x05 };

    // Start is called before the first frame update
    void Start()
    {
        helper = BluetoothHelper.GetInstance();
        helper.OnDataReceived += OnDataReceived;
        helper.OnConnectionFailed += OnConnectionFailed;
        helper.setFixedLengthBasedStream(2);
        this.device_name = Bluetooth_Connection.the_device;
        device.text="Device: "+ Bluetooth_Connection.the_device;
        helper.StartListening();
    }

    public void OnDataReceived(BluetoothHelper helper) {
        string to_string="";
        string message = helper.Read();
        if (message.Length == 2)
        {
            to_string = recive_checker(transformer(message));
            //Debug.Log("data is recived: " + final[0] + "   length: " + final.Length);
        }
        //recive.text = to_string;
    }

    public byte[] transformer(string message) {
        byte[] final = new byte[message.Length];
        for (int i = 0; i < message.Length; i++)
        {
            string msg = message.Substring(i, 1);
            byte[] decBytes = System.Text.Encoding.ASCII.GetBytes(msg);
            int temp = (short)(decBytes[0]);
            if (temp < 48) { continue; }
            else { decBytes = BitConverter.GetBytes(temp - 48); }
            final[i] = decBytes[0];
        }
        return final;
    }

    public string recive_checker(byte[] msg) {
        string final = "";
        if (msg[0] == 0x00)
        {
            final = "U are control the car";
            Debug.Log("u are control the car");
            switch (msg[1]) {
                case 0x00: 
                    recive.text = "Message: Go forward!";
                    break;

                case 0x01:
                    recive.text = "Message: Go back!";
                    break;

                case 0x02:
                    recive.text = "Message: Go left!";
                    break;

                case 0x03:
                    recive.text = "Message: Go right!";
                    break;

                case 0x04:
                    recive.text = "Message: Brake!";
                    break;

            }
        }
        else if (msg[0] == 0x01) {
            final = "U are  control the camera";
            Debug.Log("U are  control the camera");
            int temp = (short)(msg[1]);
            string t_temp = temp.ToString();
            recive.text = "Message: Detect distance: " + t_temp;
        }
        return final;
    }

    public void OnConnectionFailed(BluetoothHelper helper) {
        SceneManager.LoadScene("Connect_scene");
        device.text = "Device: null";
        helper.StartListening();
    }

    public void send_button_function() {
        if (!string.IsNullOrEmpty(send_message.text))
        {
            string msg = send_message.text;
            Debug.Log("sending message: "+msg);
            sendData_string(msg);
            send_message.text = "";
        }
    }

    public void sendData_hex(byte[] d)
    {
        helper.SendData(d);
    }

    public void sendData_string(string d)
    {
        helper.SendData(d);
    }

    // Update is called once per frame
    void Update()
    {
        if (!helper.isConnected()) { SceneManager.LoadScene("Connect_scene"); }
        if (hex_or_string==true) { 
            
        }
    }

    void OnDestroy()
    {
        helper.OnDataReceived -= OnDataReceived;
        helper.OnConnectionFailed -= OnConnectionFailed;
        helper.Disconnect();
    }

}
