namespace Luval.AI.Database.UnitTests
{
    public class When_Propmpting_To_Sql
    {
        [Fact]
        public async void It_Should_Return_A_Valid_Sql()
        {
            var dataPrompt = Utils.CreateDataPrompt();
            var result = await dataPrompt.SendAsync("Provide a breakdown of sales by territory in Q1 of 2011");
            Assert.NotNull(result);
        }
    }
}