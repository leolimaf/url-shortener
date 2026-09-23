## ADDED Requirements

### Requirement: Authenticated user can log out and revoke the session refresh token via POST /api/auth/logout

The system SHALL expose `POST /api/auth/logout` requiring a valid Bearer access token; on success the system SHALL clear the requesting user's `RefreshTokenHash` and `RefreshTokenExpiryTime` (both set to null and persisted) so the previous refresh token can no longer be used, and SHALL respond `204 No Content`.

#### Scenario: Authenticated user logs out successfully
- **WHEN** an authenticated request with a valid Bearer access token calls `POST /api/auth/logout`
- **THEN** the system clears that user's `RefreshTokenHash` and `RefreshTokenExpiryTime` and responds `204 No Content`

#### Scenario: Revoked refresh token can no longer be used
- **WHEN** a client calls `POST /api/auth/refresh-token` with the refresh token revoked by a prior logout
- **THEN** the system responds `401 Unauthorized` (no new tokens issued)

#### Scenario: Logout is idempotent when no refresh token is stored
- **WHEN** an authenticated user with no stored refresh token (already logged out) calls `POST /api/auth/logout`
- **THEN** the system responds `204 No Content` without error

#### Scenario: Unauthenticated logout is rejected
- **WHEN** a request without a Bearer token calls `POST /api/auth/logout`
- **THEN** the system responds `401 Unauthorized` and no user state changes

#### Scenario: Invalid or expired access token is rejected
- **WHEN** a request with an invalid or expired Bearer token calls `POST /api/auth/logout`
- **THEN** the system responds `401 Unauthorized` and no user state changes

#### Scenario: Token identity is unusable
- **WHEN** the validated token has no `NameIdentifier` claim or the claim value is not a valid user id
- **THEN** the system responds `401 Unauthorized` and no user state changes

### Requirement: Access token lifetime is 60 minutes

The system SHALL issue access tokens expiring 60 minutes after issuance (`JwtSettings:ExpirationInMinutes = 60`).

#### Scenario: Newly issued access token carries 60-minute expiry
- **WHEN** a client successfully calls `POST /api/auth/access-token` or `POST /api/auth/refresh-token`
- **THEN** the returned access token expires approximately 60 minutes after issuance

### Requirement: Front-end completes logout by discarding local tokens only after server confirmation

The client SHALL call `POST /api/auth/logout` with the valid Bearer access token and SHALL discard locally stored tokens (regardless of local/session storage) only after receiving a success status; on `401` the client SHALL discard locally stored tokens without retrying logout.

#### Scenario: Front-end clears storage after successful logout
- **WHEN** the server responds `204` (or `200`) to `POST /api/auth/logout`
- **THEN** the client discards the stored access and refresh tokens and redirects to login

#### Scenario: Front-end handles expired access on logout
- **WHEN** the server responds `401` to `POST /api/auth/logout`
- **THEN** the client discards the stored tokens locally without retrying the logout call
