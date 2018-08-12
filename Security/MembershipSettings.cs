using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace Nistec.Web.Security
{
    // Summary:
    //     Describes the encryption format for storing passwords for membership users.
    [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public enum MembershipPasswordFormat
    {
        // Summary:
        //     Passwords are not encrypted.
        Clear = 0,
        //
        // Summary:
        //     Passwords are encrypted one-way using the SHA1 hashing algorithm.
        Hashed = 1,
        //
        // Summary:
        //     Passwords are encrypted using the encryption settings determined by the machineKey
        //     Element (ASP.NET Settings Schema) element configuration.
        Encrypted = 2,
    }

    public class MembershipSettings
    {
        public MembershipSettings()
        {
            EnablePasswordRetrieval = false;
            EnablePasswordReset = true;
            RequiresQuestionAndAnswer = true;
            RequiresUniqueEmail = true;
            MaxInvalidPasswordAttempts = 5;
            PasswordAttemptWindow = 10;
            MinRequiredPasswordLength = 7;
            MinRequiredNonAlphanumericCharacters = 1;
            PasswordStrengthRegularExpression = "";
            PasswordFormat = MembershipPasswordFormat.Hashed;
        }

        public bool EnablePasswordRetrieval { get; set; }
        public bool EnablePasswordReset { get; set; }
        public bool RequiresQuestionAndAnswer { get; set; }
        public int MaxInvalidPasswordAttempts { get; set; }
        public int PasswordAttemptWindow { get; set; }
        public bool RequiresUniqueEmail { get; set; }
        public MembershipPasswordFormat PasswordFormat { get; set; }
        public int MinRequiredPasswordLength { get; set; }
        public int MinRequiredNonAlphanumericCharacters { get; set; }
        public string PasswordStrengthRegularExpression { get; set; }
    }
}
