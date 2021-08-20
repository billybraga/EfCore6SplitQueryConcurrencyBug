To run

```
cd EfCore6SplitQueryConcurrencyBug
dotnet ef database update -- YOUR-DB-PASSWORD

# Will log errors
# System.IndexOutOfRangeException: Index was outside the bounds of the array.
#         at Microsoft.EntityFrameworkCore.Query.Internal.BufferedDataReader.BufferedDataRecord.GetGuid
# and throw exception because result don't match expected
dotnet run -- YOUR-DB-PASSWORD

# Will use MemoryCacheExceptCommandCacheKey IMemoryCache implementation to avoid bug
# and will finish successfuly
dotnet run -- YOUR-DB-PASSWORD true
```