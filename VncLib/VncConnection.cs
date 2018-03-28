using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VncLib.VncCommands;
using Color = System.Windows.Media.Color;
using Timer = System.Timers.Timer;

namespace VncLib
{
    public class VncConnection
    {
        public delegate void UpdateScreenCallback(ScreenUpdateEventArgs newScreen);

        private readonly List<Key> _nonSpecialKeys = new List<Key>(); //A List of all Keys, that are handled by the transparent Textbox

        public readonly DispatcherTimer TmrClipboardCheck;

        public readonly DispatcherTimer TmrEllipse;
        public readonly Timer TmrScreen;
        private WriteableBitmap _bitmap;
        private RfbClient _connection;

        private readonly object _lockObj = new object();
        //private bool _ignoreNextKey;
        //private byte[] _screenData;
        private Bitmap _screen;
        private int _lastClipboardHash; //To check, if the text has changed
        private DateTime _lastMouseMove = DateTime.Now;

        //DateTime _lastUpdate = DateTime.MinValue;

        private VncLibUserCallback _vncLibUserCallback;
        private VncCommandPlayerCommandExecuted _commandExecuted;
        private VncCommandPlayerPreviewCommandExecute _previewCommandExecute;

        public VncConnection()
        {
            ServerPort = 5900;
            CreateSpecialKeys();

            TmrEllipse = new DispatcherTimer();
            TmrEllipse.Interval = TimeSpan.FromMilliseconds(200);
            TmrEllipse.Tick += tmrEllipse_Tick;

            //TODO
            //TmrClipboardCheck = new DispatcherTimer();
            //TmrClipboardCheck.Interval = TimeSpan.FromMilliseconds(500);
            //TmrClipboardCheck.Tick += tmrClipboard_Tick;
            //TmrClipboardCheck.IsEnabled = true;

            TmrScreen = new Timer(UpdateInterval);
            TmrScreen.Elapsed += tmrScreen_Tick;
        }

        /// <summary>
        /// The Port of the Remoteserver (Default: 5900)
        /// </summary>
        public int ServerPort { get; set; }

        public string ServerPassword { get; set; }

        /// <summary>
        /// The IP or Hostname of the Remoteserver
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// Update interval in milliseconds, default = 40
        /// </summary>
        public int UpdateInterval { get; set; } = 40;

        public bool AutoUpdate { get; set; }

        public VncLibUserCallback VncLibUserCallback
        {
            get => _vncLibUserCallback;
            set => _vncLibUserCallback = value;
        }

        public VncCommandPlayerCommandExecuted CommandExecuted
        {
            get => _commandExecuted;
            set => _commandExecuted = value;
        }

        public VncCommandPlayerPreviewCommandExecute PreviewCommandExecute
        {
            get => _previewCommandExecute;
            set => _previewCommandExecute = value;
        }

        public Bitmap Screenshot
        {
            get
            {
                if(_connection == null)
                    throw new InvalidOperationException("Can't get a screenshot without a connection");

                if (_screen == null)
                    return null;

                Bitmap bmp = _screen;
                return new Bitmap(bmp);
            }
        }

        public ObservableCollection<IVncCommand> ExecutedCommands => _connection.ExecutedCommands;

        /// <summary>
        /// enable the mouse action and position capturing - call after calling Connect()
        /// </summary>
        public void EnableMouseCapturing()
        {
            if (_connection != null)
                _connection.MouseActionCaptureEnabled = true;
        }

        public void DisableMouseCapturing()
        {
            if (_connection != null)
                _connection.MouseActionCaptureEnabled = false;
        }

        /// <summary>
        /// Should the interval of sending MouseMoveCommands to the VNC-Server be limited? Default=true
        /// </summary>
        public bool LimitMouseEvents { get; set; } = true;
        

        void tmrScreen_Tick(object sender, EventArgs e)
        {
            if(AutoUpdate)
                _connection?.RefreshScreen();
        }

        void tmrEllipse_Tick(object sender, EventArgs e)
        {
            TmrEllipse.Stop();
        }

        private void ConnectInternal(string serverAddress, int serverPort, string serverPassword)
        {
            //Create a connection
            _connection = new RfbClient(serverAddress, serverPort, serverPassword);

            //Is Triggered when the Screen is beeing updated
            _connection.ScreenUpdate += new RfbClient.ScreenUpdateEventHandler(Connection_ScreenUpdate);
            //Is Triggered, when the RfbClient sends a Log-Event
            //_connection.LogMessage += new RfbClient.LogMessageEventHandler(Connection_LogMessage);

            _connection.StartConnection();

            if(AutoUpdate)
                TmrScreen.Enabled = true;
        }

        private void Connection_ScreenUpdate(object sender, ScreenUpdateEventArgs e)
        {
            if (_connection != null && _connection.IsConnected)
            {
                lock (_lockObj)
                {
                    UpdateImage(e);
                }
            }
        }

        /// <summary>
        /// Update the RemoteImage
        /// </summary>
        /// <param name="newScreens"></param>
        private void UpdateImage(ScreenUpdateEventArgs newScreens)
        {
            try
            {
                if (newScreens.Rects.Count == 0)
                    return;
                //_lastUpdate = DateTime.Now;
                
                if (_bitmap == null)
                {
                    _bitmap = new WriteableBitmap(newScreens.Rects.First().Width, newScreens.Rects.First().Height, 96,
                        96, PixelFormats.Bgr32, null);
                    
                }
                
                foreach (var newScreen in newScreens.Rects)
                {
                    _bitmap.WritePixels(new Int32Rect(0, 0, newScreen.Width, newScreen.Height), newScreen.PixelData,
                        newScreen.Width * 4, newScreen.PosX, newScreen.PosY);
                }
                
                _screen = BitmapFromWriteableBitmap(_bitmap);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Checkinterval to check, if the Clipboard changed. Not a stylish way, but it works
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmrClipboard_Tick(object sender, EventArgs e)
        {
            if (_connection == null)
                return;

            if (Clipboard.ContainsText()) //Check if Clipboard contains Text
            {
                try
                {
                    if (Clipboard.GetText().Length < Int32.MaxValue) //normally it should be UInt32.MaxValue, but this is larger then .Length ever can be. So 2.1Million signs are maximum
                    {
                        if (_lastClipboardHash != Clipboard.GetText().GetHashCode()
                        ) //If the Text has changed since last check
                        {
                            //_Connection.SendClientCutText(Clipboard.GetText());
                            _lastClipboardHash = Clipboard.GetText().GetHashCode(); //Set the new Hash
                        }
                    }
                }
                catch (Exception) //
                {
                }
            }
        }

        public void Connect()
        {
            ConnectInternal(ServerAddress, ServerPort, ServerPassword);
        }

        public void Connect(string serverAddress)
        {
            ConnectInternal(serverAddress, ServerPort, "");
        }

        public void Connect(string serverAddress, string serverPassword)
        {
            ConnectInternal(serverAddress, ServerPort, serverPassword);
        }

        public void Connect(string serverAddress, int serverPort, string serverPassword)
        {
            ConnectInternal(serverAddress, serverPort, serverPassword);
        }

        public void SendKeyCombination(KeyCombination keyComb)
        {
            _connection.SendKeyCombination(keyComb);
        }

        public void Disconnect()
        {
            TmrScreen?.Stop();
            TmrEllipse?.Stop();
            TmrClipboardCheck?.Stop();
            _connection?.Disconnect();
        }

        public void UpdateScreen()
        {
            _connection.RefreshScreen();
        }


        public async Task PlayCommands(IEnumerable<IVncCommand> commands)
        {
            await _connection.Play(commands, PreviewCommandExecute, CommandExecuted);
        }

        private void CreateSpecialKeys()
        {
            _nonSpecialKeys.Add(Key.A);
            _nonSpecialKeys.Add(Key.B);
            _nonSpecialKeys.Add(Key.C);
            _nonSpecialKeys.Add(Key.D);
            _nonSpecialKeys.Add(Key.E);
            _nonSpecialKeys.Add(Key.F);
            _nonSpecialKeys.Add(Key.G);
            _nonSpecialKeys.Add(Key.H);
            _nonSpecialKeys.Add(Key.I);
            _nonSpecialKeys.Add(Key.J);
            _nonSpecialKeys.Add(Key.K);
            _nonSpecialKeys.Add(Key.L);
            _nonSpecialKeys.Add(Key.M);
            _nonSpecialKeys.Add(Key.N);
            _nonSpecialKeys.Add(Key.O);
            _nonSpecialKeys.Add(Key.P);
            _nonSpecialKeys.Add(Key.Q);
            _nonSpecialKeys.Add(Key.R);
            _nonSpecialKeys.Add(Key.S);
            _nonSpecialKeys.Add(Key.T);
            _nonSpecialKeys.Add(Key.U);
            _nonSpecialKeys.Add(Key.V);
            _nonSpecialKeys.Add(Key.W);
            _nonSpecialKeys.Add(Key.X);
            _nonSpecialKeys.Add(Key.Y);
            _nonSpecialKeys.Add(Key.Z);

            _nonSpecialKeys.Add(Key.D0);
            _nonSpecialKeys.Add(Key.D1);
            _nonSpecialKeys.Add(Key.D2);
            _nonSpecialKeys.Add(Key.D3);
            _nonSpecialKeys.Add(Key.D4);
            _nonSpecialKeys.Add(Key.D5);
            _nonSpecialKeys.Add(Key.D6);
            _nonSpecialKeys.Add(Key.D7);
            _nonSpecialKeys.Add(Key.D8);
            _nonSpecialKeys.Add(Key.D9);

            _nonSpecialKeys.Add(Key.Add);
            _nonSpecialKeys.Add(Key.Decimal);
            _nonSpecialKeys.Add(Key.Divide);
            _nonSpecialKeys.Add(Key.Multiply);
            _nonSpecialKeys.Add(Key.OemBackslash);
            _nonSpecialKeys.Add(Key.OemCloseBrackets);
            _nonSpecialKeys.Add(Key.OemComma);
            _nonSpecialKeys.Add(Key.OemMinus);
            _nonSpecialKeys.Add(Key.OemOpenBrackets);
            _nonSpecialKeys.Add(Key.OemPeriod);
            _nonSpecialKeys.Add(Key.OemPipe);
            _nonSpecialKeys.Add(Key.OemPlus);
            _nonSpecialKeys.Add(Key.OemQuestion);
            _nonSpecialKeys.Add(Key.OemQuotes);
            _nonSpecialKeys.Add(Key.OemSemicolon);
            _nonSpecialKeys.Add(Key.OemTilde);

            _nonSpecialKeys.Add(Key.NumPad0);
            _nonSpecialKeys.Add(Key.NumPad1);
            _nonSpecialKeys.Add(Key.NumPad2);
            _nonSpecialKeys.Add(Key.NumPad3);
            _nonSpecialKeys.Add(Key.NumPad4);
            _nonSpecialKeys.Add(Key.NumPad5);
            _nonSpecialKeys.Add(Key.NumPad6);
            _nonSpecialKeys.Add(Key.NumPad7);
            _nonSpecialKeys.Add(Key.NumPad8);
            _nonSpecialKeys.Add(Key.NumPad9);
        }

        private System.Drawing.Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            System.Drawing.Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                var bmp2 = new System.Drawing.Bitmap(outStream);
                bmp = new Bitmap(bmp2);
            }
            return bmp;
        }
    }
}