using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Enums
{
    public enum AuthProvider
    {
        Email,          // Email + Password
        Google,         // Gmail OAuth
        Microsoft,      // Hotmail/Outlook OAuth
        Facebook,       // Facebook OAuth
        MobileOtp,      // SMS OTP
        AadhaarOtp      // Aadhaar OTP (India KYC)
    }
    
}
