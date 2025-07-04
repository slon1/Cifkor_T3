using Newtonsoft.Json;
using System;
using System.Collections.Generic;


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

public class DogRoot {
	public List<Data> data { get; set; }
	public Meta meta { get; set; }
	public Links links { get; set; }
}

public class Elevation {
	public string unitCode { get; set; }
	public double value { get; set; }
}

public class Geometry {
	public string type { get; set; }
	public List<List<List<double>>> coordinates { get; set; }
}

public class Period {
	public int number { get; set; }
	public string name { get; set; }
	public DateTime startTime { get; set; }
	public DateTime endTime { get; set; }
	public bool isDaytime { get; set; }
	public int temperature { get; set; }
	public string temperatureUnit { get; set; }
	public string temperatureTrend { get; set; }
	public ProbabilityOfPrecipitation probabilityOfPrecipitation { get; set; }
	public string windSpeed { get; set; }
	public string windDirection { get; set; }
	public string icon { get; set; }
	public string shortForecast { get; set; }
	public string detailedForecast { get; set; }
}

public class ProbabilityOfPrecipitation {
	public string unitCode { get; set; }
	public int value { get; set; }
}

public class Properties {
	public string units { get; set; }
	public string forecastGenerator { get; set; }
	public DateTime generatedAt { get; set; }
	public DateTime updateTime { get; set; }
	public string validTimes { get; set; }
	public Elevation elevation { get; set; }
	public List<Period> periods { get; set; }
}

public class WeatherRoot {
	[JsonProperty("@context")]
	public List<object> context { get; set; }
	public string type { get; set; }
	public Geometry geometry { get; set; }
	public Properties properties { get; set; }
}
