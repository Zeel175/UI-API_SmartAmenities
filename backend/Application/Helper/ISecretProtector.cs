using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public interface ISecretProtector
    {
        string Protect(string plainText);
        string Unprotect(string protectedText);
        bool IsProtected(string value);
    }
}
