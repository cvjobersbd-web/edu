using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Tutorbub.Models;

namespace Tutorbub.Models
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Database")
                ?? throw new InvalidOperationException("Connection string 'Database' not found.");
        }

        // ===== সব ইউজার আনা =====
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                ORDER BY ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserName = reader["UserName"]?.ToString() ?? string.Empty,
                        Password = reader["Password"]?.ToString() ?? string.Empty,
                        FullName = reader["FullName"]?.ToString() ?? string.Empty,
                        Email = reader["Email"]?.ToString() ?? string.Empty,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        LastLoginAt = reader["LastLoginAt"] as DateTime?,
                        Role = reader["Role"]?.ToString() ?? "User",
                        IsActive = reader["IsActive"] as bool? ?? true,
                        MobileNumber = reader["MobileNumber"]?.ToString(),
                        ProfileImage = reader["ProfileImage"]?.ToString(),
                        Gender = reader["Gender"]?.ToString(),
                        AgeRange = reader["AgeRange"]?.ToString(),
                        PrimaryDeviceType = reader["PrimaryDeviceType"]?.ToString(),
                        YearsOfExperience = reader["YearsOfExperience"]?.ToString(),
                        AreaType = reader["AreaType"]?.ToString(),
                        Country = reader["Country"]?.ToString(),
                        StreetAddress = reader["StreetAddress"]?.ToString(),
                        PermanentAddress = reader["PermanentAddress"]?.ToString(),
                        EducationLevel = reader["EducationLevel"]?.ToString(),
                        CurrentStudyStatus = reader["CurrentStudyStatus"]?.ToString(),
                        ExamDegreeTitle = reader["ExamDegreeTitle"]?.ToString(),
                        InstitutionName = reader["InstitutionName"]?.ToString(),
                        PassingYear = reader["PassingYear"]?.ToString(),
                        IsCSEStudent = reader["IsCSEStudent"] as bool?,
                        CvLink = reader["CvLink"]?.ToString(),
                        GithubProfile = reader["GithubProfile"]?.ToString(),
                        PortfolioLink = reader["PortfolioLink"]?.ToString(),
                        LinkedInProfile = reader["LinkedInProfile"]?.ToString(),
                        ProfileImageLink = reader["ProfileImageLink"]?.ToString()
                    });
                }
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all users: " + ex.Message);
            }
        }

        // ===== ইউজার অথেন্টিকেট করা =====
        public User? AuthenticateUser(string username, string password)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                WHERE ""UserName"" = @username AND ""Password"" = @password AND ""IsActive"" = TRUE";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserName = reader["UserName"]?.ToString() ?? string.Empty,
                        Password = reader["Password"]?.ToString() ?? string.Empty,
                        FullName = reader["FullName"]?.ToString() ?? string.Empty,
                        Email = reader["Email"]?.ToString() ?? string.Empty,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        LastLoginAt = reader["LastLoginAt"] as DateTime?,
                        Role = reader["Role"]?.ToString() ?? "User",
                        IsActive = reader["IsActive"] as bool? ?? true,
                        MobileNumber = reader["MobileNumber"]?.ToString(),
                        ProfileImage = reader["ProfileImage"]?.ToString(),
                        Gender = reader["Gender"]?.ToString(),
                        AgeRange = reader["AgeRange"]?.ToString(),
                        PrimaryDeviceType = reader["PrimaryDeviceType"]?.ToString(),
                        YearsOfExperience = reader["YearsOfExperience"]?.ToString(),
                        AreaType = reader["AreaType"]?.ToString(),
                        Country = reader["Country"]?.ToString(),
                        StreetAddress = reader["StreetAddress"]?.ToString(),
                        PermanentAddress = reader["PermanentAddress"]?.ToString(),
                        EducationLevel = reader["EducationLevel"]?.ToString(),
                        CurrentStudyStatus = reader["CurrentStudyStatus"]?.ToString(),
                        ExamDegreeTitle = reader["ExamDegreeTitle"]?.ToString(),
                        InstitutionName = reader["InstitutionName"]?.ToString(),
                        PassingYear = reader["PassingYear"]?.ToString(),
                        IsCSEStudent = reader["IsCSEStudent"] as bool?,
                        CvLink = reader["CvLink"]?.ToString(),
                        GithubProfile = reader["GithubProfile"]?.ToString(),
                        PortfolioLink = reader["PortfolioLink"]?.ToString(),
                        LinkedInProfile = reader["LinkedInProfile"]?.ToString(),
                        ProfileImageLink = reader["ProfileImageLink"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user: " + ex.Message);
            }
        }

        // ===== ইউজার প্রোফাইল আপডেট করা =====
        public bool UpdateUserProfile(User user)
        {
            string query = @"
                UPDATE ""Users"" SET 
                    ""FullName"" = @fullName,
                    ""Email"" = @email,
                    ""MobileNumber"" = @mobileNumber,
                    ""Gender"" = @gender,
                    ""AgeRange"" = @ageRange,
                    ""PrimaryDeviceType"" = @primaryDeviceType,
                    ""YearsOfExperience"" = @yearsOfExperience,
                    ""AreaType"" = @areaType,
                    ""Country"" = @country,
                    ""StreetAddress"" = @streetAddress,
                    ""PermanentAddress"" = @permanentAddress,
                    ""EducationLevel"" = @educationLevel,
                    ""CurrentStudyStatus"" = @currentStudyStatus,
                    ""ExamDegreeTitle"" = @examDegreeTitle,
                    ""InstitutionName"" = @institutionName,
                    ""PassingYear"" = @passingYear,
                    ""IsCSEStudent"" = @isCSEStudent,
                    ""CvLink"" = @cvLink,
                    ""GithubProfile"" = @githubProfile,
                    ""PortfolioLink"" = @portfolioLink,
                    ""LinkedInProfile"" = @linkedInProfile,
                    ""ProfileImageLink"" = @profileImageLink
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@fullName", user.FullName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@mobileNumber", user.MobileNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gender", user.Gender ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ageRange", user.AgeRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@primaryDeviceType", user.PrimaryDeviceType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@yearsOfExperience", user.YearsOfExperience ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@areaType", user.AreaType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@country", user.Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@streetAddress", user.StreetAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@permanentAddress", user.PermanentAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@educationLevel", user.EducationLevel ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@currentStudyStatus", user.CurrentStudyStatus ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@examDegreeTitle", user.ExamDegreeTitle ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@institutionName", user.InstitutionName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@passingYear", user.PassingYear ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCSEStudent", user.IsCSEStudent ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cvLink", user.CvLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@githubProfile", user.GithubProfile ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@portfolioLink", user.PortfolioLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@linkedInProfile", user.LinkedInProfile ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@profileImageLink", user.ProfileImageLink ?? (object)DBNull.Value);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user profile: " + ex.Message);
            }
        }

        // ===== ইউজার আইডি দিয়ে খোঁজা =====
        public User? GetUserById(int userId)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserName = reader["UserName"]?.ToString() ?? string.Empty,
                        Password = reader["Password"]?.ToString() ?? string.Empty,
                        FullName = reader["FullName"]?.ToString() ?? string.Empty,
                        Email = reader["Email"]?.ToString() ?? string.Empty,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        LastLoginAt = reader["LastLoginAt"] as DateTime?,
                        Role = reader["Role"]?.ToString() ?? "User",
                        IsActive = reader["IsActive"] as bool? ?? true,
                        MobileNumber = reader["MobileNumber"]?.ToString(),
                        ProfileImage = reader["ProfileImage"]?.ToString(),
                        Gender = reader["Gender"]?.ToString(),
                        AgeRange = reader["AgeRange"]?.ToString(),
                        PrimaryDeviceType = reader["PrimaryDeviceType"]?.ToString(),
                        YearsOfExperience = reader["YearsOfExperience"]?.ToString(),
                        AreaType = reader["AreaType"]?.ToString(),
                        Country = reader["Country"]?.ToString(),
                        StreetAddress = reader["StreetAddress"]?.ToString(),
                        PermanentAddress = reader["PermanentAddress"]?.ToString(),
                        EducationLevel = reader["EducationLevel"]?.ToString(),
                        CurrentStudyStatus = reader["CurrentStudyStatus"]?.ToString(),
                        ExamDegreeTitle = reader["ExamDegreeTitle"]?.ToString(),
                        InstitutionName = reader["InstitutionName"]?.ToString(),
                        PassingYear = reader["PassingYear"]?.ToString(),
                        IsCSEStudent = reader["IsCSEStudent"] as bool?,
                        CvLink = reader["CvLink"]?.ToString(),
                        GithubProfile = reader["GithubProfile"]?.ToString(),
                        PortfolioLink = reader["PortfolioLink"]?.ToString(),
                        LinkedInProfile = reader["LinkedInProfile"]?.ToString(),
                        ProfileImageLink = reader["ProfileImageLink"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user by ID: " + ex.Message);
            }
        }

        // ===== নতুন ইউজার রেজিস্টার =====
        public bool RegisterUser(User user)
        {
            string query = @"
                INSERT INTO ""Users"" (""UserName"", ""Password"", ""FullName"", ""Email"", ""Role"")
                VALUES (@username, @password, @fullname, @email, 'User')";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", user.UserName);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@fullname", user.FullName);
                command.Parameters.AddWithValue("@email", user.Email);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user: " + ex.Message);
            }
        }

        // ===== ইউজার ডিলিট =====
        public bool DeleteUser(int userId)
        {
            string query = "DELETE FROM \"Users\" WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user: " + ex.Message);
            }
        }

        // ===== পাসওয়ার্ড আপডেট =====
        public bool UpdatePassword(int userId, string newPassword)
        {
            string query = "UPDATE \"Users\" SET \"Password\" = @password WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@password", newPassword);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating password: " + ex.Message);
            }
        }

        // ===== ইউজার স্ট্যাটাস টগল =====
        public bool ToggleUserStatus(int userId, bool isActive)
        {
            string query = "UPDATE \"Users\" SET \"IsActive\" = @isActive WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error toggling user status: " + ex.Message);
            }
        }

        // ===== ইউজারনেম চেক =====
        public bool UsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM \"Users\" WHERE \"UserName\" = @username";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking username: " + ex.Message);
            }
        }

        // ===== ইমেইল চেক =====
        public bool EmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM \"Users\" WHERE \"Email\" = @email";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking email: " + ex.Message);
            }
        }

        // ===== লাস্ট লগইন আপডেট =====
        public void UpdateLastLogin(string username)
        {
            string query = @"UPDATE ""Users"" SET ""LastLoginAt"" = @lastLogin WHERE ""UserName"" = @username";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@lastLogin", DateTime.UtcNow);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating last login: " + ex.Message);
            }
        }
    }
}