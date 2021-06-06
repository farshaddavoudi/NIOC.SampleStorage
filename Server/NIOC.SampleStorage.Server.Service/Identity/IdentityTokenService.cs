using Microsoft.EntityFrameworkCore;
using NIOC.SampleStorage.Server.Model.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Service.Identity
{
    public class IdentityTokenService : EntityService<IdentityTokenEntity>
    {
        public async Task<IdentityTokenEntity?> GetIdentityTokenById(int id, CancellationToken cancellationToken)
        {
            return await Repository
                .GetAllWithoutQueryFilter()
                .Where(token => token.IsArchived == false)
                //.Cacheable()
                .FirstOrDefaultAsync(token => token.Id == id, cancellationToken);
        }

        public async Task DeleteByUserDomainNameAsync(string userDomainName, CancellationToken cancellationToken)
        {
            await DeleteTokensWhere(token => token.UserDomainName == userDomainName, cancellationToken);
        }

        public Task DeleteTokenAsync(IdentityTokenEntity identityTokenEntity, CancellationToken cancellationToken)
        {
            return DeleteTokensAsync(new[] { identityTokenEntity }, cancellationToken);
        }

        private async Task DeleteTokensWhere(Expression<Func<IdentityTokenEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            var tokens = await Repository
                .GetAllWithoutQueryFilter()
                .Where(token => !token.IsArchived)
                .Where(predicate)
                .ToListAsync(cancellationToken);

            await DeleteTokensAsync(tokens, cancellationToken);
        }

        private Task DeleteTokensAsync(IEnumerable<IdentityTokenEntity> identityTokenEntities,
            CancellationToken cancellationToken)
        {
            return Repository.DeleteRangeAsync(identityTokenEntities, cancellationToken);
        }
    }
}