namespace Core
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public SiteConfig SiteConfig { get; set; }
    }

    public class BankConfig
    {
        public string TerminalId { get; set; }

        public string MerchantId { get; set; }

        public string MerchantKey { get; set; }

        public string ReturnUrl { get; set; }

        public string SecondReturnUrl { get; set; }

        public string PurchasePage { get; set; }

        
    }

    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumic { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }

    /// <summary>
    /// اطلاعات کاربری سامانه پیامکی
    /// </summary>
    public class SmsSettings
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Number { get; set; }
    }
   

    public class SiteConfig
    {
        public string UrlAddress { get; set; }
    }
}
