using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using WiimoteApi.Internal;

//=============================================================================
//	スクリプト
//=============================================================================
namespace WiimoteApi { 

public class WiimoteManager
{
    private const ushort vendor_id_wiimote = 0x057e;
    private const ushort product_id_wiimote = 0x0306;
    private const ushort product_id_wiimoteplus = 0x0330;


    //  接続されているWiiリモコンのリスト
    public static List<Wiimote> Wiimotes { get { return _Wiimotes; } }
    private static List<Wiimote> _Wiimotes = new List<Wiimote>();

    /// If true, WiimoteManager and Wiimote will write data reports and other debug
    /// messages to the console.  Any incorrect usages / errors will still be reported.
    public static bool Debug_Messages = false;

    /// The maximum time, in milliseconds, between data report writes.  This prevents
    /// WiimoteApi from attempting to write faster than most bluetooth drivers can handle.
    ///
    /// If you attempt to write at a rate faster than this, the extra write requests will
    /// be queued up and written to the Wii Remote after the delay is up.
    public static int MaxWriteFrequency = 20; // In ms
    private static Queue<WriteQueueData> WriteQueue;

		// ------------- RAW HIDAPI INTERFACE ------------- //

		[RuntimeInitializeOnLoadMethod]
		static void Initialize()
		{
			// wiiリモコン初期化処理
			FindWiimotes();
			int wiiNumber = 0;
			if (HasWiimote(wiiNumber))
			{
				Wiimote wm = Wiimotes[wiiNumber];
				wm.InitWiiMotionPlus();
				wm.Speaker.Init();
				int i = wiiNumber + 1;
				wm.SendPlayerLED(i == 1, i == 2, i == 3, i == 4);
				Rumble(wiiNumber, false);
			}
		}

		/// \brief Attempts to find connected Wii Remotes, Wii Remote Pluses or Wii U Pro Controllers
		/// \return If any new remotes were found.
		///
		public static bool FindWiimotes()
    {
        bool ret = _FindWiimotes(WiimoteType.WIIMOTE);
        ret = ret || _FindWiimotes(WiimoteType.WIIMOTEPLUS);
        return ret;
    }

    // Bluetoothにwiiリモコンが接続されているか確認
    private static bool _FindWiimotes(WiimoteType type)
    {
        //if (hidapi_wiimote != IntPtr.Zero)
        //    HIDapi.hid_close(hidapi_wiimote);

        ushort vendor = 0;
        ushort product = 0;

        if(type == WiimoteType.WIIMOTE) {
            vendor = vendor_id_wiimote;
            product = product_id_wiimote;
        } else if(type == WiimoteType.WIIMOTEPLUS || type == WiimoteType.PROCONTROLLER) {
            vendor = vendor_id_wiimote;
            product = product_id_wiimoteplus;
        }

        IntPtr ptr = HIDapi.hid_enumerate(vendor, product);
        IntPtr cur_ptr = ptr;

        if (ptr == IntPtr.Zero)
            return false;

        hid_device_info enumerate = (hid_device_info)Marshal.PtrToStructure(ptr, typeof(hid_device_info));

        bool found = false;

        while(cur_ptr != IntPtr.Zero)
        {
            Wiimote remote = null;
            bool fin = false;
            foreach (Wiimote r in Wiimotes)
            {
                if (fin)
                    continue;

                if (r.hidapi_path.Equals(enumerate.path))
                {
                    remote = r;
                    fin = true;
                }
            }
            if (remote == null)
            {
                IntPtr handle = HIDapi.hid_open_path(enumerate.path);

                WiimoteType trueType = type;

                // Wii U Pro Controllers have the same identifiers as the newer Wii Remote Plus except for product
                // string (WHY nintendo...)
                if(enumerate.product_string.EndsWith("UC"))
                    trueType = WiimoteType.PROCONTROLLER;

                remote = new Wiimote(handle, enumerate.path, trueType);

                if (Debug_Messages)
                    Debug.Log("Found New Remote: " + remote.hidapi_path);

                Wiimotes.Add(remote);

                remote.SendDataReportMode(InputDataType.REPORT_BUTTONS);
                remote.SendStatusInfoRequest();
            }

            cur_ptr = enumerate.next;
            if(cur_ptr != IntPtr.Zero)
                enumerate = (hid_device_info)Marshal.PtrToStructure(cur_ptr, typeof(hid_device_info));
        }

        HIDapi.hid_free_enumeration(ptr);

        return found;
    }

    // Wiiリモコンの接続を解除
    public static void Cleanup(Wiimote remote)
    {
            if (remote != null)
            {
                if (remote.hidapi_handle != IntPtr.Zero)
                {
                    HIDapi.hid_close(remote.hidapi_handle);
                }
                Wiimotes.Remove(remote);
            }
    }

    //=============================================================================
    //	関数名: public static bool HasWiimote()
    //	引数  : なし
    //	戻り値: なし
    //	説明  : Wiiリモコンが1台以上接続されている確認する
    //=============================================================================
    public static bool HasWiimote()
    {
        return !(Wiimotes.Count <= 0 || Wiimotes[0] == null || Wiimotes[0].hidapi_handle == IntPtr.Zero);
    }
    
    // 指定した番号のWiiリモコンが接続されているか確認する
	public static bool HasWiimote(int num)
	{
		return !(Wiimotes.Count <= num || Wiimotes[num] == null || Wiimotes[num].hidapi_handle == IntPtr.Zero);
	}


    /// \brief Sends RAW DATA to the given bluetooth HID device.  This is essentially a wrapper around HIDApi.
    /// \param hidapi_wiimote The HIDApi device handle to write to.
    /// \param data The data to write.
    /// \sa Wiimote::SendWithType(OutputDataType, byte[])
    /// \warning DO NOT use this unless you absolutely need to bypass the given Wiimote communication functions.
    ///          Use the functionality provided by Wiimote instead.
    public static int SendRaw(IntPtr hidapi_wiimote, byte[] data)
    {
        if (hidapi_wiimote == IntPtr.Zero) return -2;

        if (WriteQueue == null)
        {
            WriteQueue = new Queue<WriteQueueData>();
            SendThreadObj = new Thread(new ThreadStart(SendThread));
            SendThreadObj.Start();
        }

        WriteQueueData wqd = new WriteQueueData();
        wqd.pointer = hidapi_wiimote;
        wqd.data = data;
        lock(WriteQueue)
            WriteQueue.Enqueue(wqd);

        return 0;
    }

    //=============================================================================
    //	関数名: int RecieveRaw(IntPtr hidapi_wiimote, byte[] buf)
    //	引数  : Int
    //	戻り値: なし
    //	説明  : Wiiリモコンへのデータ送信関数
    //=============================================================================
    private static Thread SendThreadObj;
    private static void SendThread()
    {
        while (true)
        {
            lock (WriteQueue)
            {
                if (WriteQueue.Count != 0)
                {
                    WriteQueueData wqd = WriteQueue.Dequeue();
                    int res = HIDapi.hid_write(wqd.pointer, wqd.data, new UIntPtr(Convert.ToUInt32(wqd.data.Length)));
                    if (res == -1) Debug.LogError("HidAPI reports error " + res + " on write: " + Marshal.PtrToStringUni(HIDapi.hid_error(wqd.pointer)));
                    else if (Debug_Messages) Debug.Log("Sent " + res + "b: [" + wqd.data[0].ToString("X").PadLeft(2, '0') + "] " + BitConverter.ToString(wqd.data, 1));
                }
            }
            Thread.Sleep(MaxWriteFrequency);
        }
    }

    //=============================================================================
    //	関数名: int RecieveRaw(IntPtr hidapi_wiimote, byte[] buf)
    //	引数  : Int
    //	戻り値: なし
    //	説明  : Wiiリモコンからのデータ受信関数
    //=============================================================================
    public static int RecieveRaw(IntPtr hidapi_wiimote, byte[] buf)
    {
        if (hidapi_wiimote == IntPtr.Zero) return -2;

        HIDapi.hid_set_nonblocking(hidapi_wiimote, 1);
        int res = HIDapi.hid_read(hidapi_wiimote, buf, new UIntPtr(Convert.ToUInt32(buf.Length)));

        return res;
    }

    //=============================================================================
    //  Wiiリモコンに送信するデータ
    //=============================================================================
    private class WriteQueueData
    {
    public IntPtr pointer;
    public byte[] data;
    }

    //=============================================================================
    //	関数名: bool GetSwing( int wmNum )
    //	引数  : int wmNum : Wiiリモコン番号
    //	戻り値: なし
    //	説明  : Wiiリモコン振り判定処理関数
    //=============================================================================
    public static bool GetSwing(int wmNum)
	{
		if (!HasWiimote(wmNum)) return false;
			Wiimotes[wmNum].ReadWiimoteData();
		if (Wiimotes[wmNum].MotionPlus.GetSwing(wmNum)) return true;
		return false;
	}

    //=============================================================================
    //	関数名: bool Rumble( int wmNum, bool rumble )
    //	引数  : int wmNum : Wiiリモコン番号, bool rumle : 振動させるか止めるか
    //	戻り値: 
    //	説明  : Wiiリモコン振動制御関数
    //=============================================================================
    public static void Rumble(int wmNum, bool rumble)
    {
            if (!HasWiimote(wmNum)) return;

            Wiimotes[wmNum].RumbleOn = rumble;
            Wiimotes[wmNum].SendStatusInfoRequest();
    }

    //=============================================================================
    //	関数名: bool GetButton( int wmNum, int buttonNum )
    //	引数  : int wmNum : Wiiリモコン番号, int buttonNum : 取得するボタンの種類(ButtonDataに定義あり)
    //	戻り値: bool down : していしたボタンが押されていたらtrueを返す
    //	説明  : Wiiリモコンボタン情報取得処理関数
    //=============================================================================
    public static bool GetButton(int wmNum ,int buttonNum)
	{
		if (!HasWiimote(wmNum)) return false;

		bool down = false;
            Wiimotes[wmNum].ReadWiimoteData();
            switch (buttonNum)
		{
				case ButtonData.WMBUTTON_A:
					down = Wiimotes[wmNum].Button.a ? true : false;
					break;
				case ButtonData.WMBUTTON_B:
					down = Wiimotes[wmNum].Button.b ? true : false;
					break;

				case ButtonData.WMBUTTON_DOWN:
					down = Wiimotes[wmNum].Button.d_down ? true : false;
					break;

				case ButtonData.WMBUTTON_HOME:
					down = Wiimotes[wmNum].Button.home ? true : false;
					break;

				case ButtonData.WMBUTTON_LEFT:
					down = Wiimotes[wmNum].Button.d_left ? true : false;
					break;

				case ButtonData.WMBUTTON_MINUS:
					down = Wiimotes[wmNum].Button.minus ? true : false;
					break;

				case ButtonData.WMBUTTON_ONE:
					down = Wiimotes[wmNum].Button.one ? true : false;
					break;

				case ButtonData.WMBUTTON_PLUS:
					down = Wiimotes[wmNum].Button.plus ? true : false;
					break;

				case ButtonData.WMBUTTON_RIGHT:
					down = Wiimotes[wmNum].Button.d_right ? true : false;
					break;

				case ButtonData.WMBUTTON_TWO:
					down = Wiimotes[wmNum].Button.two ? true : false;
					break;

				case ButtonData.WMBUTTON_UP:
					down = Wiimotes[wmNum].Button.d_up ? true : false;
					break;
			}
		return down;
	}
    }
} // namespace WiimoteApi
//=============================================================================
//	End of file
//=============================================================================