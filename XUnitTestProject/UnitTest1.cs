namespace XUnitTestProject
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange

            //Act

            //Assert

            Assert.True(true);
        }
        [Fact]
        public void PassignTest()
        {
            Assert.Equal(3, Add(1, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(1, Add(1,2));
        }



        [Theory]
        [MemberData(nameof(DataSources.OddValues), MemberType = typeof(DataSources))]
 
        public void MyFirstTheory(int value,bool expectedResult)
        {
            var actualResult = IsOdd(value);
            // Assert.True(IsOdd(value));
            Assert.Equal(expectedResult, actualResult);
        }

        public int Add(int x,int y)
        {
            return x + y;
        }

        public bool IsOdd(int value)
        {
            return (value % 2 == 1);
        }
    }
}
