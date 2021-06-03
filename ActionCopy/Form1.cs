using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using ActionCopy.Properties;
using Lidgren.Network;
using VoidNetworking;
using VoidNetworking.ModulesFramework;

namespace ActionCopy
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public partial class ActionCopyForm : Form
    {
        [Module]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [SuppressMessage("ReSharper", "UnusedType.Local")]
        private static class Module
        {
            [ModuleReceiver(0)]
            private static void OnMouseUpdate((int x, int y) point, NetConnection c)
            {
                if (isServer)
                    return;
                var position = new Point(point.x, point.y);
                instance.SetCursorPosition(position);
            }

            [ModuleReceiver(1)]
            private static void OnMouseUpdate(MouseButtons buttons, NetConnection c)
            {
                instance.SetMouseState(buttons);
            }

            [ModuleReceiver(2)]
            private static void OnMouseUpdate(int wheel, NetConnection c)
            {
                instance.SetMouseWheel(wheel);
            }

            [ModuleReceiver(3)]
            private static void OnKeyDownUpdate(Keys keys, NetConnection c)
            {
                instance.SetKeyDown(keys);
            }

            [ModuleReceiver(4)]
            private static void OnKeyUpUpdate(Keys keys, NetConnection c)
            {
                instance.SetKeyUp(keys);
            }
        }

        #region Extern
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        private const int MouseEventLeftDown = 0x02;
        private const int MouseEventLeftUp = 0x04;
        private const int MouseEventRightDown = 0x08;
        private const int MouseEventRightUp = 0x10;
        private const int MouseEventMiddleDown = 0x20;
        private const int MouseEventMiddleUp = 0x40;
        private const int MouseEventWheel = 0x0800;

        public static void DoMouseEvent(int value, int data=0)
        {
            mouse_event(value, 0, 0, data, 0);
        }
        #endregion

        private static ActionCopyForm instance;

        private const int Port = 8087;

        private VoidServer server;
        private VoidClient client;
        private VoidPeer Peer => isServer ? (VoidPeer)server : client;
        private readonly List<NetConnection> connections = new List<NetConnection>();
        private static bool isServer;

        private MouseButtons previousMouseButtonsState;
        private UserActivityHook hook;
        private InputSimulator simulator;

        public ActionCopyForm()
        {
            instance = this;
            InitializeComponent();
        }

        private void ActionCopyForm_Load(object sender, EventArgs e)
        {
            actionCopyNotifyIcon.Icon = Resources.icon;

            client = new VoidClient();
            client.OnServerDiscovery += point => serversListBox.Items.Add(point.Address);
            client.SendDiscoveryRequest(Port);
            
            server = new VoidServer(Port);
            server.OnConnected += connection => connections.Add(connection);
            server.OnDisconnected += connection => connections.Remove(connection);

            simulator = new InputSimulator();
            hook = new UserActivityHook();
            hook.OnMouseActivity += OnMouseActivity;
            hook.KeyDown += (o, args) =>
            {
                if (isServer)
                    connections.ForEach(c => server.SendMessage(3, args.KeyData, c));
            };
            hook.KeyUp += (o, args) =>
            {
                if (isServer)
                    connections.ForEach(c => server.SendMessage(4, args.KeyData, c));
            };
        }

        private void startServerButton_Click(object sender, EventArgs e)
        {
            connections.Clear();

            isServer = true;
            server.Start();
            updateTimer.Enabled = true;
            SetUiActiveState();
        }
        
        private void startClientButton_Click(object sender, EventArgs e)
        {
            client = new VoidClient();
            client.OnDisconnected += connection =>
            {
                Reset();
                // ReSharper disable once LocalizableElement
                MessageBox.Show("Unable to connect");
            };
            client.Connect(hostAddressBox.Text, Port);
            updateTimer.Enabled = true;
            SetUiActiveState();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            Peer.RunQueuedActions();
            if (!isServer) return;
            
            var point = (Cursor.Position.X, Cursor.Position.Y);
            connections.ForEach(c => server.SendMessage(0, point, c));
            connections.ForEach(c => server.SendMessage(1, MouseButtons, c));
        }

        
        private void OnMouseActivity(object sender, MouseEventArgs e)
        {
            if (!isServer)
                return;
            if (e.Button != MouseButtons.None)
            {
                connections.ForEach(c => server.SendMessage(1, e.Button, c));
            }

            if (e.Delta != 0)
            {
                var clicks = e.Delta / SystemInformation.MouseWheelScrollDelta;
                connections.ForEach(c => server.SendMessage(2, clicks, c));
            }
        }

        private void SetCursorPosition(Point position)
        {
            Cursor.Position = position;
        }

        private void SetMouseState(MouseButtons buttons)
        {
            if (IsLeftPressed(buttons) && !IsLeftPressed(previousMouseButtonsState))
                DoMouseEvent(MouseEventLeftDown);
            if (!IsLeftPressed(buttons) && IsLeftPressed(previousMouseButtonsState))
                DoMouseEvent(MouseEventLeftUp);

            if (IsRightPressed(buttons) && !IsRightPressed(previousMouseButtonsState))
                DoMouseEvent(MouseEventRightDown);
            if (!IsRightPressed(buttons) && IsRightPressed(previousMouseButtonsState))
                DoMouseEvent(MouseEventRightUp);

            if (IsMiddlePressed(buttons) && !IsMiddlePressed(previousMouseButtonsState))
                DoMouseEvent(MouseEventMiddleDown);
            if (!IsMiddlePressed(buttons) && IsMiddlePressed(previousMouseButtonsState))
                DoMouseEvent(MouseEventMiddleUp);

            previousMouseButtonsState = buttons;
        }

        private void SetMouseWheel(int wheel)
        {
            simulator.Mouse.VerticalScroll(wheel);
        }

        private void SetKeyDown(Keys keys)
        {
            simulator.Keyboard.KeyDown((VirtualKeyCode) ((int) keys));
        }

        private void SetKeyUp(Keys keys)
        {
            simulator.Keyboard.KeyUp((VirtualKeyCode) ((int) keys));
        }

        private bool IsLeftPressed(MouseButtons buttons) => (buttons & MouseButtons.Left) > 0;
        private bool IsRightPressed(MouseButtons buttons) => (buttons & MouseButtons.Right) > 0;
        private bool IsMiddlePressed(MouseButtons buttons) => (buttons & MouseButtons.Middle) > 0;

        private void serversListBox_DoubleClick(object sender, MouseEventArgs e)
        {
            var index = serversListBox.IndexFromPoint(e.Location);
            if (index == ListBox.NoMatches)
                return;
            hostAddressBox.Text = serversListBox.Items[index].ToString();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            Peer.Shutdown();
            isServer = false;
            SetUiActiveState(true);
            connections.Clear();
            updateTimer.Enabled = false;
        }

        private void SetUiActiveState(bool isActive=false)
        {
            Visible = isActive;
            actionCopyNotifyIcon.Visible = !isActive;
        }

        private void refreshServersButton_Click(object sender, EventArgs e)
        {
            client = new VoidClient();
            client.OnServerDiscovery += point => serversListBox.Items.Add(point.Address);
            serversListBox.Items.Clear();
            client.SendDiscoveryRequest(Port);
        }
    }
}
