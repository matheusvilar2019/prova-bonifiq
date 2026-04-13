using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Context;

namespace ProvaPub.Services
{
	public class RandomService
	{
        TestDbContext _ctx;
		public RandomService()
        {
            var contextOptions = new DbContextOptionsBuilder<TestDbContext>()
    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Teste;Trusted_Connection=True;")
    .Options;
            _ctx = new TestDbContext(contextOptions);
        }
        public async Task<int> GetRandom()
		{            
            bool isNumberInDb = true;
            int number = 0;

            if (await isMaxLimitInDb()) throw new Exception("Limite de 100 números atingido");

            while (isNumberInDb)
            {
                number = new Random().Next(100);
                isNumberInDb = await IsNumberInDb(number);
            }

            _ctx.Numbers.Add(new RandomNumber() { Number = number });
            await _ctx.SaveChangesAsync();
            return number;
        }

        public async Task<bool> IsNumberInDb(int number)
        {
            var dbNumber = await _ctx.Numbers.FirstOrDefaultAsync(x => x.Number == number);
            return dbNumber != null;
        }

        public async Task<bool> isMaxLimitInDb()
        {
            return await _ctx.Numbers.CountAsync() >= 100;
        }
    }
}
