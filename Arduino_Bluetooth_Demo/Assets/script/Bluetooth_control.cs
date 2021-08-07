using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;
using UnityEngine.SceneManagement;

public class Bluetooth_control : MonoBehaviour
{
    private BluetoothHelper helper;
    private string device_name;
    private float current_Position;
    public Slider slider_camera;
    public static bool hex_or_string = true; //hex:true, string false
    public Text recive;
    public Text device;
    public Text the_num;
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
    public byte[] up_hex = { 0x00, 0x04 };
    public byte[] down_hex= { 0x00,0x05 };
    public byte[] right_hex = { 0x00, 0x06 };
    public byte[] left_hex = { 0x00, 0x07 };
    public byte[] brake_hex = { 0x00, 0x08 };
    public byte[] one_hex = { 0x01, 0x04 };
    public byte[] two_hex = { 0x01, 0x05 };
    public byte[] three_hex = { 0x01, 0x06 };
    public byte[] four_hex = { 0x01, 0x07 };
    public byte[] five_hex = { 0x01, 0x08 };
    public string up_string = "A";
    public string down_string = "B";
    public string left_string = "C"; 
    public string right_string = "D";
    public string brake_string = "E";
    public string one_string = "1";
    public string two_string = "2";
    public string three_string = "3";
    public string four_string = "4";
    public string five_string = "5";


    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        helper = BluetoothHelper.GetInstance();
        helper.OnDataReceived += OnDataReceived;
        helper.OnConnectionFailed += OnConnectionFailed;
        helper.setFixedLengthBasedStream(2);
        this.device_name = Bluetooth_Connection.the_device;
        device.text="Device: "+ Bluetooth_Connection.the_device;
        helper.StartListening();
        slider_camera.value = 6;

        /*
        slider_camera.onValueChanged.AddListener((value) =>
        {
            int num = (int)value;
            change_num(num*15);
            Debug.Log(slider_camera.name + "的Value值为" + num);
            byte[] final = new byte[2];
            final[0] = 0x02;
            byte[] tempdecBytes = BitConverter.GetBytes(num);
            final[1] = tempdecBytes[0];
            sendData_hex(final);
            StartCoroutine(DoSomeThingInDelay());
        }) ;
        */
    }
    /*
    IEnumerator DoSomeThingInDelay() {
        Debug.Log("waiting");
        yield return new WaitForSeconds(5.0f); //等待3秒
    }
    */
    public void reset_button_function() {
        slider_camera.value = 6;
        rotate_camera(6.0f);
    }

    private void change_num(int num) {
        the_num.text = num.ToString();
    }

    public void OnDataReceived(BluetoothHelper helper) {
        string message = helper.Read();
        if (message.Length == 2)
        {
            recive_checker(transformer(message));
            //Debug.Log("data is recived: " + final[0] + "   length: " + final.Length);
        }
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

    public void recive_checker(byte[] msg) {
        //string final = "";
        if (msg[0] == 0x00)
        {
            Debug.Log("U are controling the car");
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

        else if (msg[0]==0x01) {
            Debug.Log("U press a number");
            
        }

        else if (msg[0] == 0x02) {
            
        }
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

    public void setting_button_function() {
        Debug.Log("u press setting");
    }

    public void disconnect() {
        helper.Disconnect();
        //Application.Quit();
    }

    public void sendData_hex(byte[] d)
    {
        helper.SendData(d);
    }

    public void sendData_string(string d)
    {
        helper.SendData(d);
    }

    private void rotate_camera(float current) {
        int num = (int)current;
        change_num(num * 15);
        Debug.Log(slider_camera.name + "的Value值为" + num);
        byte[] final = new byte[2];
        final[0] = 0x02;
        byte[] tempdecBytes = BitConverter.GetBytes(num+4);
        final[1] = tempdecBytes[0];
        sendData_hex(final);
    }

    // Update is called once per frame
    void Update()
    {
        if (!helper.isConnected()) { SceneManager.LoadScene("Connect_scene"); }

        if (slider_camera.value != current_Position) {
            current_Position = slider_camera.value;
            rotate_camera(current_Position);
        }
    }

    void OnDestroy()
    {
        helper.OnDataReceived -= OnDataReceived;
        helper.OnConnectionFailed -= OnConnectionFailed;
        helper.Disconnect();
    }

    /*
     * 接下来的都是按键函数
     */

    public void click_up() {
        if (hex_or_string == true)
        {
            sendData_hex(up_hex);
        }
        else {
            sendData_string(up_string);
        }
    }

    public void click_down()
    {
        if (hex_or_string == true)
        {
            sendData_hex(down_hex);
        }
        else
        {
            sendData_string(down_string);
        }
    }

    public void click_right()
    {
        if (hex_or_string == true)
        {
            sendData_hex(right_hex);
        }
        else
        {
            sendData_string(right_string);
        }
    }

    public void click_left()
    {
        if (hex_or_string == true)
        {
            sendData_hex(left_hex);
        }
        else
        {
            sendData_string(left_string);
        }
    }

    public void click_brake()
    {
        if (hex_or_string == true)
        {
            sendData_hex(brake_hex);
        }
        else
        {
            sendData_string(brake_string);
        }
    }

    public void click_one()
    {
        if (hex_or_string == true)
        {
            sendData_hex(one_hex);
        }
        else
        {
            sendData_string(one_string);
        }
    }

    public void click_two()
    {
        if (hex_or_string == true)
        {
            sendData_hex(two_hex);
        }
        else
        {
            sendData_string(two_string);
        }
    }

    public void click_three()
    {
        if (hex_or_string == true)
        {
            sendData_hex(three_hex);
        }
        else
        {
            sendData_string(three_string);
        }
    }

    public void click_four()
    {
        if (hex_or_string == true)
        {
            sendData_hex(four_hex);
        }
        else
        {
            sendData_string(four_string);
        }
    }

    public void click_five()
    {
        if (hex_or_string == true)
        {
            sendData_hex(five_hex);
        }
        else
        {
            sendData_string(five_string);
        }
    }


    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
    }
}
