# arduino_bluetooth_app
* this app is created in unity, the plugin is "Arduino bluetooth plugin", this app only work in Android now.
* the first scene is finding device, connecting device, just input your device id, then it would show a connection message from system, of cource, this step can be skipped if you connect this device before run this app.
* the second scene is sending message to device, all buttons would send byte, and there is a input field on buttom right, it can send string.
* I make a protocal to control device, you can reference the "arduino code example" file, and Bluetooth_Control.cs in unity. 0x00, 0x01,0x02 and 0x03 are operation code, after them is data code, just make sure the device can identify the operation, since the device would receive a random code in connection or disconnect. 

