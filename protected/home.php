<?php
	session_start();

	$db = mysql_connect('localhost', '319', 'foobar');
	mysql_select_db('319');

	if (isset($_SESSION['username']) && isset($_SESSION['password'])) {
		$login_query = "SELECT * FROM `protected_login` WHERE `username` = '" . $_SESSION['username'] . "';";
		$login_result = mysql_query($login_query);
		$login = mysql_fetch_assoc($login_result);
	   	
	   	if ($login['password'] !== $_SESSION['password']) {
	   		header("location: index.php");
	   	}
	} else {
		header("location: index.php");
	}

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
	<title>/sasquatch/</title>
	<meta name="robots" content="noindex"> <!-- Prevent Google from crawling -->

	<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,300,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="../css/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="../css/site.css" />
	<script type="text/javascript" src="../js/jquery-1.8.3.js"></script>
	<script type="text/javascript" src="../js/bootstrap.min.js"></script>
	<script type="text/javascript" src="../js/bootstrap-anim.min.js"></script>
	<link rel="shortcut icon" href="../images/favicon.ico" type="image/x-icon" />

	<meta name="description" content="sasquatch is a group of 6 developers working on awesome software for CS 319 at UBC." />
	<meta name="author" content="Vlad Zaharia" />
	<meta name="keywords" content="ubc, cs, cs319, 319, sasquatch, software, software development, web, web development, vancouver, canada, bc, we are sasquatch, vlad, zaharia, vlad zaharia, michelle, chuang, michelle chuang, oliver, bozek, oliver bozek, thea, simpson, thea simpson, hanna, yoo, hanna yoo, jing, zhu, jing zhu" />
	<meta name="copyright" content="2012 Vlad Zaharia" />

	<script type="text/javascript" src="//www.hellobar.com/hellobar.js"></script>
	<script type="text/javascript">
	    new HelloBar(18603,90652);
	</script>


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
	<div class="navbar navbar-fixed-top navbar-inverse">
	  	<div class="navbar-inner">
	    	<a class="brand" href="#"></a>
	    	<ul class="nav">
	      		<li><a href="/">home</a></li>
	      		<li class="active"><a href="/protected">/protected</a></li>
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
		            		<div class="bar bar-success" style="width: 25%;">Complete</div>
		            		<div class="bar" style="width: 25%;">In Progress</div>
		            		<div class="bar" style="width: 25%;">In Progress</div>
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
	            		Jan 28 - Mar 1
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
									$total_hours = 0;
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
												$total_hours += $hours[0];

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
							    					$cmpl_data = ($task['completed'] ? '&#x2713;' : '&#x2717;');
							    					$hr_data = ($task['hours'] ? str_replace('.0', '', $task['hours']) : '0');
							    					if ($task['user'] === '0') {
							    						echo "<tr class=\"team\">";
							    					} else {
							    						echo "<tr>";
							    					}
							    					
							    					echo "<td>{$task['description']}</td>";
							    					echo "<td>{$hr_data}</td>";
							    					echo "<td>{$cmpl_data}</td>";
							    					echo "</tr>";
							    				}  ?></table>'></div>
							    				<div class="hours"><span class="hour"><?php echo ($hours[0] ? str_replace('.0', '', $hours[0]) : '0'); ?></span> hour(s)</div>
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
							    		<div class="row">
							    			<table width="100%" class="summary-table">
							    				<thead>
							    					<tr>
							    						<th width="17.5%">Total Hours</th>
							    						<th width="18%">Risk Status</th>
							    						<th width="66.5%">Comments</th>
							    					</tr>
							    				</thead>
							    				<tbody>
							    					<tr>
							    						<td><span class="hour"><?php echo $total_hours; ?></span> hour(s)</td>
							    						<td><?php 
									    					if ($week['risk_status'] === '1') {
									    						$risk_msg = "On Track";
									    						$risk_class = "success";
									    					} elseif ($week['risk_status'] === '2') {
									    						$risk_msg = "Slipping";
									    						$risk_class = "warning";
									    					} elseif ($week['risk_status'] === '3') {
									    						$risk_msg = "At Risk";
									    						$risk_class = "danger";
									    					} elseif ($week['risk_status'] === '4') {
									    						$risk_msg = "Behind";
									    						$risk_class = "inverse";
									    					} else {
									    						$risk_msg = "Week not Complete";
									    						$risk_class = "info";
									    					}
									    					echo "<span class='label label-{$risk_class}'>{$risk_msg}</span>";
									    				?></td>
							    						<td><?php echo $week['week_info']; ?></td>
							    					</tr>
							    				</tbody>
							    			</table>
							    		</div>
							    	</div>
								</div>
							</div>
							<?php } ?>
						</div>
	            	</div>
	            </div>
	            <?php if ($_SESSION['username'] === "team") { ?>
	            <div class="row">
	            	<div class="span2" style="float: right; width: 70px;">
	            		<a href="/team/home.php">Add Tasks</a>
	            	</div>
	            </div>
	            <?php } ?>
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
	            		<a href="/meetings/team/jan24.pdf">January 24th, 2013</a><br />
	            		<a href="/meetings/team/jan28.pdf">January 28th, 2013</a><br />
	            		<a href="/meetings/team/jan30.pdf">January 30th, 2013</a><br />
	            		<a href="/meetings/team/jan31.pdf">January 31th, 2013</a><br />
	            		<a href="/meetings/team/feb1.pdf">February 1st, 2013</a><br />
	            		<a href="/meetings/team/feb3.pdf">February 3rd, 2013</a><br />
	            		<a href="/meetings/team/feb6.pdf">February 6th, 2013</a><br />
	            		<a href="/meetings/team/feb7.pdf">February 7th, 2013</a><br />
	            		<a href="/meetings/team/feb11.pdf">February 11th, 2013</a><br />
	            	</div>
	            	<div class="span4">
	            		<h4>Client Meetings</h4>
	            		<a href="/meetings/client/jan24.pdf">January 24th, 2013</a><br />
	            		<a href="/meetings/client/jan25.pdf">January 25th, 2013</a>
	            	</div>
	            	<div class="span4">
	            		<h4>TA Meetings</h4>
	            		<a href="/meetings/ta/jan28.pdf">January 28th, 2013</a><br />
	            		<a href="/meetings/ta/feb4.pdf">February 4th, 2013</a><br />
	            		<strike>February 11th, 2013</strike> (Cancelled - Family Day)<br />
	            		February 18th, 2013
	            	</div>
	            </div>
	        </div>
		</section>

		<section id="risks">
			<div class="container">
	            <div class="row">
	                <div class="span12 center">
	                    <div class="title">
	                        <h1>Risks</h1>
	                    </div>
	                </div>
	            </div>

	            <div class="row content center">
	            	<div class="span12">
	            		<table width="100%" class="table table-striped">
	            			<thead>
		            			<tr>
		            				<th width="30%">Risk</th>
		            				<th width="15%">Likelihood</th>
		            				<th width="15%">Risk Level</th>
		            				<th width="40%">Mitigation Strategy</th>
		            			</tr>
	            			</thead>
	            			<tbody>
		            			<tr>
		            				<td>Scope Creep</td>
		            				<td><span class="label label-success">Unlikely</span></td>
		            				<td><span class="label label-important">High</span></td>
		            				<td><ul><li>Well-Defined requirements with client meetings and approvals up-front.</li>
		            				 <li>Whenever needed, clients are contacted to provide input in order to lower the chance of scopes changing mid-project.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Schedule Overrun</td>
		            				<td><span class="label label-warning">Moderate</span></td>
		            				<td><span class="label label-inverse">Very High</span></td>
		            				<td><ul><li>Keep close track of progress to ensure any lags are noticed and rectified early, helping prevent a chain reaction of delays.</li>
		            				 <li>Team members communicate regularly with each other and seek assistance if needed.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Requirements Not Met</td>
		            				<td><span class="label label-success">Unlikely</span></td>
		            				<td><span class="label label-important">High</span></td>
		            				<td><ul><li>Constantly verify that features being worked on and recently completed meet the requirements originally specified in the SRS.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Poor Quality, Buggy Software</td>
		            				<td><span class="label label-warning">Moderate</span></td>
		            				<td><span class="label label-important">High</span></td>
		            				<td><ul><li>Ensure proper code review system is in place, as well as adequate testing after implementation and proper documentation to allow for future revisions.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Testing Reveals Significant Flaw In Design Logic</td>
		            				<td><span class="label label-success">Unlikely</span></td>
		            				<td><span class="label label-important">High</span></td>
		            				<td><ul><li>Background in database and software design from team members.</li>
		            				<li>Specifications and design are reviewed by entire team and supervisor prior to project completion to ensure proper direction for project.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Absence of Commitment Level of Users</td>
		            				<td><span class="label label-success">Unlikely</span></td>
		            				<td><span class="label label-warning">Moderate</span></td>
		            				<td><ul><li>UX Design by team members to ensure a consistent and easy-to-use interface which rivals existing interface, while providing a new and improved experience.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Poor Working Relationship within Team</td>
		            				<td><span class="label label-success">Unlikely</span></td>
		            				<td><span class="label label-warning">Moderate</span></td>
		            				<td><ul><li>Experience working on diverse development teams.</li>
		            				 <li>Mutual interest in performing strongly and delivering a suitable product.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Project Team Availability</td>
		            				<td><span class="label label-warning">Moderate</span></td>
		            				<td><span class="label label-warning">Moderate</span></td>
		            				<td><ul><li>Common meeting times have been agreed upon for entire term, and ad-hoc meetings will be created as needed, including on weekends and during breaks if necessary. </li>
		            				 <li>Strong work ethic contributes to willingness to be available during agreed-upon meeting times.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Software Engineering Practices Used Foreign to Team</td>
		            				<td><span class="label label-success">Unlikely</span></td>
		            				<td><span class="label label-success">Low</span></td>
		            				<td><ul><li>Experience following a waterfall lifecycle.</li>
		            				 <li>Additionally, members have experience or are willing to learn other practices such as source control, proper commenting and code style, documentation, and code review practices.</li></ul></td>
		            			</tr>

		            			<tr>
		            				<td>Team's Lack of Knowledge of Required Technologies</td>
		            				<td><span class="label label-important">Likely</span></td>
		            				<td><span class="label label-success">Low</span></td>
		            				<td><ul><li>Members are quick learners and have experience picking up new technologies.</li>
		            				 <li>Research Manager will find resources to support the learning process.</li></ul></td>
		            			</tr>
	            			</tbody>
	            		</table>
	            	</div>
	            </div>
	        </div>
		</section>

		<section id="servers">
			<div class="container">
	            <div class="row">
	                <div class="span12 center">
	                    <div class="title">
	                        <h1>Deployment Servers</h1>
	                    </div>
	                </div>
	            </div>

	            <div class="row content center">
	            	<div class="span1 pre">
	            		&nbsp;
	            	</div>
	            	<div class="span4 hero-unit">
	            		<h4>Application Server</h4>
	            		<strong>Server URL:</strong> sasquatch.cloudapp.net <br />
	            		<?php if ($_SESSION['username'] === "team") { ?>
	            		<strong>Server IP:</strong> 168.61.20.17 <br />
	            		<strong>Username</strong> and <strong>Password</strong> are per-member.<br />
	            		<strong>Domain:</strong> sasquatch.cloudapp.net </br /><br />
	            		<a href="sasquatch.rdp">Connect via Remote Desktop</a>
	            		<?php } ?>
	            	</div>
	            	<div class="span4 hero-unit">
	            		<h4>Database Server</h4>
	            		<strong>Server:</strong> q1z8wwq8to.database.windows.net <br />
	            		<?php if ($_SESSION['username'] === "team") { ?>
	            		<strong>Username:</strong> team <br />
	            		<strong>Password:</strong> trecU5He </br />
	            		<strong>Database:</strong> sasquatch </br /><br />
	            		<a href="https://q1z8wwq8to.database.windows.net/#$database=sasquatch">Manage the Database</a>
	            		<?php } ?>
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
	            		Source control is hosted by GitHub, and located <a href="https://github.com/vladzaharia/cloaked-octo-bear">here</a>. Code Reviews are done through <a href="http://review.wearesasquatch.com/">Review Board</a>. Automatic CI Builds are hosted on <a href="http://sasq-bamboo.cloudapp.net/">Bamboo</a>.
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
	            <?php if ($_SESSION['username'] === "team") { ?>
	            <div class="row resource">
	            	<div class="image file span1"></div>
	            	<div class="description span6">
	            		<h4>Books</h4>
	            		<a href="../books/aspnet.pdf">Professional ASP.net MVC 4</a><br />
	            		<a href="../books/hfhtml.pdf">Head-First HTML</a><br />
	            		<a href="../books/hfhtml5.pdf">Head-First HTML5/Javascript</a><br />
	            		<a href="../books/clrcs.pdf">CLR via C# 4th Edition</a><br />
	            		<a href="../books/hfcs.pdf">Head-First C#</a>
	            	</div>
	            </div>
	            <?php } ?>
	        </div>
		</section>

		<section id="footer">
			Site handcrafted by Vlad Zaharia for sasquatch.
		</section>
	</div>

	<!-- Preloading mouseover images -->
	<div id="preload">
		<img src="../images/oliver_hover.png" />
		<img src="../images/michelle_hover.png" />
		<img src="../images/thea_hover.png" />
		<img src="../images/hanna_hover.png" />
		<img src="../images/vlad_hover.png" />
		<img src="../images/jing_hover.png" />
	</div>
</body>
</html>