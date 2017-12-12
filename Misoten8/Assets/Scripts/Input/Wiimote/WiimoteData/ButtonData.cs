namespace WiimoteApi
{
    public class ButtonData : WiimoteData
    {
        public const int WMBUTTON_LEFT = 0;
		public const int WMBUTTON_RIGHT = 1;
		public const int WMBUTTON_DOWN = 2;
		public const int WMBUTTON_UP = 3;
        public const int WMBUTTON_PLUS = 4;
		public const int WMBUTTON_TWO = 5;
		public const int WMBUTTON_ONE = 6;
		public const int WMBUTTON_B = 7;
		public const int WMBUTTON_A = 8;
		public const int WMBUTTON_MINUS = 9;
		public const int WMBUTTON_HOME = 10;
        public const int WMBUTTON_MAX = 11;

        public struct WMBUTTON
        {
            public bool a;
            public bool b;
            public bool one;
            public bool two;
            public bool plus;
            public bool minus;
            public bool up;
            public bool down;
            public bool left;
            public bool right;
            public bool home;
        };

        public WMBUTTON wmButton {  get { return _wmButton; } }
        private WMBUTTON _wmButton;

        public WMBUTTON wmButtonOld { get { return _wmButtonOld; } }
        private WMBUTTON _wmButtonOld;

        private int _first = 0;

        public ButtonData(Wiimote Owner) : base(Owner) { }

        public override bool InterpretData(byte[] data)
        {
            if (data == null || data.Length != 2) return false;

            if (_first == 0)
            {
                _first++;
            }
            else
            {
                _wmButtonOld = _wmButton;
            }

            _wmButton.left = (data[0] & 0x01) == 0x01;
            _wmButton.right = (data[0] & 0x02) == 0x02;
            _wmButton.down = (data[0] & 0x04) == 0x04;
            _wmButton.up = (data[0] & 0x08) == 0x08;
            _wmButton.plus = (data[0] & 0x10) == 0x10;

            _wmButton.two = (data[1] & 0x01) == 0x01;
            _wmButton.one = (data[1] & 0x02) == 0x02;
            _wmButton.b = (data[1] & 0x04) == 0x04;
            _wmButton.a = (data[1] & 0x08) == 0x08;
            _wmButton.minus = (data[1] & 0x10) == 0x10;
            _wmButton.home = (data[1] & 0x80) == 0x80;
            
            return true;
        }

        public  void UpdateButton()
        {
            _wmButtonOld = _wmButton;
        }
    }
}