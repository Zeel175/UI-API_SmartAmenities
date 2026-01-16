using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Helper
{
    public static class HikvisionEventParser
    {
        public static string TryGetEmployeeNo(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            // 1) Try XML parsing (most ISAPI events are XML)
            try
            {
                // Some devices send XML with namespaces; XElement handles it
                var doc = XDocument.Parse(raw);

                // Common fields in access control events
                // employeeNoString / employeeNo / cardNo — vary by firmware
                var emp =
                    FindFirstValue(doc, "employeeNoString") ??
                    FindFirstValue(doc, "employeeNo") ??
                    FindFirstValue(doc, "cardNo") ??
                    FindFirstValue(doc, "CardNo");

                if (!string.IsNullOrWhiteSpace(emp))
                    return emp.Trim();
            }
            catch
            {
                // ignore and fallback to regex
            }

            // 2) Fallback regex extraction
            var m = Regex.Match(raw, @"<employeeNoString>\s*(.*?)\s*</employeeNoString>", RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups[1].Value.Trim();

            m = Regex.Match(raw, @"""employeeNoString""\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups[1].Value.Trim();

            return null;
        }

        private static string FindFirstValue(XDocument doc, string localName)
        {
            foreach (var e in doc.Descendants())
            {
                if (string.Equals(e.Name.LocalName, localName, StringComparison.OrdinalIgnoreCase))
                    return e.Value;
            }
            return null;
        }
    }
}
