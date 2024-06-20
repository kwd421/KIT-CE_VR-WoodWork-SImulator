# VR WoodWork Simulator

# Sample Video
[![Video Label](http://img.youtube.com/vi/GGONLWkufok/0.jpg)](https://youtu.be/GGONLWkufok)

# Problem
1. OnTriggerEnter가 중복적용되는것에 대한 Lock이 걸려있지 않아 Nail과 Wood의 결합작용이 중복될때 있음.
2. 결합체와 결합체가 결합하면 한쪽 결합체의 목재가 부모에서 떨어지는 문제 발생함.
3. 위 문제 해결 위해 Collider 중복 제거 알고리즘을 일단 제거하였으나, 그로 인해 Collider들이 중복되어 결합체에 자재가 추가될때마다 부하가 커져 점점 렉이 걸림.
