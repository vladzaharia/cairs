<?php
	session_start();
	$db = mysql_connect('localhost', '319', 'foobar');
	mysql_select_db('319');

	if (isset($_SESSION['username']) && isset($_SESSION['password'])) {

		$login_query = "SELECT * FROM `protected_login` WHERE `username` = '" . $_SESSION['username'] . "';";
		$login_result = mysql_query($login_query);
		$login = mysql_fetch_assoc($login_result);
	   	
	   	if ($login['password'] === $_SESSION['password']) {
	   		header("location: home.php");
	   	}
	}

	if (isset($_REQUEST['password'])) {
		$salted_hash = md5($_REQUEST['password'] . "7513c22e67b476366339039082a5f2f4");

		$login_query = "SELECT * FROM `protected_login` WHERE `username` = '" . $_REQUEST['user'] . "';";
		$login_result = mysql_query($login_query);
		$login = mysql_fetch_assoc($login_result);
	   	
	   	if ($login['password'] === $salted_hash) {
	   		$_SESSION['username'] = $login['username'];
	   		$_SESSION['password'] = $login['password'];

	   		header("location: home.php");
	   	}
	}
?>
<html lang="en">
<head>
	<title>/sasquatch/ Protected Login</title>
	<meta name="robots" content="noindex"> <!-- Prevent Google from crawling -->

	<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,300,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="../css/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="../css/site.css" />
	<script type="text/javascript" src="../js/jquery-1.8.3.js"></script>
	<script type="text/javascript" src="../js/bootstrap.min.js"></script>
	<script type="text/javascript" src="../js/bootstrap-anim.min.js"></script>
	<link rel="shortcut icon" href="../images/favicon.ico" type="image/x-icon" />
</head>
<body class="login">
	<div id="container">
		<section id="header" class="login">
			<div class="wearesasquatch"></div>
		</section>

		<section id="progress" class="login">
			<div class="container">
	            <div class="row">
	                <div class="span12 center">
	                    <div class="title">
	                        <h1>Login</h1>
	                    </div>
	                </div>
	            </div>

	            <div class="row">
	            	<div class="span12 center">
	            		<br />&nbsp;
	            		Please select your role to continue.<br />&nbsp;
	            	</div>
	            </div>

	            <div class="row login">
            		<div class="span2 center">
	    				<a href="index.php?user=prof" class="photo prof"></a>
	    			</div>

	    			<div class="span2 center">
	    				<a href="index.php?user=ta" class="photo ta"></a>
	    			</div>

	    			<div class="span2 center">
	    				<a href="../team/login.php?state=protected" class="photo team"></a>
	    			</div>
	            </div>

	            <?php if($_REQUEST['user']) { ?>
	            	<div class="row password">
	            		<div class="span6 center">
	            			<form action="index.php" method="POST" class="form-horizontal">
						    	<input type="password" id="password" name="password" class="input-large" placeholder="Password" />
						    	<input type="hidden" name="user" value="<?php echo $_REQUEST['user']; ?>" />
								<button type="submit" class="btn btn-primary">Login</button>
							</form>
	            		</div>
	            	</div>
	            <?php } ?>
	        </div>
		</section>

		<section id="footer" class="login">
			Site handcrafted by Vlad Zaharia for sasquatch.
		</section>
	</div>
</body>
</html>