using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
	[SerializeField] private HttpGetConfig _config;
	[SerializeField] private GameObject itemViewPrefab;
	[SerializeField] private int startCoin = 1000;
	
	public override void InstallBindings() {
		
		Container.Bind<HttpGetConfig>().FromInstance(_config).AsSingle();

		
		Container.Bind<IHttpGetService>().To<HttpGetService>().AsSingle();

		
		Container.Bind<IHttpGetJsonService>().To<HttpGetJsonService>().AsSingle();

		Container.BindFactory<ItemView, ItemView.Factory>().FromComponentInNewPrefab(itemViewPrefab);
		Container.Bind<IGUIManager>().FromInstance(GetComponent<GUIManager>()).AsSingle();


		Container.Bind<GameStateModel>().AsSingle().WithArguments(startCoin);
		Container.BindInterfacesAndSelfTo<GamePresenter>().AsSingle().WithArguments(GetComponent<GameView>());


		
	}
}