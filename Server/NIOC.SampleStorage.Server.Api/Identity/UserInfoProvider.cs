using Bit.Core.Contracts;
using Bit.Core.Exceptions;
using NIOC.SampleStorage.Server.Model.Contracts;
using NIOC.SampleStorage.Shared.Core.Dto;
using NIOC.SampleStorage.Shared.Core.Dto.Identity;
using NIOC.SampleStorage.Shared.Core.Extensions;
using System.Collections.Generic;

namespace NIOC.SampleStorage.Server.Api.Identity
{
    public class UserInfoProvider : IUserInfoProvider
    {
        public IUserInformationProvider UserInformationProvider { get; set; } = default!; //Property Injection
        public IRequestInformationProvider RequestInformationProvider { get; set; } = default!; //Property Injection


        public UserDto? CurrentUser()
        {
            if (!IsAuthenticated())
                return null;

            var firstName = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.FirstName));
            var lastName = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.LastName));
            var ssoToken = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.SSOToken));
            var identityTokenId = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.IdentityTokenId));
            var personnelCode = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.PersonnelCode));
            var unitName = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.UnitName));
            var jobTitle = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.JobTitle));
            var rahkaranId = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.RahkaranId));
            var roles = GetCustomPropFromBitJwtToken(nameof(CustomPropsDto.Roles));

            return new UserDto
            {
                UserDomainName = UserDomainName(),
                FirstName = firstName,
                LastName = lastName,
                PersonnelCode = personnelCode!.ToInt(),
                SSOToken = ssoToken,
                RahkaranId = rahkaranId!.ToInt(),
                UnitName = unitName,
                JobTitle = jobTitle,
                IdentityTokenId = identityTokenId,
                Roles = roles?.DeserializeToModel<List<string>>()  // workaround
            };
        }

        public string UserDomainName()
        {
            if (!IsAuthenticated())
                throw new UnauthorizedException("این کاربر هنوز لاگین نشده و نمی‌تواند شناسه‌ای داشته باشد");

            var userDomainName = UserInformationProvider.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userDomainName))
                throw new UnauthorizedException("این کاربر معتبر نمی‌باشد");

            return userDomainName;

        }

        public bool IsAuthenticated()
        {
            return UserInformationProvider.IsAuthenticated();
        }

        public List<string> UserRoles()
        {
            if (!IsAuthenticated())
                throw new UnauthorizedException("این کاربر هنوز لاگین نشده و نمی‌تواند نقشی داشته باشد");

            return CurrentUser()?.Roles ?? new List<string>();
        }

        public string IpAddress()
        {
            return RequestInformationProvider.ClientIp;
        }

        private string? GetCustomPropFromBitJwtToken(string propName)
        {
            var propertyName = propName == nameof(CustomPropsDto.SSOToken)
                ? propName
                : propName.ToLowerFirstChar();

            UserInformationProvider.GetBitJwtToken().CustomProps.TryGetValue(propertyName, out string? prop);

            return prop;
        }
    }
}