## Culling

#### 뷰프러스텀 컬링

뷰프러스텀 컬링, 컴퓨터그래픽스에서 자주들어봤던 용어다. 카메라의 범위를 절두체 모양의 상자(뷰프러스텀)이라고 하는데, 이 뷰프러스텀 이외에 들어있는 오브젝트들은 유니티가 자동으로 컬링을 해주는 기능이다. 카메라 컴포넌트를 보면 Far와 Near가 있는데 Far부분을 조절하면 먼부분 컬링 시켜줄 수 있다. 근데 이렇게 하면 갑자기 잘려서 화면이 어색해지지않는가?

그렇기 때문에 Window -> Rendering -> Lighting Settings에서 Fog기능을 활성화해서 어색함을 줄이는 방법이 있다.

이러한 이슈 때문에 모바일 게임에서는 프리룩보다는 탑뷰나 쿼터뷰카메라가 좀더 드로우콜 최적화에 용이하다.



#### 오클루전 컬링

다른 오브젝트에 의해 숨겨진 오브젝트를 걸러내는 기능. Window -> Rendering -> Occlusion Culling을 선택하여 오클루전 컬링을 사용할 수 있다. 이 과정에서 Static 정보를 토대로 연산이 이루어진다. 오클루전 컬링용 데이터는 씬을 일정 셸로 나눈 데이터이다. 이러한 셀의 크기는 Smallest Occluder를 수정함으로써 조절할 수 있다. 그런데 이것도 데이터기때문에 오히려 CPU 오버헤드가 발생할 수 있다. 그러니 적절한 파라미터 수치들을 바꿔가면서 테스트해보자.