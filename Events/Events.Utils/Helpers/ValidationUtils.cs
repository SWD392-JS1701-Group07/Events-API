using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Utils.Helpers
{
    public static class ValidationUtils
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPhoneNumber(string phone)
        {
            return phone.All(char.IsDigit);
        }

        public static void ValidateNoWhitespaceOnly(object dto)
        {
            var stringProperties = dto.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => new { p.Name, Value = p.GetValue(dto) as string });

            foreach (var prop in stringProperties)
            {
                if (string.IsNullOrWhiteSpace(prop.Value))
                {
                    throw new Exception($"{prop.Name} should not be empty or contain only whitespace.");
                }
            }
        }
    }
}
