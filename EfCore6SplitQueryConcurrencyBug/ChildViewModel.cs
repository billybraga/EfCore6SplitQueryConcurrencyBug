using System;

namespace EfCore6SplitQueryConcurrencyBug
{
    public class ChildViewModel
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; } = null!;
    }
}