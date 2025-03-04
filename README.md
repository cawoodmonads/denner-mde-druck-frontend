# denner-mde-druck
MDE Druck

## Deployment
* Deployment is managed by [GitHub Actions](https://github.com/cawoodmonads/denner-mde-druck-frontend/actions)
  * FrontEnd Angular app is built and hosted
  * Backend CSharp API is built and hosted
* Site is deployed to: https://proud-pond-06779f303.6.azurestaticapps.net/

## Security


## Access Control
New users can be invited to the app from the Azure Portal -> Static Web Apps -> denner-mde-druck-frontend -> Settings -> Role Management -> Invite

## Configuration
Authentication and Authorization rules are defined in /staticwebapp.config.json [Link](https://learn.microsoft.com/en-us/azure/static-web-apps/configuration)
User information is visible from the path `/.auth/me` [docs](https://learn.microsoft.com/en-us/azure/static-web-apps/user-information)

**Notes**
* `"redirect": "/.auth/login/aad"` Sets the identity provider to Azure Active Directory (aka Microsoft Entra)