namespace TrackIT_backend.Entities
{
    public class TrackItem : EntityBase
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public bool Completed { get; set; }
    }
}
