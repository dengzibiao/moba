#ifndef __EXCG_INCLUDE__
	fixed _Wrap;
	fixed _Intensity;
	
	fixed4 WrapTexture(sampler2D tex,fixed2 uv){
		return tex2D(tex,uv * _Wrap) * _Intensity;
	}
#endif