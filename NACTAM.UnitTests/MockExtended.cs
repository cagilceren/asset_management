using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
using Moq.Language.Flow;
namespace NACTAM.UnitTestExtension;


/// <summary>
/// extension method overriding Moq.EntityFrameworkCore for easier setup with
/// mutable IList instead of IQueryable
///
/// Author: Tuan Bui
/// </summary>
public static class MockExtended {
	public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(this ISetup<T, DbSet<TEntity>> setupResult, List<TEntity> entities, Mock<DbSet<TEntity>>? dbSetMock = null)
		where T : class
		where TEntity : class {

		dbSetMock = dbSetMock ?? new Mock<DbSet<TEntity>>();

		var entitiesAsQueryable = entities.AsQueryable();

		dbSetMock.As<IAsyncEnumerable<TEntity>>()
			.Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
			.Returns(new InMemoryDbAsyncEnumerator<TEntity>(entitiesAsQueryable.GetEnumerator()));

		dbSetMock.As<IQueryable<TEntity>>()
			.Setup(m => m.Provider)
			.Returns(new InMemoryAsyncQueryProvider<TEntity>(entitiesAsQueryable.Provider));


		// finding the key type:
		PropertyInfo? prop = typeof(T).GetProperty("Id");
		Type keyType = prop?.PropertyType ?? typeof(TEntity);
		dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(entitiesAsQueryable.Expression);
		dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(entitiesAsQueryable.ElementType);
		dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => entitiesAsQueryable.GetEnumerator());
		dbSetMock.Setup(m => m.Add(It.IsAny<TEntity>())).Callback<TEntity>((s) => entities.Add(s));
		dbSetMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Callback<TEntity>((s) => entities.Remove(s));
		dbSetMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>())).Callback<IEnumerable<TEntity>>(x => entities.RemoveAll(x.Contains));
		dbSetMock.Setup(d => d.AddAsync(It.IsAny<TEntity>(), default(CancellationToken))).Callback<TEntity, CancellationToken>((s, _) => entities.Add(s));
		return setupResult.Returns(dbSetMock.Object);
	}

}


