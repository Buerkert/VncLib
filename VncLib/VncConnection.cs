// Copyright 2017 The VncLib Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace VncLib
{
    public class VncConnection : IDisposable
    {
        private readonly VncClient _vnc;
        private readonly AutoResetEvent _screenshotEvent = new AutoResetEvent(false);
        private readonly object _bitmapLock = new object();
        private Bitmap _bitmap;
        private EventHandler<Bitmap> _vncUpdate;
        private int _subscriberCount;

        public void Dispose()
        {
            Disconnect();
        }


        #region vncConnection

        public bool IsConnectionActive { get; private set; }


        #region composition

        public void RequestScreenUpdate(bool refreshFullScreen)
        {
            _vnc.RequestScreenUpdate(refreshFullScreen);
        }


        public int FramebufferWidth => _vnc.Framebuffer.Width;


        public int FramebufferHeight => _vnc.Framebuffer.Height;


        public RectangleF FramebufferRectangle => _vnc.Framebuffer.Rectangle;


        /// <summary>
        /// Raised when Point event is send to vnc
        /// </summary>
        public event EventHandler<Tuple<byte, Point>> VncTouchHandler;


        private void OnTouch(byte action, Point p)
        {
            VncTouchHandler?.Invoke(this, new Tuple<byte, Point>(action, p));
        }


        public void WritePointer(byte action, Point pointer)
        {
            lock (_vnc)
            {
                _vnc.WritePointerEvent(action, pointer);
                OnTouch(action, pointer);
            }
        }

        #endregion


        #region events

        private void VncOnConnectionLost(object sender, EventArgs eventArgs)
        {
            lock (_vnc)
            {
                if (IsConnectionActive)
                {
                    _vnc.Disconnect();
                    IsConnectionActive = false;
                }
            }
            _vnc.ConnectionLost -= VncOnConnectionLost;
            OnConnectionLost("VNC connection lost!");
        }


        /// <summary>
        /// Raised when the VNC Host drops the connection.
        /// </summary>
        public event EventHandler<string> ConnectionLost;


        private void OnConnectionLost(string message)
        {
            if (ConnectionLost != null)
            {
                ConnectionLost(this, message);
            }
        }


        private void OnVncUpdateInternal(object sender, VncEventArgs e)
        {
            lock (_bitmapLock)
            {
                e.DesktopUpdater.Draw(_bitmap);
            }
            _screenshotEvent.Set();
            Task.Run(() =>
            {
                lock (_bitmapLock)
                {
                    OnVncUpdate(new Bitmap(_bitmap));
                }
            });
        }


        

        // vnc update internal starts if anyone listens to vncupdate
        public event EventHandler<Bitmap> VncUpdate
        {
            add
            {
                _vncUpdate += value;
                if (_subscriberCount == 0)
                {
                    _vnc.VncUpdate += OnVncUpdateInternal;
                }
            }
            remove
            {
                _vncUpdate -= value;
                if (_vncUpdate == null || _vncUpdate.GetInvocationList().Length == 0)
                {
                    _subscriberCount = 0;
                    _vnc.VncUpdate -= OnVncUpdateInternal;
                }
                else
                {
                    _subscriberCount = _vncUpdate.GetInvocationList().Length;
                }
            }
        }


        private void OnVncUpdate(Bitmap picture)
        {
            _vncUpdate?.Invoke(this, picture);
        }

        #endregion


        public VncConnection(string bindAddress, int port, string password)
        {
            IsConnectionActive = false;
            if (bindAddress == null) throw new ArgumentNullException(nameof(bindAddress));

            // Start protocol-level handling and determine whether a password is needed
            _vnc = new VncClient();
            bool passwordPending = _vnc.Connect(bindAddress, 0, port, false);
            _vnc.ConnectionLost += VncOnConnectionLost;
            if (passwordPending)
            {
                // Server needs a password, so call which ever method is refered to by the GetPassword delegate.
                if (password == null)
                {
                    // No password could be obtained (e.g., user clicked Cancel), so stop connecting
                    return;
                }
                Authenticate(password);
            }
            else
            {
                // No password needed, so go ahead and Initialize here
                Initialize();
            }
        }


        /// <summary>
        /// Authenticate with the VNC Host using a user supplied password.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected. />.</exception>
        /// <exception cref="System.NullReferenceException">Thrown if the password is null.</exception>
        /// <param name="password">The user's password.</param>
        private void Authenticate(string password)
        {
            if (password == null) throw new NullReferenceException("password");

            if (_vnc.Authenticate(password))
            {
                Initialize();
            }
            else
            {
                OnConnectionLost("Authentification failed!");
            }
        }


        /// <summary>
        /// After protocol-level initialization and connecting is complete, the local GUI objects have to be set-up, and requests for updates to the remote host begun.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already in the Connected state. />.</exception>		
        private void Initialize()
        {
            // Finish protocol handshake with host now that authentication is done.
            _vnc.Initialize();
            lock (_bitmapLock)
            {
                _bitmap = new Bitmap(_vnc.Framebuffer.Width, _vnc.Framebuffer.Height, PixelFormat.Format32bppPArgb);
            }
            _vnc.StartUpdates();
            IsConnectionActive = true;
        }


        /// <summary>
        /// Stops the remote host from sending further updates and disconnects.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not already in the Connected state. />.</exception>
        public void Disconnect()
        {
            lock (_vnc)
            {
                if (IsConnectionActive)
                {
                    _vnc.Disconnect();
                    IsConnectionActive = false;
                }
            }
            OnConnectionLost("Disconnected!");
        }

        #endregion


        #region screenshot
        public Bitmap Screenshot(int timeoutMs = 10000)
        {
            if (IsConnectionActive)
            {
                try
                {
                    _vnc.VncUpdate += OnVncUpdateInternal;
                    lock (_vnc)
                    {
                        _vnc.RequestScreenUpdate(true);
                    }
                    if (_screenshotEvent.WaitOne(timeoutMs)) // success
                    {
                        lock (_bitmapLock)
                        {

                            return new Bitmap(_bitmap);
                        }
                    }
                }
                finally
                {
                    _vnc.VncUpdate -= OnVncUpdateInternal;
                }
            }
            return null;
        }

        #endregion screenshot
    }
}