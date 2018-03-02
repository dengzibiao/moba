using UnityEngine;

public class DemoUI : MonoBehaviour
{
	protected SSAOPro m_SSAOPro;

	void Start()
	{
		m_SSAOPro = GetComponent<SSAOPro>();
	}

	void OnGUI()
	{
		GUI.Box(new Rect(10, 10, 130, 194), "");

		GUI.BeginGroup(new Rect(20, 15, 200, 200));
			m_SSAOPro.enabled = GUILayout.Toggle(m_SSAOPro.enabled, "Enable SSAO");
			m_SSAOPro.DebugAO = GUILayout.Toggle(m_SSAOPro.DebugAO, "Show AO Only");

			bool blur = m_SSAOPro.Blur == SSAOPro.BlurMode.HighQualityBilateral;
			blur = GUILayout.Toggle(blur, "HQ Bilateral Blur");
			m_SSAOPro.Blur = blur ? SSAOPro.BlurMode.HighQualityBilateral : SSAOPro.BlurMode.None;

			GUILayout.Space(10);

			bool quality = m_SSAOPro.Samples == SSAOPro.SampleCount.VeryLow;
			quality = GUILayout.Toggle(quality, "4 samples");
			m_SSAOPro.Samples = quality ? SSAOPro.SampleCount.VeryLow : m_SSAOPro.Samples;

			quality = m_SSAOPro.Samples == SSAOPro.SampleCount.Low;
			quality = GUILayout.Toggle(quality, "8 samples");
			m_SSAOPro.Samples = quality ? SSAOPro.SampleCount.Low : m_SSAOPro.Samples;

			quality = m_SSAOPro.Samples == SSAOPro.SampleCount.Medium;
			quality = GUILayout.Toggle(quality, "12 samples");
			m_SSAOPro.Samples = quality ? SSAOPro.SampleCount.Medium : m_SSAOPro.Samples;

			quality = m_SSAOPro.Samples == SSAOPro.SampleCount.High;
			quality = GUILayout.Toggle(quality, "16 samples");
			m_SSAOPro.Samples = quality ? SSAOPro.SampleCount.High : m_SSAOPro.Samples;

			quality = m_SSAOPro.Samples == SSAOPro.SampleCount.Ultra;
			quality = GUILayout.Toggle(quality, "20 samples");
			m_SSAOPro.Samples = quality ? SSAOPro.SampleCount.Ultra : m_SSAOPro.Samples;
		GUI.EndGroup();
	}
}
