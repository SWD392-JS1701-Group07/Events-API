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
            var properties = dto.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(dto) as string;
                    if (property.Name != "AvatarUrl" && string.IsNullOrWhiteSpace(value))
                    {
                        throw new Exception($"{property.Name} should not be empty or contain only whitespace.");
                    }
                }
            }
        }
    }
}
