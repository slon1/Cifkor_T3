using System.ComponentModel.Design;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
	[SerializeField] private HttpGetConfig _config;
	[SerializeField] private GameObject itemViewPrefab;
	public override void InstallBindings() {
		// Конфиг
		Container.Bind<HttpGetConfig>().FromInstance(_config).AsSingle();

		// Внутренний сервис (IHttpGetService)
		Container.Bind<IHttpGetService>().To<HttpGetService>().AsSingle();

		// Обёртка с десериализацией
		Container.Bind<IHttpGetJsonService>().To<HttpGetJsonService>().AsSingle();

		Container.BindFactory<ItemView, ItemView.Factory>().FromComponentInNewPrefab(itemViewPrefab);
		Container.Bind<IGUIManager>().FromInstance(GetComponent<GUIManager>()).AsSingle();


		Container.Bind<GameStateModel>().AsSingle().WithArguments(1000);
		Container.BindInterfacesAndSelfTo<GamePresenter>().AsSingle().WithArguments(GetComponent<GameView>());
	}
}