﻿using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [Compare(nameof(NewPasswordCompare))]
        public string NewPassword { get; set; }

        [Required]
        public string NewPasswordCompare { get; set; }
    }
}