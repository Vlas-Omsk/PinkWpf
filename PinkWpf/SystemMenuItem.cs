using PinkWpf.WinApi;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PinkWpf
{
    public sealed class SystemMenuItem
    {
        public SystemMenu Owner { get; internal set; }
        internal SystemMenu SubMenuInternal { get; set; }

        internal MENUITEMINFO _info = new MENUITEMINFO();

        private static int _counter = 1;

        public SystemMenuItem(int id)
        {
            _info.cbSize = Marshal.SizeOf<MENUITEMINFO>();
            _info.wID = (uint)id;
            _info.fMask |= MIIM.ID;
        }

        public SystemMenuItem() : this(_counter++)
        {
        }

        public int Id
        {
            get => (int)_info.wID;
            set
            {
                _info.wID = (uint)value;
                _info.fMask |= MIIM.ID;

                Update();
            }
        }

        public string Header
        {
            get => _info.dwTypeData;
            set
            {
                _info.dwTypeData = value;
                _info.cch = value != null ? 
                    (uint)value.Length : 
                    0;
                _info.fMask |= MIIM.STRING;

                Update();
            }
        }

        public SystemMenu SubMenu
        {
            get => SubMenuInternal;
            set
            {
                SubMenuInternal = value;

                if (value == null)
                    _info.hSubMenu = IntPtr.Zero;
                else
                    _info.hSubMenu = value._handle;

                _info.fMask |= MIIM.SUBMENU;

                Update();
            }
        }

        public bool Checked
        {
            get => _info.fState.HasFlag(MFS.CHECKED);
            set
            {
                SetState(MFS.CHECKED, value);
                Update();
            }
        }

        public bool Default
        {
            get => _info.fState.HasFlag(MFS.DEFAULT);
            set
            {
                SetState(MFS.DEFAULT, value);

                if (Owner != null)
                    Owner.VerifySharedState(this, MFS.DEFAULT);

                Update();
            }
        }

        public bool Disabled
        {
            get => _info.fState.HasFlag(MFS.DISABLED);
            set
            {
                SetState(MFS.DISABLED, value);
                Update();
            }
        }

        public bool Highlited
        {
            get => _info.fState.HasFlag(MFS.HILITE);
            set
            {
                SetState(MFS.HILITE, value);

                if (Owner != null)
                    Owner.VerifySharedState(this, MFS.HILITE);

                Update();
            }
        }

        public FlowDirection FlowDirection
        {
            get => _info.fType.HasFlag(MFT.RIGHTORDER) ?
                FlowDirection.RightToLeft :
                FlowDirection.LeftToRight;
            set
            {
                SetType(MFT.RIGHTORDER, value == FlowDirection.RightToLeft);
                Update();
            }
        }

        public bool Radiocheck
        {
            get => _info.fType.HasFlag(MFT.RADIOCHECK);
            set
            {
                SetType(MFT.RADIOCHECK, value);
                Update();
            }
        }

        public bool Separator
        {
            get => _info.fType.HasFlag(MFT.SEPARATOR);
            set
            {
                SetType(MFT.SEPARATOR, value);
                Update();
            }
        }

        public MenuBreak Break
        {
            get
            {
                if (_info.fType.HasFlag(MFT.MENUBARBREAK))
                    return MenuBreak.BarBreak;
                if (_info.fType.HasFlag(MFT.MENUBREAK))
                    return MenuBreak.Break;
                return MenuBreak.None;
            }
            set
            {
                SetType(MFT.MENUBARBREAK, value == MenuBreak.BarBreak);
                SetType(MFT.MENUBREAK, value == MenuBreak.Break);
                Update();
            }
        }

        internal void ResetMask()
        {
            _info.fMask = 0;
        }

        internal void SetFullMask()
        {
            _info.fMask = MIIM.STRING | MIIM.ID | MIIM.STATE | MIIM.FTYPE | MIIM.SUBMENU;
        }

        private void SetState(MFS state, bool value)
        {
            if (value)
                _info.fState |= state;
            else
                _info.fState &= ~state;
            _info.fMask |= MIIM.STATE;
        }

        private void SetType(MFT type, bool value)
        {
            if (value)
                _info.fType |= type;
            else
                _info.fType &= ~type;
            _info.fMask |= MIIM.FTYPE;
        }

        private void Update()
        {
            if (Owner == null)
                return;

            Owner.UpdateAt(Owner.IndexOf(this), this);
            ResetMask();
        }

        internal void RaiseClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseStateChanged(MF mf)
        {
            if (mf.HasFlag(MF.CHECKED))
                _info.fState |= MFS.CHECKED;

            if (mf.HasFlag(MF.DISABLED) || mf.HasFlag(MF.GRAYED))
                _info.fState |= MFS.DISABLED;

            if (mf.HasFlag(MF.HILITE) || mf.HasFlag(MF.MOUSESELECT))
            {
                _info.fState |= MFS.HILITE;
                Owner.VerifySharedState(this, MFS.HILITE);
            }

            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Click;
        public event EventHandler StateChanged;
    }
}
