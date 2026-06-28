# GatherUp - Event Management System

A REST API for managing events, participants, polls, and finances.

## Architecture

```
GatherUp.Core           → Models, Interfaces, Exceptions
GatherUp.BL             → Business Logic (Services)
GatherUp.Infrastructure → Data Access (XML), Email, Notifications
GatherUp.API            → REST API (Controllers, JWT)
```

## Getting Started

### Prerequisites
- .NET 8 SDK

### Run
```bash
cd GatherUp.API
dotnet run
```

Swagger UI is available at `https://localhost:{port}/swagger` in development.

### Authentication
1. `POST /api/auth/register` - create a new account
2. `POST /api/auth/login` - returns a JWT token
3. Click **Authorize** in Swagger and enter: `Bearer <token>`

## API Endpoints

### Auth
| Method | Endpoint | Auth |
|--------|----------|------|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |

### Events
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | `/api/events/{id}` | Public |
| GET | `/api/events/user/{userId}` | Required |
| POST | `/api/events` | Manager |
| PUT | `/api/events` | Required (owner only) |

### Participants
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | `/api/participants/event/{eventId}` | Required |
| GET | `/api/participants/event/{eventId}/pending` | Required |
| POST | `/api/participants/event/{eventId}` | Manager |
| PUT | `/api/participants/{id}/attendance` | Required |
| PUT | `/api/participants/{id}/payment` | Manager |

### Polls
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | `/api/polls/{pollId}/results` | Required |
| POST | `/api/polls/event/{eventId}` | Manager |
| POST | `/api/polls/{pollId}/question` | Manager |
| POST | `/api/polls/{pollId}/answer` | Required |

### Financial
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | `/api/financial/event/{eventId}/vendors` | Manager |
| GET | `/api/financial/event/{eventId}/summary` | Manager |
| GET | `/api/financial/event/{eventId}/receipts` | Manager |
| POST | `/api/financial/event/{eventId}/vendor` | Manager |
| PUT | `/api/financial/vendor/{vendorId}/debt` | Manager |

## Roles

| Role | Description |
|------|-------------|
| `Manager` | Full access - creates events, manages participants and finances |
| `Host` | Can view and update event details |
| `Participant` | Can confirm attendance, submit poll answers |

## Data Storage

Data is persisted as XML files under `GatherUp.API/DataXML/`:
- `Event.xml`, `Participant.xml`, `EventManager.xml`, `EventHost.xml`
- `VendorAllocation.xml`, `Poll.xml`
- `ReceiptDetails.xml`, `UploadedReceipts/`
- `emails.txt` - log of all sent email notifications

## Configuration

`appsettings.json`:
```json
{
  "Jwt": {
    "Key": "<your-secret-key>",
    "Issuer": "<issuer>",
    "Audience": "<audience>",
    "ExpiryMinutes": 60
  }
}
```
