using System;
using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Domain
{
    public class User
    {
        public const string RoleAdmin = "Admin";
        public const string RoleUser = "User";

        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        // Store hashed password later (never store plain text).
        [StringLength(500)]
        public string? PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = RoleUser; // Admin / User

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public void Normalise() // Trim and standardize case for email and role.
        {
            FullName = FullName.Trim();
            Email = Email.Trim().ToLowerInvariant();
            Role = Role.Trim();
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                throw new ArgumentException("FullName is required.");

            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentException("Email is required.");

            if (!IsValidRole(Role))
                throw new ArgumentException($"Role must be '{RoleAdmin}' or '{RoleUser}'.");
        }

        public static bool IsValidRole(string role) =>
            string.Equals(role, RoleAdmin, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, RoleUser, StringComparison.OrdinalIgnoreCase);
    }
}
