namespace BTMBackend.Utilities
{
    public class SMSMessages()
    {
        public static string SendOTP(string code)
        {
            string MessageAr = "رمز التحقق الخاص بك هو " + code;
            string MessageEn = "Your Verification Code is " + code;
            return MessageAr + "\n" + MessageEn;
        }

        public static string TempPassword(string password)
        {
            string MessageAr = "الرمز المؤقت الخاص بك هو " + password;
            string MessageEn = "Your Temporary Password is " + password;
            return MessageAr + "\n" + MessageEn;
        }
    }
}
