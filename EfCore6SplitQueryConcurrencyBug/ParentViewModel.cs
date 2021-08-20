using System;
using System.Collections.Generic;

namespace EfCore6SplitQueryConcurrencyBug
{
    public class ParentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<ChildViewModel> Children { get; set; } = null!;
    }
}