using Zenject;

public class TestMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
       // Container.BindInterfacesTo<TestController>().AsSingle();
        TestInstaller.Install(Container);
    }
}