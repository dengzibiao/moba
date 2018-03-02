//#define DEBUG_OPTIMIZATIONS
#define USE_SINGLE_TAP_EARLY_OUT

//Branching is a *lot* slower on OpenGL, so don't do it
#ifndef SHADER_API_OPENGL 
	#define USE_REDUCED_SAMPLES_OPTIMIZATION
#endif //SHADER_API_OPENGL 
		
//If undef will use the same random offset every sample instead of recomputing
#define DOF_RANDOM_OFFSET_PER_SAMPLE
#define SAMPLE_RADIUS_RANDOM_OFFSET

#define SINGLE_TAP_EARLY_OUT_PIXEL_RADIUS 1.0f

#define DOF_SECOND_CIRCLE_START 2.0f
#define DOF_SECOND_CIRCLE_FADE_LENGTH 1.0f
#define DOF_SECOND_CIRCLE_INV_FADE_LENGTH (1.0f/DOF_SECOND_CIRCLE_FADE_LENGTH)

#define DOF_THIRD_CIRCLE_START 4.0f
#define DOF_THIRD_CIRCLE_FADE_LENGTH 1.0f
#define DOF_THIRD_CIRCLE_INV_FADE_LENGTH (1.0f/DOF_THIRD_CIRCLE_FADE_LENGTH)

#define DOF_FOURTH_CIRCLE_START 8.0f
#define DOF_FOURTH_CIRCLE_FADE_LENGTH 2.0f
#define DOF_FOURTH_CIRCLE_INV_FADE_LENGTH (1.0f/DOF_FOURTH_CIRCLE_FADE_LENGTH)

static const float2 ConcentricTaps9[9] = { float2(0.0f, 0.0f), float2(0.4714045f, -0.4714045f), float2(-0.6666666f, 7.853982E-06f), 
float2(0.4714045f, 0.4714045f), float2(-7.838672E-06f, -0.6666666f), float2(-7.896953E-06f, 0.6666666f), float2(-0.4714045f, -0.4714045f), 
float2(0.6666666f, 7.853982E-06f), float2(-0.4714045f, 0.4714045f) };

static const float2 ConcentricTaps25[25] = { float2(0.0f, 0.0f), float2(0.5656855f, -0.5656855f), float2(-0.7391037f, -0.3061467f), float2(-0.8f, 7.853982E-06f), 
float2(-0.7391037f, 0.3061467f), float2(-0.5656855f, 0.5656855f), float2(0.3061468f, -0.7391036f), float2(0.2828427f, -0.2828427f), float2(-0.4f, 7.853982E-06f), 
float2(0.2828427f, 0.2828427f), float2(0.3061467f, 0.7391036f), float2(-7.785161E-06f, -0.8f), float2(-7.850328E-06f, -0.4f), float2(-7.885297E-06f, 0.4f), 
float2(-7.855097E-06f, 0.8f), float2(-0.3061467f, -0.7391036f), float2(-0.2828427f, -0.2828427f), float2(0.4f, 7.853982E-06f), float2(-0.2828427f, 0.2828427f), 
float2(-0.3061468f, 0.7391036f), float2(-0.5656854f, -0.5656855f), float2(0.7391036f, -0.3061467f), float2(0.8f, 7.853982E-06f), float2(0.7391036f, 0.3061467f), 
float2(-0.5656854f, 0.5656854f) }; 

//static const float2 ConcentricTaps49[49] = { float2(0.0f, 0.0f), float2(0.6060915f, -0.6060915f), float2(-0.7423075f, -0.4285714f), float2(-0.8279364f, -0.2218449f), 
//float2(-0.8571429f, 7.853982E-06f), float2(-0.8279364f, 0.2218449f), float2(-0.7423075f, 0.4285714f), float2(-0.6060916f, 0.6060915f), float2(0.4285715f, -0.7423075f), 
//float2(0.404061f, -0.404061f), float2(-0.5279311f, -0.2186763f), float2(-0.5714285f, 7.853982E-06f), float2(-0.5279311f, 0.2186763f), float2(0.404061f, 0.404061f), 
//float2(0.4285714f, 0.7423075f), float2(0.2218449f, -0.8279364f), float2(0.2186763f, -0.5279311f), float2(0.2020305f, -0.2020305f), float2(-0.2857143f, 7.853982E-06f), 
//float2(0.2020305f, 0.2020305f), float2(0.2186762f, 0.5279311f), float2(0.2218449f, 0.8279364f), float2(-7.830346E-06f, -0.8571429f), float2(-7.808775E-06f, -0.5714285f), 
//float2(-7.855323E-06f, -0.2857143f), float2(-7.880301E-06f, 0.2857143f), float2(-7.858731E-06f, 0.5714285f), float2(-7.90528E-06f, 0.8571428f), float2(-0.2218448f, -0.8279365f), 
//float2(-0.2186762f, -0.5279311f), float2(-0.2020305f, -0.2020305f), float2(0.2857143f, 7.853982E-06f), float2(-0.2020305f, 0.2020305f), float2(-0.2186763f, 0.5279311f), 
//float2(-0.221845f, 0.8279364f), float2(-0.4285714f, -0.7423075f), float2(-0.404061f, -0.404061f), float2(0.5279311f, -0.2186763f), float2(0.5714285f, 7.853982E-06f), 
//float2(0.5279311f, 0.2186763f), float2(-0.404061f, 0.404061f), float2(-0.4285715f, 0.7423074f), float2(-0.6060915f, -0.6060916f), float2(0.7423074f, -0.4285714f), 
//float2(0.8279364f, -0.2218449f), float2(0.8571428f, 7.853982E-06f), float2(0.8279364f, 0.2218449f), float2(0.7423074f, 0.4285714f), float2(-0.6060915f, 0.6060915f) }; 

static const float2 ConcentricTaps49[49] = { float2(0.0f, 0.0f), float2(0.2020305f, -0.2020305f), float2(-0.2857143f, 7.853982E-06f), float2(0.2020305f, 0.2020305f), 
float2(-7.855323E-06f, -0.2857143f), float2(-7.880301E-06f, 0.2857143f), float2(-0.2020305f, -0.2020305f), float2(0.2857143f, 7.853982E-06f), float2(-0.2020305f, 0.2020305f), 
float2(0.404061f, -0.404061f), float2(-0.5279311f, -0.2186763f), float2(-0.5714285f, 7.853982E-06f), float2(-0.5279311f, 0.2186763f), float2(0.404061f, 0.404061f), 
float2(0.2186763f, -0.5279311f), float2(0.2186762f, 0.5279311f), float2(-7.808775E-06f, -0.5714285f), float2(-7.858731E-06f, 0.5714285f), float2(-0.2186762f, -0.5279311f), 
float2(-0.2186763f, 0.5279311f), float2(-0.404061f, -0.404061f), float2(0.5279311f, -0.2186763f), float2(0.5714285f, 7.853982E-06f), float2(0.5279311f, 0.2186763f), 
float2(-0.404061f, 0.404061f), float2(0.6060915f, -0.6060915f), float2(-0.7423075f, -0.4285714f), float2(-0.8279364f, -0.2218449f), float2(-0.8571429f, 7.853982E-06f), 
float2(-0.8279364f, 0.2218449f), float2(-0.7423075f, 0.4285714f), float2(-0.6060916f, 0.6060915f), float2(0.4285715f, -0.7423075f), float2(0.4285714f, 0.7423075f), 
float2(0.2218449f, -0.8279364f), float2(0.2218449f, 0.8279364f), float2(-7.830346E-06f, -0.8571429f), float2(-7.90528E-06f, 0.8571428f), float2(-0.2218448f, -0.8279365f), 
float2(-0.221845f, 0.8279364f), float2(-0.4285714f, -0.7423075f), float2(-0.4285715f, 0.7423074f), float2(-0.6060915f, -0.6060916f), float2(0.7423074f, -0.4285714f), 
float2(0.8279364f, -0.2218449f), float2(0.8571428f, 7.853982E-06f), float2(0.8279364f, 0.2218449f), float2(0.7423074f, 0.4285714f), float2(-0.6060915f, 0.6060915f) }; 

//static const float2 ConcentricTaps81[81] = { float2(0.0f, 0.0f), float2(0.6285394f, -0.6285394f), float2(-0.7390841f, -0.4938402f), float2(-0.8212263f, -0.3401631f), 
//float2(-0.8718092f, -0.1734136f), float2(-0.8888889f, 7.853982E-06f), float2(-0.8718092f, 0.1734136f), float2(-0.8212263f, 0.3401631f), float2(-0.7390841f, 0.4938402f), 
//float2(-0.6285394f, 0.6285394f), float2(0.4938402f, -0.7390841f), float2(0.4714045f, -0.4714045f), float2(-0.5773502f, -0.3333333f), float2(-0.6439505f, -0.172546f), 
//float2(-0.6666666f, 7.853982E-06f), float2(-0.6439505f, 0.172546f), float2(-0.5773502f, 0.3333333f), float2(0.4714045f, 0.4714045f), float2(0.4938401f, 0.7390841f), 
//float2(0.3401631f, -0.8212262f), float2(0.3333333f, -0.5773502f), float2(0.3142697f, -0.3142697f), float2(-0.4106131f, -0.1700815f), float2(-0.4444444f, 7.853982E-06f), 
//float2(-0.4106131f, 0.1700815f), float2(0.3142697f, 0.3142697f), float2(0.3333333f, 0.5773503f), float2(0.340163f, 0.8212262f), float2(0.1734136f, -0.8718091f), 
//float2(0.1725461f, -0.6439505f), float2(0.1700816f, -0.4106131f), float2(0.1571348f, -0.1571348f), float2(-0.2222222f, 7.853982E-06f), float2(0.1571348f, 0.1571348f), 
//float2(0.1700815f, 0.4106131f), float2(0.172546f, 0.6439505f), float2(0.1734135f, 0.8718091f), float2(-7.802468E-06f, -0.8888889f), float2(-7.838672E-06f, -0.6666666f),
//float2(-7.821894E-06f, -0.4444444f), float2(-7.831608E-06f, -0.2222222f), float2(-7.851035E-06f, 0.2222222f), float2(-7.860749E-06f, 0.4444444f), 
//float2(-7.896953E-06f, 0.6666666f), float2(-7.880176E-06f, 0.8888888f), float2(-0.1734135f, -0.8718092f), float2(-0.172546f, -0.6439505f), float2(-0.1700815f, -0.4106131f), 
//float2(-0.1571348f, -0.1571348f), float2(0.2222222f, 7.853982E-06f), float2(-0.1571348f, 0.1571348f), float2(-0.1700816f, 0.4106131f), float2(-0.1725461f, 0.6439505f), 
//float2(-0.1734136f, 0.8718091f), float2(-0.3401631f, -0.8212262f), float2(-0.3333333f, -0.5773503f), float2(-0.3142697f, -0.3142697f), float2(0.4106131f, -0.1700815f), 
//float2(0.4444444f, 7.853982E-06f), float2(0.4106131f, 0.1700815f), float2(-0.3142697f, 0.3142697f), float2(-0.3333333f, 0.5773502f), float2(-0.3401631f, 0.8212262f),
//float2(-0.4938402f, -0.7390841f), float2(-0.4714045f, -0.4714045f), float2(0.5773502f, -0.3333333f), float2(0.6439505f, -0.172546f), float2(0.6666666f, 7.853982E-06f), 
//float2(0.6439505f, 0.172546f), float2(0.5773502f, 0.3333333f), float2(-0.4714045f, 0.4714045f), float2(-0.4938403f, 0.739084f), float2(-0.6285393f, -0.6285394f), 
//float2(0.7390841f, -0.4938402f), float2(0.8212262f, -0.3401631f), float2(0.8718091f, -0.1734136f), float2(0.8888888f, 7.853982E-06f), float2(0.8718091f, 0.1734136f), 
//float2(0.8212262f, 0.3401631f), float2(0.7390841f, 0.4938402f), float2(-0.6285393f, 0.6285393f) }; 

static const float2 ConcentricTaps81[81] = { float2(0.0f, 0.0f), float2(0.1571348f, -0.1571348f), float2(-0.2222222f, 7.853982E-06f), float2(0.1571348f, 0.1571348f), 
float2(-7.831608E-06f, -0.2222222f), float2(-7.851035E-06f, 0.2222222f), float2(-0.1571348f, -0.1571348f), float2(0.2222222f, 7.853982E-06f), float2(-0.1571348f, 0.1571348f), 
float2(0.3142697f, -0.3142697f), float2(-0.4106131f, -0.1700815f), float2(-0.4444444f, 7.853982E-06f), float2(-0.4106131f, 0.1700815f), float2(0.3142697f, 0.3142697f), 
float2(0.1700816f, -0.4106131f), float2(0.1700815f, 0.4106131f), float2(-7.821894E-06f, -0.4444444f), float2(-7.860749E-06f, 0.4444444f), float2(-0.1700815f, -0.4106131f), 
float2(-0.1700816f, 0.4106131f), float2(-0.3142697f, -0.3142697f), float2(0.4106131f, -0.1700815f), float2(0.4444444f, 7.853982E-06f), float2(0.4106131f, 0.1700815f), 
float2(-0.3142697f, 0.3142697f), float2(0.4714045f, -0.4714045f), float2(-0.5773502f, -0.3333333f), float2(-0.6439505f, -0.172546f), float2(-0.6666666f, 7.853982E-06f), 
float2(-0.6439505f, 0.172546f), float2(-0.5773502f, 0.3333333f), float2(0.4714045f, 0.4714045f), float2(0.3333333f, -0.5773502f), float2(0.3333333f, 0.5773503f), 
float2(0.1725461f, -0.6439505f), float2(0.172546f, 0.6439505f), float2(-7.838672E-06f, -0.6666666f), float2(-7.896953E-06f, 0.6666666f), float2(-0.172546f, -0.6439505f), 
float2(-0.1725461f, 0.6439505f), float2(-0.3333333f, -0.5773503f), float2(-0.3333333f, 0.5773502f), float2(-0.4714045f, -0.4714045f), float2(0.5773502f, -0.3333333f), 
float2(0.6439505f, -0.172546f), float2(0.6666666f, 7.853982E-06f), float2(0.6439505f, 0.172546f), float2(0.5773502f, 0.3333333f), float2(-0.4714045f, 0.4714045f), 
float2(0.6285394f, -0.6285394f), float2(-0.7390841f, -0.4938402f), float2(-0.8212263f, -0.3401631f), float2(-0.8718092f, -0.1734136f), float2(-0.8888889f, 7.853982E-06f), 
float2(-0.8718092f, 0.1734136f), float2(-0.8212263f, 0.3401631f), float2(-0.7390841f, 0.4938402f), float2(-0.6285394f, 0.6285394f), float2(0.4938402f, -0.7390841f), 
float2(0.4938401f, 0.7390841f), float2(0.3401631f, -0.8212262f), float2(0.340163f, 0.8212262f), float2(0.1734136f, -0.8718091f), float2(0.1734135f, 0.8718091f), 
float2(-7.802468E-06f, -0.8888889f), float2(-7.880176E-06f, 0.8888888f), float2(-0.1734135f, -0.8718092f), float2(-0.1734136f, 0.8718091f), float2(-0.3401631f, -0.8212262f), 
float2(-0.3401631f, 0.8212262f), float2(-0.4938402f, -0.7390841f), float2(-0.4938403f, 0.739084f), float2(-0.6285393f, -0.6285394f), float2(0.7390841f, -0.4938402f), 
float2(0.8212262f, -0.3401631f), float2(0.8718091f, -0.1734136f), float2(0.8888888f, 7.853982E-06f), float2(0.8718091f, 0.1734136f), float2(0.8212262f, 0.3401631f), 
float2(0.7390841f, 0.4938402f), float2(-0.6285393f, 0.6285393f) }; 

static const float2 ConcentricTaps169[169] = { float2(0.0f, 0.0f), float2(0.652714f, -0.652714f), float2(-0.7323261f, -0.5619337f), float2(-0.7994081f, -0.4615384f), 
float2(-0.8528119f, -0.3532462f), float2(-0.8916239f, -0.2389099f), float2(-0.9151799f, -0.1204857f), float2(-0.9230769f, 7.853982E-06f), float2(-0.9151799f, 0.1204857f), 
float2(-0.8916239f, 0.2389099f), float2(-0.8528119f, 0.3532462f), float2(-0.7994081f, 0.4615384f), float2(-0.7323261f, 0.5619336f), float2(-0.652714f, 0.652714f), 
float2(0.5619336f, -0.7323261f), float2(0.5439283f, -0.5439283f), float2(-0.6223208f, -0.4521425f), float2(-0.6853896f, -0.3492235f), float2(-0.731582f, -0.2377054f), 
float2(-0.7597603f, -0.1203342f), float2(-0.7692308f, 7.853982E-06f), float2(-0.7597603f, 0.1203342f), float2(-0.731582f, 0.2377054f), float2(-0.6853896f, 0.3492235f), 
float2(-0.6223208f, 0.4521425f), float2(-0.5439283f, 0.5439283f), float2(0.5619336f, 0.7323261f), float2(0.4615385f, -0.7994081f), float2(0.4521424f, -0.6223208f), 
float2(0.4351426f, -0.4351426f), float2(-0.5116736f, -0.3418894f), float2(-0.5685412f, -0.2354975f), float2(-0.6035601f, -0.1200556f), float2(-0.6153846f, 7.853982E-06f), 
float2(-0.6035601f, 0.1200556f), float2(-0.5685412f, 0.2354975f), float2(-0.5116736f, 0.3418894f), float2(0.4351426f, 0.4351426f), float2(0.4521425f, 0.6223207f), 
float2(0.4615384f, 0.7994081f), float2(0.3532463f, -0.8528119f), float2(0.3492236f, -0.6853896f), float2(0.3418894f, -0.5116735f), float2(0.3263569f, -0.3263569f), 
float2(-0.399704f, -0.2307692f), float2(-0.4458119f, -0.1194549f), float2(-0.4615384f, 7.853982E-06f), float2(-0.4458119f, 0.1194549f), float2(-0.399704f, 0.2307692f), 
float2(0.3263569f, 0.3263569f), float2(0.3418893f, 0.5116736f), float2(0.3492235f, 0.6853896f), float2(0.3532462f, 0.8528118f), float2(0.2389099f, -0.8916239f), 
float2(0.2377054f, -0.7315819f), float2(0.2354975f, -0.5685412f), float2(0.2307692f, -0.399704f), float2(0.2175713f, -0.2175713f), float2(-0.2842706f, -0.1177488f), 
float2(-0.3076923f, 7.853982E-06f), float2(-0.2842706f, 0.1177488f), float2(0.2175713f, 0.2175713f), float2(0.2307692f, 0.399704f), float2(0.2354975f, 0.5685412f), 
float2(0.2377053f, 0.7315819f), float2(0.2389099f, 0.8916238f), float2(0.1204857f, -0.9151799f), float2(0.1203342f, -0.7597603f), float2(0.1200556f, -0.6035601f), 
float2(0.119455f, -0.4458119f), float2(0.1177488f, -0.2842706f), float2(0.1087857f, -0.1087857f), float2(-0.1538461f, 7.853982E-06f), float2(0.1087857f, 0.1087857f), 
float2(0.1177487f, 0.2842706f), float2(0.119455f, 0.4458119f), float2(0.1200555f, 0.6035601f), float2(0.1203341f, 0.7597603f), float2(0.1204856f, 0.9151798f), 
float2(-7.772444E-06f, -0.9230769f), float2(-7.852529E-06f, -0.7692308f), float2(-7.822574E-06f, -0.6153846f), float2(-7.847638E-06f, -0.4615384f), 
float2(-7.836023E-06f, -0.3076923f), float2(-7.842748E-06f, -0.1538461f), float2(-7.856197E-06f, 0.1538461f), float2(-7.862923E-06f, 0.3076923f), 
float2(-7.887988E-06f, 0.4615384f), float2(-7.876372E-06f, 0.6153846f), float2(-7.919776E-06f, 0.7692307f), float2(-7.853142E-06f, 0.9230769f), 
float2(-0.1204857f, -0.9151799f), float2(-0.1203341f, -0.7597603f), float2(-0.1200555f, -0.6035601f), float2(-0.119455f, -0.4458119f), float2(-0.1177487f, -0.2842706f), 
float2(-0.1087857f, -0.1087857f), float2(0.1538461f, 7.853982E-06f), float2(-0.1087857f, 0.1087857f), float2(-0.1177488f, 0.2842706f), float2(-0.119455f, 0.4458119f), 
float2(-0.1200556f, 0.6035601f), float2(-0.1203342f, 0.7597603f), float2(-0.1204857f, 0.9151798f), float2(-0.2389098f, -0.8916239f), float2(-0.2377054f, -0.731582f), 
float2(-0.2354975f, -0.5685412f), float2(-0.2307692f, -0.399704f), float2(-0.2175713f, -0.2175713f), float2(0.2842706f, -0.1177488f), float2(0.3076923f, 7.853982E-06f), 
float2(0.2842706f, 0.1177488f), float2(-0.2175713f, 0.2175713f), float2(-0.2307692f, 0.399704f), float2(-0.2354975f, 0.5685412f), float2(-0.2377054f, 0.7315819f), 
float2(-0.23891f, 0.8916238f), float2(-0.3532462f, -0.8528119f), float2(-0.3492234f, -0.6853897f), float2(-0.3418893f, -0.5116736f), float2(-0.3263569f, -0.3263569f), 
float2(0.399704f, -0.2307692f), float2(0.4458119f, -0.1194549f), float2(0.4615384f, 7.853982E-06f), float2(0.4458119f, 0.1194549f), float2(0.399704f, 0.2307692f), 
float2(-0.3263569f, 0.3263569f), float2(-0.3418894f, 0.5116735f), float2(-0.3492235f, 0.6853896f), float2(-0.3532463f, 0.8528118f), float2(-0.4615384f, -0.7994081f), 
float2(-0.4521424f, -0.6223208f), float2(-0.4351426f, -0.4351426f), float2(0.5116736f, -0.3418894f), float2(0.5685412f, -0.2354975f), float2(0.6035601f, -0.1200556f), 
float2(0.6153846f, 7.853982E-06f), float2(0.6035601f, 0.1200556f), float2(0.5685412f, 0.2354975f), float2(0.5116736f, 0.3418894f), float2(-0.4351426f, 0.4351426f), 
float2(-0.4521424f, 0.6223207f), float2(-0.4615385f, 0.799408f), float2(-0.5619336f, -0.7323262f), float2(-0.5439283f, -0.5439283f), float2(0.6223207f, -0.4521425f), 
float2(0.6853896f, -0.3492235f), float2(0.7315819f, -0.2377054f), float2(0.7597603f, -0.1203342f), float2(0.7692307f, 7.853982E-06f), float2(0.7597603f, 0.1203342f), 
float2(0.7315819f, 0.2377054f), float2(0.6853896f, 0.3492235f), float2(0.6223207f, 0.4521425f), float2(-0.5439283f, 0.5439283f), float2(-0.5619336f, 0.7323261f), 
float2(-0.6527139f, -0.652714f), float2(0.7323261f, -0.5619336f), float2(0.799408f, -0.4615384f), float2(0.8528118f, -0.3532462f), float2(0.8916238f, -0.2389099f), 
float2(0.9151798f, -0.1204857f), float2(0.9230769f, 7.853982E-06f), float2(0.9151798f, 0.1204857f), float2(0.8916238f, 0.2389099f), float2(0.8528118f, 0.3532462f), 
float2(0.799408f, 0.4615384f), float2(0.7323261f, 0.5619336f), float2(-0.6527139f, 0.6527139f) }; 

static const float2 ConcentricTapsPentagon49[49] = { float2(0.0f, 0.0f), float2(0.5861275f, -0.5783641f), float2(-0.729998f, -0.4279785f), float2(-0.7979411f, -0.2195195f), 
float2(-0.8215557f, -0.005469591f), float2(-0.800225f, 0.2087119f), float2(-0.7345075f, 0.4175641f), float2(-0.5988626f, 0.5909305f), float2(0.4228533f, -0.7212558f), 
float2(0.3907517f, -0.3855761f), float2(-0.5129526f, -0.2164895f), float2(-0.5477039f, -0.003643885f), float2(-0.515224f, 0.2093997f), float2(0.3934186f, 0.3986995f), 
float2(0.4077172f, 0.7171866f), float2(0.2259254f, -0.8212264f), float2(0.2196911f, -0.5205384f), float2(0.1953758f, -0.192788f), float2(-0.2738519f, -0.001818179f), 
float2(0.1967093f, 0.1993498f), float2(0.2063294f, 0.5076698f), float2(0.2074152f, 0.795253f), float2(0.005527173f, -0.8302022f), float2(0.003682274f, -0.553468f), 
float2(0.001837308f, -0.2767337f), float2(-0.001852556f, 0.2767347f), float2(-0.003697456f, 0.5534689f), float2(-0.005542421f, 0.830203f), float2(-0.2074151f, -0.7952532f), 
float2(-0.2063294f, -0.5076698f), float2(-0.1967093f, -0.1993498f), float2(0.2738518f, 0.001833234f), float2(-0.1953758f, 0.192788f), float2(-0.2196911f, 0.5205384f), 
float2(-0.2259255f, 0.8212262f), float2(-0.4077172f, -0.7171866f), float2(-0.3934186f, -0.3986995f), float2(0.515224f, -0.2093997f), float2(0.5477037f, 0.003658941f), 
float2(0.5129526f, 0.2164895f), float2(-0.3907517f, 0.3855761f), float2(-0.4228533f, 0.7212557f), float2(-0.590128f, -0.5980493f), float2(0.7345074f, -0.4175641f), 
float2(0.800225f, -0.2087119f), float2(0.8215557f, 0.005484648f), float2(0.7979411f, 0.2195195f), float2(0.7299979f, 0.4279785f), float2(-0.5861275f, 0.5783641f) }; 
	
//background, center, foreground
float3 DepthWeights(float sampleDepth, float centerDepth)
{
	float3 depthWeights = 0.0f;		
	depthWeights.y = 1.0f - saturate(abs(sampleDepth - centerDepth));
	depthWeights.xz = sampleDepth > centerDepth ? float2(1.0f - depthWeights.y, 0.0f) : float2(0.0f, 1.0f - depthWeights.y);
	return depthWeights;
	
	depthWeights.y = saturate(centerDepth - sampleDepth + 3.0f) * saturate(abs(centerDepth - sampleDepth + 3.0f));
	depthWeights.xz = sampleDepth > centerDepth ? float2(1.0f - depthWeights.y, 0.0f) : float2(0.0f, 1.0f - depthWeights.y);
	return depthWeights;
}

float2 ShiftUV(float2 uv, float iter)
{
	return uv * HalfResSize * (13.0f * iter + 1.0f);
}

void DepthOfFieldTapIteration(int tapIteration, float centerDepth, float2 inputUV, float2 sampleOffset, float2 randomOffset, float sampleMultiplier,
								inout float4 backgroundValue, inout float4 centerValue, inout float4 foregroundValue, float focalDistance)
{				
	//First iteration should have no offsets
	float isNotFirstIter = saturate((float)tapIteration);
	sampleOffset *= isNotFirstIter;
	randomOffset *= isNotFirstIter; 
				
	#if 1
	float2 uv = inputUV + (sampleOffset+randomOffset) * InvHalfResSize; 
	#else
	sampleOffset = sampleOffset + randomOffset;
	float2 uv = inputUV + sampleOffset * InvHalfResSize;
	#endif
	
	float4 sampleLighting 	= float4(tex2Dlod(_HalfResSourceTexture, float4(uv, 0.0f, 0.0f)).xyz, 1.0f);
	float sampleDepth 		= DecodeDepth01(tex2Dlod(_HalfResDepthTexture, float4(uv, 0.0f, 0.0f)).x);		
	
	float sampleCoC 		= CoCFromDepth(sampleDepth, focalDistance);	
	float invCircleArea 	= InvCircleAreaClamped(sampleCoC);	
	
	//Intersection test
	float dist 				= sqrt(dot(sampleOffset, sampleOffset) + 1e-4f);
	float intersection		= saturate((sampleCoC - dist) * 2.0f);
	//intersection = dist > sampleCoC ? 0.0f : 1.0f;

	float3 depthWeights 	= DepthWeights(sampleDepth, centerDepth);			
	float weightMultiplier	= sampleMultiplier * intersection * invCircleArea;
	depthWeights 			= depthWeights * weightMultiplier;	
	
	//Assumes the first sample is offset by [0,0]
	depthWeights = lerp(float3(0.0f, invCircleArea, 0.0f), depthWeights, isNotFirstIter);
	
	float isForeground 			= saturate(focalDistance - sampleDepth);
	float isForegroundFactor 	= 1.0f + isForeground;	
	
	float4 backgroundAddition 	= sampleLighting * depthWeights.x;
	float4 centerAddition 		= sampleLighting * depthWeights.y * isForegroundFactor;
	float4 foregroundAddition 	= sampleLighting * depthWeights.z * isForegroundFactor;
	
	#ifdef SC_DOF_MASK_ON
	//Compiler should flatten this
	if (tapIteration != 0)
	{
		float sampleValidity = tex2Dlod(_ExclusionMask, float4(uv, 0.0f, 0.0f)).x;	
		backgroundAddition 	*= sampleValidity;
		centerAddition 		*= sampleValidity;
		foregroundAddition	*= sampleValidity;
	}
	#endif
		
	backgroundValue 	+= backgroundAddition;
	centerValue 		+= centerAddition;
	foregroundValue 	+= foregroundAddition;
}

//Returns foreground alpha as .w component
float4 CombineDepthOfFieldLayers(float4 backgroundValue, float4 centerValue, float4 foregroundValue, float centerDepth, float focalDistance)
{
	float3 result = 0;
	float weightAccum = 1e-5f;
	
	float backgroundValidity = saturate(centerDepth - focalDistance);
	backgroundValue *= backgroundValidity;
	
	foregroundValue.w = foregroundValue.w + 1e-7f;
	centerValue.w = centerValue.w + 1e-7f;
	backgroundValue.w = backgroundValue.w + 1e-7f;
	
	float newFore = min(foregroundValue.w, 1.0f);
	foregroundValue.xyz = foregroundValue.xyz * (newFore / foregroundValue.w);
	foregroundValue.w = newFore;
	weightAccum += foregroundValue.w;
	result += foregroundValue.xyz;
	
	float newCenter = min(centerValue.w, 1.0f - weightAccum);
	centerValue.xyz = centerValue.xyz * (newCenter / centerValue.w);
	centerValue.w = newCenter;
	weightAccum += centerValue.w;
	result += centerValue.xyz;
	
	float newBack = min(backgroundValue.w, 1.0f - weightAccum);
	backgroundValue.xyz = backgroundValue.xyz * (newBack / backgroundValue.w);
	backgroundValue.w = newBack;
	weightAccum += backgroundValue.w;
	result += backgroundValue.xyz;
	
	//return float4(result / weightAccum, foregroundValue.w);
	return float4(result / weightAccum, foregroundValue.w);
}	

void NormalizeDiscreteSampling(float numSamples, float searchRadius, inout float4 backgroundValue, inout float4 centerValue, inout float4 foregroundValue)
{
	float weightNormalizer = (searchRadius*searchRadius*PI + 1e-4f) / numSamples;	
	
	backgroundValue = backgroundValue * weightNormalizer;
	centerValue 	= centerValue * weightNormalizer;
	foregroundValue = foregroundValue * weightNormalizer;
}

float DepthOfFieldAlpha(float foregroundAlpha, float centerDepth, float centerCoC, float focalDistance)
{		
	float alpha = saturate(centerCoC*0.5f + foregroundAlpha*2.0f);
	return alpha;
}					
						
void DepthOfFieldTaps(float2 inputUV, float searchRadius, float centerDepth, float2 offsetArray[TAPS_AMOUNT], float offsetScale, int iterStart, int iterEnd,
						inout float4 backgroundValue, inout float4 centerValue, inout float4 foregroundValue, float sampleMultiplier, float focalDistance)
{				
	float offsetMultiplier = searchRadius * offsetScale;
	
	#ifndef DOF_RANDOM_OFFSET_PER_SAMPLE
	float2 randomOffset = RandomOffset(ShiftUV(inputUV, 0.0f));
	#endif
	
	for (int k = iterStart; k <= iterEnd; k++)  
	{
		#ifdef DOF_RANDOM_OFFSET_PER_SAMPLE
		float2 randomOffset = RandomOffset(ShiftUV(inputUV, k));
		#endif
		
		#ifdef SAMPLE_RADIUS_RANDOM_OFFSET
			#if (SHADER_API_D3D11 || SHADER_API_XBOXONE)
			float kSeed = k * 634.0f;
			float randomValue01 = frac(abs(sin(inputUV.x + inputUV.y * 541.17f + kSeed) * 273351.5f - kSeed));
			#else
			float randomValue01 = (randomOffset.x + randomOffset.y) * 0.25f + 0.5f; 
			#endif 	
		
		float offsetMult = 0.92f + randomValue01 * 0.16f;
		float2 sampleOffset = offsetArray[k] * offsetMultiplier * offsetMult;	
		#else
		float2 sampleOffset = offsetArray[k] * offsetMultiplier;	
		#endif 	
														
		DepthOfFieldTapIteration(k, centerDepth, inputUV, sampleOffset, randomOffset, sampleMultiplier, backgroundValue, centerValue, foregroundValue, focalDistance);
	}
}
											
float4 BlurTapPass(v2f i) : SV_Target0
{		
	float4 tiledNeighbourhood = tex2Dlod(_TiledNeighbourhoodData, float4(i.uv, 0.0f, 0.0f));
	float searchRadius = tiledNeighbourhood.x; //Neighbourhood max CoC
	float centerDepth = DecodeDepth01(tex2Dlod(_HalfResDepthTexture, float4(i.uv, 0.0f, 0.0f)).x);
	float focalDistance = GetFocalDistance();
	float centerCoC = CoCFromDepth(centerDepth, focalDistance);
	
	//return abs(centerDepth-focalDistance).xxxx;
	//return abs(focalDistance - centerDepth).xxxx;
	//return centerCoC.xxxx;
	//return float4(tex2Dlod(_HalfResSourceTexture, float4(i.uv, 0.0f, 0.0f)).xyz, 1.0f);
	//return tiledNeighbourhood;
	
	#ifdef SC_DOF_MASK_ON	
	//This is either 0 or 1, depending on if the object is excluded or not
	float sampleValidity = tex2Dlod(_ExclusionMask, float4(i.uv, 0.0f, 0.0f)).x;
	//return sampleValidity > 0.5f ? float4(0,1,0,1) : float4(1,0,0,1);
	searchRadius = searchRadius * sampleValidity;
	#endif
	
	//return float4(1,0,0,1);
	
	#ifdef USE_SINGLE_TAP_EARLY_OUT
	SCION_BRANCH if (searchRadius < SINGLE_TAP_EARLY_OUT_PIXEL_RADIUS)
	{
		float alpha = DepthOfFieldAlpha(0.0f, centerDepth, centerCoC, focalDistance);
	 	float3 clr = tex2Dlod(_HalfResSourceTexture, float4(i.uv, 0.0f, 0.0f)).xyz;
		#ifdef DEBUG_OPTIMIZATIONS
	 	clr = clr * float3(1.0f,0.3f,0.3f);
		#endif
		return float4(clr, alpha);
	}
	#endif
		
	float4 backgroundValue = 0.0f, centerValue = 0.0f, foregroundValue = 0.0f;	
	float4 depthOfFieldTaps;
	
	#ifdef USE_REDUCED_SAMPLES_OPTIMIZATION
	SCION_BRANCH if (searchRadius < DOF_SECOND_CIRCLE_START)	//1 circle
	{
		//First circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, SAMPLING_OFFSET_MULT_1, 0, 8,
					backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);						
	
		float samplesWeight = 9.0f;
		NormalizeDiscreteSampling(samplesWeight, searchRadius, backgroundValue, centerValue, foregroundValue);	
		depthOfFieldTaps = CombineDepthOfFieldLayers(backgroundValue, centerValue, foregroundValue, centerDepth, focalDistance);	
		
		#ifdef DEBUG_OPTIMIZATIONS
	 	depthOfFieldTaps.xyz = depthOfFieldTaps.xyz * float3(0.3f,1.0f,0.3f); //Green
		#endif	
	}
	else if (searchRadius < DOF_THIRD_CIRCLE_START)				//2 circles
	{
		float searchScaleFactor = saturate((searchRadius - DOF_SECOND_CIRCLE_START) * DOF_SECOND_CIRCLE_INV_FADE_LENGTH);
		float offsetMultiplier = lerp(SAMPLING_OFFSET_MULT_1, SAMPLING_OFFSET_MULT_2, searchScaleFactor);
		
		//First circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, offsetMultiplier, 0, 8,
					backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);	
		//Second circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, SAMPLING_OFFSET_MULT_2, 9, 24,
						backgroundValue, centerValue, foregroundValue, searchScaleFactor, focalDistance);		
		
		float samplesWeight = 9.0f + 16.0f * searchScaleFactor;		
		NormalizeDiscreteSampling(samplesWeight, searchRadius, backgroundValue, centerValue, foregroundValue);	
		depthOfFieldTaps = CombineDepthOfFieldLayers(backgroundValue, centerValue, foregroundValue, centerDepth, focalDistance);	
		
		#ifdef DEBUG_OPTIMIZATIONS
	 	depthOfFieldTaps.xyz = depthOfFieldTaps.xyz * float3(0.3f,1.0f,1.0f); //Teal
		#endif
	}
	#ifdef SC_DOF_QUALITY_MAX
	else if (searchRadius < DOF_FOURTH_CIRCLE_START)		//3 circles
	#else
	else
	#endif
	{
		float searchScaleFactor = saturate((searchRadius - DOF_THIRD_CIRCLE_START) * DOF_THIRD_CIRCLE_INV_FADE_LENGTH);
		float offsetMultiplier = lerp(SAMPLING_OFFSET_MULT_2, SAMPLING_OFFSET_MULT_3, searchScaleFactor);			
		
		//First circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, offsetMultiplier, 0, 8,
					backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);	
		//Second circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, offsetMultiplier, 9, 24,
						backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);
		//Third circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, SAMPLING_OFFSET_MULT_3, 25, 48,
						backgroundValue, centerValue, foregroundValue, searchScaleFactor, focalDistance);	
		
		float samplesWeight = 25.0f + 24.0f * searchScaleFactor;	
		NormalizeDiscreteSampling(samplesWeight, searchRadius, backgroundValue, centerValue, foregroundValue);	
		depthOfFieldTaps = CombineDepthOfFieldLayers(backgroundValue, centerValue, foregroundValue, centerDepth, focalDistance);
		
		#ifdef DEBUG_OPTIMIZATIONS
	 	depthOfFieldTaps.xyz = depthOfFieldTaps.xyz * float3(1.0f,1.0f,0.3f); //Yellow
		#endif	
	}
	#ifdef SC_DOF_QUALITY_MAX
	else //4 circles
	{
		float searchScaleFactor = saturate((searchRadius - DOF_FOURTH_CIRCLE_START) * DOF_FOURTH_CIRCLE_INV_FADE_LENGTH);
		float offsetMultiplier = lerp(SAMPLING_OFFSET_MULT_3, SAMPLING_OFFSET_MULT_4, searchScaleFactor);			
		
		//First circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, offsetMultiplier, 0, 8,
					backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);	
		//Second circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, offsetMultiplier, 9, 24,
						backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);
		//Third circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, offsetMultiplier, 25, 48,
						backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);	
		//Fourth circle
		DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, SAMPLING_OFFSET_MULT_4, 49, 80,
						backgroundValue, centerValue, foregroundValue, searchScaleFactor, focalDistance);	
		
		float samplesWeight = 49.0f + 32.0f * searchScaleFactor;	
		NormalizeDiscreteSampling(samplesWeight, searchRadius, backgroundValue, centerValue, foregroundValue);	
		depthOfFieldTaps = CombineDepthOfFieldLayers(backgroundValue, centerValue, foregroundValue, centerDepth, focalDistance);
		
		#ifdef DEBUG_OPTIMIZATIONS
	 	depthOfFieldTaps.xyz = depthOfFieldTaps.xyz * float3(1.0f,0.3f,1.0f); //Purple
		#endif	
	}
	#endif
	
	#else
	
	#ifdef SC_DOF_QUALITY_MAX
	DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, 1.0f, 0, 80,
						backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);
	NormalizeDiscreteSampling(81.0f, searchRadius, backgroundValue, centerValue, foregroundValue);	
	depthOfFieldTaps = CombineDepthOfFieldLayers(backgroundValue, centerValue, foregroundValue, centerDepth, focalDistance);
	#else
	DepthOfFieldTaps(i.uv, searchRadius, centerDepth, TAPS_ARRAY, 1.0f, 0, 48,
						backgroundValue, centerValue, foregroundValue, 1.0f, focalDistance);
	NormalizeDiscreteSampling(49.0f, searchRadius, backgroundValue, centerValue, foregroundValue);	
	depthOfFieldTaps = CombineDepthOfFieldLayers(backgroundValue, centerValue, foregroundValue, centerDepth, focalDistance);
	#endif
	
	#endif
		
	float alpha = DepthOfFieldAlpha(depthOfFieldTaps.w, centerDepth, centerCoC, focalDistance);
	//alpha = saturate(depthOfFieldTaps.wwww);
	
	return float4(depthOfFieldTaps.xyz, alpha);
}