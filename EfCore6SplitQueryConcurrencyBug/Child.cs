using System;

namespace EfCore6SplitQueryConcurrencyBug
{
    public class Child
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Parent Parent { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}