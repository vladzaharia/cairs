<?php
/// Google Auth Setup
require_once (__DIR__ . '/gapi/src/apiClient.php');
require_once (__DIR__ . '/gapi/src/contrib/apiOauth2Service.php');
session_start();

// Setup OAuth Parameters
$client = new apiClient();
$client->setApplicationName("/sasquatch/ Login");
$client->setClientId('834418064869.apps.googleusercontent.com');
$client->setClientSecret('QLhDD5Hxyf5DwtPseVkvkbk2');
$client->setRedirectUri('http://wearesasquatch.com/team/login.php');
$client->setDeveloperKey('AIzaSyABrjVfPjHJHUdZRAcb3bWoUXqrjrpHCOE');
$client->setState($_GET['state']);

// Setup OAuth Object
$oauth2 = new apiOauth2Service($client);

if (isset($_GET['code'])) {
  $client->authenticate();
  $_SESSION['token'] = $client->getAccessToken();
  $redirect = 'http://' . $_SERVER['HTTP_HOST'] . $_SERVER['PHP_SELF'];
  header('Location: ' . filter_var($redirect, FILTER_SANITIZE_URL));
}

if (isset($_SESSION['token'])) {
 $client->setAccessToken($_SESSION['token']);
}

if ($client->getAccessToken()) {
  $user = $oauth2->userinfo->get();

  $email = $user['email'];

  $db = mysql_connect('localhost', '319', 'foobar');
  mysql_select_db('319');

  $login_query = "SELECT * FROM `user` WHERE `email` = '" . $email . "';";
  $login_result = mysql_query($login_query);
  $login = mysql_fetch_assoc($login_result);

  echo $login_query;

  if ($login) {
  	// Store Access Token in a session
  	$_SESSION['email'] = $email;
    $_SESSION['user'] = $login['id'];

    if ($_GET['state']) {
      if ($_GET['state'] === "protected") {
        $login2_query = "SELECT * FROM `protected_login` WHERE `username` = 'team';";
        $login2_result = mysql_query($login2_query);
        $login2 = mysql_fetch_assoc($login2_result);

        $_SESSION['username'] = 'team';
        $_SESSION['password'] = $login2['password'];
        header('Location: ../protected/home.php');
      }
    } else {
  	  header('Location: home.php');
    }
  	exit;
  } else {
    unset($_SESSION['token']);
    $client->revokeToken();
    header('Location: index.html');
    exit;
  }
} else {
  if (isset($_REQUEST['error'])) 
    header('Location: index.html');
  else 
    header('Location: ' . $client->createAuthUrl());
	exit;
}
?>