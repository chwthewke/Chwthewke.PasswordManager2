using Autofac;
using Moq;

namespace Chwthewke.PasswordManager.Test.Util
{
    internal class MockModule<T> : Module where T : class
    {
        public Mock<T> Mock { get; private set; }

        public MockModule( Mock<T> mock )
        {
            Mock = mock;
        }

        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterInstance( Mock.Object ).As<T>( );
        }
    }
}