# auth-me Specification

## Purpose
Authenticated user self-profile retrieval via `GET /api/auth/me`. TBD - expand with broader auth-me capability context if needed.

## Requirements

### Requirement: Authenticated user can retrieve own profile via GET /api/auth/me

The system SHALL expose `GET /api/auth/me` that returns the `GetUserResponse` of the user identified by the validated JWT access token's `NameIdentifier` claim.

#### Scenario: Authenticated user retrieves own profile
- **WHEN** an authenticated request with a valid Bearer access token calls `GET /api/auth/me`
- **THEN** the system responds `200 OK` with `GetUserResponse` containing the requester's `Id`, `FullName`, `BirthDate`, `Phone`, `Email` and `IsEmailConfirmed`

#### Scenario: Unauthenticated request is rejected
- **WHEN** a request without a Bearer token calls `GET /api/auth/me`
- **THEN** the system responds `401 Unauthorized`

#### Scenario: Invalid or expired token is rejected
- **WHEN** a request with an invalid or expired Bearer token calls `GET /api/auth/me`
- **THEN** the system responds `401 Unauthorized`

#### Scenario: Token identity is unusable
- **WHEN** the validated token has no `NameIdentifier` claim or the claim value is not a valid user id
- **THEN** the system responds `401 Unauthorized`

#### Scenario: Token user no longer exists
- **WHEN** the `NameIdentifier` id has no matching user record
- **THEN** the system responds `404 Not Found`

### Requirement: Endpoint is available to any authenticated role

The system SHALL allow both `user` and `admin` roles to call `GET /api/auth/me` (authentication required, no role restriction), and SHALL NOT change the existing admin-only restriction of `GET /api/auth/user/{id}`.

#### Scenario: Standard user accesses own profile
- **WHEN** a valid token with role `user` calls `GET /api/auth/me`
- **THEN** the system responds `200 OK` with that user's `GetUserResponse`

#### Scenario: Admin accesses own profile
- **WHEN** a valid token with role `admin` calls `GET /api/auth/me`
- **THEN** the system responds `200 OK` with that admin's `GetUserResponse`

#### Scenario: Existing admin-only lookup unchanged
- **WHEN** a valid token with role `user` calls `GET /api/auth/user/{id}`
- **THEN** the system still responds `403 Forbidden` (behavior unchanged)
