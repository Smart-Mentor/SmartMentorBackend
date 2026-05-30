# Community Discussions Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build career-goal discussion communities with multi-tag posts, comments, and post likes for authenticated users.

**Architecture:** Add dedicated community domain entities, a focused `CommunityService`, DTO contracts, and a new `CommunityController`. Keep the existing layered architecture intact and avoid leaking EF entities through API responses.

**Tech Stack:** ASP.NET Core, Entity Framework Core, ASP.NET Identity, SQL Server, layered architecture

---

## File Structure

### New files

- `SmartMentor.Domain/Entiies/CommunityPost.cs`
- `SmartMentor.Domain/Entiies/CommunityPostCareerGoalTag.cs`
- `SmartMentor.Domain/Entiies/CommunityComment.cs`
- `SmartMentor.Domain/Entiies/CommunityPostReaction.cs`
- `SmartMentor.Domain/EntityConfigurations/CommunityPostConfiguration.cs`
- `SmartMentor.Domain/EntityConfigurations/CommunityPostCareerGoalTagConfiguration.cs`
- `SmartMentor.Domain/EntityConfigurations/CommunityCommentConfiguration.cs`
- `SmartMentor.Domain/EntityConfigurations/CommunityPostReactionConfiguration.cs`
- `SmartMentor.Abstraction/Dto/Requests/CommunityRequests/CreateCommunityPostRequest.cs`
- `SmartMentor.Abstraction/Dto/Requests/CommunityRequests/CreateCommunityCommentRequest.cs`
- `SmartMentor.Abstraction/Dto/Responses/CommunityResponses/CommunityPostSummaryResponse.cs`
- `SmartMentor.Abstraction/Dto/Responses/CommunityResponses/CommunityPostDetailsResponse.cs`
- `SmartMentor.Abstraction/Services/CommunityService/ICommunityService.cs`
- `SmartMentor.Application/Implementations/CommunityService/CommunityService.cs`
- `SmartMentorApi/Controllers/CommunityController/CommunityController.cs`
- `SmartMentor.Persistence/Migrations/<timestamp>_AddCommunityDiscussions.cs`
- `SmartMentor.Persistence/Migrations/<timestamp>_AddCommunityDiscussions.Designer.cs`

### Modified files

- `SmartMentor.Persistence.Identity/ApplicationUser.cs`
- `SmartMentor.Domain/Entiies/CareerGoal.cs`
- `SmartMentor.Persistence/Data/ApplicationDbContext.cs`
- `SmartMentorApi/Extentions/ServiceCollectionExtentions.cs`
- `SmartMentor.Persistence/Migrations/ApplicationDbContextModelSnapshot.cs`

---

### Task 1: Add Community Domain Entities

**Files:**
- Create: `SmartMentor.Domain/Entiies/CommunityPost.cs`
- Create: `SmartMentor.Domain/Entiies/CommunityPostCareerGoalTag.cs`
- Create: `SmartMentor.Domain/Entiies/CommunityComment.cs`
- Create: `SmartMentor.Domain/Entiies/CommunityPostReaction.cs`
- Modify: `SmartMentor.Domain/Identity/ApplicationUser.cs`
- Modify: `SmartMentor.Domain/Entiies/CareerGoal.cs`

- [ ] **Step 1: Write the failing build expectation**

Expected new model shape:

```csharp
public class CommunityPost
{
    public int Id { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int PrimaryCareerGoalId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

- [ ] **Step 2: Run build to verify the feature is missing**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS before change, but the community model types do not exist yet.

- [ ] **Step 3: Add the four domain entities and navigation properties**

Create focused entity files and extend `ApplicationUser` plus `CareerGoal` with navigation collections.

- [ ] **Step 4: Run build to verify entities compile**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add SmartMentor.Domain/Entiies/CommunityPost.cs SmartMentor.Domain/Entiies/CommunityPostCareerGoalTag.cs SmartMentor.Domain/Entiies/CommunityComment.cs SmartMentor.Domain/Entiies/CommunityPostReaction.cs SmartMentor.Domain/Identity/ApplicationUser.cs SmartMentor.Domain/Entiies/CareerGoal.cs
git commit -m "feat: add community discussion domain entities"
```

### Task 2: Add EF Configurations and DbSets

**Files:**
- Create: `SmartMentor.Domain/EntityConfigurations/CommunityPostConfiguration.cs`
- Create: `SmartMentor.Domain/EntityConfigurations/CommunityPostCareerGoalTagConfiguration.cs`
- Create: `SmartMentor.Domain/EntityConfigurations/CommunityCommentConfiguration.cs`
- Create: `SmartMentor.Domain/EntityConfigurations/CommunityPostReactionConfiguration.cs`
- Modify: `SmartMentor.Persistence/Data/ApplicationDbContext.cs`

- [ ] **Step 1: Write the failing model expectation**

Required relationships:

```csharp
builder.HasOne(x => x.Author)
    .WithMany(u => u.CommunityPosts)
    .HasForeignKey(x => x.AuthorUserId);
```

- [ ] **Step 2: Run build to verify configs are not present**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS before change, but no EF mappings exist for community entities.

- [ ] **Step 3: Add entity configurations and `DbSet`s**

Rules to enforce:

- `CommunityPost.Title` max length 200
- `CommunityComment.Content` required
- composite keys for tag and reaction bridge entities
- soft-delete fields kept as plain columns

- [ ] **Step 4: Run build to verify mappings compile**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add SmartMentor.Domain/EntityConfigurations/CommunityPostConfiguration.cs SmartMentor.Domain/EntityConfigurations/CommunityPostCareerGoalTagConfiguration.cs SmartMentor.Domain/EntityConfigurations/CommunityCommentConfiguration.cs SmartMentor.Domain/EntityConfigurations/CommunityPostReactionConfiguration.cs SmartMentor.Persistence/Data/ApplicationDbContext.cs
git commit -m "feat: map community discussion entities"
```

### Task 3: Add Community DTOs and Service Contract

**Files:**
- Create: `SmartMentor.Abstraction/Dto/Requests/CommunityRequests/CreateCommunityPostRequest.cs`
- Create: `SmartMentor.Abstraction/Dto/Requests/CommunityRequests/CreateCommunityCommentRequest.cs`
- Create: `SmartMentor.Abstraction/Dto/Responses/CommunityResponses/CommunityPostSummaryResponse.cs`
- Create: `SmartMentor.Abstraction/Dto/Responses/CommunityResponses/CommunityPostDetailsResponse.cs`
- Create: `SmartMentor.Abstraction/Services/CommunityService/ICommunityService.cs`

- [ ] **Step 1: Write the failing API contract expectation**

Required methods:

```csharp
Task<IReadOnlyList<CommunityPostSummaryResponse>> GetPostsByCareerGoalAsync(int careerGoalId, Guid currentUserId, CancellationToken cancellationToken = default);
Task<CommunityPostDetailsResponse> GetPostByIdAsync(int postId, Guid currentUserId, CancellationToken cancellationToken = default);
Task<CommunityPostDetailsResponse> CreatePostAsync(Guid userId, CreateCommunityPostRequest request, CancellationToken cancellationToken = default);
Task<CommunityCommentResponse> AddCommentAsync(Guid userId, int postId, CreateCommunityCommentRequest request, CancellationToken cancellationToken = default);
Task ToggleLikeAsync(Guid userId, int postId, CancellationToken cancellationToken = default);
```

- [ ] **Step 2: Run build to verify contract is absent**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS before change, but no community DTO/service contract exists.

- [ ] **Step 3: Add request/response DTOs and the interface**

Include response fields for:

- author name
- primary career goal
- tag list
- like count
- comment count
- current-user-liked flag

- [ ] **Step 4: Run build to verify DTOs compile**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add SmartMentor.Abstraction/Dto/Requests/CommunityRequests/CreateCommunityPostRequest.cs SmartMentor.Abstraction/Dto/Requests/CommunityRequests/CreateCommunityCommentRequest.cs SmartMentor.Abstraction/Dto/Responses/CommunityResponses/CommunityPostSummaryResponse.cs SmartMentor.Abstraction/Dto/Responses/CommunityResponses/CommunityPostDetailsResponse.cs SmartMentor.Abstraction/Services/CommunityService/ICommunityService.cs
git commit -m "feat: add community DTOs and service contract"
```

### Task 4: Implement Community Service

**Files:**
- Create: `SmartMentor.Application/Implementations/CommunityService/CommunityService.cs`

- [ ] **Step 1: Write the failing service behavior test plan**

Behavior targets:

- creating a post validates the primary career goal and all tags
- community feed includes posts by primary goal or tag
- duplicate tags do not duplicate results
- liking the same post twice toggles off or remains single-row depending on chosen behavior

- [ ] **Step 2: Run build to verify service is missing**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS before change, but `ICommunityService` has no implementation.

- [ ] **Step 3: Implement minimal service methods**

Service responsibilities:

- resolve and validate authenticated user
- validate career-goal IDs
- create post and bridge tags
- fetch post summaries with counts
- fetch post details with comments
- create comments
- toggle like rows in `CommunityPostReaction`

- [ ] **Step 4: Run build to verify service compiles**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add SmartMentor.Application/Implementations/CommunityService/CommunityService.cs
git commit -m "feat: implement community discussion service"
```

### Task 5: Register Community Service

**Files:**
- Modify: `SmartMentorApi/Extentions/ServiceCollectionExtentions.cs`

- [ ] **Step 1: Write the failing DI expectation**

Required registration:

```csharp
services.AddScoped<ICommunityService, CommunityService>();
```

- [ ] **Step 2: Run build to verify DI is missing**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS before change, but controller injection would fail once added.

- [ ] **Step 3: Add DI registration**

- [ ] **Step 4: Run build to verify registration compiles**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add SmartMentorApi/Extentions/ServiceCollectionExtentions.cs
git commit -m "feat: register community service"
```

### Task 6: Add Community Controller Endpoints

**Files:**
- Create: `SmartMentorApi/Controllers/CommunityController/CommunityController.cs`

- [ ] **Step 1: Write the failing endpoint expectation**

Required endpoints:

```csharp
[HttpGet("career-goals/{careerGoalId}/posts")]
[HttpGet("posts/{postId}")]
[HttpPost("posts")]
[HttpPost("posts/{postId}/comments")]
[HttpPost("posts/{postId}/like")]
```

- [ ] **Step 2: Run build to verify controller is missing**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS before change, but no API surface exists for the feature.

- [ ] **Step 3: Implement controller using the existing response style**

Controller rules:

- `[Authorize]`
- read current user ID from claims
- use `SuccessResponse` and `ErrorResponse`
- return `BadRequest` for invalid route/body states
- return `NotFound` for missing posts or career goals

- [ ] **Step 4: Run build to verify controller compiles**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add SmartMentorApi/Controllers/CommunityController/CommunityController.cs
git commit -m "feat: add community discussion endpoints"
```

### Task 7: Add Database Migration

**Files:**
- Create: `SmartMentor.Persistence/Migrations/<timestamp>_AddCommunityDiscussions.cs`
- Create: `SmartMentor.Persistence/Migrations/<timestamp>_AddCommunityDiscussions.Designer.cs`
- Modify: `SmartMentor.Persistence/Migrations/ApplicationDbContextModelSnapshot.cs`

- [ ] **Step 1: Write the failing migration expectation**

Expected schema additions:

- `CommunityPosts`
- `CommunityComments`
- `CommunityPostCareerGoalTags`
- `CommunityPostReactions`

- [ ] **Step 2: Generate or write migration**

Run:

```bash
dotnet ef migrations add AddCommunityDiscussions --project SmartMentor.Persistence --startup-project SmartMentorApi
```

Expected: migration files created successfully

- [ ] **Step 3: Review migration output**

Verify:

- foreign keys point to `AspNetUsers` and `CareerGoals`
- composite keys exist for tag/reaction tables
- no unintended changes to unrelated tables

- [ ] **Step 4: Build after migration**

Run: `dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add SmartMentor.Persistence/Migrations SmartMentor.Persistence/Migrations/ApplicationDbContextModelSnapshot.cs
git commit -m "feat: add community discussion schema"
```

### Task 8: Verify End-to-End

**Files:**
- Modify if needed: community files only

- [ ] **Step 1: Apply migration**

Run:

```bash
dotnet ef database update --project SmartMentor.Persistence --startup-project SmartMentorApi
```

Expected: database update completes successfully

- [ ] **Step 2: Run the application build**

Run:

```bash
dotnet build SmartMentorApi\SmartMentorApi.sln --no-restore
```

Expected: `0 errors`

- [ ] **Step 3: Smoke test endpoints**

Test these flows:

- create a post with multiple tags
- fetch community feed for the primary goal
- fetch community feed for a tagged goal
- add a comment
- like a post
- unlike the same post

- [ ] **Step 4: Final commit**

```bash
git add .
git commit -m "feat: add community discussions feature"
```

---

## Self-Review

- Spec coverage:
  - posts, comments, likes: covered
  - multiple career-goal tags: covered
  - open browsing across communities: covered
  - layered architecture: covered through dedicated service and controller tasks
- Placeholder scan:
  - migration timestamp remains to be generated at execution time
  - endpoint implementations and DTO names are concrete
- Type consistency:
  - one dedicated `CommunityService`
  - one primary-career-goal plus tag bridge model across all tasks

The only intentional variable is the actual EF migration timestamp, which must be generated when executing the plan.
