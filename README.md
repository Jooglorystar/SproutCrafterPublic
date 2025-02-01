## 게임 개요
게임 제목 : Sprout Crafter 

게임 목표 : 자신의 땅에서 작물을 심고 키워서 수확하기 <br><br>

## 개발 환경

Engine : Unity 2022.3.17f1 

Language : C# 

IDE : Visual Studio 2022, JetBrains Rider 2024.2.7

사용 패키지 : Input System, URP, TMP, Cinemachine, Dotween <br><br>

## 개발 인원 소개 및 파트 분배

개발 기간 : 24.11.25 ~ 25.01.20

(팀장) 김민준 - UI, NPC, 이외 / https://github.com/ekrxjvpvj0110  
(부팀장) 조민솔 - Player, 미니게임, 퀘스트 / https://github.com/dearnm  
(팀원) 김성호 - 데이터 관리, 인게임 자원 관리 / https://github.com/KimSungHo-web  
(팀원) 주창규 - 작물 관리, 타일 관리, 건축 시스템 / https://github.com/Jooglorystar  <br><br>


## 게임 사이클과 로직

게임 시작(기상) -> 취침전까지 스태미너를 소모하는 모든 행위(경작 등) or npc 상호작용 -> 취침

https://www.figma.com/board/za5vAhhjlROnAfzCLS09xe/22?node-id=0-1&p=f&t=e7UZTSN07YFAEEvS-0  
링크의 게임 사이클과 구조를 참고하는것이 더 빠를 수 있음  <br><br>

## 메인 기능  

---
### Integrated Managers - 게임 시작부터 게임 종료까지 항상 필요한 매니저들
---
### AudioManager

모든 오디오를 담고있는 AudioListSo에 각 종류별 오디오(Sfx, Music, Ambient 등) So가 존재  
종류별 Enum값을 이용하여 필요시 호출, 모든 AudioSource를 한 곳에서 관리하여 불필요한 메모리 중복 낭비를 막고 데이터의 관리 편리성을 높임

### DataManger

플레이어와 관련된 모든 데이터 정보를 관리하는 역할

### UIManager

모든 UI를 생성 및 Close해주는 기능을 수행, 필요한 UI만 생성하고 사용하지 않을때는 꺼줌(Disable)으로써 메모리 효율성을 높임  
한번 생성된 UI는 리스트에서 관리하여 다시 필요해지는 순간이 온다면 단순히 OnEnable 해줌 

### CursorManager

마우스의 위치에 따라 커서를 변경해주는 역할(Ex. 경작 가능한 땅에서 호미 이미지로 변하는 식) <br><br><br><br>


---
### Independ Managers - 인게임에서만 필요한 매니저들 
---
### CropManager
플레이어가 작물과 상호작용할때, 작물의 생성 -> 상태 갱신 -> 제거 및 데이터 저장/로드 의 기능을 수행 

### LightCycle
인게임 시간에 따라 조명의 밝기를 조절하여주는 기능을 수행

### NPCManager
NPC와 상호작용시 UI를 띄우고, 필요한 대사 스크립트와 NPC의 정보를 통합 관리

### PlayerSkillManager
플레이어의 경험치를 관리하고, 활성화된 스킬의 여부를 돌려주는 기능을 수행

### PoolManager
인게임에서 재사용되는 모든 오브젝트들을 통합 관리

### SaveManager
플레이어가 선택한 슬롯의 데이터를 불러오고 저장하는 기능을 수행

### SceneControlManger
플레이어가 씬을 이동할때 씬을 비동기적으로 로드/언로드 하는 기능을 수행

### TimeManager
인게임에서 사용되는 시간을 관리, 이것을 기반으로 인게임의 모든 이벤트가 관리됨  

### QuestManager
인게임에서 퀘스트를 관리하는 기능을 수행  <br><br><br><br>

---
### 부가 기능
---
### GoogleSheetManager
구글 시트의 API를 이용해 에디터상에서 필요한 SO를 만들고 관리하는 기능을 수행

### InGameControlWindow
EditorWindow를 이용하여 인게임에서 개발자가 테스트하는데 사용하는 시간을 줄임, 일종의 치트모드  
경로 : Window-> Customizing -> InGameControl

### CinemachineConfinerChanger
해당 게임은 InGame이라는 메인씬은 계속 존재하고 필요한 씬만 Addtive모드로 로드/언로드하기때문에 해당 씬에 존재하는 컨파이너를 찾아주는 기능을 수행



