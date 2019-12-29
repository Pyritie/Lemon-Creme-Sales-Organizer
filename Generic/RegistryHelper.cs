using Microsoft.Win32;

namespace SalesOrganizer.Generic
{
    public static class RegistryHelper
    {
        private readonly static RegistryKey s_key;

        static RegistryHelper()
        {
            s_key = Registry.CurrentUser.CreateSubKey("Software/Pyritie/SalesOrganizer");
        }



        public static string InventoryFilePath
        {
            get => s_key.GetValue(nameof(InventoryFilePath), "").ToString();
            set => s_key.SetValue(nameof(InventoryFilePath), value, RegistryValueKind.String);
        }

        public static string SalesFilePath
        {
            get => s_key.GetValue(nameof(SalesFilePath), "").ToString();
            set => s_key.SetValue(nameof(SalesFilePath), value, RegistryValueKind.String);
        }
    }
}
