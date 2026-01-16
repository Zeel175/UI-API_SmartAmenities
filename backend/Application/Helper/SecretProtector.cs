using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public sealed class SecretProtector : ISecretProtector
    {
        private readonly IDataProtector _protector;

        public SecretProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("SmartAmenities.Building.DevicePassword.v1");
        }

        public string Protect(string plainText) => _protector.Protect(plainText);

        public string Unprotect(string protectedText) => _protector.Unprotect(protectedText);

        // helper to detect if DB has plain text
        public bool IsProtected(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            try
            {
                _protector.Unprotect(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
