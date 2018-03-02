using UnityEngine;
using System.Collections;

namespace ScionEngine
{
	public class ScionGraphics 
	{	
		public static void Blit(Material fxMaterial, int passNr) 
		{           			
			GL.PushMatrix ();
			GL.LoadOrtho ();    
			
			fxMaterial.SetPass(passNr);    
			
			GL.Begin (GL.QUADS);
			
			GL.MultiTexCoord2 (0, 0.0f, 0.0f); 
			GL.Vertex3 (0.0f, 0.0f, 0.0f); // BL
			
			GL.MultiTexCoord2 (0, 1.0f, 0.0f); 
			GL.Vertex3 (1.0f, 0.0f, 0.0f); // BR
			
			GL.MultiTexCoord2 (0, 1.0f, 1.0f); 
			GL.Vertex3 (1.0f, 1.0f, 0.0f); // TR
			
			GL.MultiTexCoord2 (0, 0.0f, 1.0f); 
			GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL
			
			GL.End ();
			GL.PopMatrix ();
		}
		public static void Blit(RenderTexture dest, Material fxMaterial, int passNr) 
		{           
			Graphics.SetRenderTarget(dest);
			
			GL.PushMatrix ();
			GL.LoadOrtho ();    
			
			fxMaterial.SetPass(passNr);    
			
			GL.Begin (GL.QUADS);
			
			GL.MultiTexCoord2 (0, 0.0f, 0.0f); 
			GL.Vertex3 (0.0f, 0.0f, 0.0f); // BL
			
			GL.MultiTexCoord2 (0, 1.0f, 0.0f); 
			GL.Vertex3 (1.0f, 0.0f, 0.0f); // BR
			
			GL.MultiTexCoord2 (0, 1.0f, 1.0f); 
			GL.Vertex3 (1.0f, 1.0f, 0.0f); // TR
			
			GL.MultiTexCoord2 (0, 0.0f, 1.0f); 
			GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL
			
			GL.End ();
			GL.PopMatrix ();
		}
	}
}