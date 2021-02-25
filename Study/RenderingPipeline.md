## 렌더링 파이프라인

### 1-1. 버텍스 쉐이더

1. **오브젝트 스페이스**
   - 폴리곤 메쉬를 표현하는 방법은 모든 정점들의 좌표를 찍은 정점 배열을 만들고 해당 인덱스들을 조합해서 인덱스 배열을 만든다. 
   - 평먼의 노멀은 외적, 정점의 노멀은 정점을 공유하는 평면들의 평균을 통해서 구할 수 있다. 역시 노멀또한 버텍스 쉐이더에서 넘겨준다.

2. **월드스페이스 (#월드변환에 의해 넘어옴)**
   - 2차원 변환에서 3 * 3 행렬의 앞부분 2 * 2 부분은 L 그 뒷 부분은 T로 나뉜다. L은 오로지 선형변환(Scaling, Rotation)만 이루어지고 T부분은 선형 + Translation이 이루어진다. 순서는 항상 L먼저 적용하고 T 적용
   - 내가 헷갈렸던건 버텍스쉐이더에서 오브젝트 스페이스에서 월드스페이스로 변환하는 월드변환을 왜 전역 uniform으로 쓰느냐였는데, 모든 월드에 동일하게 적용하는게 아니라 해당 오브젝트 하나만 적용하는거였음
   - 노멀도 월드좌표에맞게 변환해줘야하는데 -T (transpose * inverse) 해줘야한다
3. **뷰스페이스 (#뷰 변환에 의해 넘어옴, #뷰프러스텀)**
   - 뷰에서 중요한건 1. eye좌표, 바라보고있는 곳 at벡터, 카메라 상단을 가리키는 up벡터
   - 위 3개를 가지고 카메라의 u,v,n을 구할 수 있다. n (eye와 at을 잇는 벡터), u (up과 n을 내적), v(n과u를 내적)
   - eye좌표와, uvn을 알 고 있으니 카메라 변환이 가능(역변환을 이용해서)
4. **클립스페이스(#프로젝션 변환에의해 넘어옴, #NDC , #버텍스쉐이더 마지막단계)**
   - 뷰프러스텀(절두체)를 -1~1 크기의 정육면체로 변경한다 이 공간을 NDC라고 부름.
   - 이 작업이 끝나면 그 결과를 래스터라이저(하드웨어)에게 넘긴다

### 1-2. 쉐이딩

- uniform(전역)으로 월드, 뷰, 프로젝션 행렬들을 받는다
- in(입력)으로 position, normal, 텍스쳐 등을 받을 수 있다
- out(출력)으로 노멀, 텍스쳐 등을 보낼 수 있다
- position은 래스터라이즈로 따로 넘기는게아니라 gl_postition이라는 내장 변수에 저장

```glsl
#version 300
uniform mat4 worldMat, viewMat, projMat;

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoord;

out vec3 v_normal;
out vec2 v_texCoord;

void main()
{
    gl_position = projMat * viewMat * worldMat * vec4(position,1.0);
    v_normal = normalize(transpose(inverse(mat3(worldMat))) * normal);
    v_texCoord = texCoord;
}
```

- 드로우콜

  - 버텍스 쉐이더가 필요로하는 애트리뷰터와 유니폼을 모두 제공했다면, 이제 해당 폴리곤 메시를 그리는 명령을 내리는데 이를 드로우콜이라고 함

  

### 2. 래스터라이저

1. **클리핑**
   - 뷰프러스텀 관점에서 많이 설명하는데 사실은 이미 버텍스쉐이더에 의해 NDC(-1~1)에서 이루어지는 작업임
2. **원근 나눗셈 perspective division**
   - 투영변환에 의해 변환된 좌표의 마지막 인자값으로 나눠줌으로 인해 원근법을 표현함 멀 수록 인자값이 커져서 큰 수로 나뉘어져 실제값은 작아지기때문에
3. **백페이스 컬링**
   - 카메라스페이스에서 생각해보면 물체가 가진 평면의 노멀과 Look at 뷰를 내적했을때 부호를 통해서 알수있을것임, 하지만 백페이스 컬링은 래스터라이즈에서 일어나는 일이기때문에 이미 클립스페이스로 넘어온상태임 좌표도 클립스페이스 좌표일거고
   - Look at 벡터가 z축으로 이미 이동되었기때문에 오브젝트 평면의 x,y만 생각해주면됨. 해당 오브젝트 평면의 좌표를(v1~v2, v1~v3) 행렬식 (ad-bc) 가 양수면 front 왜냐? 반시계 방향이라, 음수면 back
4. **뷰포트 트랜스폼**
   - 클립스페이스는 정육면체, 스크린스페이스는 직육면체임
   - 래스터라이저는 노멀이라던가, 카메라뷰벡터라던가 버텍스와 버텍스사이의 요소들을 보간해줌
5. **스캔컨버젼(#래스터라이즈 마지막단계)**
   - **노멀**이라던가, **카메라뷰벡터**라던가 버텍스와 버텍스사이의 요소들을 보간해줌



### 3. 프래그먼트 쉐이더

래스터라이저에 의해서 보간된 프래그먼트들을 받아옴

1. **텍스쳐링**

   - 프래그먼트들의 s,t 좌표를 보간으로 알 수 있는데 텍스쳐의 해상도 x,y를 s,t에 곱해서 텍셀이 어느 프래그먼트로 들어갈지 정해줌

   - 매그니피케이션 **(픽셀이 텍셀보다 많은경우)**

     - 그냥하는 방법으로는 모자이크가 나타남
     - 투영된 픽셀을 둘러싼 네개의 텍셀을 선형보간해서 하면 좀더 부드럽게 됨

   - 미니피케이션 **(픽셀이 텍셀보다 적은경우)**

     - 그대로 줄여버리면 원하는 텍스쳐링 결과가 나오지 않을 수 있음

     - 이문제는 에일리어싱의 한 예이다.

     - 밋맵으로 텍셀의 갯수를 픽셀에 맞춰서 낮춰나가는 작업을함으로써 안티에일리어싱을 구현

       

2. **라이팅**

   - 가장 대표적인 퐁모델
     1. 디퓨즈(난반사, 광원에 따른 표면의 색)
        - 라이트 l벡터와 프래그먼트의 노멀벡터n을 내적한 값만큼 RGB에 곱해줌 하지만 0미만의 값은 될 수 없음
     2. 스펙큘러(정반사, 하이라이트를 만듬, 반사된 빛이 카메라에 들어오는 빛)
        - l벡터와 n벡터가있으면 r벡터를 구할 수 있는데 r벡터와 뷰벡터v를 내적한 값을 곱해줌
        - 하지만 그냥 이대로하면 어색하기 때문에 빛나는정도 sh를 지수로 올린후 곱해줌
     3. 엠비언트(간접조명, 공간 내 다양한 물체로부터 반사된 빛)
        - 노멀과 광원의 방향에 영향이 없음
        - 표면과 광원색의 곱으로만 이루어짐
     4. 에미션(발산광)

```glsl
#version 300
uniform mat4 worldMat, viewMat, projMat;
uniform vec3 eyePos;

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoord;

out vec3 v_normal;
out vec2 v_texCoord;

void main()
{
    v_normal = normalize(transpose(inverse(mat3(worldMat))) * normal);
	vec3 worldPos = (worldMat * vec4(position, 1.0)).xyz;
    v_view - normalize(eyePos - worldPos);
    v_texCoord = texCoord;
        
    
    gl_position = projMat * viewMat * vec4(worldPos,1.0);
    v_texCoord = texCoord;
}
```



```glsl
#version 300
precision mediump float;

uniform sampler2D colorMap;
uniform vec3 matSpec, matAmbi, matEmit; // 물체의 해당 계수
uniform float matSh;
uniform vec3 srcDiff, srcSpec, srcAmbi; // 빛의 해당 계수
uniform vec3 lightDir;

in vec3 v_normal, v_view;
in vec2 v_texCoord;

layout(location = 0) out vec4 fragColor;

void main()
{
    // normalzie
    vec3 normal = normalize(v_normal);
    vec3 view = normalize(v_view);
    vec3 light = normalize(lightDir);
    
    // diffuse
    vec3 matDiff = texture(colorMap, v_texCoord).rgb;
    vec3 diff = max(dot(normal, light),0.0) * srcDiff * matDiff;
    
    // specular
    vec3 ref1 = 2.0 * normal * dot(normal, light) - light; // r벡터
    vec3 spec = pow(max(dot(ref1, view),0.0), matSh) * srcSpec * matSpec; // 스펙큘러 계수 matSpec은 회색조를 띈다
    
    // ambient
    vec3 ambi = srcAmbi * matAmbi;
    
    fragColor r = vec4(diff + spec + ambi + matEmit, 1.0);
}
```





### 4. 출력 병합

뷰포트는 실제로 스크린에 보여질영역인데 이걸 잠시 담고있는 세가지 버퍼가있음

1. **Color 버퍼**

   - 픽셀을 잠시 보관 해상도 w*h를 보관

2. **Depth 버퍼(Z버퍼)**

   - color버퍼에 저장된 z값을 저장(역시 스캔컨버젼때 z값도 보간이됨)

   - 값이 작으면 갱신해나가서 카메라 앞에 물체나 나오도록함
   - 알파블렌딩
     - 알파가 없으면 정렬없이 해도 상관없는데 알파가 있는경우엔 불투명한 삼각형 먼저 처리한 뒤 반투명한 삼각형들을 뒤에서부터 앞으로 정렬해서 처리해줌. 그래서 반투명한 삼각형이 많을 수록 계산부하가 커짐
     - 먼저 불투명한 삼각형을 그려줘서 픽셀에 칼라를 넣어줌
     - 반투명한 삼각형의 프래그먼트 칼라값과 해당 픽셀 칼라값을 식에 맞춰서 새로운 픽셀 칼라를 넣어줌

3. **Stencil 버퍼**

   - 

이셋을합쳐 frame버퍼라고함

