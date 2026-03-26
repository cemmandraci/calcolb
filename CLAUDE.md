# CLAUDE.md — Calcolb Project Guide

Bu dosya Calcolb projesini geliştiren AI agent için rehber dokümandır.
Projeye başlamadan önce bu dosyayı dikkatlice oku.

---

## Proje Nedir?

Calcolb, bireylerin ve grupların planladıkları etkinliklerin tahmini maliyetini hesaplamalarına yardımcı olan açık kaynaklı bir web uygulamasıdır.

**Temel soru:** "Bu etkinlik bize ne kadar mal olur?"

Kullanıcı adım adım sihirbaz (wizard) arayüzü üzerinden etkinlik bilgilerini girer. Sistem gerçek zamanlı verilerle (market fiyatları, akaryakıt, mesafe) tahmini toplam maliyeti ve kişi başı tutarı hesaplar.

---

## Teknik Stack

| Katman | Teknoloji |
|--------|-----------|
| Backend | .NET (ASP.NET Core Web API) |
| ORM | Entity Framework Core |
| In-process Messaging | Mediator (MediatR alternatifi, ücretsiz) |
| Veritabanı | PostgreSQL |
| Validasyon | FluentValidation (Apache 2.0, ücretsiz) |
| Loglama | Serilog |
| Test | xUnit + Shouldly |
| Market Fiyatları | marketfiyati.org.tr API (TÜBİTAK) |
| Akaryakıt Fiyatları | EPDK API |
| Mesafe Hesabı | Google Maps Directions API |
| Paylaşım | WhatsApp Deep Link |

> **Not:** MediatR kullanma, ticari lisansa geçti. Bunun yerine `Mediator` NuGet paketini kullan.

---

## Mimari

### Modular Monolith + DDD

- Her modül bağımsız bir .NET projesidir
- Modüller birbirini doğrudan çağırmaz
- Context'ler arası iletişim **Domain Event + Mediator Notification** üzerinden yapılır
- Dış API entegrasyonları sadece **Infrastructure** katmanında olur, Domain bunları bilmez

### Proje Yapısı

```
Calcolb/
├── src/
│   ├── Modules/
│   │   ├── Event/
│   │   │   ├── Calcolb.Modules.Event.Domain/
│   │   │   ├── Calcolb.Modules.Event.Application/
│   │   │   └── Calcolb.Modules.Event.Infrastructure/
│   │   ├── Estimation/
│   │   │   ├── Calcolb.Modules.Estimation.Domain/
│   │   │   ├── Calcolb.Modules.Estimation.Application/
│   │   │   └── Calcolb.Modules.Estimation.Infrastructure/
│   │   ├── Transport/
│   │   │   ├── Calcolb.Modules.Transport.Domain/
│   │   │   ├── Calcolb.Modules.Transport.Application/
│   │   │   └── Calcolb.Modules.Transport.Infrastructure/
│   │   ├── Shopping/
│   │   │   ├── Calcolb.Modules.Shopping.Domain/
│   │   │   ├── Calcolb.Modules.Shopping.Application/
│   │   │   └── Calcolb.Modules.Shopping.Infrastructure/
│   │   └── Expense/
│   │       ├── Calcolb.Modules.Expense.Domain/
│   │       ├── Calcolb.Modules.Expense.Application/
│   │       └── Calcolb.Modules.Expense.Infrastructure/
│   ├── Calcolb.API/
│   └── Calcolb.Shared/
├── tests/
│   └── Modules/
│       ├── Event/
│       │   ├── Calcolb.Modules.Event.Domain.Tests/
│       │   └── Calcolb.Modules.Event.Application.Tests/
│       ├── Estimation/
│       ├── Transport/
│       ├── Shopping/
│       └── Expense/
├── Calcolb.sln
└── README.md
```

### Katman Referans Kuralları

```
Domain         →  hiçbir projeye referans vermez (saf domain mantığı)
Application    →  sadece Domain'e referans verir
Infrastructure →  Domain + Application'a referans verir
API            →  Application'a referans verir
Shared         →  tüm katmanlar kullanabilir
```

> **Kritik:** Bu kuralları asla ihlal etme. Infrastructure'dan Domain'e referans verebilirsin, ama Domain'den Infrastructure'a asla.

---

## Calcolb.Shared İçeriği

Tüm modüllerin ortak kullandığı base class'lar burada yaşar:

- `Entity` base class
- `AggregateRoot` base class
- `ValueObject` base class
- `IDomainEvent` interface
- Ortak exception'lar (`DomainException` vb.)

> **Uyarı:** Shared'ı şişirme. Sadece gerçekten evrensel olan şeyler buraya girer.

---

## Application Katmanı Yapısı (CQRS Lite)

Her modülün Application katmanı şu yapıyı takip eder:

```
Application/
├── Commands/
│   └── CreateEvent/
│       ├── CreateEventCommand.cs
│       └── CreateEventCommandHandler.cs
└── Queries/
    └── GetEvent/
        ├── GetEventQuery.cs
        └── GetEventQueryHandler.cs
```

> Full CQRS (ayrı read model, ayrı DB) uygulanmıyor. Sadece Command/Query ayrımı yapılıyor.

---

## Bounded Context'ler

### 1. EventContext
**Sorumluluk:** "Ne yapıyoruz?" sorusunu cevaplar. Diğer tüm context'leri tetikler.

```
Event (Aggregate Root)
├── EventId         : Guid
├── EventType       : Value Object → Piknik | Kamp | DoğumGünü | EvPartisi | SporOutdoor
├── ParticipantCount: Value Object → min: 1
└── EventDate       : Value Object → opsiyonel
```

**İş Kuralları:**
- EventType zorunlu
- ParticipantCount minimum 1, üst sınır yok
- EventDate opsiyonel

---

### 2. EstimationContext
**Sorumluluk:** Session yönetimi. Wizard boyunca büyür. Tüm context'lerden gelen maliyetleri toplar.

```
EstimationSession (Aggregate Root)
├── EstimationSessionId : Guid
├── EventId             : Guid
├── TransportPlanId     : Guid
├── ShoppingCartId      : Guid
├── ExpensePlanId       : Guid
├── Status              : Enum → InProgress | Completed
├── CreatedAt           : DateTime
└── Summary             : EstimationSummary (Value Object)

EstimationSummary (Value Object)
├── TotalTransportCost  : decimal
├── TotalShoppingCost   : decimal
├── TotalExpenseCost    : decimal
├── BufferAmount        : decimal
├── GrandTotal          : decimal
└── CostPerPerson       : decimal → GrandTotal / ParticipantCount
```

**Dinlenen Domain Events:**
- `TransportPlanCalculated` → TransportContext fırlatır
- `ShoppingCartCalculated` → ShoppingContext fırlatır
- `ExpensePlanCalculated` → ExpenseContext fırlatır

---

### 3. TransportContext
**Sorumluluk:** Ulaşım maliyeti hesabı. EPDK ve Google Maps API ile çalışır.

```
TransportPlan (Aggregate Root)
├── TransportPlanId     : Guid
├── EventId             : Guid
├── TransportType       : Value Object → Arabayla | TopluTasima | Yuruyerek
├── Vehicles            : List<Vehicle>
└── PublicTransportCost : Value Object (sadece TopluTasima seçilince)

Vehicle (Entity)
├── VehicleId           : Guid
├── TransportPlanId     : Guid
├── PassengerCount      : Value Object → min: 1
├── FuelType            : Value Object → Benzin | Dizel | LPG
└── FuelConsumption     : Value Object → varsayılan 8lt/100km

RouteDestination (Entity)
├── RouteDestinationId  : Guid
├── VehicleId           : Guid → Vehicle ile bire-bir ilişki
├── Origin              : string
├── Destination         : string
└── Distance            : decimal → Google Maps API'den hesaplanır (km)
```

**İş Kuralları:**
- `Arabayla` → Vehicle listesi zorunlu. Tüm Vehicle'lardaki PassengerCount toplamı = Event.ParticipantCount
- `TopluTasima` → PublicTransportCost zorunlu, Vehicle eklenmez
- `Yuruyerek` → Maliyet = 0₺, ne Vehicle ne PublicTransportCost

**Fırlatılan Domain Event:** `TransportPlanCalculated`

---

### 4. ShoppingContext
**Sorumluluk:** Ürün kataloğu ve sepet yönetimi. marketfiyati.org.tr API ile çalışır.

```
Product (Aggregate Root)
├── ProductId    : Guid
├── Name         : string
├── CategoryId   : Guid
├── Barcode      : string
├── Unit         : string → kg | adet | litre
├── MarketType   : string
└── IsActive     : bool → fiyat geliyor mu?

Category (Entity)
├── CategoryId      : Guid
├── Name            : string
└── ParentCategoryId: Guid? → opsiyonel üst kategori

ShoppingCart (Aggregate Root)
├── ShoppingCartId  : Guid
├── EventId         : Guid
└── CartItems       : List<CartItem>

CartItem (Entity)
├── CartItemId   : Guid
├── ProductId    : Guid → sadece referans, Product nesnesi taşınmaz
├── Quantity     : decimal
├── Unit         : string
├── MarketType   : string
├── UnitPrice    : decimal → ekleme anındaki API fiyatı
└── TotalPrice   : decimal → Quantity x UnitPrice
```

**Infrastructure:**
- `IPriceProvider` interface → Domain içinde tanımlı
- `MarketFiyatiPriceProvider : IPriceProvider` → Infrastructure'da implemente edilir
- `ProductSyncJob` → Periyodik ürün kataloğu senkronizasyonu (Infrastructure görevi)

**İş Kuralları:**
- `IsActive = false` olan ürün disabled gösterilir, sepete eklenemez
- UnitPrice ekleme anında CartItem'a kaydedilir, sonraki fiyat değişikliklerinden etkilenmez
- Karışık market seçimi serbesttir

**Fırlatılan Domain Event:** `ShoppingCartCalculated`

---

### 5. ExpenseContext
**Sorumluluk:** Kullanıcının serbest girdiği ek kalemler ve buffer hesabı.

```
ExpensePlan (Aggregate Root)
├── ExpensePlanId    : Guid
├── EventId          : Guid
├── ExpenseItems     : List<ExpenseItem>
├── BufferEnabled    : bool → varsayılan false
└── BufferPercentage : decimal → Faz 1'de sabit %10

ExpenseItem (Entity)
├── ExpenseItemId    : Guid
├── ExpensePlanId    : Guid
├── Name             : string
└── Amount           : decimal
```

**İş Kuralları:**
- ExpenseItem eklemek tamamen opsiyoneldir
- BufferEnabled = true ise toplam tutara %10 eklenir
- BufferPercentage Faz 1'de sabittir

**Fırlatılan Domain Event:** `ExpensePlanCalculated`

---

## Wizard Akışı

```
Adım 1: Etkinlik    → EventContext      → EstimationSession başlar
Adım 2: Ulaşım      → TransportContext  → TransportPlan oluşur
Adım 3: Alışveriş   → ShoppingContext   → ShoppingCart oluşur
Adım 4: Diğer Gider → ExpenseContext    → ExpensePlan oluşur
Adım 5: Özet        → EstimationContext → Summary hesaplanır
```

**Temel kural:** Hiçbir adım tamamen atlanamaz. Her adımın içinde "beni etkilemiyor" seçeneği vardır.

---

## Etkinlik Tipleri ve Şablonları

| Tip | Ulaşım | Alışveriş Kategorileri |
|-----|--------|------------------------|
| Piknik | Zorunlu | Gıda, İçecek, Tek Kullanımlık |
| Kamp | Zorunlu | Gıda, İçecek, Kamp Malzemesi |
| Doğum Günü | Opsiyonel | Gıda, İçecek, Pasta, Dekorasyon |
| Ev Partisi | Yok | Gıda, İçecek, Dekorasyon |
| Spor / Outdoor | Zorunlu | Atıştırmalık, İçecek, Spor Malzemesi |

---

## Dış Entegrasyonlar

| Servis | Amaç | Katman |
|--------|------|--------|
| marketfiyati.org.tr | Market ürün fiyatları | Infrastructure |
| EPDK | Akaryakıt fiyatları | Infrastructure |
| Google Maps Directions API | Rota mesafesi | Infrastructure |
| WhatsApp Deep Link | Özet paylaşımı | API/Presentation |

> **Önemli:** Dış API çağrıları sadece Infrastructure katmanında yapılır. Domain bu servisleri interface üzerinden tanır, implementasyonu bilmez.

---

## Faz 2'ye Bırakılan Özellikler

Bunları Faz 1'de geliştirme, sadece yapıyı buna hazır kur:

- Redis cache (Product kataloğu için)
- RabbitMQ message broker
- Araç marka/model seçimi ve gerçek tüketim verisi
- Harita gösterimi
- Toplu taşıma fiyat entegrasyonu
- Kişi başı otomatik miktar önerisi
- Buffer oranı kullanıcı seçimi
- UX geri dönüş akışı (wizard'da önceki adıma dönme)
- Full CQRS (ayrı read model)

---

## Veritabanı

**PostgreSQL** kullanılır.

- Açık kaynak, ücretsiz, Docker ile kolay kurulum
- Her modülün tabloları kendi prefix'i ile başlar:
    - `event_*`, `transport_*`, `shopping_*`, `expense_*`, `estimation_*`
- Migration'lar her modülün Infrastructure katmanında ayrı yönetilir

---

## Loglama

**Serilog** kullanılır.

```
Faz 1 Sink'ler:
- Console  → geliştirme ortamı
- File     → production ortamı (günlük rotate)
```

- Structured logging zorunlu, string interpolation kullanma
- Her log satırında minimum: `ModuleName`, `CorrelationId`, `Timestamp`
- Hassas veri (fiyat dışı kişisel bilgi) loglanmaz

---

## Exception Handling

Global exception middleware + .NET 8 built-in **Problem Details** (RFC 7807) standardı kullanılır.

```
DomainException      → 400 Bad Request
NotFoundException    → 404 Not Found
ValidationException  → 422 Unprocessable Entity
Exception            → 500 Internal Server Error
```

- Domain katmanında iş kuralı ihlallerinde `DomainException` fırlatılır
- Controller veya endpoint hiçbir zaman try/catch içermez
- Tüm exception'lar middleware'de yakalanır, Problem Details formatında döner

---

## Validasyon

İki katmanlı validasyon stratejisi uygulanır:

**1. Domain Validasyonu** (iş kuralları)
- Guard clause'lar ile Domain nesneleri kendi kendini valide eder
- Dış bağımlılık yoktur
- Kural ihlalinde `DomainException` fırlatılır

```csharp
// Örnek
public static ParticipantCount Create(int value)
{
    if (value < 1)
        throw new DomainException("Katılımcı sayısı en az 1 olmalıdır.");
    return new ParticipantCount(value);
}
```

**2. Request Validasyonu** (API girdisi)
- **FluentValidation** kullanılır (Apache 2.0, tamamen ücretsiz)
- Application katmanında her Command/Query için ayrı validator yazılır
- API'ye gelen request formatı burada kontrol edilir

```csharp
// Örnek
public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.ParticipantCount).GreaterThan(0);
        RuleFor(x => x.EventType).IsInEnum();
    }
}
```

---

## Test Stratejisi

**Framework:** xUnit
**Assertion:** Shouldly (FluentAssertions ticari lisansa geçti, Shouldly ücretsiz alternatif)

```
Domain.Tests      → iş kuralları, saf unit test, mock yok
Application.Tests → use case'ler, repository ve dış servisler mock'lanır
Infrastructure    → Faz 2 (integration test)
```

**Kurallar:**
- Her Domain sınıfı için test zorunlu
- Her Command/Query Handler için test zorunlu
- Test metod isimlendirmesi: `MethodName_Scenario_ExpectedResult`

```csharp
// Örnek
[Fact]
public void Create_WhenParticipantCountIsZero_ThrowsDomainException()
```

---

## Branch Yönetimi (GitHub Flow)

```
main      → her zaman stabil, production-ready
develop   → aktif geliştirme branch'i
feature/  → yeni özellik         → feature/event-module
fix/      → hata düzeltme        → fix/transport-calculation
chore/    → teknik borç, refactor → chore/update-dependencies
```

**Kurallar:**
- `main`'e doğrudan commit atılmaz
- Her özellik `develop`'tan açılan `feature/*` branch'inde geliştirilir
- PR açılmadan merge yapılmaz
- PR açıklaması ne yapıldığını net anlatır

---

## Commit Convention (Conventional Commits)

```
feat:      yeni özellik          → feat: add event creation endpoint
fix:       hata düzeltme         → fix: correct fuel cost calculation
chore:     teknik işler          → chore: update nuget packages
docs:      dokümantasyon         → docs: update CLAUDE.md
test:      test ekleme/düzenleme → test: add ParticipantCount domain tests
refactor:  kod düzenleme         → refactor: extract price calculation logic
```

---

## Geliştirme Kuralları

1. **Domain'i temiz tut.** Domain katmanı hiçbir dış bağımlılık içermez. NuGet paketi eklemeden önce iki kez düşün.
2. **Her iş kuralı Domain'de.** Validation, hesaplama, iş mantığı Application veya Infrastructure'da değil, Domain'de yazar.
3. **Interface'ler Domain'de, implementasyonlar Infrastructure'da.** `IPriceProvider` gibi dış servis interface'leri Domain içinde tanımlanır.
4. **Context'ler arası doğrudan referans yok.** Bir context başka bir context'in projesine referans veremez. İletişim sadece Domain Event üzerinden.
5. **Her modül kendi veritabanı tablosuna sahip.** Tablo prefix'i modül adıyla başlar: `event_`, `transport_`, `shopping_`, `expense_`, `estimation_`
6. **Unit test zorunlu.** Her Domain ve Application sınıfı için test yazılır. Infrastructure test Faz 2.
7. **Mediator kullan, MediatR değil.** MediatR ticari lisansa geçti.
8. **FluentValidation kullan.** Apache 2.0 lisansı, tamamen ücretsiz.
9. **Serilog kullan.** Structured logging zorunlu.
10. **Global exception middleware.** Controller'larda try/catch yazılmaz.
11. **Conventional Commits.** Commit mesajları kurala uyar.
12. **main'e doğrudan commit atılmaz.** Her değişiklik PR üzerinden gelir.