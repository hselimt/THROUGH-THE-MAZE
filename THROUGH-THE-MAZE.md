# BÖLÜM 1: UNITY TEMELLERİ

## 1.1 MonoBehaviour Nedir?

```csharp
public class Enemy : MonoBehaviour  // ← Tüm Unity scriptleri bundan türer
```

**Normal C# ile fark:**
- Unity'de `new Enemy()` ile obje oluşturamazsın
- Unity kendi lifecycle'ını yönetir
- Bir script'in GameObject'e eklenebilmesi için MonoBehaviour'dan türemesi **ZORUNLU**

## 1.2 GameObject ve Component Sistemi

```
GameObject (Sahne'deki her obje)
├── Transform (pozisyon, rotation, scale) → HER OBJEDE ZORUNLU
├── SpriteRenderer (görsel çizen component)
├── Collider2D (çarpışma algılama)
└── YourScript.cs (davranış) → MonoBehaviour'dan türer
```

**Component'e erişim:**
```csharp
// Aynı objedeki component'e erişim
SpriteRenderer sr = GetComponent<SpriteRenderer>();

// Başka objedeki component'e erişim
PlayerController player = otherObject.GetComponent<PlayerController>();

// Tag ile obje bulma
GameObject player = GameObject.FindGameObjectWithTag("Player");

// Type ile ilk bulunan obje
LevelManager lm = FindFirstObjectByType<LevelManager>();
```

## 1.3 Unity Lifecycle Methods

```csharp
public class Example : MonoBehaviour
{
    // ═══════════════════════════════════════════════════════════════
    // 1️⃣ AWAKE - Obje oluşturulunca İLK çağrılan
    // Kullanım: Singleton init, kendi değişkenlerini hazırla
    // ═══════════════════════════════════════════════════════════════
    private void Awake()
    {
        Instance = this;  // Singleton pattern
    }
    
    // ═══════════════════════════════════════════════════════════════
    // 2️⃣ START - Tüm Awake'ler bittikten sonra, ilk frame'den ÖNCE
    // Kullanım: Diğer objelere referans al (onların Awake'i bitmiş olur)
    // ═══════════════════════════════════════════════════════════════
    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // 3️⃣ UPDATE - HER FRAME çağrılır (60 FPS = saniyede 60 kez)
    // Kullanım: Input, hareket, AI kararları
    // ═══════════════════════════════════════════════════════════════
    private void Update()
    {
        // Time.deltaTime = son frame'den bu yana geçen süre (saniye)
        // Frame rate'den bağımsız hareket için ZORUNLU
        transform.position += direction * speed * Time.deltaTime;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // 4️⃣ FIXED UPDATE - Fizik için sabit aralıklarla (0.02s default)
    // Kullanım: Rigidbody işlemleri
    // ═══════════════════════════════════════════════════════════════
    private void FixedUpdate()
    {
        // Rigidbody force/velocity işlemleri burada
    }
    
    // ═══════════════════════════════════════════════════════════════
    // 5️⃣ TRIGGER/COLLISION CALLBACKS
    // Collider isTrigger=true ise OnTrigger, değilse OnCollision
    // ═══════════════════════════════════════════════════════════════
    private void OnTriggerEnter2D(Collider2D other)
    {
        // other = çarpıştığımız objenin collider'ı
        if (other.CompareTag("Enemy"))
        {
            // Enemy ile çarpıştık
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // 6️⃣ ON DESTROY - Obje yok edilirken
    // Kullanım: Cleanup, event unsubscribe
    // ═══════════════════════════════════════════════════════════════
    private void OnDestroy()
    {
        // Temizlik işlemleri
    }
}
```

### Çağrılma Sırası Diyagramı

```
Scene Load
    │
    ▼
┌─────────────────────────────┐
│  Tüm objelerin Awake()      │ → Singleton'lar hazır
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│  Tüm objelerin Start()      │ → Cross-referanslar güvenli
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────────────┐
│  GAME LOOP (her frame)              │
│  ┌────────────────────────────────┐ │
│  │ Update()      - her frame      │ │
│  │ FixedUpdate() - fizik          │ │
│  │ LateUpdate()  - kamera takip   │ │
│  └────────────────────────────────┘ │
└─────────────────────────────────────┘
```

## 1.4 SerializeField ve Inspector

```csharp
public class Enemy : MonoBehaviour
{
    // PUBLIC = Inspector'da görünür, dışarıdan erişilebilir
    public float moveSpeed = 4f;
    
    // [SerializeField] = Inspector'da görünür, dışarıdan erişilemez
    [SerializeField] private float attackDamage = 10f;
    
    // [Header] = Inspector'da başlık ekler
    [Header("Movement Settings")]
    public float jumpHeight = 5f;
    
    // PRIVATE = Inspector'da görünmez (default)
    private float internalTimer;
}
```

## 1.5 Prefab Sistemi

```
Prefab = Hazır şablon obje (Blueprint)
         ↓
         Instantiate() ile clone oluşturulur
         ↓
         Scene'de instance olarak görünür
```

```csharp
// Prefab'dan yeni obje oluşturma
public GameObject hunterPrefab;  // Inspector'dan atanır

void SpawnEnemy()
{
    // Instantiate(ne, nerede, rotation)
    GameObject newEnemy = Instantiate(
        hunterPrefab,           // Kaynak prefab
        spawnPosition,          // Vector3 pozisyon
        Quaternion.identity     // Rotasyon (identity = döndürme yok)
    );
    
    // Oluşan objeye erişim
    HunterEnemy hunter = newEnemy.GetComponent<HunterEnemy>();
    hunter.Initialize(data, renderer, pos, player);
}
```

## 1.6 Vector2 / Vector3 ve Transform

```csharp
// Vector2Int - Grid pozisyonu (tam sayı)
Vector2Int gridPos = new Vector2Int(5, 10);

// Vector3 - Dünya koordinatı (float)
Vector3 worldPos = new Vector3(5.5f, 10.2f, 0f);

// Transform - Her GameObject'in pozisyon/rotation/scale bilgisi
transform.position = new Vector3(0, 0, 0);  // Pozisyon set
transform.position += Vector3.right * speed * Time.deltaTime;  // Hareket

// Grid → World dönüşümü (bu projede)
Vector3 worldPos = mazeRenderer.GetWorldPosition(gridX, gridY);
```

## 1.7 Time Sınıfı

```csharp
Time.time          // Oyun başından beri geçen süre (saniye)
Time.deltaTime     // Son frame'den beri geçen süre (~0.016s @ 60FPS)
Time.unscaledTime  // Pause'dan etkilenmeyen zaman
Time.timeScale     // 1 = normal, 0 = pause, 2 = 2x hız

// Frame-rate bağımsız hareket:
transform.position += direction * speed * Time.deltaTime;

// Timer örneği:
if (Time.time >= nextUpdateTime)
{
    nextUpdateTime = Time.time + 0.5f;  // 0.5 saniye sonra
    DoSomething();
}
```

---

# BÖLÜM 2: PROJE MİMARİSİ

## 2.1 Proje Yapısı

```
Assets/
├── Scripts/
│   ├── Enemies/              → Düşman AI sınıfları
│   │   ├── Enemy.cs          → Base class (abstract)
│   │   ├── HunterEnemy.cs    → A* ile takip
│   │   ├── PatrolEnemy.cs    → Random yürüyüş
│   │   ├── TeleporterEnemy.cs → Işınlanma
│   │   └── SpawnerEnemy.cs   → Mini hunter spawn
│   │
│   ├── Items/                → Toplanabilir objeler
│   │   ├── Collectible.cs    → Base class (abstract)
│   │   ├── Crystal.cs        → Ana hedef
│   │   ├── HealthPickup.cs   → Can yenileme
│   │   ├── Shield.cs         → Koruma
│   │   ├── SpeedBoost.cs     → Hız artışı
│   │   └── FreezeBomb.cs     → Düşman dondurma
│   │
│   ├── Managers/             → Oyun yönetimi (Singleton'lar)
│   │   ├── GameManager.cs    → Merkezi kontrol
│   │   ├── LevelManager.cs   → Level/spawn yönetimi
│   │   ├── UIManager.cs      → UI kontrolü
│   │   └── SaveManager.cs    → Kayıt sistemi
│   │
│   ├── Maze/                 → Labirent sistemi
│   │   ├── MazeCell.cs       → Tek hücre verisi
│   │   ├── MazeData.cs       → Tüm labirent verisi
│   │   ├── MazeGenerator.cs  → Labirent oluşturma
│   │   ├── MazeRenderer.cs   → Görsel çizim
│   │   └── Pathfinding.cs    → A* algoritması
│   │
│   ├── Player/
│   │   ├── PlayerController.cs → Hareket ve input
│   │   └── PlayerHealth.cs   → Can sistemi
│   │
│   ├── Effects/
│   │   ├── GridBackground.cs → Arka plan efekti
│   │   └── WallGlow.cs       → Duvar parlaması
│   │
│   └── Utils/
│       └── SwipeDetector.cs  → Mobil swipe algılama
│
└── Prefabs/                  → Hazır obje şablonları
    ├── Enemies/
    ├── Collectibles/
    └── Effects/
```

## 2.2 Mimari Diyagramı

```
┌─────────────────────────────────────────────────────────────────┐
│                        GAME MANAGER                              │
│                    (Singleton - Merkez)                          │
├─────────────────────────────────────────────────────────────────┤
│  • Score, Health, Level State                                    │
│  • Pause/Resume, Game Over                                       │
│  • Combo System                                                  │
└──────────────────────────────┬──────────────────────────────────┘
                               │
          ┌────────────────────┼────────────────────┐
          ▼                    ▼                    ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  LEVEL MANAGER  │  │   UI MANAGER    │  │  SAVE MANAGER   │
│  • Enemy Spawn  │  │   • HUD         │  │  • HighScore    │
│  • Item Spawn   │  │   • Menus       │  │  • JSON I/O     │
│  • Level Flow   │  │   • Panels      │  │  • Persistence  │
└────────┬────────┘  └─────────────────┘  └─────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│                         MAZE SYSTEM                              │
│  MazeGenerator → MazeData (veri) → MazeRenderer (görsel)        │
│                      ↓                                           │
│              MazeCell[12, 20] array                              │
└─────────────────────────────────────────────────────────────────┘
         │
         ├────────────────────┬─────────────────────┐
         ▼                    ▼                     ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│     PLAYER      │  │    ENEMIES      │  │  COLLECTIBLES   │
│ PlayerController│  │  Enemy (base)   │  │ Collectible(base│
│  PlayerHealth   │  │  HunterEnemy    │  │ Crystal         │
│                 │  │  PatrolEnemy    │  │ HealthPickup    │
│                 │  │  TeleporterEnemy│  │ Shield          │
│                 │  │  SpawnerEnemy   │  │ SpeedBoost      │
│                 │  │                 │  │ FreezeBomb      │
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

---

# BÖLÜM 3: DESIGN PATTERNS

## 3.1 Singleton Pattern

**Neden?**
- Oyunda TEK BİR GameManager olmalı
- Her yerden `GameManager.Instance.XXX()` ile erişilebilmeli

```csharp
public class GameManager : MonoBehaviour
{
    // ═══════════════════════════════════════════════════════════════
    // Static property - GameManager.Instance ile erişim
    // ═══════════════════════════════════════════════════════════════
    public static GameManager Instance { get; private set; }
    
    // Game state
    private int score = 0;
    private int currentLevel = 1;
    private bool isGamePaused = false;
    
    public bool IsGamePaused => isGamePaused;  // Read-only property
    
    private void Awake()
    {
        // İlk oluşturulan mı kontrol et
        if (Instance == null)
        {
            Instance = this;  // Kendini kaydet
            Application.targetFrameRate = 60;  // FPS limiti
        }
        else
        {
            // Zaten bir Instance var = bu fazlalık, yok et
            Destroy(gameObject);
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // Public methods - her yerden çağrılabilir
    // ═══════════════════════════════════════════════════════════════
    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance?.UpdateScoreText(score);
    }
    
    public void PlayerHitByEnemy()
    {
        // Player'a hasar ver
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerHealth?.TakeDamage(1);
    }
    
    public void CollectCrystal(Crystal crystal)
    {
        AddScore(10 * comboMultiplier);
        crystalsCollected++;
        
        if (crystalsCollected >= requiredCrystals)
        {
            LevelComplete();
        }
    }
}

// ═══════════════════════════════════════════════════════════════
// Kullanım - HERHANGİ BİR SCRIPT'TEN:
// ═══════════════════════════════════════════════════════════════
GameManager.Instance.AddScore(100);
GameManager.Instance.PlayerHitByEnemy();

if (GameManager.Instance.IsGamePaused)
    return;
```

### DontDestroyOnLoad Variant

```csharp
// SaveManager - Sahne değişse bile yok edilmez
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // ← Sahne değişiminde korunur
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
```

## 3.2 Template Method Pattern (Enemy AI)

**Neden?**
- Tüm enemy'ler ortak davranışlara sahip (hareket, freeze, collision)
- AMA her enemy farklı "karar" veriyor
- Base class ortak kodu içerir, abstract method farklılığı tanımlar

```
         ┌──────────────────┐
         │      Enemy       │  ← Abstract base class
         │    (abstract)    │
         └────────┬─────────┘
                  │
                  │ DecideNextMove() → abstract (her child farklı implement eder)
                  │ MoveToTarget()   → virtual (override edilebilir)
                  │ Freeze()         → concrete (ortak)
                  │
    ┌─────────────┼─────────────┬─────────────┐
    ▼             ▼             ▼             ▼
┌────────┐  ┌────────┐  ┌────────────┐  ┌─────────┐
│ Hunter │  │ Patrol │  │ Teleporter │  │ Spawner │
│  A*    │  │ Random │  │  Işınlan   │  │  Spawn  │
└────────┘  └────────┘  └────────────┘  └─────────┘
```

---

# BÖLÜM 4: ENEMY SİSTEMİ (TAM KOD)

## 4.1 Enemy Base Class

```csharp
// ═══════════════════════════════════════════════════════════════════
// ABSTRACT CLASS - Direkt obje oluşturulamaz, türetilmeli
// ═══════════════════════════════════════════════════════════════════
public abstract class Enemy : MonoBehaviour
{
    // ───────────────────────────────────────────────────────────────
    // INSPECTOR FIELDS - Unity Editor'da görünür ve düzenlenebilir
    // ───────────────────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 4f;
    
    // ───────────────────────────────────────────────────────────────
    // PROTECTED FIELDS - Bu class ve türeyen class'lar erişebilir
    // ───────────────────────────────────────────────────────────────
    protected Vector2Int currentGridPos;   // Şu anki grid pozisyonu (x,y)
    protected Vector2Int targetGridPos;    // Hedef grid pozisyonu
    protected bool isMoving = false;       // Hareket halinde mi?
    
    protected MazeData mazeData;           // Labirent verisi referansı
    protected MazeRenderer mazeRenderer;   // Görsel renderer referansı
    protected Transform playerTransform;   // Player'ın Transform component'i
    
    // AI güncelleme aralığı - her frame DEĞİL, performance için
    protected float updateInterval = 0.2f;
    protected float nextUpdateTime = 0f;
    
    // ───────────────────────────────────────────────────────────────
    // PRIVATE FIELDS - Sadece bu class erişebilir
    // ───────────────────────────────────────────────────────────────
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    private SpriteRenderer sr;
    private Color originalColor;
    
    // ═══════════════════════════════════════════════════════════════
    // VIRTUAL METHOD - Alt class'lar override edebilir, default var
    // ═══════════════════════════════════════════════════════════════
    public virtual void Initialize(MazeData data, MazeRenderer renderer, 
                                   Vector2Int startPos, Transform player)
    {
        mazeData = data;
        mazeRenderer = renderer;
        currentGridPos = startPos;
        targetGridPos = startPos;
        playerTransform = player;
        
        // Grid pozisyonunu dünya koordinatına çevir
        // Örn: (3,5) → (3.5f, 5.5f, 0f) gibi
        transform.position = mazeRenderer.GetWorldPosition(startPos.x, startPos.y);
        
        // Bu hücreyi "Enemy var" olarak işaretle
        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Enemy;
        
        // Random başlangıç zamanı - tüm enemy'ler aynı anda güncellenmesin
        // Bu sayede CPU yükü framelere dağılır
        nextUpdateTime = Time.time + Random.Range(0f, updateInterval);
        
        // Component referansını cache'le (her frame GetComponent çağırmamak için)
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PROTECTED VIRTUAL UPDATE - Her frame çağrılır
    // Alt class'lar override edip kendi logic'ini ekleyebilir
    // ═══════════════════════════════════════════════════════════════
    protected virtual void Update()
    {
        // ─────────────────────────────────────────────────────────
        // PAUSE CHECK - Oyun pause'daysa hiçbir şey yapma
        // ─────────────────────────────────────────────────────────
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused)
            return;
        
        // ─────────────────────────────────────────────────────────
        // FREEZE CHECK - Donmuşsak timer'ı azalt ve çık
        // ─────────────────────────────────────────────────────────
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;  // Her frame azalt
            if (freezeTimer <= 0f)
            {
                Unfreeze();
            }
            return;  // Donmuşken hareket yok
        }
        
        // ─────────────────────────────────────────────────────────
        // MOVEMENT veya AI DECISION
        // ─────────────────────────────────────────────────────────
        if (isMoving)
        {
            MoveToTarget();  // Hedefe doğru ilerle
        }
        else if (Time.time >= nextUpdateTime)
        {
            // AI güncelleme zamanı geldi
            nextUpdateTime = Time.time + updateInterval;
            DecideNextMove();  // ABSTRACT - her enemy farklı implement eder
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ABSTRACT METHOD - Alt class MUTLAKA implement etmeli
    // Implementation yoksa compile hatası verir
    // ═══════════════════════════════════════════════════════════════
    protected abstract void DecideNextMove();
    
    // ═══════════════════════════════════════════════════════════════
    // PROTECTED METHODS - Alt class'lar kullanabilir
    // ═══════════════════════════════════════════════════════════════
    protected virtual void MoveToTarget()
    {
        // Grid pozisyonunu dünya koordinatına çevir
        Vector3 targetWorldPos = mazeRenderer.GetWorldPosition(targetGridPos.x, targetGridPos.y);
        
        // ─────────────────────────────────────────────────────────
        // Vector3.MoveTowards = A'dan B'ye maxDelta kadar yaklaş
        // Time.deltaTime ile çarpma = frame rate'den bağımsız hareket
        // 60 FPS'de deltaTime ≈ 0.016s, 30 FPS'de ≈ 0.033s
        // ─────────────────────────────────────────────────────────
        transform.position = Vector3.MoveTowards(
            transform.position,           // Nereden
            targetWorldPos,               // Nereye
            moveSpeed * Time.deltaTime    // Bu frame'de ne kadar hareket
        );
        
        // Hedefe ulaştık mı kontrol et (0.01 unit'ten az mesafe)
        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            // Tam hedefe snap (küçük kaymaları önle)
            transform.position = targetWorldPos;
            
            // Grid verilerini güncelle
            mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Empty;
            currentGridPos = targetGridPos;
            mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Enemy;
            
            isMoving = false;
        }
    }
    
    // ─────────────────────────────────────────────────────────────
    // TryMove - Belirtilen pozisyona hareket etmeyi dene
    // Return: Başarılı mı?
    // ─────────────────────────────────────────────────────────────
    protected bool TryMove(Vector2Int newPos)
    {
        // Duvar kontrolü - bu iki hücre arasında geçiş var mı?
        if (mazeData.CanMoveTo(currentGridPos.x, currentGridPos.y, newPos.x, newPos.y))
        {
            targetGridPos = newPos;
            isMoving = true;
            return true;
        }
        return false;
    }
    
    // ─────────────────────────────────────────────────────────────
    // TeleportTo - Anında pozisyon değiştir (TeleporterEnemy için)
    // ─────────────────────────────────────────────────────────────
    protected void TeleportTo(Vector2Int newPos)
    {
        // Eski hücreyi temizle
        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.Empty;
        
        // Yeni pozisyona git
        currentGridPos = newPos;
        targetGridPos = newPos;
        transform.position = mazeRenderer.GetWorldPosition(newPos.x, newPos.y);
        
        // Yeni hücreyi işaretle
        mazeData.GetCell(newPos.x, newPos.y).Content = CellContent.Enemy;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PUBLIC METHODS - Dışarıdan çağrılabilir
    // ═══════════════════════════════════════════════════════════════
    public void Freeze(float duration)
    {
        isFrozen = true;
        freezeTimer = duration;
        
        // Görsel feedback - mavi ton
        if (sr != null)
        {
            sr.color = new Color(0.5f, 0.7f, 1f);
        }
    }
    
    private void Unfreeze()
    {
        isFrozen = false;
        freezeTimer = 0f;
        
        // Orijinal renge dön
        if (sr != null)
        {
            sr.color = originalColor;
        }
    }
    
    public Vector2Int GetGridPosition()
    {
        return currentGridPos;
    }
}
```

## 4.2 HunterEnemy - A* Pathfinding Kullanan

```csharp
public class HunterEnemy : Enemy
{
    // Hunter'a özgü field'lar
    private Pathfinding pathfinding;           // A* algoritması instance
    private List<Vector2Int> currentPath;      // Şu anki yol
    private int pathIndex = 0;                 // Yolda kaçıncı adımdayız
    
    // Path yenileme zamanlaması
    private float pathRecalculateTime = 0f;
    private const float PATH_RECALCULATE_INTERVAL = 0.4f;  // 0.4 saniyede bir yenile
    
    // Cached referans
    private PlayerController cachedPlayer;
    
    // ═══════════════════════════════════════════════════════════════
    // OVERRIDE - Base class'ın methodunu değiştir
    // ═══════════════════════════════════════════════════════════════
    public override void Initialize(MazeData data, MazeRenderer renderer, 
                                    Vector2Int startPos, Transform player)
    {
        // base.Initialize() = Parent class'ın Initialize'ını çağır
        // Ortak işlemler (pozisyon ayarlama, referanslar) orada yapılıyor
        base.Initialize(data, renderer, startPos, player);
        
        // Hunter'a özgü ayarlar
        pathfinding = new Pathfinding(mazeData);  // A* instance oluştur
        updateInterval = 0.25f;  // Base'deki 0.2'yi override
        moveSpeed = 4f;          // Hızlı
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ABSTRACT METHOD IMPLEMENTATION
    // Base class bunu abstract olarak tanımladı, biz implement ediyoruz
    // ═══════════════════════════════════════════════════════════════
    protected override void DecideNextMove()
    {
        // ─────────────────────────────────────────────────────────
        // Player referansını lazy load
        // ─────────────────────────────────────────────────────────
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null) return;
        }
        
        if (cachedPlayer == null)
        {
            cachedPlayer = playerTransform.GetComponent<PlayerController>();
            if (cachedPlayer == null) return;
        }
        
        // Player'ın grid pozisyonunu al
        Vector2Int playerGridPos = cachedPlayer.GetGridPosition();
        
        // ─────────────────────────────────────────────────────────
        // PATH RECALCULATION
        // Her frame değil, belirli aralıklarla yeniden hesapla
        // Sürekli hesaplamak CPU'yu yorar
        // ─────────────────────────────────────────────────────────
        if (Time.time >= pathRecalculateTime)
        {
            pathRecalculateTime = Time.time + PATH_RECALCULATE_INTERVAL;
            
            // A* ile path bul
            currentPath = pathfinding.FindPath(currentGridPos, playerGridPos);
            pathIndex = 0;
        }
        
        // ─────────────────────────────────────────────────────────
        // PATH FOLLOWING
        // Bulunan path'i takip et
        // ─────────────────────────────────────────────────────────
        if (currentPath != null && currentPath.Count > 1)
        {
            // Path henüz bitmedi mi?
            if (pathIndex < currentPath.Count - 1)
            {
                pathIndex++;
                Vector2Int nextPos = currentPath[pathIndex];
                TryMove(nextPos);  // Base class'tan miras
            }
            else
            {
                // Path bitti, yeni path lazım
                currentPath = null;
            }
        }
    }
}
```

## 4.3 PatrolEnemy - Random Yürüyüş

```csharp
public class PatrolEnemy : Enemy
{
    private Vector2Int patrolDirection;
    private bool hasChosenDirection = false;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, 
                                    Vector2Int startPos, Transform player)
    {
        base.Initialize(data, renderer, startPos, player);
        
        moveSpeed = 2f;  // Yavaş hareket
        
        // PatrolEnemy farklı content type kullanıyor (ayırt etmek için)
        mazeData.GetCell(currentGridPos.x, currentGridPos.y).Content = CellContent.PatrolEnemy;
        
        ChooseRandomDirection();
    }
    
    protected override void DecideNextMove()
    {
        if (!hasChosenDirection)
        {
            ChooseRandomDirection();
        }
        
        // Mevcut yönde ilerlemeye çalış
        Vector2Int nextPos = currentGridPos + patrolDirection;
        
        // Hareket edemediyse (duvar var) yön değiştir
        if (!TryMove(nextPos))
        {
            // Duvardan sekme
            ChooseRandomDirection();
            
            // Yeni yönde dene
            nextPos = currentGridPos + patrolDirection;
            TryMove(nextPos);
        }
    }
    
    private void ChooseRandomDirection()
    {
        // 4 ana yön
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // Yukarı
            new Vector2Int(0, -1),  // Aşağı
            new Vector2Int(-1, 0),  // Sol
            new Vector2Int(1, 0)    // Sağ
        };
        
        // Random.Range(min, max) - max dahil DEĞİL
        // Range(0, 4) = 0, 1, 2 veya 3 döner
        patrolDirection = directions[Random.Range(0, directions.Length)];
        hasChosenDirection = true;
    }
}
```

## 4.4 TeleporterEnemy - Işınlanma

```csharp
public class TeleporterEnemy : Enemy
{
    private float teleportCooldown = 3f;   // Işınlanma aralığı
    private float lastTeleportTime = 0f;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, 
                                    Vector2Int startPos, Transform player)
    {
        base.Initialize(data, renderer, startPos, player);
        
        moveSpeed = 0f;  // Normal hareket yok
        updateInterval = 0.5f;
    }
    
    protected override void DecideNextMove()
    {
        // Cooldown bitti mi?
        if (Time.time - lastTeleportTime >= teleportCooldown)
        {
            TeleportToRandomLocation();
            lastTeleportTime = Time.time;
        }
    }
    
    private void TeleportToRandomLocation()
    {
        Vector2Int newPos;
        int maxAttempts = 20;  // Sonsuz döngü önleme
        int attempts = 0;
        
        // Boş bir hücre bulana kadar rastgele dene
        do
        {
            newPos = new Vector2Int(
                Random.Range(0, MazeData.MAZE_WIDTH),
                Random.Range(0, MazeData.MAZE_HEIGHT)
            );
            attempts++;
        }
        while (mazeData.GetCell(newPos.x, newPos.y).Content != CellContent.Empty 
               && attempts < maxAttempts);
        
        // Boş hücre bulduk
        if (attempts < maxAttempts)
        {
            TeleportTo(newPos);  // Base class method
        }
    }
}
```

## 4.5 SpawnerEnemy - Mini Hunter Spawn

```csharp
public class SpawnerEnemy : Enemy
{
    // ───────────────────────────────────────────────────────────────
    // INSPECTOR FIELDS
    // ───────────────────────────────────────────────────────────────
    [Header("Spawning")]
    public GameObject hunterPrefab;      // Spawn edilecek prefab (Inspector'dan ata)
    public int maxMiniHunters = 3;       // Maksimum spawn sayısı
    public float spawnCooldown = 15f;    // Spawn aralığı
    public float miniHunterLifetime = 12f; // Mini hunter'ın ömrü
    
    // ───────────────────────────────────────────────────────────────
    // Spawn edilen hunter'ları takip et
    // ───────────────────────────────────────────────────────────────
    private List<HunterEnemy> miniHunters = new List<HunterEnemy>();
    private List<float> miniHunterSpawnTimes = new List<float>();  // Ne zaman spawn edildi
    private float lastSpawnTime = 0f;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, 
                                    Vector2Int startPos, Transform player)
    {
        // Spawner player'ı takip etmiyor, bu yüzden player = null
        base.Initialize(data, renderer, startPos, null);
        
        moveSpeed = 0f;  // Yerinde durur
        updateInterval = 1f;
    }
    
    protected override void DecideNextMove()
    {
        // ─────────────────────────────────────────────────────────
        // CLEANUP - Ölmüş veya süresi dolmuş hunter'ları temizle
        // NOT: Liste üzerinde silme yaparken TERSTEN iterate et!
        // ─────────────────────────────────────────────────────────
        for (int i = miniHunters.Count - 1; i >= 0; i--)
        {
            // null = Unity tarafından yok edilmiş (player öldürmüş olabilir)
            if (miniHunters[i] == null)
            {
                miniHunters.RemoveAt(i);
                miniHunterSpawnTimes.RemoveAt(i);
            }
            // Lifetime dolmuş mu?
            else if (Time.time - miniHunterSpawnTimes[i] >= miniHunterLifetime)
            {
                // LevelManager'dan çıkar (tracking için)
                LevelManager levelManager = FindFirstObjectByType<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.UnregisterEnemy(miniHunters[i]);
                }
                
                // GameObject'i yok et
                Destroy(miniHunters[i].gameObject);
                
                // Listelerden çıkar
                miniHunters.RemoveAt(i);
                miniHunterSpawnTimes.RemoveAt(i);
            }
        }
        
        // ─────────────────────────────────────────────────────────
        // SPAWN LOGIC
        // ─────────────────────────────────────────────────────────
        bool canSpawn = Time.time - lastSpawnTime >= spawnCooldown;
        bool hasRoom = miniHunters.Count < maxMiniHunters;
        
        if (canSpawn && hasRoom)
        {
            SpawnMiniHunter();
            lastSpawnTime = Time.time;
        }
    }
    
    private void SpawnMiniHunter()
    {
        // Yanındaki boş bir hücre bul
        Vector2Int spawnPos = FindEmptyAdjacentCell();
        if (spawnPos == new Vector2Int(-1, -1)) return;  // Bulunamadı
        
        // Grid → World koordinat dönüşümü
        Vector3 worldPos = mazeRenderer.GetWorldPosition(spawnPos.x, spawnPos.y);
        
        // ─────────────────────────────────────────────────────────
        // INSTANTIATE - Prefab'dan yeni obje oluştur
        // ─────────────────────────────────────────────────────────
        GameObject hunterObj = Instantiate(
            hunterPrefab,           // Şablon prefab
            worldPos,               // Spawn pozisyonu
            Quaternion.identity     // Rotasyon (yok)
        );
        
        // Mini hunter küçük boyutlu
        hunterObj.transform.localScale = Vector3.one * 0.7f;
        
        // Tag ayarla (collision detection için)
        hunterObj.tag = "Enemy";
        
        // Component'i al ve initialize et
        HunterEnemy hunter = hunterObj.GetComponent<HunterEnemy>();
        if (hunter != null)
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            hunter.Initialize(mazeData, mazeRenderer, spawnPos, playerTransform);
            
            // Tracking listelerine ekle
            miniHunters.Add(hunter);
            miniHunterSpawnTimes.Add(Time.time);
            
            // LevelManager'a kaydet
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager?.RegisterEnemy(hunter);
        }
    }
    
    private Vector2Int FindEmptyAdjacentCell()
    {
        // 4 komşu hücre
        Vector2Int[] offsets = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };
        
        // Shuffle for randomness
        for (int i = offsets.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (offsets[i], offsets[j]) = (offsets[j], offsets[i]);  // Swap
        }
        
        foreach (var offset in offsets)
        {
            Vector2Int checkPos = currentGridPos + offset;
            
            // Geçerli pozisyon mu ve boş mu?
            if (mazeData.IsValidPosition(checkPos.x, checkPos.y) &&
                mazeData.GetCell(checkPos.x, checkPos.y).Content == CellContent.Empty)
            {
                return checkPos;
            }
        }
        
        return new Vector2Int(-1, -1);  // Bulunamadı
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ON DESTROY - Bu obje yok edildiğinde
    // Spawn ettiği tüm hunter'ları da temizle
    // ═══════════════════════════════════════════════════════════════
    private void OnDestroy()
    {
        foreach (var hunter in miniHunters)
        {
            if (hunter != null && hunter.gameObject != null)
            {
                Destroy(hunter.gameObject);
            }
        }
        miniHunters.Clear();
    }
}
```

---

# BÖLÜM 5: A* PATHFINDING ALGORİTMASI

## 5.1 Algoritma Mantığı

```
Start: (0,0)  →  Target: (3,2)

    0   1   2   3
  ┌───┬───┬───┬───┐
2 │   │ █ │   │ T │  T = Target (Hedef)
  ├───┼───┼───┼───┤
1 │   │ █ │   │   │  █ = Wall (Duvar)
  ├───┼───┼───┼───┤
0 │ S │   │   │   │  S = Start (Başlangıç)
  └───┴───┴───┴───┘

COST HESAPLAMA:
G Cost = Başlangıçtan bu node'a kaç adım (gerçek mesafe)
H Cost = Bu node'dan hedefe tahmini mesafe (Manhattan)
F Cost = G + H (toplam maliyet)

HER ADIMDA:
1. Open list'ten en düşük F cost'lu node'u seç
2. Bu node hedefe ulaştıysa → path bul, return
3. Komşuları değerlendir:
   - Duvar varsa atla
   - Closed set'teyse atla
   - Daha iyi G cost bulunduysa güncelle
4. Tekrarla
```

## 5.2 Tam Implementation

```csharp
public class Pathfinding
{
    private MazeData mazeData;
    
    // ═══════════════════════════════════════════════════════════════
    // OBJECT POOLING - Performance optimization
    // Her FindPath çağrısında yüzlerce Node oluşturmak yerine
    // kullanılmış Node'ları havuzda tutup tekrar kullan
    // ═══════════════════════════════════════════════════════════════
    private static Stack<Node> nodePool = new Stack<Node>(100);
    private static List<Node> activeNodes = new List<Node>(100);
    
    public Pathfinding(MazeData data)
    {
        mazeData = data;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ANA METOD - Path bul ve Vector2Int listesi olarak dön
    // ═══════════════════════════════════════════════════════════════
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        // Geçersiz pozisyon kontrolü
        if (!mazeData.IsValidPosition(start.x, start.y) || 
            !mazeData.IsValidPosition(target.x, target.y))
        {
            return null;
        }
        
        // ───────────────────────────────────────────────────────────
        // A* DATA STRUCTURES
        // ───────────────────────────────────────────────────────────
        List<Node> openList = new List<Node>(50);        // Değerlendirilecek
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>(); // Değerlendirilmiş
        
        // Başlangıç node'unu oluştur
        // G=0 (başlangıçtayız), H=hedefe mesafe
        Node startNode = GetNode(start, null, 0, GetHeuristic(start, target));
        openList.Add(startNode);
        activeNodes.Add(startNode);
        
        int iterations = 0;
        const int MAX_ITERATIONS = 200;  // Sonsuz döngü önleme
        
        while (openList.Count > 0 && iterations < MAX_ITERATIONS)
        {
            iterations++;
            
            // ─────────────────────────────────────────────────────────
            // EN DÜŞÜK F COST'LU NODE'U SEÇ
            // ─────────────────────────────────────────────────────────
            Node current = GetLowestFCost(openList);
            openList.Remove(current);
            closedSet.Add(current.Position);
            
            // Hedefe ulaştık mı?
            if (current.Position == target)
            {
                List<Vector2Int> path = ReconstructPath(current);
                ReturnNodesToPool();  // Cleanup
                return path;
            }
            
            // ─────────────────────────────────────────────────────────
            // KOMŞULARI DEĞERLENDİR
            // ─────────────────────────────────────────────────────────
            List<Vector2Int> neighbors = GetWalkableNeighbors(current.Position);
            
            foreach (Vector2Int neighbor in neighbors)
            {
                // Zaten değerlendirildiyse atla
                if (closedSet.Contains(neighbor))
                    continue;
                
                // Yeni G cost = current'ın G'si + 1 (bir adım)
                float newGCost = current.GCost + 1;
                
                // Bu komşu open list'te var mı?
                Node neighborNode = openList.Find(n => n.Position == neighbor);
                
                if (neighborNode == null)
                {
                    // İlk kez görüyoruz, ekle
                    neighborNode = GetNode(neighbor, current, newGCost, 
                                          GetHeuristic(neighbor, target));
                    openList.Add(neighborNode);
                    activeNodes.Add(neighborNode);
                }
                else if (newGCost < neighborNode.GCost)
                {
                    // Daha iyi bir yol bulduk, güncelle
                    neighborNode.GCost = newGCost;
                    neighborNode.Parent = current;
                }
            }
        }
        
        // Path bulunamadı
        ReturnNodesToPool();
        return null;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // HEURISTIC - Hedefe tahmini mesafe
    // Manhattan Distance: |x1-x2| + |y1-y2|
    // Grid-based oyunlarda diagonal hareket yoksa ideal
    // ═══════════════════════════════════════════════════════════════
    private float GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // EN DÜŞÜK F COST'LU NODE'U BUL
    // ═══════════════════════════════════════════════════════════════
    private Node GetLowestFCost(List<Node> nodes)
    {
        Node lowest = nodes[0];
        
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].FCost < lowest.FCost)
            {
                lowest = nodes[i];
            }
            // F cost eşitse, H cost'a bak (hedefe yakın olanı tercih et)
            else if (nodes[i].FCost == lowest.FCost && nodes[i].HCost < lowest.HCost)
            {
                lowest = nodes[i];
            }
        }
        
        return lowest;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // WALKABLE NEIGHBORS - Gidilebilir komşu hücreleri bul
    // Duvar kontrolü dahil
    // ═══════════════════════════════════════════════════════════════
    private List<Vector2Int> GetWalkableNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>(4);  // Max 4 komşu
        
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // Yukarı
            new Vector2Int(1, 0),   // Sağ
            new Vector2Int(0, -1),  // Aşağı
            new Vector2Int(-1, 0)   // Sol
        };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            
            // CanMoveTo duvar kontrolü yapıyor
            if (mazeData.CanMoveTo(pos.x, pos.y, neighbor.x, neighbor.y))
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PATH RECONSTRUCTION
    // Hedefe ulaştıktan sonra parent zincirini takip ederek path oluştur
    // ═══════════════════════════════════════════════════════════════
    private List<Vector2Int> ReconstructPath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = endNode;
        
        // Parent null olana kadar (start node) geri git
        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }
        
        // Liste end→start sırasında, ters çevir
        path.Reverse();
        return path;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // OBJECT POOLING METHODS
    // ═══════════════════════════════════════════════════════════════
    private Node GetNode(Vector2Int pos, Node parent, float gCost, float hCost)
    {
        Node node;
        
        if (nodePool.Count > 0)
        {
            // Havuzdan al ve reset et
            node = nodePool.Pop();
            node.Position = pos;
            node.Parent = parent;
            node.GCost = gCost;
            node.HCost = hCost;
        }
        else
        {
            // Yeni oluştur
            node = new Node(pos, parent, gCost, hCost);
        }
        
        return node;
    }
    
    private void ReturnNodesToPool()
    {
        foreach (var node in activeNodes)
        {
            nodePool.Push(node);
        }
        activeNodes.Clear();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // INNER CLASS - Node
    // ═══════════════════════════════════════════════════════════════
    private class Node
    {
        public Vector2Int Position;  // Grid pozisyonu
        public Node Parent;          // Hangi node'dan geldik (path reconstruction için)
        public float GCost;          // Başlangıçtan mesafe (gerçek)
        public float HCost;          // Hedefe mesafe (tahmini)
        public float FCost => GCost + HCost;  // Toplam maliyet
        
        public Node(Vector2Int position, Node parent, float gCost, float hCost)
        {
            Position = position;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }
    }
}
```

---

# BÖLÜM 6: COLLECTIBLE SİSTEMİ

## 6.1 Class Hierarchy

```
       ┌──────────────────┐
       │   Collectible    │  ← Abstract base
       │    (abstract)    │
       └────────┬─────────┘
                │
    ┌───────────┴───────────┐
    ▼                       ▼
┌────────────┐    ┌─────────────────┐
│ Collectible│    │ TimedCollectible│ ← Timer ekler
│  (direct)  │    │    (abstract)   │
└─────┬──────┘    └────────┬────────┘
      │                    │
   Crystal             HealthPickup
                       SpeedBoost
                       Shield
                       FreezeBomb
```

## 6.2 Base Class: Collectible.cs

```csharp
public abstract class Collectible : MonoBehaviour
{
    protected Vector2Int gridPosition;
    protected MazeData mazeData;
    protected MazeRenderer mazeRenderer;
    
    // Virtual = Override edilebilir, default implementasyon var
    public virtual void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        mazeData = data;
        mazeRenderer = renderer;
        gridPosition = pos;
        
        // Grid → World dönüşümü
        transform.position = mazeRenderer.GetWorldPosition(pos.x, pos.y);
    }
    
    // Abstract = Her child MUTLAKA implement etmeli
    public abstract void Collect();
    
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
}
```

## 6.3 Crystal.cs

```csharp
public class Crystal : Collectible
{
    private SpriteRenderer sr;
    private float pulseTime = 0f;
    
    // Update optimization
    private float nextUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.05f;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        nextUpdateTime = Time.time + Random.Range(0f, UPDATE_INTERVAL);
    }
    
    private void Update()
    {
        // Her frame değil, belirli aralıklarla
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + UPDATE_INTERVAL;
        
        // Renk pulse efekti
        if (sr != null)
        {
            pulseTime += Time.deltaTime * 2f;
            
            // PingPong = 0→1→0→1 şeklinde salınım
            float t = Mathf.PingPong(pulseTime, 1f);
            
            // Lerp = iki renk arasında geçiş (t=0: yellow, t=1: white)
            sr.color = Color.Lerp(Color.yellow, Color.white, t);
        }
    }
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        base.Initialize(data, renderer, pos);
        mazeData.GetCell(pos.x, pos.y).Content = CellContent.Crystal;
    }
    
    public override void Collect()
    {
        // GameManager'a bildir (skor + combo)
        GameManager.Instance?.CollectCrystal(this);
        
        // Hücreyi boşalt
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        
        // Kendini yok et
        Destroy(gameObject);
    }
}
```

## 6.4 TimedCollectible ve Türevleri

```csharp
// Zaman sınırlı item'lar için orta katman
public abstract class TimedCollectible : Collectible
{
    [Header("Timer")]
    public float lifetime = 5f;
    
    private float spawnTime;
    
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        base.Initialize(data, renderer, pos);
        spawnTime = Time.time;
    }
    
    protected virtual void Update()
    {
        // Süre doldu mu?
        if (Time.time - spawnTime >= lifetime)
        {
            // Hücreyi temizle
            mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
            Destroy(gameObject);
        }
    }
}

// ───────────────────────────────────────────────────────────────────
// HEALTH PICKUP
// ───────────────────────────────────────────────────────────────────
public class HealthPickup : TimedCollectible
{
    public override void Initialize(MazeData data, MazeRenderer renderer, Vector2Int pos)
    {
        base.Initialize(data, renderer, pos);
        mazeData.GetCell(pos.x, pos.y).Content = CellContent.Health;
    }
    
    public override void Collect()
    {
        // Player'ın canını artır
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerHealth?.Heal(1);
        
        // Skor ekle
        GameManager.Instance?.AddScore(100);
        
        // Temizlik
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}

// ───────────────────────────────────────────────────────────────────
// SPEED BOOST
// ───────────────────────────────────────────────────────────────────
public class SpeedBoost : TimedCollectible
{
    public float boostDuration = 4f;
    public float speedMultiplier = 2f;
    
    public override void Collect()
    {
        // Player'a hız boost'u ver
        PlayerController player = FindFirstObjectByType<PlayerController>();
        player?.ApplySpeedBoost(speedMultiplier, boostDuration);
        
        GameManager.Instance?.AddScore(25);
        
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}

// ───────────────────────────────────────────────────────────────────
// FREEZE BOMB
// ───────────────────────────────────────────────────────────────────
public class FreezeBomb : TimedCollectible
{
    public float freezeDuration = 3f;
    
    public override void Collect()
    {
        // TÜM düşmanları dondur
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            enemy.Freeze(freezeDuration);
        }
        
        GameManager.Instance?.AddScore(40);
        
        mazeData.GetCell(gridPosition.x, gridPosition.y).Content = CellContent.Empty;
        Destroy(gameObject);
    }
}
```

---

# BÖLÜM 7: DATA STRUCTURES

## 7.1 MazeCell

```csharp
public class MazeCell
{
    public int X, Y;              // Grid pozisyonu
    
    // 4 duvar durumu
    public bool TopWall = true;
    public bool RightWall = true;
    public bool BottomWall = true;
    public bool LeftWall = true;
    
    // Hücre içeriği
    public CellContent Content = CellContent.Empty;
    
    public MazeCell(int x, int y)
    {
        X = x;
        Y = y;
    }
}

// Enum - Hücre içerik türleri
public enum CellContent
{
    Empty,
    Player,
    Enemy,
    PatrolEnemy,
    Crystal,
    Health,
    Shield,
    SpeedBoost,
    FreezeBomb
}
```

## 7.2 MazeData

```csharp
public class MazeData
{
    // Sabit boyutlar
    public const int MAZE_WIDTH = 12;
    public const int MAZE_HEIGHT = 20;
    
    // 2D array - tüm hücreler
    public MazeCell[,] Cells;
    
    public MazeData()
    {
        Cells = new MazeCell[MAZE_WIDTH, MAZE_HEIGHT];
        
        // Tüm hücreleri oluştur
        for (int x = 0; x < MAZE_WIDTH; x++)
        {
            for (int y = 0; y < MAZE_HEIGHT; y++)
            {
                Cells[x, y] = new MazeCell(x, y);
            }
        }
    }
    
    public MazeCell GetCell(int x, int y)
    {
        if (IsValidPosition(x, y))
            return Cells[x, y];
        return null;
    }
    
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < MAZE_WIDTH && y >= 0 && y < MAZE_HEIGHT;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CanMoveTo - İki hücre arasında geçiş var mı?
    // Duvar kontrolü yapar
    // ═══════════════════════════════════════════════════════════════
    public bool CanMoveTo(int fromX, int fromY, int toX, int toY)
    {
        // Hedef geçerli mi?
        if (!IsValidPosition(toX, toY))
            return false;
        
        MazeCell fromCell = GetCell(fromX, fromY);
        MazeCell toCell = GetCell(toX, toY);
        
        if (fromCell == null || toCell == null)
            return false;
        
        // Yön hesapla
        int dx = toX - fromX;
        int dy = toY - fromY;
        
        // Duvar kontrolü
        if (dy == 1)  // Yukarı gitmek istiyoruz
            return !fromCell.TopWall;
        if (dy == -1) // Aşağı gitmek istiyoruz
            return !fromCell.BottomWall;
        if (dx == 1)  // Sağa gitmek istiyoruz
            return !fromCell.RightWall;
        if (dx == -1) // Sola gitmek istiyoruz
            return !fromCell.LeftWall;
        
        return false;
    }
}
```

---

# BÖLÜM 8: PLAYER SİSTEMİ

## 8.1 PlayerController

```csharp
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    
    private Vector2Int currentGridPos;
    private Vector2Int targetGridPos;
    private bool isMoving = false;
    
    private MazeData mazeData;
    private MazeRenderer mazeRenderer;
    
    // Power-up states
    private bool isGhostMode = false;      // Shield aktif mi
    private float speedMultiplier = 1f;
    
    private void Update()
    {
        if (GameManager.Instance?.IsGamePaused == true) return;
        
        if (isMoving)
        {
            MoveToTarget();
        }
        else
        {
            HandleInput();
        }
    }
    
    private void HandleInput()
    {
        Vector2Int direction = Vector2Int.zero;
        
        // Keyboard input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            direction = new Vector2Int(0, 1);
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            direction = new Vector2Int(0, -1);
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            direction = new Vector2Int(-1, 0);
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            direction = new Vector2Int(1, 0);
        
        if (direction != Vector2Int.zero)
        {
            TryMove(currentGridPos + direction);
        }
    }
    
    private bool TryMove(Vector2Int newPos)
    {
        if (mazeData.CanMoveTo(currentGridPos.x, currentGridPos.y, newPos.x, newPos.y))
        {
            targetGridPos = newPos;
            isMoving = true;
            return true;
        }
        return false;
    }
    
    private void MoveToTarget()
    {
        Vector3 targetWorld = mazeRenderer.GetWorldPosition(targetGridPos.x, targetGridPos.y);
        float speed = moveSpeed * speedMultiplier;
        
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorld,
            speed * Time.deltaTime
        );
        
        if (Vector3.Distance(transform.position, targetWorld) < 0.01f)
        {
            transform.position = targetWorld;
            currentGridPos = targetGridPos;
            isMoving = false;
            
            // Bu hücredeki collectible'ları kontrol et
            CheckCollectibles();
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // COLLISION DETECTION - Trigger collider ile
    // ═══════════════════════════════════════════════════════════════
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy ile çarpışma (Ghost mode değilse)
        if (other.CompareTag("Enemy") && !isGhostMode)
        {
            GameManager.Instance?.PlayerHitByEnemy();
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // POWER-UP METHODS
    // ═══════════════════════════════════════════════════════════════
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }
    
    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);  // Bekle
        speedMultiplier = 1f;  // Normal hıza dön
    }
    
    public void ActivateShield(float duration)
    {
        StartCoroutine(ShieldCoroutine(duration));
    }
    
    private IEnumerator ShieldCoroutine(float duration)
    {
        isGhostMode = true;
        // Görsel değişiklik (yarı saydam)
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        
        yield return new WaitForSeconds(duration);
        
        isGhostMode = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    
    public Vector2Int GetGridPosition() => currentGridPos;
}
```

---

# BÖLÜM 9: SAVE SİSTEMİ

```csharp
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    
    private string savePath;
    private int highScore = 0;
    
    public int HighScore => highScore;
    
    // ═══════════════════════════════════════════════════════════════
    // SERIALIZABLE CLASS - JSON'a çevrilebilir
    // ═══════════════════════════════════════════════════════════════
    [System.Serializable]
    private class SaveData
    {
        public int highScore;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Platform bağımsız kayıt yolu
            // Android: /data/data/com.company.app/files/
            // Windows: C:\Users\Name\AppData\LocalLow\Company\App\
            // iOS: /var/mobile/.../Documents/
            savePath = Application.persistentDataPath + "/highscore.json";
            
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void TrySetHighScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }
    }
    
    private void SaveHighScore()
    {
        SaveData data = new SaveData { highScore = highScore };
        
        // Object → JSON string
        string json = JsonUtility.ToJson(data);
        
        // Dosyaya yaz
        File.WriteAllText(savePath, json);
    }
    
    private void LoadHighScore()
    {
        if (File.Exists(savePath))
        {
            // Dosyayı oku
            string json = File.ReadAllText(savePath);
            
            // JSON → Object
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            highScore = data.highScore;
        }
    }
}
```

---

# BÖLÜM 10: PERFORMANCE OPTİMİZASYONLARI

| Teknik | Nerede | Açıklama |
|--------|--------|----------|
| **Object Pooling** | Pathfinding.cs | Node'ları sürekli new'lemek yerine havuzda tutup reuse |
| **Update Interval** | Enemy.cs | Her frame değil, 0.2s aralıklarla AI kararı |
| **Cached References** | Tüm class'lar | GetComponent'i Start'ta çağır, field'da tut |
| **HashSet** | Pathfinding.cs | closedSet için O(1) lookup (List O(n) olurdu) |
| **Random Start Time** | Enemy.cs | Tüm enemy'ler aynı frame'de güncellenmez, yük dağılır |

```csharp
// YANLIŞ - Her frame GetComponent çağrısı
void Update()
{
    GetComponent<SpriteRenderer>().color = Color.red;  // Yavaş!
}

// DOĞRU - Cache'lenmiş referans
private SpriteRenderer sr;

void Awake()
{
    sr = GetComponent<SpriteRenderer>();  // Bir kere çağır
}

void Update()
{
    sr.color = Color.red;  // Hızlı!
}
```

---

# BÖLÜM 11: LEVEL SİSTEMİ

## 11.1 Level Progression Tablosu

| Level | Hunter | Patrol | Teleporter | Spawner |
|-------|--------|--------|------------|---------|
| 1 | 1 | 1 | 0 | 0 |
| 2 | 2 | 1 | 0 | 0 |
| 3 | 2 | 1 | 1 | 0 |
| 4 | 2 | 1 | 1 | 1 |
| 5+ | 1 | 1 | 2 | 2 |

Level 5'ten sonra **endless mode** - yeni power-up'lar spawn olmaya devam eder.

## 11.2 Scoring System

```
Crystal:     10 × combo multiplier
Health:      100
Shield:      20
SpeedBoost:  25
FreezeBomb:  40

Level Clear: 100 + time bonus
Time Bonus:
├─ < 30s = +100
├─ < 45s = +50
└─ < 60s = +25

Combo: 2 saniye içinde crystal topla → x2, x3, x4...
```

---

# BÖLÜM 12: INTERVIEW İÇİN ÖZET KARTLAR

## Unity Lifecycle
```
Awake → Start → [Update/FixedUpdate loop] → OnDestroy
```

## Singleton Pattern
```csharp
public static GameManager Instance { get; private set; }
void Awake() { Instance = this; }
// Kullanım: GameManager.Instance.Method()
```

## Template Method (Enemy)
```csharp
abstract class Enemy {
    abstract void DecideNextMove();  // Her child farklı implement eder
}
```

## A* Algorithm
```
F = G + H
G = gerçek mesafe (başlangıçtan)
H = tahmini mesafe (hedefe, Manhattan)
Her adımda en düşük F seçilir
```

## Component Access
```csharp
GetComponent<T>()              // Aynı objede
other.GetComponent<T>()        // Başka objede
FindFirstObjectByType<T>()     // Sahnede ilk bulunan
```

## Coroutine (Zamanlı işlem)
```csharp
StartCoroutine(MyCoroutine());

IEnumerator MyCoroutine() {
    yield return new WaitForSeconds(2f);  // 2 saniye bekle
    DoSomething();
}
```

---

# SON NOTLAR

Bu doküman Unity maze game projesinin tüm teknik detaylarını içermektedir:

1. **Unity Temelleri** - MonoBehaviour, lifecycle, component sistemi
2. **Mimari** - Manager'lar, singleton pattern
3. **Enemy AI** - Template method, inheritance, 4 enemy tipi
4. **A* Pathfinding** - Tam algoritma implementasyonu
5. **Collectibles** - Abstract class hierarchy
6. **Data Structures** - MazeCell, MazeData
7. **Save System** - JSON serialization
8. **Performance** - Caching, pooling, interval updates
