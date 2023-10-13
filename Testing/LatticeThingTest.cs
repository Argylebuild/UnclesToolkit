using Argyle.UnclesToolkit;
using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;

public class LatticeThingTest : ArgyleComponent
{
	private MeshRenderer rend;
	private Color baseColor = Color.gray;
	private Color HighlightColor = Color.green;
	
	
	
	[Button]
	public async void Highlight()
	{
		if (!rend)
			rend = GetComponent<MeshRenderer>();

		rend.material.color = HighlightColor;
		await UniTask.Delay(2000);
		rend.material.color = baseColor;
	}
	
	
	
}
