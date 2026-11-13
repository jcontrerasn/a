using System;
using System.Collections.Generic;
using System.Linq;
using TrackIT_backend.Entities;
using TrackIT_backend.Interfaces;

namespace TrackIT_backend.Repositories
{
    // Temporary persistence for development. Replace this class with an EF repository in production.
    public class InMemoryTrackItemRepository : ITrackItemRepository
    {
        private static readonly object SyncRoot = new object();
        private static readonly IList<TrackItem> Items = new List<TrackItem>();
        private static int nextId;

        public IEnumerable<TrackItem> GetAll()
        {
            lock (SyncRoot)
            {
                return Items.ToList();
            }
        }

        public TrackItem GetById(int id)
        {
            lock (SyncRoot)
            {
                return Items.FirstOrDefault(item => item.Id == id);
            }
        }

        public TrackItem Add(TrackItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            lock (SyncRoot)
            {
                entity.Id = ++nextId;
                entity.CreatedAt = DateTime.UtcNow;
                Items.Add(entity);
                return entity;
            }
        }
    }
}
