# ASD_Optimization

**컴퓨터 그래픽스**와 유니티 엔진 **최적화작업**을 공부한 내용을 정리하고 직접 프로젝트에 적용

## Study

- [렌더링파이프라인](https://github.com/JuicyPark/ASD_Optimization/blob/master/Study/RenderingPipeline.md)
- [필레이트](https://github.com/JuicyPark/ASD_Optimization/blob/master/Study/Fillrate.md)
- [감마vs리니어](https://github.com/JuicyPark/ASD_Optimization/blob/master/Study/GammaVsLinear.md)
- [드로우콜](https://github.com/JuicyPark/ASD_Optimization/blob/master/Study/DrawCall.md)
- [컬링](https://github.com/JuicyPark/ASD_Optimization/blob/master/Study/Culling.md)

***

## Optimize

#### 1. Sprite Atlas

단순히 스프라이트들을 Atlas에 집어넣기만 했는데도 Batch수가 상당히 줄어들었다.단 이미지 규격을 잘맞추지 않으면 Start의 칼이미지 처럼 깨져보이는 이미지들이 보였다.

![image](https://user-images.githubusercontent.com/31693348/109277152-42521f00-785a-11eb-9aeb-f7b7aebae784.png)

***

#### 2. Sorting Order

BlankBox와 Dot만 찍어놓은 화면인데 Batches가 33이나 된다. 분명 같은 스프라이트를 사용하면 드로우콜이 증가하지 않을텐데 이상하게 생각했다.

![image](https://user-images.githubusercontent.com/31693348/109477481-6c564c00-7abb-11eb-859f-c706efa2960b.png)

프레임디버거를 이용해서 확인해보니 이상하게 Dot이 찍힐때도 있고 BlankBox가 찍힐때도 있는걸 확인할 수 있었다. 

![image](https://user-images.githubusercontent.com/31693348/109477922-f56d8300-7abb-11eb-91a5-63b48821ba84.png)

따로 찍어보면 예상과같이 Batches가 1씩 찍힌다.

![image](https://user-images.githubusercontent.com/31693348/109478095-2948a880-7abc-11eb-8494-5561e135bf05.png)

문제는 생각보다 쉽게 해결했다. 왠지 둘의 Order in Layer가 겹쳐서 그렇지 않을까 생각했는데 예상이 적중하였다!

![](https://user-images.githubusercontent.com/31693348/109478243-572ded00-7abc-11eb-90f9-b0d685997fa8.png)

