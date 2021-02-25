#### Gamma vs Linear

유니티 edit -> project settings -> player를 보면 color space에서 Linear와 Gamma가 있다. 둘의 차이가 무엇일까? 간단하게 사실적인색을 Linear, Linear Space이미지에 Gamma 보정을 한 경우 Gamma 라고한다. 엥? Gamma는 실제색이아닌가? Gamma는 사람이 자연스럽다고 느끼는 색을 만들어낸다. Linear는 왜곡되지 않는 선형모양의 형태를 띄고있다. 하지만 Gamma는 이미지를 좀더 밝게 만든뒤 HDD나 SSD에 저장을 해둔다. 이때를 **Gamma공간 또는 sRGB** 라고한다. 그리고 다시 디스플레이에 표시할때 어둡게 만들어서 Linear공간으로 만들어준다. 그럼 똑같이 선형적인 모양이되는데 그림을 보면 어두운영역이 더 넓게 표현이 된다.

Linear든 Gamma든 컴퓨터에서는 무조건 Linear공간에서 Gamma 공간으로 바꾸고 Gamma에서 다시 Linear로 바꾸는 작업을 해준다. 그럼 Gamma옵션과 Linear옵션을 선택하는 이유는뭘까? 색연산을 할때 어디서 연산을 하느냐이다. 감마에서는 색을 합칠때 합친 결과에 Gamma스페이스에서 더 밝게만들고, Linear는 개별로 다시 Linear 스페이스로 떨어뜨린 색을 합치기 때문에 더욱 정확함