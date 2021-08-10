using System;
using TutorStudent.Domain.Interfaces;

namespace TutorStudent.Domain.Implementations
{
    public abstract class Entity : IEntity
    {
        protected Entity()
        {
            Id = Comb.Create();
        }

        public Guid Id { get; set; }
    }
}