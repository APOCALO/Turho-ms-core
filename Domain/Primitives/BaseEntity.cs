namespace Domain.Primitives
{
    public abstract class BaseEntity
    {
        public DateTime DateCreated { get; private set; } = DateTime.UtcNow;
        public Guid CreatedById { get; private set; }
        public DateTime? DateUpdated { get; private set; }
        public Guid? UpdatedById { get; private set; }

        public void SetAuditUpdate(Guid updatedById)
        {
            DateUpdated = DateTime.UtcNow;
            UpdatedById = updatedById;
        }
    }
}
