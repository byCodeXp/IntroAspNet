namespace IntroAspNet
{
    public class ENV
    {
        public class Role
        {
            public const string Master = "Master"; // Can manage admins
            public const string Admin = "Admin"; // Can manage customers
            public const string Customer = "Customer";
        }

        public class Path
        {
            public const string ProductPreview = @"/images/products/";
            public const string CategoryPreview = @"/resources/preview/category/";
        }
    }
}
