using OtpNet;

namespace BTMBackend.Utilities
{
    public class RandomCodeGenerator
    {
        public static string GeneratePassword()
        {
            int length = 8;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
            var random = new Random();

            string password = new(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
        public static string GenerateOTP()
        {
            string secretKey = "JBSWY3DPEHPK3PXP";

            // Convert the secret key from Base32 to bytes
            var bytes = Base32Encoding.ToBytes(secretKey);

            // Create a TOTP instance with a time step of 300 seconds (5 minutes)
            var totp = new Totp(bytes, step: 300);

            // Compute the OTP for the current time (UTC)
            string otp = totp.ComputeTotp(DateTime.UtcNow);

            return otp;

            // You can also verify an OTP entered by the user
            // Example: bool isValid = totp.VerifyTotp(userInput, out _);

        }
    }
}
