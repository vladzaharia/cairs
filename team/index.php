<?php
	session_start();
	if (isset($_SESSION['user'])) {
		header('location: home.php');
	}
?>

<html lang="en">
<head>
	<title>/sasquatch/ Team Area Login</title>
	<meta name="robots" content="noindex"> <!-- Prevent Google from crawling -->

	<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,300,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="../css/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="../css/site.css" />
	<script type="text/javascript" src="../js/jquery-1.8.3.js"></script>
	<script type="text/javascript" src="../js/bootstrap.min.js"></script>
	<script type="text/javascript" src="../js/bootstrap-anim.min.js"></script>
	<link rel="shortcut icon" href="../images/favicon.ico" type="image/x-icon" />

	<script type="text/javascript">
		$(function() {
			$(".photo").mouseenter(function() {
				$(this).addClass("hover");
			}).mouseleave(function() {
				$(this).removeClass("hover");
			});

			$(".collapse").collapse()
		});
	</script>
</head>
<body class="login">
	<div id="container">
		<section id="home-team" class="login">
			<div class="container">
				<div class="row">
					<div class="span12 center">
						<div class="logo"></div>
					</div>
				</div>
			</div>
		</section>

		<section id="progress">
			<div class="container">
	            <div class="row">
            		<div class="span12 center">
	    				<a onClick="_gaq.push(['_trackEvent', 'Login', 'Click', 'Team']);" ref="login.php" class="login_google"></a>
	    			</div>
	            </div>
	        </div>
		</section>

		<section id="footer" class="login">
			Site handcrafted by Vlad Zaharia for sasquatch.
		</section>
	</div>
	<script type="text/javascript">
	  var _gaq = _gaq || [];
	  _gaq.push(['_setAccount', 'UA-253443-13']);
	  _gaq.push(['_trackPageview']);

	  (function() {
	    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
	    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
	    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
	  })();
	</script>
</body>
</html>