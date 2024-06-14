using System.Runtime.InteropServices;

namespace SoundCloudDownloader.Utils;

internal static class NativeMethods
{
    public static class Windows
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int MessageBox(nint hWnd, string text, string caption, uint type);
    }
}
