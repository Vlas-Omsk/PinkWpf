using PinkWpf.WinApi;
using PinkWpf.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace PinkWpf
{
    public sealed class SystemMenu : IList<SystemMenuItem>, IList, IDisposable
    {
        private readonly List<SystemMenuItem> _items = new List<SystemMenuItem>();
        internal readonly IntPtr _handle;
        private bool _disposable;
        private bool _opened;
        private HwndSource _hwndSource;
        private bool _removeHookOnNextMessage;

        internal SystemMenu(IntPtr handle) : this(handle, true)
        {
        }

        internal SystemMenu(IntPtr handle, bool disposable)
        {
            _handle = handle;
            _disposable = disposable;

            EnumerateItems();
        }

        ~SystemMenu()
        {
            Dispose();
        }

        #region IList Generic

        public int Count => _items.Count;
        public bool IsReadOnly { get; } = false;

        public SystemMenuItem this[int index]
        {
            get => _items[index];
            set
            {
                VerifyOwner(value);
                VerifyCommonStates(value);

                value.SetFullMask();
                UpdateAt(index, value);
                value.Owner = this;
                _items[index] = value;
            }
        }

        public void Add(SystemMenuItem item)
        {
            Insert(Count, item);
        }

        public void Clear()
        {
            for (var i = 0; i < _items.Count; i++)
                RemoveAt(i);
            foreach (var item in _items)
                item.Owner = null;
            _items.Clear();
        }

        public bool Contains(SystemMenuItem item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(SystemMenuItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int IndexOf(SystemMenuItem item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, SystemMenuItem item)
        {
            VerifyOwner(item);
            VerifyCommonStates(item);

            if (!User32.InsertMenuItem(_handle, (uint)index, true, ref item._info))
                throw new Win32Exception();
            item.ResetMask();
            item.Owner = this;
            _items.Insert(index, item);
        }

        public bool Remove(SystemMenuItem item)
        {
            if (item == null)
                return false;
            var id = _items.IndexOf(item);
            if (id == -1)
                return false;
            RemoveAt(id);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (!User32.RemoveMenu(_handle, (uint)index, MF.BYPOSITION))
                throw new Win32Exception();
            var item = _items[index];
            item.Owner = null;
            _items.RemoveAt(index);
        }

        public IEnumerator<SystemMenuItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IList

        bool IList.IsReadOnly { get; } = false;
        bool IList.IsFixedSize { get; } = false;
        int ICollection.Count => Count;
        object ICollection.SyncRoot => ((IList)_items).SyncRoot;
        bool ICollection.IsSynchronized => ((IList)_items).IsSynchronized;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (SystemMenuItem)value;
        }

        int IList.Add(object value)
        {
            Add((SystemMenuItem)value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return ((IList)_items).Contains(value);
        }

        void IList.Clear()
        {
            Clear();
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_items).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (SystemMenuItem)value);
        }

        void IList.Remove(object value)
        {
            Remove((SystemMenuItem)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_items).CopyTo(array, index);
        }

        #endregion

        public void Show(Window window, int x, int y)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            _hwndSource = HwndSource.FromHwnd(windowInteropHelper.Handle);

            _hwndSource.AddHook(Hook);

            if (!User32.TrackPopupMenuEx(_handle, 0x0002, x, y, windowInteropHelper.Handle, IntPtr.Zero))
                throw new Win32Exception();
        }

        private void EnumerateItems()
        {
            var count = User32.GetMenuItemCount(_handle);

            for (var i = 0u; i < count; i++)
            {
                var item = new SystemMenuItem(0)
                {
                    Owner = this
                };
                item.SetFullMask();

                if (!User32.GetMenuItemInfo(_handle, i, true, ref item._info))
                    throw new Win32Exception();

                item._info.cch++;
                item._info.dwTypeData = new string(' ', (int)item._info.cch);

                if (!User32.GetMenuItemInfo(_handle, i, true, ref item._info))
                    throw new Win32Exception();

                item.ResetMask();

                if (item._info.hSubMenu != IntPtr.Zero)
                    item.SubMenuInternal = new SystemMenu(item._info.hSubMenu);

                _items.Add(item);
            }
        }

        internal IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var lRet = IntPtr.Zero;
            var wmMsg = (WM)msg;

            if (wmMsg == WM.MENUSELECT)
            {
                var lo = ParamHelper.LowWord(wParam);
                var hi = ParamHelper.HighWord(wParam);

                if (hi == 0xffff && lParam == IntPtr.Zero)
                {
                    if (_opened)
                    {
                        RaiseClosed();
                        handled = true;
                    }
                }
                else
                {
                    var item = GetItem(lo, lParam);

                    if (item != null)
                    {
                        item.RaiseStateChanged((MF)hi);
                        handled = true;
                    }
                }
            }

            if (wmMsg == WM.INITMENUPOPUP && wParam == _handle)
            {
                RaiseOpening();
                handled = true;
            }

            if (wmMsg == WM.COMMAND)
            {
                var lo = ParamHelper.LowWord(wParam);
                var hi = ParamHelper.HighWord(wParam);

                if (hi == 0)
                {
                    var item = GetItem(lo, IntPtr.Zero);

                    if (item != null)
                    {
                        item.RaiseClick();
                        handled = true;
                    }
                }
            }

            if (wmMsg == WM.SYSCOMMAND)
            {
                var item = GetItem((int)wParam, IntPtr.Zero);
                item?.RaiseClick();
            }

            if (!_opened && _removeHookOnNextMessage)
            {
                _hwndSource.RemoveHook(Hook);
                _hwndSource = null;

                _removeHookOnNextMessage = false;
            }

            if (_disposable && wmMsg == WM.EXITMENULOOP)
                _removeHookOnNextMessage = true;

            return lRet;
        }

        private SystemMenuItem GetItem(int itemId, IntPtr menuHandle)
        {
            if (menuHandle == IntPtr.Zero ||
                _handle == menuHandle)
            {
                foreach (var item in _items)
                {
                    if (item.Id == itemId)
                        return item;
                }
            }

            foreach (var item in _items)
            {
                var item2 = item.SubMenu?.GetItem(itemId, menuHandle);

                if (item2 != null)
                    return item2;
            }

            return null;
        }

        internal void UpdateAt(int index, SystemMenuItem item)
        {
            if (!User32.SetMenuItemInfo(_handle, (uint)index, true, ref item._info))
                throw new Win32Exception();
            item.ResetMask();
        }

        private void VerifyOwner(SystemMenuItem item)
        {
            if (item.Owner != null)
                throw new ArgumentException("Must disconnect specified menu item from current owner before attaching to new owner");
        }

        private void VerifyCommonStates(SystemMenuItem item)
        {
            VerifySharedState(item, MFS.DEFAULT);
            VerifySharedState(item, MFS.HILITE);
        }

        internal void VerifySharedState(SystemMenuItem item, MFS mfs)
        {
            foreach (var item2 in _items)
            {
                if (item2 == item)
                    continue;

                item2._info.fState &= ~mfs;
            }
        }

        public void Dispose()
        {
            if (!_disposable)
                return;

            User32.DestroyMenu(_handle);

            GC.SuppressFinalize(this);
        }

        public static SystemMenu Create()
        {
            var handle = User32.CreateMenu();
            return new SystemMenu(handle);
        }

        public static SystemMenu CreatePopup()
        {
            var handle = User32.CreatePopupMenu();
            return new SystemMenu(handle);
        }

        private void RaiseClosed()
        {
            _opened = false;

            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseOpening()
        {
            _opened = true;

            Opening?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Closed;
        public event EventHandler Opening;
    }
}
