using BakWeb.Reservation.Entities;
using Konstrukt;
using Konstrukt.Persistence;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace BakWeb.Reservation.Repositories;

[Obsolete("Currently not in use and may be removed")]
public class ReservationRepository : KonstruktRepository<ReservationEntity, int>
{
    private readonly KonstruktRepositoryContext _konstruktContext;
    private readonly IScopeProvider _scopeProvider;

    public ReservationRepository(KonstruktRepositoryContext context, IScopeProvider scopeProvider) : base(context)
    {
        _konstruktContext = context;
        _scopeProvider = scopeProvider;
    }

    protected override int GetIdImpl(ReservationEntity entity) => entity.Id;

    protected override void DeleteImpl(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        scope.Database.Delete<ReservationRepository>(id);
    }

    protected override IEnumerable<ReservationEntity> GetAllImpl(Expression<Func<ReservationEntity, bool>> whereClause, Expression<Func<ReservationEntity, object>> orderBy, SortDirection orderByDirection)
    {
        using var scope = _scopeProvider.CreateScope();
        var query = scope.Database.Query<ReservationEntity>().Where(whereClause);

        if (orderByDirection == SortDirection.Ascending)
        {
            query = query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderByDescending(orderBy);
        }

        return query.ToList();
    }

    protected override long GetCountImpl(Expression<Func<ReservationEntity, bool>> whereClause)
    {
        using var scope = _scopeProvider.CreateScope();
        return scope.Database.Query<ReservationEntity>().Where(whereClause).Count();
    }

    protected override ReservationEntity GetImpl(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        return scope.Database.SingleOrDefaultById<ReservationEntity>(id);
    }

    protected override PagedResult<ReservationEntity> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<ReservationEntity, bool>> whereClause, Expression<Func<ReservationEntity, object>> orderBy, SortDirection orderByDirection)
    {
        using var scope = _scopeProvider.CreateScope();
        var query = scope.Database.Query<ReservationEntity>().Where(whereClause);

        if (orderByDirection == SortDirection.Ascending)
        {
            query = query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderByDescending(orderBy);
        }

        var result = query.ToPage(pageNumber, pageSize);

        return new PagedResult<ReservationEntity>(result.TotalItems, pageNumber, pageSize)
        {
            Items = result.Items
        };
    }

    protected override ReservationEntity SaveImpl(ReservationEntity entity)
    {
        using var scope = _scopeProvider.CreateScope();

        if (entity.Id == 0)
        {
            scope.Database.Insert(entity);
        }
        else
        {
            scope.Database.Update(entity);
        }
        return entity;
    }
}
