using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class PowerPointViewer : MonoBehaviour
{
    public const uint WM_KEYDOWN = 0x0100;
    public const uint WM_KEYUP = 0x0101;
    public const uint WM_LBUTTONDOWN = 0x0201;
    public const uint WM_LBUTTONUP = 0x0202;

    uWindowCapture.UwcWindowTexture uwcWindowTexture = null;

    public class Window
    {
        public string ClassName;
        public string Title;
        public IntPtr hWnd;
        public int Style;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    protected static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    [DllImport("user32")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern uint MapVirtualKey(uint uCode, uint uMapType);

    // Start is called before the first frame update
    void Start()
    {
        uwcWindowTexture = this.gameObject.GetComponent<uWindowCapture.UwcWindowTexture>();
    }

    bool isKeyDown = false;

    // Update is called once per frame
    void Update()
    {
        if (uwcWindowTexture.window != null)
        {

            if (Input.GetKey(KeyCode.Return))
            {
                if (!isKeyDown)
                {
                    var h = FindTarget(GetWindow(uwcWindowTexture.window.handle));
                    sendKey(h, VKeys.KEY_RETURN, false);
                    isKeyDown = true;
                }
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                if (!isKeyDown)
                {
                    var h = FindTarget(GetWindow(uwcWindowTexture.window.handle));
                    //sendKey(h, VKeys.KEY_LEFT, false);
                    PrevPage();
                    isKeyDown = true;
                }
            }
            else if (isKeyDown)
            {
                isKeyDown = false;
            }
        }
    }

    public void NextPage()
    {
        var h = FindTarget(GetWindow(uwcWindowTexture.window.handle));
        sendKey(h, VKeys.KEY_RETURN, false);
    }

    private int MAKELPARAM(int p, int p_2)
    {
        return ((p_2 << 16) | (p & 0xFFFF));
    }

    public void PrevPage()
    {
        var h = FindTargetF3Server(GetWindow(uwcWindowTexture.window.handle));
        //sendKey(h, VKeys.KEY_LEFT, false);
        SendMessage(h, WM_LBUTTONDOWN, 0, (uint)MAKELPARAM(10, 10));
        SendMessage(h, WM_LBUTTONUP, 0, (uint)MAKELPARAM(10, 10));
    }

    public static void sendKey(IntPtr hwnd, VKeys keyCode, bool extended)
    {
        //uint scanCode = MapVirtualKey((uint)keyCode, 0);
        uint scanCode = (uint)keyCode;
        uint lParam;

        //KEY DOWN
        lParam = (0x00000001 | (scanCode << 16));
        if (extended)
        {
            lParam |= 0x01000000;
        }
        PostMessage(hwnd, WM_KEYDOWN, (uint)keyCode, lParam);

        //KEY UP
        lParam |= 0xC0000000;  // set previous key and transition states (bits 30 and 31)
        PostMessage(hwnd, WM_KEYUP, (uint)keyCode, lParam);
    }

    public static int GWL_STYLE = -16;

    // ウィンドウハンドルを渡すと、ウィンドウテキスト（ラベルなど）、クラス、スタイルを取得してWindowsクラスに格納して返す
    private static Window GetWindow(IntPtr hWnd)
    {
        int textLen = GetWindowTextLength(hWnd);
        string windowText = null;
        if (0 < textLen)
        {
            //ウィンドウのタイトルを取得する
            StringBuilder windowTextBuffer = new StringBuilder(textLen + 1);
            GetWindowText(hWnd, windowTextBuffer, windowTextBuffer.Capacity);
            windowText = windowTextBuffer.ToString();
        }

        //ウィンドウのクラス名を取得する
        StringBuilder classNameBuffer = new StringBuilder(256);
        GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);

        // スタイルを取得する
        int style = GetWindowLong(hWnd, GWL_STYLE);
        return new Window() { hWnd = hWnd, Title = windowText, ClassName = classNameBuffer.ToString(), Style = style };
    }

    // 全てのボタンを列挙し、そのPPTFrameClassのウィンドウハンドルを返す
    private static IntPtr FindTarget(Window top)
    {
        var all = GetAllChildWindows(top, new List<Window>());
        return all.Where(x => x.ClassName == "mdiClass").First().hWnd;
    }

    // 全てのボタンを列挙し、そのPPTFrameClassのウィンドウハンドルを返す
    private static IntPtr FindTargetF3Server(Window top)
    {
        var all = GetAllChildWindows(top, new List<Window>());
        return all.Where(x =>
        {
            if (x.ClassName.Length > 8 && x.ClassName.Substring(0, 9) == "F3 Server")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        ).First().hWnd;
    }


    // 指定したウィンドウの全ての子孫ウィンドウを取得し、リストに追加する
    private static List<Window> GetAllChildWindows(Window parent, List<Window> dest)
    {
        dest.Add(parent);
        EnumChildWindows(parent.hWnd).ToList().ForEach(x => GetAllChildWindows(x, dest));
        return dest;
    }

    // 与えた親ウィンドウの直下にある子ウィンドウを列挙する（孫ウィンドウは見つけてくれない）
    private static IEnumerable<Window> EnumChildWindows(IntPtr hParentWindow)
    {
        IntPtr hWnd = IntPtr.Zero;
        while ((hWnd = FindWindowEx(hParentWindow, hWnd, null, null)) != IntPtr.Zero) { yield return GetWindow(hWnd); }
    }

    public enum VKeys
    {
        KEY_0 = 0x30, //0 key 
        KEY_1 = 0x31, //1 key 
        KEY_2 = 0x32, //2 key 
        KEY_3 = 0x33, //3 key 
        KEY_4 = 0x34, //4 key 
        KEY_5 = 0x35, //5 key 
        KEY_6 = 0x36, //6 key 
        KEY_7 = 0x37, //7 key 
        KEY_8 = 0x38, //8 key 
        KEY_9 = 0x39, //9 key
        KEY_MINUS = 0xBD, // - key
        KEY_PLUS = 0xBB, // + key
        KEY_A = 0x41, //A key 
        KEY_B = 0x42, //B key 
        KEY_C = 0x43, //C key 
        KEY_D = 0x44, //D key 
        KEY_E = 0x45, //E key 
        KEY_F = 0x46, //F key 
        KEY_G = 0x47, //G key 
        KEY_H = 0x48, //H key 
        KEY_I = 0x49, //I key 
        KEY_J = 0x4A, //J key 
        KEY_K = 0x4B, //K key 
        KEY_L = 0x4C, //L key 
        KEY_M = 0x4D, //M key 
        KEY_N = 0x4E, //N key 
        KEY_O = 0x4F, //O key 
        KEY_P = 0x50, //P key 
        KEY_Q = 0x51, //Q key 
        KEY_R = 0x52, //R key 
        KEY_S = 0x53, //S key 
        KEY_T = 0x54, //T key 
        KEY_U = 0x55, //U key 
        KEY_V = 0x56, //V key 
        KEY_W = 0x57, //W key 
        KEY_X = 0x58, //X key 
        KEY_Y = 0x59, //Y key 
        KEY_Z = 0x5A, //Z key 
        KEY_LBUTTON = 0x01, //Left mouse button 
        KEY_RBUTTON = 0x02, //Right mouse button 
        KEY_CANCEL = 0x03, //Control-break processing 
        KEY_MBUTTON = 0x04, //Middle mouse button (three-button mouse) 
        KEY_BACK = 0x08, //BACKSPACE key 
        KEY_TAB = 0x09, //TAB key 
        KEY_CLEAR = 0x0C, //CLEAR key 
        KEY_RETURN = 0x0D, //ENTER key 
        KEY_SHIFT = 0x10, //SHIFT key 
        KEY_CONTROL = 0x11, //CTRL key 
        KEY_MENU = 0x12, //ALT key 
        KEY_PAUSE = 0x13, //PAUSE key 
        KEY_CAPITAL = 0x14, //CAPS LOCK key 
        KEY_ESCAPE = 0x1B, //ESC key 
        KEY_SPACE = 0x20, //SPACEBAR 
        KEY_PRIOR = 0x21, //PAGE UP key 
        KEY_NEXT = 0x22, //PAGE DOWN key 
        KEY_END = 0x23, //END key 
        KEY_HOME = 0x24, //HOME key 
        KEY_LEFT = 0x25, //LEFT ARROW key 
        KEY_UP = 0x26, //UP ARROW key 
        KEY_RIGHT = 0x27, //RIGHT ARROW key 
        KEY_DOWN = 0x28, //DOWN ARROW key 
        KEY_SELECT = 0x29, //SELECT key 
        KEY_PRINT = 0x2A, //PRINT key 
        KEY_EXECUTE = 0x2B, //EXECUTE key 
        KEY_SNAPSHOT = 0x2C, //PRINT SCREEN key 
        KEY_INSERT = 0x2D, //INS key 
        KEY_DELETE = 0x2E, //DEL key 
        KEY_HELP = 0x2F, //HELP key 
        KEY_NUMPAD0 = 0x60, //Numeric keypad 0 key 
        KEY_NUMPAD1 = 0x61, //Numeric keypad 1 key 
        KEY_NUMPAD2 = 0x62, //Numeric keypad 2 key 
        KEY_NUMPAD3 = 0x63, //Numeric keypad 3 key 
        KEY_NUMPAD4 = 0x64, //Numeric keypad 4 key 
        KEY_NUMPAD5 = 0x65, //Numeric keypad 5 key 
        KEY_NUMPAD6 = 0x66, //Numeric keypad 6 key 
        KEY_NUMPAD7 = 0x67, //Numeric keypad 7 key 
        KEY_NUMPAD8 = 0x68, //Numeric keypad 8 key 
        KEY_NUMPAD9 = 0x69, //Numeric keypad 9 key 
        KEY_SEPARATOR = 0x6C, //Separator key 
        KEY_SUBTRACT = 0x6D, //Subtract key 
        KEY_DECIMAL = 0x6E, //Decimal key 
        KEY_DIVIDE = 0x6F, //Divide key 
        KEY_F1 = 0x70, //F1 key 
        KEY_F2 = 0x71, //F2 key 
        KEY_F3 = 0x72, //F3 key 
        KEY_F4 = 0x73, //F4 key 
        KEY_F5 = 0x74, //F5 key 
        KEY_F6 = 0x75, //F6 key 
        KEY_F7 = 0x76, //F7 key 
        KEY_F8 = 0x77, //F8 key 
        KEY_F9 = 0x78, //F9 key 
        KEY_F10 = 0x79, //F10 key 
        KEY_F11 = 0x7A, //F11 key 
        KEY_F12 = 0x7B, //F12 key 
        KEY_SCROLL = 0x91, //SCROLL LOCK key 
        KEY_LSHIFT = 0xA0, //Left SHIFT key 
        KEY_RSHIFT = 0xA1, //Right SHIFT key 
        KEY_LCONTROL = 0xA2, //Left CONTROL key 
        KEY_RCONTROL = 0xA3, //Right CONTROL key 
        KEY_LMENU = 0xA4, //Left MENU key 
        KEY_RMENU = 0xA5, //Right MENU key 
        KEY_COMMA = 0xBC, //, key
        KEY_PERIOD = 0xBE, //. key
        KEY_PLAY = 0xFA, //Play key 
        KEY_ZOOM = 0xFB, //Zoom key 
        NULL = 0x0,
    }
}
