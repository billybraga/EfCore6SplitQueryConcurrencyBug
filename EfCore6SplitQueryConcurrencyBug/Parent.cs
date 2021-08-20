using System;
using System.Collections.Generic;

namespace EfCore6SplitQueryConcurrencyBug
{
    public class Parent
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Child> Children { get; set; } = null!;
    }
}