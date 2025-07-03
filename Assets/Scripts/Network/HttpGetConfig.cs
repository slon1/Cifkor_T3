using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HttpGetConfig", menuName = "Config/HttpGetConfig")]
public class HttpGetConfig : ScriptableObject {
	public float TimeoutSeconds = 10f;
	public Dictionary<string, string> DefaultHeaders = new();
}
