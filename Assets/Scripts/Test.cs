using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Test : MonoBehaviour
{
    private IHttpGetJsonService jsonService;
	[Inject]
    private void Construct(IHttpGetJsonService jsonService) { 
        this.jsonService = jsonService;
    }
    // Start is called before the first frame update
    async void Start()
    {

		//var result = await jsonService.GetJsonAsync<Root>("https://api.weather.gov/gridpoints/TOP/32,81/forecast", ()=>print(1), ()=>print(2));
		//  print(result.Value.properties.periods.FirstOrDefault(x => x.name.Equals("Today")).temperature);
		var result =  jsonService.GetJsonAsync<Root1>("https://dogapi.dog/api/v2/breeds", ()=>print(1), ()=>print(2));
		result =  jsonService.GetJsonAsync<Root1>("https://dogapi.dog/api/v2/breeds", () => print(3), () => print(4));
		result = jsonService.GetJsonAsync<Root1>("https://dogapi.dog/api/v2/breeds", () => print(13), () => print(14));
		var result1 = await jsonService.GetJsonAsync<Root1>("https://dogapi.dog/api/v2/breeds", () => print(5), () => print(6));
		
		//print(result.Value.properties.periods.FirstOrDefault(x => x.name.Equals("Today")).temperature);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
public class Attributes {
	public string name { get; set; }
	public string description { get; set; }
	public Life life { get; set; }
	public MaleWeight male_weight { get; set; }
	public FemaleWeight female_weight { get; set; }
	public bool hypoallergenic { get; set; }
}

public class Data {
	public string id { get; set; }
	public string type { get; set; }
	public Attributes attributes { get; set; }
	public Relationships relationships { get; set; }
}

public class FemaleWeight {
	public int max { get; set; }
	public int min { get; set; }
}

public class Group {
	public Data data { get; set; }
}

public class Life {
	public int max { get; set; }
	public int min { get; set; }
}

public class Links {
	public string self { get; set; }
	public string current { get; set; }
	public string next { get; set; }
	public string last { get; set; }
}

public class MaleWeight {
	public int max { get; set; }
	public int min { get; set; }
}

public class Meta {
	public Pagination pagination { get; set; }
}

public class Pagination {
	public int current { get; set; }
	public int next { get; set; }
	public int last { get; set; }
	public int records { get; set; }
}

public class Relationships {
	public Group group { get; set; }
}

public class Root1 {
	public List<Data> data { get; set; }
	public Meta meta { get; set; }
	public Links links { get; set; }
}
