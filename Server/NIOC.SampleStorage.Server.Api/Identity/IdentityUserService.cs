using Bit.Core.Contracts;
using Bit.Core.Exceptions;
using Bit.Core.Models;
using Bit.Data.Contracts;
using Bit.IdentityServer.Implementations;
using IdentityServer3.Core.Models;
using NIOC.SampleStorage.Server.Model.Entities.Identity;
using NIOC.SampleStorage.Shared.App;
using NIOC.SampleStorage.Shared.Core.Dto.Identity;
using NIOC.SampleStorage.Shared.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Api.Identity
{
    public class IdentityUserService : UserService
    {
        #region Property Injections

        public IRequestInformationProvider? RequestInformationProvider { get; set; }
        public IDateTimeProvider? DateTimeProvider { get; set; }
        public IRepository<IdentityTokenEntity> Repository { get; set; } = default!;
        public IContentFormatter ContentFormatter { get; set; } = default!;

        #endregion
        public override async Task<BitJwtToken> LocalLogin(LocalAuthenticationContext context, CancellationToken cancellationToken)
        {
            var props = new Dictionary<string, string>();

            string username = context.UserName.ToLower();
            string password = context.Password;
            string clientId = context.SignInMessage.ClientId.ToLower();

            var user = "null"; // Get from SSO

            if (user == null)
                throw new DomainLogicException("Login Failed");

            var userFull = "";
            //await ATAOrgInfoProvider.GetUserByIdAsync(user.UserID, cancellationToken, context.UserName);

            var userRoles = new List<string>();
            // await SSOClientService.GetUserRolesAsync(username, password, cancellationToken);

            // If user do not have any roles, so it mean he/she do not access to the app.
            if (userRoles!.Count == 0)
                throw new ForbiddenException("شما به این سامانه دسترسی ندارید");

            var identityToken = await Repository!.AddAsync(new IdentityTokenEntity
            {
                ClientName = clientId,
                IPAddress = RequestInformationProvider!.ClientIp,
                DeviceName = RequestInformationProvider?.ClientType,
                ExpiresAt = DateTimeProvider!.GetCurrentUtcDateTime() + AppConstants.JwtTokenLifetime,
                UserDomainName = "", //user.UserID,
                CreatedOn = DateTimeOffset.Now,
                ModifiedOn = DateTimeOffset.Now
            }, cancellationToken);

            props.Add(nameof(CustomPropsDto.FirstName).ToLowerFirstChar(), user);
            props.Add(nameof(CustomPropsDto.LastName).ToLowerFirstChar(), user);
            props.Add(nameof(CustomPropsDto.SSOToken), user);
            props.Add(nameof(CustomPropsDto.IdentityTokenId).ToLowerFirstChar(), identityToken.Id.ToString());
            props.Add(nameof(CustomPropsDto.PersonnelCode).ToLowerFirstChar(), user);
            props.Add(nameof(CustomPropsDto.RahkaranId).ToLowerFirstChar(), user);
            props.Add(nameof(CustomPropsDto.UnitName).ToLowerFirstChar(), user);
            props.Add(nameof(CustomPropsDto.JobTitle).ToLowerFirstChar(), user);
            props.Add(nameof(CustomPropsDto.Roles).ToLowerFirstChar(), ContentFormatter.Serialize(userRoles));

            return new BitJwtToken
            {
                UserId = user,
                CustomProps = props
            };
        }

    }
}
