# Community Discussions Design

**Date:** 2026-05-16

## Goal

Add a community discussions feature where logged-in users can browse any career-goal community, create discussion posts, comment, and like posts. Posts must support multiple career-goal tags so one discussion can appear across related communities.

## Product Scope

### Included in V1

- Career-goal discussion communities
- Post creation by authenticated users
- One primary career goal per post
- Multiple career-goal tags per post
- Post listing by community
- Post details with comments
- Comment creation by authenticated users
- Post likes/upvotes by authenticated users
- Admin-safe architecture using the existing layered pattern

### Excluded from V1

- Comment replies
- Post editing history
- Reporting/moderation workflows
- Saved posts
- Rich media attachments
- Real-time notifications

## Recommended Approach

Use a hybrid model:

- Each post has a **primary career goal** that acts as its home community.
- Each post can also have **multiple career-goal tags** through a bridge table.
- Community feeds show posts where either:
  - `PrimaryCareerGoalId == requestedCareerGoalId`
  - or the post has a matching tag for that career goal

This approach preserves a strong community identity while still supporting cross-community discovery.

## Architecture

The feature should follow the current project architecture:

- `SmartMentor.Domain`: entities and EF configuration
- `SmartMentor.Abstraction`: DTOs and service contracts
- `SmartMentor.Application`: business logic service implementation
- `SmartMentor.Persistence`: migrations and data access
- `SmartMentorApi`: controller endpoints

To avoid bloating `AdminService` or `UserProfileService`, community behavior should live in a dedicated `CommunityService`.

## Domain Model

### `CommunityPost`

Represents a user-created discussion post.

Suggested fields:

- `Id`
- `AuthorUserId`
- `Title`
- `Content`
- `PrimaryCareerGoalId`
- `CreatedAt`
- `UpdatedAt`
- `IsDeleted`

Navigation:

- `Author`
- `PrimaryCareerGoal`
- `Comments`
- `CareerGoalTags`
- `Reactions`

### `CommunityPostCareerGoalTag`

Bridge entity for multi-career-goal tagging.

Suggested fields:

- `PostId`
- `CareerGoalId`

Navigation:

- `Post`
- `CareerGoal`

### `CommunityComment`

Represents comments on posts.

Suggested fields:

- `Id`
- `PostId`
- `AuthorUserId`
- `Content`
- `CreatedAt`
- `UpdatedAt`
- `IsDeleted`

Navigation:

- `Post`
- `Author`

### `CommunityPostReaction`

Represents one like/upvote per user per post.

Suggested fields:

- `PostId`
- `UserId`
- `CreatedAt`

Navigation:

- `Post`
- `User`

Constraint:

- composite primary key on `PostId + UserId`

## Authorization Rules

- Any authenticated user can browse communities.
- Any authenticated user can create posts.
- Any authenticated user can comment on posts.
- Any authenticated user can like/unlike posts.
- Only the post/comment author should be able to edit or soft-delete their own content in future versions.
- Admin moderation can be added later without changing the core model.

## API Shape

### Community browsing

- `GET /api/community/career-goals/{careerGoalId}/posts`
  - returns posts visible in that community

- `GET /api/community/posts/{postId}`
  - returns full post details with comments and reaction summary

### Post actions

- `POST /api/community/posts`
  - create a post with a primary career goal and tag list

### Comment actions

- `POST /api/community/posts/{postId}/comments`
  - add a comment

### Reaction actions

- `POST /api/community/posts/{postId}/like`
  - like a post

- `DELETE /api/community/posts/{postId}/like`
  - remove like

## DTO Direction

Use dedicated community DTOs rather than exposing entities.

Expected DTO groups:

- create post request
- create comment request
- post summary response
- post details response
- comment response
- reaction summary response

Post responses should include:

- author basic info
- primary career goal
- tag list
- comment count
- like count
- whether the current user liked the post

## Query Behavior

### Community feed query

The feed for a career goal should:

- include posts whose primary career goal matches
- include posts tagged with that career goal
- avoid duplicates when both match
- sort newest first by default

### Post detail query

Should load:

- post author
- primary career goal
- tagged career goals
- comments with authors
- reactions summary

## Data Integrity Rules

- Every post must have exactly one primary career goal.
- A post can have zero or more additional tags.
- The primary career goal may also appear inside the tags list, but the service should normalize this to avoid duplicates if preferred.
- A user can react to a post only once.
- Soft-deleted posts/comments should not appear in regular queries.

## Error Handling

The community service should validate:

- authenticated user exists
- primary career goal exists
- all tag career goals exist
- post exists before comment/like actions
- duplicate likes do not create duplicate records

Controllers should return the same response style already used elsewhere in the API.

## Testing Strategy

At minimum, test:

- creating a post with multiple career-goal tags
- listing a community feed includes tagged posts
- post details load comments and like counts
- liking a post twice does not duplicate the reaction
- removing a like works correctly

## Implementation Order

1. Domain entities and EF configuration
2. Migration
3. DTOs and service interface
4. Community service
5. Controller endpoints
6. Verification build/tests

## Notes

This design deliberately avoids introducing explicit “community membership” because the product requirement is open browsing across career goals. That keeps V1 aligned with the user experience you described while preserving room for future group membership features if needed.
