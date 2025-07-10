# SampleBlazorOIDPing

Sample Blazor (Server) project using .Net Core 8, OpenIdConnect, and Ping authentication
## Features

- ✅ Blazor Server with Interactive Server Components
- ✅ ASP.NET Core Identity for user management
- ✅ OpenID Connect integration for external authentication
- ✅ Entity Framework Core with SQL Server
- ✅ Responsive UI with Bootstrap

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB included with Visual Studio)
- An OpenID Connect provider (Azure AD, Auth0, Google, Ping Identity, etc.)

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd SampleBlazorOIDPing
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update database**
   ```bash
   dotnet ef database update
   ```

4. **Configure OpenID Connect** (see configuration section below)

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Navigate to** `https://localhost:7086`

## OpenID Connect Configuration

### 1. Install Required Package

The OpenID Connect package is already included in the project:
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="8.0.17" />
```

### 2. Configure Your Provider

Update `appsettings.Development.json` with your provider's settings:

```json
{
  "OpenIdConnect": {
    "Authority": "https://your-provider-authority",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "ResponseType": "code",
    "SaveTokens": true,
    "GetClaimsFromUserInfoEndpoint": true,
    "Scope": "openid profile email"
  }
}
```

### 3. Common Provider Examples

#### Azure AD / Microsoft Entra ID (Recommended for testing)

```json
{
  "OpenIdConnect": {
    "Authority": "https://login.microsoftonline.com/{tenant-id}/v2.0",
    "ClientId": "your-application-client-id",
    "ClientSecret": "your-client-secret",
    "ResponseType": "code",
    "SaveTokens": true,
    "GetClaimsFromUserInfoEndpoint": false,
    "Scope": "openid profile email"
  }
}
```

**Setup Steps:**
1. Go to [Azure Portal](https://portal.azure.com) (free account available)
2. Navigate to **Azure Active Directory** > **App registrations**
3. Create new registration with redirect URI: `https://localhost:7086/signin-oidc`
4. Generate client secret and copy values

#### Ping Identity

```json
{
  "OpenIdConnect": {
    "Authority": "https://your-ping-domain.com",
    "ClientId": "your-ping-client-id",
    "ClientSecret": "your-ping-client-secret",
    "ResponseType": "code",
    "SaveTokens": true,
    "GetClaimsFromUserInfoEndpoint": true,
    "Scope": "openid profile email"
  }
}
```

#### Auth0

```json
{
  "OpenIdConnect": {
    "Authority": "https://your-domain.auth0.com",
    "ClientId": "your-auth0-client-id",
    "ClientSecret": "your-auth0-client-secret",
    "ResponseType": "code",
    "SaveTokens": true,
    "GetClaimsFromUserInfoEndpoint": true,
    "Scope": "openid profile email"
  }
}
```

#### Google

```json
{
  "OpenIdConnect": {
    "Authority": "https://accounts.google.com",
    "ClientId": "your-google-client-id.googleusercontent.com",
    "ClientSecret": "your-google-client-secret",
    "ResponseType": "code",
    "SaveTokens": true,
    "GetClaimsFromUserInfoEndpoint": true,
    "Scope": "openid profile email"
  }
}
```

## Project Structure

```
SampleBlazorOIDPing/
├── Components/
│   ├── Account/           # Identity UI components
│   ├── Layout/            # App layout components
│   └── Pages/             # Application pages
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── ApplicationUser.cs
│   └── Migrations/
├── Program.cs             # Application configuration
├── appsettings.json       # Base configuration
└── appsettings.Development.json  # Development secrets
```

## How It Works

1. **Local Authentication**: Users can register/login with email/password using ASP.NET Core Identity
2. **External Authentication**: Users can login using configured OpenID Connect providers
3. **Account Linking**: External accounts are automatically linked to local user accounts
4. **Claims Integration**: User claims from external providers are mapped to local identity

## Security Notes

- ✅ Sensitive configuration is stored in `appsettings.Development.json` (ignored by Git)
- ✅ Production secrets should use Azure Key Vault or environment variables
- ✅ HTTPS is enforced in production
- ✅ Proper error handling for authentication failures

## Troubleshooting

### Common Issues

1. **"Invalid redirect URI"**: Ensure redirect URI matches exactly: `https://localhost:7086/signin-oidc`
2. **"Authority not found"**: Verify the Authority URL is correct and accessible
3. **"Client authentication failed"**: Check ClientId and ClientSecret values
4. **Database errors**: Run `dotnet ef database update` to apply migrations

### Debug Logging

Enable detailed authentication logging in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Microsoft.AspNetCore.Authentication.OpenIdConnect": "Debug"
    }
  }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
