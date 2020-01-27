using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Hate9;

namespace Hotkeys
{
    public class Hotkeys : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        private Identification ident;
        private Dictionary<int, Action> actions;


        public Hotkeys()
        {
            int seed = ((GuidAttribute)(typeof(Program).Assembly).GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value.GetHashCode();

            ident = new Identification(seed);
            actions = new Dictionary<int, Action>();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                /*Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);*///I might need 'em, who knows.

                actions[m.WParam.ToInt32()]?.Invoke();
            }
        }

        public void SetHotkey(Hotkey hotkey)
        {
            int id = ident.CreateId();
            actions.Add(id, hotkey.OnPress);
            RegisterHotKey(Handle, id, hotkey.modifiers, hotkey.key);
        }

        public void UnsetHotkey(int id)
        {
            UnregisterHotKey(Handle, id);
            actions.Remove(id);
            ident.DeleteId(id);
        }

        public void UnsetHotkeys()
        {
            List<int> ids = new List<int>(ident.ids);
            foreach (int id in ids)
            {
                UnsetHotkey(id);
            }
        }
    }

    public class Hotkey
    {
        public Action OnPress;
        public int modifiers, key;

        public Hotkey(Action onPress, int modifiers, Keys key)
        {
            OnPress = onPress;
            this.modifiers = modifiers;
            this.key = key.GetHashCode();
        }

        public Hotkey(Action onPress, Hotkeys.KeyModifier modifier, Keys key)
        {
            OnPress = onPress;
            modifiers = (int)modifier;
            this.key = key.GetHashCode();
        }
    }
}
