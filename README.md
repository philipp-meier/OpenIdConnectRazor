# OpenIdConnectRazor
Demonstrates how Keycloak (v18.0.0) OpenID Connect authentication can be used with ASP.NET Core Razor Pages.  

## Summary
This is an alternative to using ASP.NET Core Identity. Keycloak is used as an Identity Provider (IdP), so the application does not have to manage its own identities and only consumes the OIDC information.  

To achieve this, two authentication schemes are used:
- **OpenID Connect**: Used to challenge the authentication. Users will be redirected to the OIDC provider, so they can log in and return with an identity.
- **Cookie**: To persist the identity of the user by using the OIDC information.

When logging out, the cookie will be removed and a logout request to the IdP will be made.

## Usage
By clicking on the `Privacy`-menu, the user will be redirected to the Keycloak login page.  
The user will be redirected to the `Privacy` page again, once he is authenticated and the `Logout` menu appears.

## Keycloak settings
Ensure that your Keycloak client uses the `openid-connect` protocol and the access type is set to `confidential`.  
To test this locally, your configured root URL should be https://localhost:7298/ and the "valid redirect URIs" should be the following:
- http://localhost:5211/*
- http://localhost:5211/*