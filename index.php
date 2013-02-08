<?php
	// CS319 Project - Team Task Entry
	session_start();
?>

<html lang="en">
<head>
	<title>/sasquatch/</title>
	<meta name="robots" content="noindex"> <!-- Prevent Google from crawling -->

	<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,300,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="css/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="css/site.css" />
	<script type="text/javascript" src="js/jquery-1.8.3.js"></script>
	<script type="text/javascript" src="js/bootstrap.min.js"></script>
	<script type="text/javascript" src="js/bootstrap-anim.min.js"></script>
	<link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />

	<meta name="description" content="sasquatch is a group of 6 developers working on awesome software for CS 319 at UBC." />
	<meta name="author" content="Vlad Zaharia" />
	<meta name="keywords" content="ubc, cs, cs319, 319, sasquatch, software, software development, web, web development, vancouver, canada, bc, we are sasquatch, vlad, zaharia, vlad zaharia, michelle, chuang, michelle chuang, oliver, bozek, oliver bozek, thea, simpson, thea simpson, hanna, yoo, hanna yoo, jing, zhu, jing zhu" />
	<meta name="copyright" content="2012 Vlad Zaharia" />

	<script type="text/javascript">
		$(function() {
			$(".photo").mouseenter(function() {
				$(this).addClass("hover");
			}).mouseleave(function() {
				$(this).removeClass("hover");
			});

			$(".collapse").collapse();

			$(".tasks-progress").popover({
				'html': true,
				'placement': 'right',
				'trigger': 'hover'
			});
		});
	</script>
</head>
<body>
	<?php if (isset($_SESSION['username'])) { ?>
		<div class="navbar navbar-fixed-top navbar-inverse">
		  	<div class="navbar-inner">
		    	<a class="brand" href="#"></a>
		    	<ul class="nav">
		      		<li class="active"><a href="/">home</a></li>
		      		<li><a href="/protected">/protected</a></li>
		      		<?php if ($_SESSION['username'] === 'team') { ?> <li><a href="/team">/team</a></li> <?php } ?>
		      		<li><a href="https://github.com/vladzaharia/cloaked-octo-bear">/repo</a></li>
	      			<li><a href="http://review.wearesasquatch.com/">/review</a></li>
		      		<li><a href="http://blossom.io">/tasks</a></li>
		      		<li><a href="https://www.facebook.com/groups/ubc319/">/fb</a></li>
		      		<li><a href="https://docs.google.com/folder/d/0BwY0CeqgUaLQYlpZT2RnTWJBUkk/edit">/docs</a></li>
		    	</ul>
		  	</div>
		</div>
		<div id="container" style="margin-top: 40px;">
	<?php } else { ?>
		<div id="container">
	<?php } ?>
		<section id="header">
			<div class="wearesasquatch"></div>
		</section>

		<section id="home">
			<div class="welcome"></div>
		</section>

		<section id="team">
			<div class="container">
	            <div class="row">
	                <div class="span12 center">
	                    <div class="title">
	                        <h1>Team</h1>
	                    </div>
	                </div>
	            </div>

	            <div id="team" class="row">
			        <div class="span4 center">
			            
			            <div class="photo oliver">
			            	<div class="information three">
			            		<h3>Oliver Bozek</h3>
			            		<p>
			            			Research Manager<br />
			            			Version Control Manager<br />
			            			System Design Lead
			            		</p>
			            		<p>
			            			<a href="mailto:&#111;&#108;&#105;&#118;&#101;&#114;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;">&#111;&#108;&#105;&#118;&#101;&#114;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;</a>
			            		</p>
			            	</div>
			            </div>
			            
			        </div>
			        <div class="span4 center">
			            <div class="photo michelle">
			            	<div class="information">
			            		<h3>Michelle Chuang</h3>
			            		<p>
			            			Project Manager<br />
			            			Coding Lead
			            		</p>
			            		<p><a href="mailto:&#109;&#105;&#099;&#104;&#101;&#108;&#108;&#101;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;">&#109;&#105;&#099;&#104;&#101;&#108;&#108;&#101;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;</a></p>
			            	</div>
			            </div>
			        </div>
			        <div class="span4 center">
			            <div class="photo thea">
			            	<div class="information">
			            		<h3>Thea Simpson</h3>
			            		<p>
			            			Communication Manager<br />
			            			System/Acceptance Testing Lead
			            		</p>
			            		<p><a href="mailto:&#116;&#104;&#101;&#097;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;">&#116;&#104;&#101;&#097;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;</a></p>
			            	</div>
			            </div>
			        </div>
			    </div>
	            <div id="team" class="row">
		            <div class="span4 center">
		                <div class="photo hanna">
			            	<div class="information">
			            		<h3>Hanna Yoo</h3>
			            		<p>
			            			Configuration Manager<br />
			            			Unit/Integration Testing Lead
			            		</p>
			            		<p><a href="mailto:&#104;&#097;&#110;&#110;&#097;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;">&#104;&#097;&#110;&#110;&#097;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;</a></p>
			            	</div>
			            </div>
		            </div>
		            <div class="span4 center">
		                <div class="photo vlad">
			            	<div class="information three">
			            		<h3>Vlad Zaharia</h3>
			            		<p>
			            			Web Manager<br />
			            			Progress Manager<br />
			            			Program Design Lead
			            		</p>
			            		<p><a href="mailto:&#118;&#108;&#097;&#100;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;">&#118;&#108;&#097;&#100;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;</a></p>
			            	</div>
			            </div>
		            </div>
		            <div class="span4 center">
		                <div class="photo jing">
			            	<div class="information three">
			            		<h3>Jing Zhu</h3>
			            		<p>
			            			Minutes Manager<br />
			            			Risk Manager<br />
			            			Requirements Lead
			            		</p>
			            		<p>
			            		<p><a href="mailto:&#106;&#105;&#110;&#103;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;">&#106;&#105;&#110;&#103;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;</a></p>
			            	</div>
			            </div>
		            </div>
		        </div>
		        <div class="row">
		        	<div class="span12 center team-contact">
		        		Or you can contact our entire team at <a href="mailto:&#116;&#101;&#097;&#109;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;">&#116;&#101;&#097;&#109;&#064;&#119;&#101;&#097;&#114;&#101;&#115;&#097;&#115;&#113;&#117;&#097;&#116;&#099;&#104;&#046;&#099;&#111;&#109;</a>.
		        	</div>
		        </div>
		    </div>
		</section>

		<section id="footer">
			Site handcrafted by Vlad Zaharia for sasquatch.
			<a class="clickable" href="/protected/"></a>
		</section>
	</div>

	<!-- Preloading mouseover images -->
	<div id="preload">
		<img src="images/oliver_hover.png" />
		<img src="images/michelle_hover.png" />
		<img src="images/thea_hover.png" />
		<img src="images/hanna_hover.png" />
		<img src="images/vlad_hover.png" />
		<img src="images/jing_hover.png" />
	</div>
</body>
</html>