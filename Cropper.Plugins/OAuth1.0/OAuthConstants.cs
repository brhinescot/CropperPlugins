// OAuthConstants.cs
// ------------------------------------------------------------------
//
// Constants for use with OAuth.
//
// ------------------------------------------------------------------
//
// Copyright (c) 2011 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

namespace CropperPlugins.OAuth
{
  public static class OAuthConstants
  {
      public static readonly string
          URL_REQUEST_TOKEN       = "https://api.twitter.com/oauth/request_token",
          URL_AUTHORIZE           = "https://api.twitter.com/oauth/authorize?oauth_token=",
          URL_ACCESS_TOKEN        = "https://api.twitter.com/oauth/access_token",
          URL_VERIFY_CREDS        = "https://api.twitter.com/1/account/verify_credentials.json",
          AUTHENTICATION_REALM    = "http://api.twitter.com/";
  }
}
