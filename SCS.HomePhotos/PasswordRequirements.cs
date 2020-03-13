using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.HomePhotos
{
    public class PasswordRequirements
    {
        public int MinLength { get; set; }

        public int UppercaseCharacters { get; set; }

        public int Digits { get; set; }

        public int SpecialCharacters { get; set; }
    }
}
