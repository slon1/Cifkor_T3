using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using static UnityEditor.Progress;

public class Page3Scr : ScrAbs
{
	private const string ServerUrl = "https://dogapi.dog/api/v2/breeds";
	private IHttpGetJsonService jsonService;
	private ItemView.Factory factory;
	private AsyncTimer timer;
	private int rowCount = 10;
	private List<Data> data;


	[SerializeField]
	private GameObject content;

	private IGUIManager gui;

	[Inject]
	private void Construct(IHttpGetJsonService jsonService, ItemView.Factory factory, IGUIManager gui) {
		this.jsonService = jsonService;
		this.factory = factory;
		this.gui = gui;	
	}
	public override async void Show() {
		base.Show();
		data= await GetNamesAsync();
		var names= data.Select(x => x.attributes.name).Take(rowCount).ToList();
		LoadNames(names);
	}

	private void LoadNames(List<string> names) {
		foreach (var item in names) {
			var itemView=factory.Create();
			itemView.Init(content);
			itemView.SetText(item);
			itemView.OnClick=ItemView_OnClick;
		}
	}

	private void ItemView_OnClick(string str) {
		var id = data.FirstOrDefault(x => x.attributes.name==str).id;
		var id1 = data.FirstOrDefault(x => x.attributes.name == str).attributes.description;
		gui.ShowPanelModal(PanelId.info, true);
		gui.Execute<string>(PanelId.info, PageActionId.SetTitleInfo, str);
		gui.Execute<string>(PanelId.info, PageActionId.SetTextInfo, id1);
	}

	public override void Hide() {
		base.Hide();
	}

	private async UniTask<List<Data>> GetInfoAsync(string id) {
		var result = await jsonService.GetJsonAsync<Root1>(
			Path.Combine(ServerUrl,id),
			() => RaiseEvent(ButtonId.ShowTimer),
			() => RaiseEvent(ButtonId.HideTimer)
		);

		return result.Value.data;

	}

	private async UniTask<List<Data>> GetNamesAsync() {
		var result = await jsonService.GetJsonAsync<Root1>(
			ServerUrl,
			() => RaiseEvent(ButtonId.ShowTimer),
			() => RaiseEvent(ButtonId.HideTimer)
		);		

		return result.Value.data;
		
	}

}
