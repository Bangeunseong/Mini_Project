# Match The Mate
Mini-Project given by Sparta-CodingClub

## 핵심기능


이 매칭 게임을 구현하기 위해 팀원들의 정보를 한 곳에 저장하는 Object가 필요했는데 우리는 ScriptableObject를 상속하는 `MemberTable` 클래스를 활용하여 팀원의 사진과 좋아하는 것들의 집합을 연관지어 저장해 사용하였다.  **`MemberTable` 클래스 내부에는 `MemberInfo`라는 클래스를 담는 List 자료형과 Id를 key 값으로 가지고 MemberInfo를 value값으로 가지는 Dictionary**로 이루어져 있다. 

`[SerializeField]`와 `[Serializable]` 키워드를 사용하면 원래 Inspector 창에 표시되지 않는 **`private` 접근 제어자의 `MemberInfo`와 `MemberTable` 같은 Attribute 객체**들도 **Inspector에서 확인하고 수정**할 수 있게 됩니다.

- Source Code (MemberTable, MemberInfo)
    
    ```csharp
    [CreateAssetMenu(fileName = "MemberTable", menuName = "Scriptable Objects/MemberTable")]
    public class MemberTable : ScriptableObject
    {
        [SerializeField] private List<MemberInfo> _members = new List<MemberInfo>();
        public Dictionary<int, MemberInfo> MemberDicts = new();
    
        private void OnEnable()
        {
            foreach (MemberInfo m in _members)
            {
    		        // Store MemberInfo using id stored in MemberInfo
                MemberDicts[m.Id] = m;
            }
        }
    		
    		// Get MemberInfo by Id
        public MemberInfo GetMemberInfoById(int _id)
        {
            if(MemberDicts.TryGetValue(_id, out MemberInfo m)) return m;
    
            Debug.Log($"Invalid Member Id! : {_id}");
            return null;
        }
    }
    ```
    
    ```csharp
    [Serializable] public class Pair
    {
        public int Key;
        public List<ImageInfo> Values;
    }
    
    [Serializable] public class ImageInfo
    {
        public int ParentId;
        public string Description;
        public Sprite Image;
    }
    
    [Serializable] public class MemberInfo
    {
        public int Id;
        public string Name;
        public List<Sprite> Selfies;
        public List<Pair> PairOfImages;
    }
    ```
    

 카드는 `MemberTable` 클래스에 있는 사진들을 기반으로 설정되며, 각각의 사진에는 **0부터 19까지의 번호**가 부여됩니다. 이 번호 중 **0부터 9까지는 인물과 관련된 이미지**를, **10부터 19까지는 해당 인물의 사진**을 의미합니다. 이 번호들은 배열(`Array`)을 이용해 관리되며, 이를 통해 각 인물과 관련 정보가 서로 연결되도록 구성됩니다.

- Source Code (Card Randomization, Card Setup)
    
    ```csharp
    // Image Randomization
    int[] arr = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
    arr = arr.OrderBy(x => Random.Range(0f, 19f)).ToArray();
    
    // Set Position of cards
    Vector2 position = new Vector2(xOffset, yOffset);
    for (int i = 0; i < cardCount; i++)
    {
        ...
        // Instantiate Card GameObject
        GameObject card = Instantiate(CardPrefab, transform);
        ...
        // Setup Card Game Object
        card.GetComponent<CardController>().Set(_category, arr[i]);
        ...
    }
    ```
    
    ```csharp
    // Set Card Entity using index and category
    public void Set(int category, int index)
    {
    		// MemberInfo Table
        MemberTable memberTable = TableManager.Instance.GetTable<MemberTable>();
    		
    		// Images of whose Favorites
        if (index < 10)
        {
            Id = -1;             // If id is -1, it is not facial image
            ParentId = index / 2;// If ParentId is -1, it is not image of Favorites
            Category = category; // Category of image
            Index = index;       // Store index of image
            Image.sprite = memberTable.GetMemberInfoById(ParentId).PairOfImages[Category].Values[index % 2].Image;
        }
        else
        {
            Category = category;
            Id = (index % 10) / 2;
            ParentId = -1;
            Index = index;
            Image.sprite = memberTable.GetMemberInfoById(Id).Selfies[index % 2];
        }
    }
    ```
    

이러한 구조를 활용하여, 만약 선택된 두 카드가 **모두 10 미만이거나 모두 10 이상인 경우**, 즉 인물 사진과 관련 정보가 짝을 이루지 않는 경우에는 **카드를 다시 뒤집도록** 설정했습니다. 반대로 한 장은 10 미만, 다른 한 장은 10 이상인 경우에는 **두 카드 간의 관계성을 검증**하게 됩니다.

- Source Code (Resolve Connnection between Selected Cards)
    
    ```csharp
    public void MatchCards()
    {
        // If both cards are all face images or category images, close cards and reset memory.
        if ((FirstCard.Index >= 10 && SecondCard.Index >= 10) || (FirstCard.Index < 10 && SecondCard.Index < 10)) 
        { 
            FirstCard.CloseCard(); SecondCard.CloseCard(); 
            FirstCard = SecondCard = null;
            return;
        }
    
        // When FirstCard is not face images,
        // Compare Second Card (Id -> Member Id) and First Card (Parent Id -> Connected Member Id)
        if(FirstCard.Id < 0)
        {
            if(SecondCard.Id == FirstCard.ParentId)
            {
                audioSource.PlayOneShot(_clip);
                FirstCard.DestroyCard(); SecondCard.DestroyCard();
                CardCount -= 2;
            }
            else FirstCard.CloseCard(); SecondCard.CloseCard();
        }
        // When SecondCard is not face images,
        // Compare First Card (Parent Id -> Paired Member Id) and Second Card (Id -> Member Id)
        else
        {
            if (FirstCard.Id == SecondCard.ParentId)
            {
                // Play Audio and Destroy Cards
                audioSource.PlayOneShot(_clip);
                FirstCard.DestroyCard(); SecondCard.DestroyCard();
                CardCount -= 2;
            }
            // Close Cards
            else FirstCard.CloseCard(); SecondCard.CloseCard();
        }
    
        // Reset Memory
        FirstCard = SecondCard = null;
    }
    ```
    

## 추가기능


기존의 1대1 매칭 카드 게임이 아닌 인물의 사진과 그의 관심사를 매칭할 수 있는 새로운 형식의 카드 게임.

카드 뒤집기 게임에 없었던 애니메이션 및 스테이지 별 사운드 삽입

힌트를 제공하는 UI 제작

게임 종료 시 스테이지 선택이나 재시작 등을 할 수 있는 다양한 선택을 제공하는 UI 제작