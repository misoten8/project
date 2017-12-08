using UnityEngine;
using System.Collections;


public class Visualizer : MonoBehaviour
{
	private Reaktion.Reaktor spectrum1;
	private Reaktion.Reaktor spectrum2;
	private Reaktion.Reaktor spectrum3;
	private Reaktion.Reaktor spectrum4;
	public Vector4 spectrum;

	private void Start()
	{
		spectrum1 = GameObject.Find("MusicPlayer(Reaktor)/Spectrum 1").GetComponent<Reaktion.Reaktor>();
		spectrum2 = GameObject.Find("MusicPlayer(Reaktor)/Spectrum 2").GetComponent<Reaktion.Reaktor>();
		spectrum3 = GameObject.Find("MusicPlayer(Reaktor)/Spectrum 3").GetComponent<Reaktion.Reaktor>();
		spectrum4 = GameObject.Find("MusicPlayer(Reaktor)/Spectrum 4").GetComponent<Reaktion.Reaktor>();
	}

	void Update()
	{
		spectrum = new Vector4(spectrum1.Output, spectrum2.Output, spectrum3.Output, spectrum4.Output);
	}

	void OnWillRenderObject()
	{
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null) { return; }
		Material mat = GetComponent<Renderer>().material;

		if (Vector4.Dot(spectrum, spectrum) <= 1.0f)
		{
			mat.SetVector("_Spectra", spectrum);
		}
	}
}
