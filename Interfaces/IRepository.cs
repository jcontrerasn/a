using System.Collections.Generic;
using TrackIT_backend.Entities;

namespace TrackIT_backend.Interfaces
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetById(int id);

        TEntity Add(TEntity entity);
    }
}
