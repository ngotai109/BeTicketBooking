using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Helpers
{
    public static class IntentHelper
    {
        public static string DetectIntent(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "out_of_scope";

            message = message.ToLower();

            if (message.Contains("đặt vé") || message.Contains("mua vé"))
                return "book_ticket";

            if (message.Contains("đi") || message.Contains("chuyến"))
                return "search_trip";

            if (message.Contains("văn phòng") || message.Contains("địa chỉ"))
                return "office_info";

            if (message.Contains("hủy"))
                return "cancel_ticket";

            if (message.Contains("chào") || message.Contains("hello"))
                return "small_talk";

            return "out_of_scope";
        }
    }
}
