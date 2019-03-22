using Codit.LevelTwo;
using Xunit;

namespace Codit.UnitTest
{
    public class AutoMapperTestFixture
    {
        public AutoMapperTestFixture()
        {
            AutoMapperConfig.Initialize();
        }        
    }

    [CollectionDefinition("AutoMapper")]
    public class AutoMapperCollection : ICollectionFixture<AutoMapperTestFixture>
    {
    }

}
