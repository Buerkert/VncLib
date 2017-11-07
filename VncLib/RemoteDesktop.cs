// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace VncLib
{
    public class RemoteDesktop : IDisposable
    {
        private Bitmap _desktop;
        private object _desktopLock = new object();
        private Thread _updateThread;
        private bool _updateThreadShouldStop;
        private VncConnection _vncConnection;
        private AutoResetEvent _vncUpdated = new AutoResetEvent(true);
        private Dictionary<DateTime, TimeSpan> TimeDisplay = new Dictionary<DateTime, TimeSpan>();


        public RemoteDesktop(VncConnection vncConnection)
        {
            _vncConnection = vncConnection;

            // Create a buffer on which updated rectangles will be drawn and draw a "please wait..." 
            // message on the buffer for initial display until we start getting rectangles
            SetupDesktop();

            // Start getting updates from the remote host (vnc.StartUpdates will begin a worker thread).
            _vncConnection.VncUpdate += VncUpdate;
            _vncConnection.ConnectionLost += VncConnectionOnConnectionLost;

            _updateThreadShouldStop = false;
            _updateThread = new Thread(UpdateThreadRunInternal);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }


        private Bitmap Desktop
        {
            get => _desktop;
            set
            {
                if (_desktop != null)
                {
                    _desktop.Dispose();
                }
                _desktop = value;
                OnDesktopUpdated();
            }
        }


        public TimeSpan AverageUpdateTime { get; set; }


        public void Dispose()
        {
            _vncConnection.VncUpdate -= VncUpdate;
            _vncConnection.ConnectionLost -= VncConnectionOnConnectionLost;
        }


        private void VncConnectionOnConnectionLost(object sender, string e)
        {
            StopUpdateThread();
            _vncConnection.VncUpdate -= VncUpdate;
            DrawDesktopMessage(e);
        }


        private void StopUpdateThread()
        {
            if (_updateThread != null)
            {
                _updateThreadShouldStop = true;
                _vncUpdated.Set();
                if (!_updateThread.Join(TimeSpan.FromSeconds(2))) // if thread doesn't end properly
                    _updateThread.Abort(); // abort it
                _updateThread = null;
            }
        }


        private void UpdateThreadRunInternal()
        {
            Stopwatch st = new Stopwatch();
            while (!_updateThreadShouldStop)
            {
                if (_vncConnection.IsConnectionActive)
                {
                    st.Restart();
                    _vncConnection.RequestScreenUpdate(true);
                    _vncUpdated.WaitOne();
                    Thread.Sleep(200); // just give it some time...
                    st.Stop();
                    lock (TimeDisplay)
                    {
                        var tooOld = TimeDisplay.Keys.Where(k => k.AddSeconds(1) < DateTime.Now).ToList();
                        foreach (var key in tooOld)
                        {
                            TimeDisplay.Remove(key);
                        }
                        TimeDisplay.Add(DateTime.Now, st.Elapsed);
                        AverageUpdateTime = TimeSpan.FromMilliseconds(TimeDisplay.Values.Average(i => i.TotalMilliseconds));
                    }
                }
            }
        }


        // This event handler deals with Frambebuffer Updates coming from the host. An
        // EncodedRectangle object is passed via the VncEventArgs (actually an IDesktopUpdater
        // object so that *only* Draw() can be called here--Decode() is done elsewhere).
        // The VncClient object handles thread marshalling onto the UI thread.
        private void VncUpdate(object sender, Bitmap bitmap)
        {
            lock (_desktopLock)
            {
                Desktop = new Bitmap(bitmap);
            }
            _vncUpdated.Set();
        }


        /// <summary>
        /// Creates and initially sets-up the local bitmap that will represent the remote desktop image.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not already in the Connected state. See <see cref="RemoteDesktop.IsConnected" />.</exception>
        private void SetupDesktop()
        {
            // Create a new bitmap to cache locally the remote desktop image.  Use the geometry of the
            // remote framebuffer, and 32bpp pixel format (doesn't matter what the server is sending--8,16,
            // or 32--we always draw 32bpp here for efficiency).
            lock (_desktopLock)
            {
                Desktop = new Bitmap(_vncConnection.FramebufferWidth, _vncConnection.FramebufferHeight, PixelFormat.Format32bppPArgb);
            }
            // Draw a "please wait..." message on the local desktop until the first
            // rectangle(s) arrive and overwrite with the desktop image.
            DrawDesktopMessage("Connecting to VNC host, please wait...");
            _vncConnection.RequestScreenUpdate(true);
        }


        /// <summary>
        /// Draws the given message (white text) on the local desktop (all black).
        /// </summary>
        /// <param name="message">The message to be drawn.</param>
        private void DrawDesktopMessage(string message)
        {
            // Draw the given message on the local desktop
            lock (_desktopLock)
            {
                using (Graphics g = Graphics.FromImage(Desktop))
                {
                    g.FillRectangle(Brushes.Black, _vncConnection.FramebufferRectangle);

                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString(message,
                        new Font("Arial", 12),
                        new SolidBrush(Color.White),
                        new PointF(_vncConnection.FramebufferWidth / 2, _vncConnection.FramebufferHeight / 2), format);
                }
            }
            OnDesktopUpdated();
        }


        /// <summary>
        /// Raised when the VNC Host drops the connection.
        /// </summary>
        public event EventHandler<Bitmap> DesktopUpdated;


        private void OnDesktopUpdated()
        {
            if (DesktopUpdated != null)
            {
                Bitmap clone;
                lock (_desktopLock)
                {
                    clone = new Bitmap(Desktop);
                }
                DesktopUpdated(this, clone);
            }
        }


        public bool WritePointer(Point pointer, byte action)
        {
            if (_vncConnection.IsConnectionActive)
            {
                //Point current = pointer;
                //byte mask = 0;

                //if (Control.MouseButtons == MouseButtons.Left) mask += 1;
                //if (Control.MouseButtons == MouseButtons.Middle) mask += 2;
                //if (Control.MouseButtons == MouseButtons.Right) mask += 4;
                //byte mask = action;

                _vncConnection.WritePointer(action, pointer);
                return true;
            }
            return false;
        }

        public void WriteMouseAction(Point p, MouseEventArgs e)
        {
            byte mask = 0;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mask += 1;
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                mask += 2;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                mask += 4;
            }
            
            WritePointer(p, mask);
        }

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream(); // ATTENTION ! NO USING!
            src.Save(ms, ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}