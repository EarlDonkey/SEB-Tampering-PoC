using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Keyboard;
using SafeExamBrowser.Settings.Monitoring;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Contracts.Events;

namespace SafeExamBrowser.Monitoring.Keyboard
{
	// Token: 0x0200000D RID: 13
	public class KeyboardInterceptor : IKeyboardInterceptor
	{
		// Token: 0x06000059 RID: 89 RVA: 0x000038FA File Offset: 0x00001AFA
		public KeyboardInterceptor(ILogger logger, INativeMethods nativeMethods, KeyboardSettings settings)
		{
			this.logger = logger;
			this.nativeMethods = nativeMethods;
			this.settings = settings;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003917 File Offset: 0x00001B17
		public void Start()
		{
			this.hookId = new Guid?(this.nativeMethods.RegisterKeyboardHook(new KeyboardHookCallback(this.KeyboardHookCallback)));
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000393B File Offset: 0x00001B3B
		public void Stop()
		{
			if (this.hookId != null)
			{
				this.nativeMethods.DeregisterKeyboardHook(this.hookId.Value);
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003960 File Offset: 0x00001B60
		private bool KeyboardHookCallback(int keyCode, KeyModifier modifier, KeyState state)
		{
			bool flag = false;
			Key key = KeyInterop.KeyFromVirtualKey(keyCode);
			bool flag2 = flag | key == Key.Apps | (key == Key.Escape && modifier == KeyModifier.None && !this.settings.AllowEsc) | (key == Key.F1 && !this.settings.AllowF1) | (key == Key.F2 && !this.settings.AllowF2) | (key == Key.F3 && !this.settings.AllowF3) | (key == Key.F4 && !this.settings.AllowF4) | (key == Key.F5 && !this.settings.AllowF5) | (key == Key.F6 && !this.settings.AllowF6) | (key == Key.F7 && !this.settings.AllowF7) | (key == Key.F8 && !this.settings.AllowF8) | (key == Key.F9 && !this.settings.AllowF9) | (key == Key.F10 && !this.settings.AllowF10) | (key == Key.F11 && !this.settings.AllowF11) | (key == Key.F12 && !this.settings.AllowF12) | (key == Key.LWin && !this.settings.AllowSystemKey) | (key == Key.Snapshot && !this.settings.AllowPrintScreen) | (key == Key.RWin && !this.settings.AllowSystemKey) | (modifier.HasFlag(KeyModifier.Alt) && key == Key.Escape && !this.settings.AllowAltEsc) | (modifier.HasFlag(KeyModifier.Alt) && key == Key.F4 && !this.settings.AllowAltF4) | (modifier.HasFlag(KeyModifier.Alt) && key == Key.Space) | (modifier.HasFlag(KeyModifier.Alt) && key == Key.Tab) | (modifier.HasFlag(KeyModifier.Ctrl) && key == Key.C && !this.settings.AllowCtrlC) | (modifier.HasFlag(KeyModifier.Ctrl) && key == Key.Escape && !this.settings.AllowCtrlEsc) | (modifier.HasFlag(KeyModifier.Ctrl) && key == Key.V && !this.settings.AllowCtrlV) | (modifier.HasFlag(KeyModifier.Ctrl) && key == Key.X && !this.settings.AllowCtrlX) | modifier.HasFlag(KeyModifier.Injected);
			if (flag2)
			{
				this.Log(key, keyCode, modifier, state);
			}
			return flag2;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003C40 File Offset: 0x00001E40
		private void Log(Key key, int keyCode, KeyModifier modifier, KeyState state)
		{
			IEnumerable<KeyModifier> modifierFlags = from m in Enum.GetValues(typeof(KeyModifier)).OfType<KeyModifier>()
			where m != KeyModifier.None && modifier.HasFlag(m)
			select m;
			string modifiers = modifierFlags.Any<KeyModifier>() ? (string.Join<KeyModifier>(" + ", modifierFlags) + " + ") : string.Empty;
			this.logger.Info(string.Format("Blocked '{0}{1}' ({2} = {3}) when {4}.", new object[]
			{
				modifiers,
				key,
				key,
				keyCode,
				state.ToString().ToLower()
			}));
		}

		// Token: 0x0400002D RID: 45
		private Guid? hookId;

		// Token: 0x0400002E RID: 46
		private readonly ILogger logger;

		// Token: 0x0400002F RID: 47
		private readonly INativeMethods nativeMethods;

		// Token: 0x04000030 RID: 48
		private readonly KeyboardSettings settings;
	}
}
