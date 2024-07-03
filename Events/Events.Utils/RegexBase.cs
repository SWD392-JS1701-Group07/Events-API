using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Utils
{
	public class RegexBase
	{
		public const string PhoneNumberRegex = @"^(0|\+84|84)(3|5|7|8|9)[0-9]{8}$";
		public const string MustNumberRegex = @"^\d+(\.\d+)?$";
		public const string SingleSpaceCharacterRegex = @"^[^\s\d]+(\s[^\s\d]+)*$";
		public const string ErrorMessageSingleSpaceCharacterRegex = "only charactor and does not contain multiple consecutive spaces";
	}
}
