# C# application with Patreon API
Using a C# application to obtain an OAuth2 authorization code, then an Access Token from Patreon for use. 

# Resources
The source code here, is a C# WPF application that uses an HTML website to connect to Patreon's API.
Node version: 6.0

Dependancies/NutGets: Newtonsoft

You can open Microsoft Visual Studio and create a new C# WPF application then import the code given here if that's easier for you

# Redirect URI/Server
I put redirect.html on my website which I hosted with Linode and used it as an endpoint like so: https://mywebsite.com/redirect. This means, that is my redirectURI.
 
MAKE SURE TO INCLUDE THAT URL IN YOUR PATREON CLIENT
