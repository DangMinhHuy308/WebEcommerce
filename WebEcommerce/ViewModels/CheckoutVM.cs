﻿using System.ComponentModel.DataAnnotations;

namespace WebEcommerce.ViewModels
{
    public class CheckoutVM
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ShippingMethod { get; set; }
        public DateTime OrderDate { get; set; }

    }
}