<?php
	// CS319 Project - Team Task Entry
	$db = mysql_connect('localhost', '319', 'foobar');
	mysql_select_db('319');

	// Retrieve All Weeks
	$wk_query = "SELECT * FROM `week` WHERE `released` < '" . time() . "';";
	$wk_result = mysql_query($wk_query);
	$weeks = array();
    while ($weeks[] = mysql_fetch_assoc($wk_result)) {}
   	array_pop($weeks);

	// Retrieve All Users
	$us_query = "SELECT * FROM `user`;";
	$us_result = mysql_query($us_query);
	$users = array();
    while ($users[] = mysql_fetch_assoc($us_result)) {}
    array_pop($users);
?>

<html lang="en">
<head>
	<title>CS319 Project</title>
	<meta name="robots" content="noindex"> <!-- Prevent Google from crawling -->

	<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,300,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="css/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="css/site.css" />
	<script type="text/javascript" src="js/jquery-1.8.3.js"></script>
	<script type="text/javascript" src="js/bootstrap.min.js"></script>
	<script type="text/javascript" src="js/bootstrap-anim.min.js"></script>

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
	<div id="container">
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
			            			<a href="&#x6d;&#x61;&#105;&#x6c;&#116;&#111;&#58;&#111;&#x6d;&#98;&#x6f;&#122;&#101;&#x6b;&#x40;&#x67;&#x6d;&#x61;&#x69;&#x6c;&#46;&#99;&#x6f;&#x6d;">&#x6f;&#x6d;&#x62;&#x6f;&#x7a;&#x65;&#107;&#64;&#103;&#109;&#97;&#105;&#x6c;&#x2e;&#99;&#x6f;&#x6d;</a>
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
			            		<p><a href="&#109;&#x61;&#x69;&#x6c;&#116;&#111;&#58;&#109;&#105;&#99;&#x68;&#x65;&#46;&#99;&#x68;&#x75;&#97;&#110;&#103;&#x40;&#x67;&#109;&#x61;&#105;&#x6c;&#x2e;&#99;&#x6f;&#x6d;">&#x6d;&#105;&#99;&#104;&#101;&#x2e;&#x63;&#x68;&#x75;&#x61;&#x6e;&#x67;&#x40;&#x67;&#x6d;&#x61;&#105;&#x6c;&#46;&#99;&#111;&#x6d;</a></p>
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
			            		<p><a href="&#x6d;&#97;&#x69;&#x6c;&#116;&#x6f;&#x3a;&#x74;&#104;&#x65;&#97;&#106;&#x75;&#x73;&#x74;&#105;&#110;&#x65;&#x40;&#114;&#117;&#x6e;&#98;&#x6f;&#x78;&#x2e;&#99;&#x6f;&#x6d;">&#x74;&#x68;&#101;&#97;&#106;&#117;&#x73;&#116;&#x69;&#x6e;&#x65;&#64;&#x72;&#117;&#110;&#x62;&#x6f;&#120;&#x2e;&#99;&#111;&#x6d;</a></p>
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
			            		<p><a href="&#x6d;&#x61;&#x69;&#x6c;&#116;&#111;&#58;&#97;&#x6d;&#x6f;&#x72;&#x30;&#x36;&#x32;&#52;&#46;&#121;&#x6f;&#x6f;&#x40;&#103;&#x6d;&#x61;&#x69;&#108;&#46;&#x63;&#x6f;&#109;">&#97;&#x6d;&#x6f;&#x72;&#48;&#54;&#x32;&#52;&#46;&#x79;&#x6f;&#x6f;&#64;&#103;&#109;&#x61;&#x69;&#x6c;&#46;&#x63;&#x6f;&#109;</a></p>
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
			            		<p><a href="&#109;&#97;&#105;&#108;&#116;&#111;&#58;&#109;&#101;&#64;&#118;&#108;&#97;&#100;&#122;&#97;&#104;&#97;&#114;&#105;&#97;&#46;&#99;&#111;&#109;">&#109;&#101;&#64;&#118;&#108;&#97;&#100;&#122;&#97;&#104;&#97;&#114;&#105;&#97;&#46;&#99;&#111;&#109;</a></p>
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
			            		<p><a href="&#109;&#x61;&#x69;&#x6c;&#x74;&#111;&#x3a;&#x6a;&#122;&#104;&#117;&#x31;&#x39;&#64;&#x67;&#x6d;&#97;&#105;&#108;&#46;&#99;&#111;&#109;">&#x6a;&#122;&#104;&#x75;&#49;&#x39;&#64;&#x67;&#109;&#x61;&#x69;&#108;&#46;&#99;&#111;&#109;</a></p>
			            	</div>
			            </div>
		            </div>
		        </div>
		    </div>
		</section>

		<section id="progress">
			<div class="container">
	            <div class="row">
	                <div class="span12 center">
	                    <div class="title">
	                        <h1>Progress</h1>
	                    </div>
	                </div>
	            </div>

	            <div class="row">
	            	<div class="span12">
	            		<h3>Overall Progress</h3>
	            	</div>
	            </div>
	            <div class="row">
	            	<div class="span12">
		            	<div class="progress progress-striped">
		            		<div class="bar" style="width: 25%;">In Progress</div>
		            		<div class="bar bar-none" style="width: 25%;"></div>
		            		<div class="bar bar-none" style="width: 25%;"></div>
		            		<div class="bar bar-none" style="width: 25%;"></div>
						</div>
					</div>
	            </div>
	            <div class="row center progress-descriptions">
	            	<div class="span3">
	            		<h4>Requirements</h4>
	            		Jan 14 - Feb 8
	            	</div>
	            	<div class="span3">
	            		<h4>Design</h4>
	            		Jan 28 - Feb 8
	            	</div>
	            	<div class="span3">
	            		<h4>Implementation</h4>
	            		Feb 11 - Mar 8
	            	</div>
	            	<div class="span3">
	            		<h4>Testing</h4>
	            		Mar 11 - Mar 29
	            	</div>
	            </div>
	            <div class="row">
	            	<div class="span12">
	            		<h3>Weekly Progress</h3>
	            	</div>
	            </div>

	            <div class="row">
	            	<div class="span12">
						<div class="accordion" id="accordion">
							<?php 
								foreach($weeks as $week) {
							?>
							<div class="accordion-group">
							    <div class="accordion-heading">
									<a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapse<?php echo $week['id']; ?>">
										<span class="week"><?php echo $week['name']; ?></span> <span class="date"><?php echo $week['date']; ?></span>
									</a>
							    </div>
								<div id="collapse<?php echo $week['id']; ?>" class="accordion-body collapse in">
							    	<div class="accordion-inner">
							    		<div class="row">
							    		<?php
											foreach($users as $user) {
												// Retrieve All Tasks for User
												$tk_query = "SELECT * FROM `task` 
															 WHERE (`user` = '" . $user['id'] . "' OR `user` = '0')
															 AND `week` = '" . $week['id'] ."';";
												$tk_result = mysql_query($tk_query);
												$tasks = array();
											    while ($tasks[] = mysql_fetch_assoc($tk_result)) {}
											   	array_pop($tasks);

											   	$hours_query = "SELECT SUM(`hours`) FROM `task`
																WHERE (`user` = '" . $user['id'] . "' OR `user` = '0')
															 	AND `week` = '" . $week['id'] ."';";
												$hours_result = mysql_query($hours_query);
												$hours = mysql_fetch_array($hours_result);

												$count_query = "SELECT COUNT(*) FROM `task`
																WHERE `user` = '" . $user['id'] . "'
															 	AND `week` = '" . $week['id'] ."';";
												$count_result = mysql_query($count_query);
												$counts = mysql_fetch_array($count_result);

												$complete_query = "SELECT COUNT(*) FROM `task`
																WHERE `user` = '" . $user['id'] . "'
															 	AND `week` = '" . $week['id'] ."'
															 	AND `completed` = '1';";
												$complete_result = mysql_query($complete_query);
												$completes = mysql_fetch_array($complete_result);

												if ($counts[0]) {
													$width_done = $completes[0] / $counts[0] * 100 . "%";
													$width_not = 100 - ($completes[0] / $counts[0] * 100) . "%";
												} else {
													$width_done = "100%";
													$width_not = "0%";
												}
										?>
											<div class="span2 center">
							    				<div class="tasks-progress photo <?php echo $user['username']; ?>" data-placement="bottom" data-content='<table><tr><th width="80%">Task</th><th width="10%">hr</th><th width="10%">&#x2713;</th></tr>
							    				<?php foreach($tasks as $task) {
							    					$cmpl_data = ($task['completed'] ? '&#x2713;' : 'x');
							    					$hr_data = ($task['hours'] ? str_replace('.0', '', $task['hours']) : '0');

							    					echo "<tr>";
							    					echo "<td>{$task['description']}</td>";
							    					echo "<td>{$hr_data}</td>";
							    					echo "<td>{$cmpl_data}</td>";
							    					echo "</tr>";
							    				}  ?></table>'></div>
							    				<div class="hours"><span class="hour"><?php echo ($hours[0] ? str_replace('.0', '', $hours[0]) : '0'); ?></span> hours</div>
							    				<div class="progress progress-striped">
							    					<div class="bar bar-success" style="width: <?php echo $width_done; ?>;"></div>
							    					<div class="bar bar-danger" style="width: <?php echo $width_not; ?>;"></div>
							    				</div>
							    				<div class="tasks"><?php echo ($completes[0] ? $completes[0] : '0'); ?>/<?php echo ($counts[0] ? $counts[0] : '0'); ?> tasks</div>
							    			</div>
										<?php
											}
										?>
							    		</div>
							    	</div>
								</div>
							</div>
							<?php } ?>
						</div>
	            	</div>
	            </div>
	        </div>
		</section>

		<section id="minutes">
			<div class="container">
	            <div class="row">
	                <div class="span12 center">
	                    <div class="title">
	                        <h1>Meeting Minutes</h1>
	                    </div>
	                </div>
	            </div>

	            <div class="row content center">
	            	<div class="span4">
	            		<h4>Team Meetings</h4>
	            		Minutes will be posted here.
	            	</div>
	            	<div class="span4">
	            		<h4>Customer Meetings</h4>
	            		Minutes will be posted here.
	            	</div>
	            	<div class="span4">
	            		<h4>TA Meetings</h4>
	            		Minutes will be posted here.
	            	</div>
	            </div>
	        </div>
		</section>

		<section id="resources">
			<div class="container">
	            <div class="row">
	                <div class="span12 center">
	                    <div class="title">
	                        <h1>Resources</h1>
	                    </div>
	                </div>
	            </div>

	            <div class="row resource">
	            	<div class="image source span1"></div>
	            	<div class="description span6">
	            		<h4>Source Control</h4>
	            		Source control is hosted by BitBucket, and located <a href="http://repo.vladzaharia.com/cpsc-319-project">here</a>.
	            	</div>
	            </div>
	            <div class="row resource">
	            	<div class="image management span1"></div>
	            	<div class="description span6">
	            		<h4>Project Management</h4>
	            		The tasks/project management are hosted by Blossom.io, found <a href="http://blossom.io">here</a>.
	            	</div>
	            </div>
	            <div class="row resource">
	            	<div class="image communication span1"></div>
	            	<div class="description span6">
	            		<h4>Communication</h4>
	            		Our Facebook group is located <a href="https://www.facebook.com/groups/ubc319/">here</a> for communication.
	            	</div>
	            </div>
	        </div>
		</section>

		<section id="footer" class="center">
			Site made by Vlad Zaharia for CS319.
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