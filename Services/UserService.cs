using TailorSoftAPI.DTOs.Common;
using TailorSoftAPI.DTOs.Users;
using TailorSoftAPI.Interfaces.Repositories;
using TailorSoftAPI.Interfaces.Services;
using TailorSoftAPI.Models;
using System.Net.Mail;

namespace TailorSoftAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "User repository cannot be null");
        }

        public async Task<ResultDto<Guid>> CreateAsync(CreateUserDto dto)
        {
            if (dto is null)
                return ResultDto<Guid>.Failure("Create user DTO cannot be null");

            var EmailValidation = ValidateEmail(dto.Email);

            if (!EmailValidation.IsValid)
                return ResultDto<Guid>.Failure(EmailValidation.Error ?? string.Empty);

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                return ResultDto<Guid>.Failure("First name is required");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                return ResultDto<Guid>.Failure("Last name is required");
            if(this.ExistsByEmailAsync(dto.Email).Result.Value)
                return ResultDto<Guid>.Failure("Email already exists");

            var guid = await _repository.CreateAsync(dto);
            if (guid is null || guid == Guid.Empty)
                return ResultDto<Guid>.Failure("Failed to create user");

            return ResultDto<Guid>.Success(guid.Value);
        }

        public async Task<ResultDto<UserResponseDto>> GetByIdAsync(Guid id)
        {
            if(id == Guid.Empty)
                return ResultDto<UserResponseDto>.Failure("User ID cannot be empty");

            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return ResultDto<UserResponseDto>.Failure("User with Id:" + id + " not found");

            return ResultDto<UserResponseDto>.Success(MapToUserResponseDto(user));
        }

        public async Task<ResultDto<UserResponseDto>> GetByEmailAsync(string email)
        {
            var emailValidation = ValidateEmail(email);
            if (!emailValidation.IsValid)
                return ResultDto<UserResponseDto>.Failure(emailValidation.Error ?? string.Empty);


            var user = await _repository.GetByEmailAsync(email);

            if (user == null)
                return ResultDto<UserResponseDto>.Failure("User with Email:" + email + " not found");

            return ResultDto<UserResponseDto>.Success(MapToUserResponseDto(user));
        }

        public async Task<ResultDto<List<UserResponseDto>>> GetAllAsync(PagedRequestDto dto)
        {
            var validationResult = ValidatePagedRequest(dto);
            if (!validationResult.IsSuccess)
                return ResultDto<List<UserResponseDto>>.Failure(validationResult.Error??string.Empty);

            var users = await _repository.GetAllAsync(dto);
            var userDtos = users.Select(MapToUserResponseDto).ToList();
            if (userDtos.Count == 0)
                return ResultDto<List<UserResponseDto>>.Failure("No users found");

            return ResultDto<List<UserResponseDto>>.Success(userDtos);
        }

        public async Task<ResultDto<bool>> UpdateAsync(Guid userId, UpdateUserDto dto)
        {
            if (userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");
           
            if (dto == null)
                return ResultDto<bool>.Failure("Update user DTO cannot be null");
            var emailValidation = ValidateEmail(dto.Email);
            if (!emailValidation.IsValid)
                return ResultDto<bool>.Failure(emailValidation.Error ?? string.Empty);
            if(string.IsNullOrWhiteSpace(dto.FirstName))
                return ResultDto<bool>.Failure("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                return ResultDto<bool>.Failure("Last name cannot be empty");

            var result = await _repository.UpdateAsync(userId, dto);
            if(result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to update user");
        }

        public async Task<ResultDto<bool>> UpdateLastLogin(Guid userId)
        {
            if(userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            var result = await _repository.UpdateLastLogin(userId);
            if(result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to update last login");
        }

        public async Task<ResultDto<bool>> DeleteAsync(Guid userId)
        {
            if(userId == Guid.Empty)
                return ResultDto<bool>.Failure("User ID cannot be empty");

            var result = await _repository.DeleteAsync(userId);
            if(result)
                return ResultDto<bool>.Success(result);
            else
                return ResultDto<bool>.Failure("Failed to delete user");
        }

        public async Task<ResultDto<bool>> ExistsByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ResultDto<bool>.Failure("Email cannot be empty");
            if (!email.Contains("@") || !email.Contains("."))
                return ResultDto<bool>.Failure("Email format is invalid");
            if (email.Length > 255)
                return ResultDto<bool>.Failure("Email cannot exceed 255 characters");
            if (email.Length < 5)
                return ResultDto<bool>.Failure("Email must be at least 5 characters long");
        
            var exists = await _repository.ExistsByEmailAsync(email);
            return ResultDto<bool>.Success(exists);
        }

        #region Mapping Methods

        private UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };
        }

        #endregion

        #region Validation Methods

        private (bool IsValid, string? Error) ValidateEmail(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
                return (false, "Email cannot be empty");
       
            if(email.Length > 255)
                return (false, "Email cannot exceed 255 characters");

            if (email.Length < 5)
                return (false, "Email must be at least 5 characters long");

            if(MailAddress.TryCreate(email, out var _))
                return (true, null);
            else
                return (false, "Email format is invalid");
        }

        private ResultDto<bool> ValidatePagedRequest(PagedRequestDto dto)
        {
            if (dto == null)
                return ResultDto<bool>.Failure("Paged request DTO cannot be null");

            if (dto.PageNumber < 1)
                return ResultDto<bool>.Failure("Page number must be greater than 0");

            if (dto.PageSize < 1)
                return ResultDto<bool>.Failure("Page size must be greater than 0");

            return ResultDto<bool>.Success(true);
        }

        #endregion
    }
}