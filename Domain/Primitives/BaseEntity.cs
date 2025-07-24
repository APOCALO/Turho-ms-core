using Domain.Utils;

namespace Domain.Primitives
{
    public abstract class BaseEntity
    {
        public DateTime DateCreated { get; private set; } = DateTime.Now.ToColombiaTime();
        public Guid CreatedById { get; private set; }
        public DateTime? DateUpdated { get; private set; }
        public Guid? UpdatedById { get; private set; }

        public void SetAuditUpdate(Guid updatedById)
        {
            DateUpdated = DateTime.Now.ToColombiaTime();
            UpdatedById = updatedById;
        }
    }
}
