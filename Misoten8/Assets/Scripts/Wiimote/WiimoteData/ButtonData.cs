namespace WiimoteApi
{
    public class ButtonData : WiimoteData
    {
        ///// Button: D-Pad Left
        //public bool d_left { get { return _d_left; } }
        //private bool _d_left;
        ///// Button: D-Pad Right
        //public bool d_right { get { return _d_right; } }
        //private bool _d_right;
        ///// Button: D-Pad Up
        //public bool d_up { get { return _d_up; } }
        //private bool _d_up;
        ///// Button: D-Pad Down
        //public bool d_down { get { return _d_down; } }
        //private bool _d_down;
        ///// Button: A
        //public bool a { get { return _a; } }
        //private bool _a;
        ///// Button: B
        //public bool b { get { return _b; } }
        //private bool _b;
        ///// Button: 1 (one)
        //public bool one { get { return _one; } }
        //private bool _one;
        ///// Button: 2 (two)
        //public bool two { get { return _two; } }
        //private bool _two;
        ///// Button: + (plus)
        //public bool plus { get { return _plus; } }
        //private bool _plus;
        ///// Button: - (minus)
        //public bool minus { get { return _minus; } }
        //private bool _minus;
        ///// Button: Home
        //public bool home { get { return _home; } }
        //private bool _home;

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

            //_d_left = (data[0] & 0x01) == 0x01;
            //_d_right = (data[0] & 0x02) == 0x02;
            //_d_down = (data[0] & 0x04) == 0x04;
            //_d_up = (data[0] & 0x08) == 0x08;
            //_plus = (data[0] & 0x10) == 0x10;

            _wmButton.left = (data[0] & 0x01) == 0x01;
            _wmButton.right = (data[0] & 0x02) == 0x02;
            _wmButton.down = (data[0] & 0x04) == 0x04;
            _wmButton.up = (data[0] & 0x08) == 0x08;
            _wmButton.plus = (data[0] & 0x10) == 0x10;

            //_two = (data[1] & 0x01) == 0x01;
            //_one = (data[1] & 0x02) == 0x02;
            //_b = (data[1] & 0x04) == 0x04;
            //_a = (data[1] & 0x08) == 0x08;
            //_minus = (data[1] & 0x10) == 0x10;

            //_home = (data[1] & 0x80) == 0x80;

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