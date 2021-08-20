// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EfCore6SplitQueryConcurrencyBug
{
    class Program
    {
        private static readonly Expression<Func<Parent, ParentViewModel>> SelectExpression = x => new ParentViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Children = x
                .Children
                .Select(
                    c => new ChildViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentId = c.ParentId,
                    }
                )
                .ToArray(),
        };

        private static readonly Func<Parent, ParentViewModel> Project = SelectExpression.Compile();

        static async Task Main(string[] args)
        {
            var iterationCount = 1000;
            
            var task1 = GetEntityAsync(
                new Parent
                {
                    Id = new Guid("13319478-bec7-4a9f-bddc-dde28c1b5787"),
                    Name = "Entity1",
                    Children = new List<Child>
                    {
                        new()
                        {
                            Id = new Guid("70c7ddba-7e09-4b28-8c27-609afe777d57"),
                            ParentId = new Guid("13319478-bec7-4a9f-bddc-dde28c1b5787"),
                            Name = "Children1.1",
                        }
                    },
                }
            );
            var task2 = GetEntityAsync(
                new Parent
                {
                    Id = new Guid("249cad7c-7706-4a1e-84a6-b7f37485c71b"),
                    Name = "Entity2",
                    Children = new List<Child>
                    {
                        new()
                        {
                            Id = new Guid("bd3b9d1b-f6fd-40d9-9a34-a8658e81f2c6"),
                            ParentId = new Guid("249cad7c-7706-4a1e-84a6-b7f37485c71b"),
                            Name = "Children2.1",
                        }
                    },
                }
            );

            await Task.WhenAll(task1, task2);

            async Task GetEntityAsync(Parent parent)
            {
                var expectedSerialized = Serialize(Project(parent));
                await using var dbContext = AppDbContextFactory.Create(args);

                try
                {
                    for (var i = 0; i < iterationCount; i++)
                    {
                        var fetchedParent = await dbContext
                            .Parents
                            .Select(SelectExpression)
                            .SingleAsync(x => x.Id == parent.Id);
                        var fetchParentSerialized = Serialize(fetchedParent);
                        if (fetchParentSerialized != expectedSerialized)
                        {
                            throw new Exception(
                                $"Got\n{fetchParentSerialized}\nbut expected\n{expectedSerialized}"
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Exception occured in thread for {parent.Name}",
                        ex
                    );
                }
            }
        }

        private static string Serialize(ParentViewModel parent)
        {
            return JsonSerializer.Serialize(
                parent,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                }
            );
        }
    }
}