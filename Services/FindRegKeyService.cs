using Lab1.Services.Interface;
using Microsoft.Win32;

namespace Lab1.ViewModels
{
    class FindRegKeyService : IFindRegistryKey
    {
        public string? Find(string? name)
        {
            string str = @"SYSTEM\ControlSet001\Services\" + name;

            using (RegistryKey hKey = Registry.LocalMachine.OpenSubKey(str)) 
            {
                if (hKey is not null)
                    return $"HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Services\\{name}";

                return null;
            }
        }
    }
}
