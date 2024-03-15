# C# application with Patreon API
Using a C# application to obtain an OAuth2 authorization code, then an Access Token from Patreon for use. 

![Cat](https://cdn.discordapp.com/attachments/765174282010492941/1218270406872399892/PatreonExample.png?ex=66070dbb&is=65f498bb&hm=17c125bf91568951af0b8b5e2183b41d3ff5035f3349cd5687adf87fb7832396&)

# Resources
The source code here, is a C# WPF application that uses an HTML website to connect to Patreon's API.
Node version: 6.0

Dependancies/NutGets: Newtonsoft

You can open Microsoft Visual Studio and create a new C# WPF application then import the code given here if that's easier for you

# Redirect URI/Server
I put redirect.html on my website which I hosted with Linode and used it as an endpoint like so: https://mywebsite.com/redirect. This means, that is my redirectURI.
 
MAKE SURE TO INCLUDE THAT URL IN YOUR PATREON CLIENT

# C# Client Application
The code I've given here is an example of a use for Patreon's API in C#. In order, click the top button, then middle, then bottom. 
