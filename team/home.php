<?php
	// CS319 Project - Team Task Entry

	session_start();
	if (!isset($_SESSION['user'])) {
		header('Location: index.php');
		exit;
	}

	$db = mysql_connect('localhost', '319', 'foobar');
	mysql_select_db('319', $db);

	// Insert Task
	if ($_GET['delete']) {
		$delete_query = "DELETE FROM `task` WHERE id = '{$_REQUEST['task']}';";
		mysql_query($delete_query, $db);
	} elseif ($_POST['task']) {
		$update_query = sprintf("UPDATE `task` SET description='%s', completed='%s', hours='%s' WHERE id = '%s';",
			$_REQUEST['description'], 
			$_REQUEST['completed'], 
			$_REQUEST['hours'], 
			$_REQUEST['task']);	

		mysql_query($update_query, $db);
	} elseif ($_POST['description']) {
		if (isset($_POST['ovr_usr']) && $_POST['ovr_usr'] !== '') {
			$user = $_POST['ovr_usr'];
		} else {
			$user = $_SESSION['user'];
		}

		if (isset($_POST['ovr_wk']) && $_POST['ovr_wk'] !== '') {
			$week = $_POST['ovr_wk'];
		} else {
			$week = $_REQUEST['week'];
		}

		$insert_query = sprintf("INSERT INTO `task` (user, week, description, completed, hours) VALUES ('%s', '%s', '%s', '%s', '%s');",
			$user, 
			$week, 
			$_REQUEST['description'], 
			$_REQUEST['completed'], 
			$_REQUEST['hours']);	

		mysql_query($insert_query, $db);
	}

	// Retrieve All Weeks
	$wk_query = "SELECT * FROM `week` WHERE `released` < '" . time() . "';";
	$wk_result = mysql_query($wk_query, $db);
	$weeks = array();
    while ($weeks[] = mysql_fetch_assoc($wk_result)) {}
   	array_pop($weeks);

	// Retrieve All Tasks for User
	$tk_query = "SELECT * FROM `task` WHERE `user` = '" . $_SESSION['user'] . "' ORDER BY `week` DESC;";
	$tk_result = mysql_query($tk_query, $db);
	$tasks = array();
    while ($tasks[] = mysql_fetch_assoc($tk_result)) {}
   	array_pop($tasks);

   // Retrieve User Info
	$us_query = "SELECT * FROM `user` WHERE `id` = '" . $_SESSION['user'] . "';";
	$us_result = mysql_query($us_query, $db);
	$user = mysql_fetch_assoc($us_result);

   	$max_query = "SELECT MAX(week) AS max FROM `task` WHERE `user` = '" . $_SESSION['user'] . "' ORDER BY `week` DESC;";
   	$max_result = mysql_query($max_query);
   	$max = mysql_fetch_assoc($max_result);
?>

<html lang="en">
<head>
	<title>/sasquatch/ Team Area</title>
	<meta name="robots" content="noindex"> <!-- Prevent Google from crawling -->

	<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,300,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="../css/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="../css/site.css" />
	<script type="text/javascript" src="../js/jquery-1.8.3.js"></script>
	<script type="text/javascript" src="../js/bootstrap.min.js"></script>
	<script type="text/javascript" src="../js/bootstrap-anim.min.js"></script>
	<link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />

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

			$(".collapse").collapse()
		});
	</script>
</head>
<body>
	<div class="navbar navbar-fixed-top navbar-inverse">
	  	<div class="navbar-inner">
	    	<a class="brand" href="#"></a>
	    	<ul class="nav">
	      		<li><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Home']);" href="/">home</a></li>
	      		<li class="active"><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Protected']);" href="/protected">/protected</a></li>
	      		<li><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Team']);" href="/team">/team</a></li>
	      		<?php if ($user['manager'] === 'progress') { ?> <li><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Team Report']);" href="/team/report.php">/team Reports</a></li> <?php } ?>
	      		<li><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Repo']);" href="https://github.com/vladzaharia/cloaked-octo-bear">/repo</a></li>
	      		<li><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Review']);" href="http://review.wearesasquatch.com/">/review</a></li>
	      		<li><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Facebook']);" href="https://www.facebook.com/groups/ubc319/">/fb</a></li>
	      		<li><a onClick="_gaq.push(['_trackEvent', 'Menu', 'Click', 'Docs']);" href="https://docs.google.com/folder/d/0BwY0CeqgUaLQYlpZT2RnTWJBUkk/edit">/docs</a></li>
	    	</ul>
	  	</div>
	</div>
	<div id="container" style="margin-top: 40px;">
		<section id="home-team">
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
	                    <div class="title">
	                        <h1>Task Entry</h1>
	                    </div>
	                </div>
	            </div>

	            <div class="row">
	            	<div class="span12">
	            		<h3>Current Tasks</h3>
	            	</div>
	            </div>
	            <div class="row">
	            	<div class="span12">
		            	<table>
		            		<tr>
		            			<th width="10%" class='center'>Week</th>
		            			<th width="65%">Description</th>
		            			<th width="10%" class='center'>Hours</th>
		            			<th width="10%" class='center'>Complete</th>
		            			<th width="5%" class='center'></th>
		            		</tr>
		            		<?php
		            			foreach($tasks as $task) {
		            				if ($max['max'] > $task['week']) {
		            					echo "<tr class='archived'>";
		            				} else {
		            					echo "<tr>";
		            				}
		            				echo "<td class='center'>" . $task['week'] . "</td>";
		            				echo "<td>" . $task['description'] . "</td>";
		            				echo "<td class='center'>" . ($task['hours'] ? $task['hours'] : '0') . "</td>";
		            				echo "<td class='center'>" . ($task['completed'] ? "Yes" : "No") . "</td>";
		            				echo "<td class='center'><a href='home.php?user={$_SESSION['user']}&task={$task['id']}'><i class='icon-pencil icon-white'></i></a><a href='home.php?user={$_SESSION['user']}&delete=1&task={$task['id']}'>&nbsp;&nbsp;<i class='icon-remove icon-white'></i></a></td>";
		            				echo "</tr>";
		            			}
		            		?>
		            	</table>
					</div>
	            </div>
	            
	            <?php 
	            	if ($_GET['task'] && !isset($_REQUEST['delete'])) { 
	            		$tk_query = "SELECT * FROM `task` WHERE `id` = '" . $_REQUEST['task'] . "';";
						$tk_result = mysql_query($tk_query, $db);
						$edit_task = mysql_fetch_assoc($tk_result);
	            	?>
	            <div class="row spacer">
	            	<div class="span12">
	            		<h3>Edit Task</h3>
	            	</div>
	            </div>

	            <div class="row">
	            	<div class="span12">
						<div class="new-task">
							<form action="home.php" method="POST" class="form-horizontal">
								<div class="control-group first">
									<label class="control-label" for="description">Description</label>
									<div class="controls">
								    	<input type="text" id="description" name="description" class="input-xlarge" placeholder="Description" value="<?php echo $edit_task['description']; ?>" autocomplete="off" />
									</div>
								</div>
								<div class="control-group">
									<label class="control-label" for="hours">Hours</label>
									<div class="controls">
								    	<input type="text" id="hours" name="hours" class="input-small" placeholder="Hours" value="<?php echo $edit_task['hours']; ?>" />
									</div>
								</div>
								<div class="control-group">
									<label class="control-label" for="hours">Complete?</label>
									<div class="controls">
								    	<label class="radio">
											<input type="radio" name="completed" id="completed" value="1" <?php if ($edit_task['completed']) { echo "checked"; } ?> autocomplete="off" />
											Yes
										</label>
										<label class="radio">
											<input type="radio" name="completed" id="completed" value="0" <?php if (!$edit_task['completed']) { echo "checked"; } ?> autocomplete="off" />
											No
										</label>
									</div>
								</div>

								<div class="form-actions">
									<input type="hidden" name="task" value="<?php echo $_REQUEST['task']; ?>" />
									<button type="submit" class="btn btn-primary">Edit Task</button>
								</div>
							</form>
						</div>
	            	</div>
	            </div>
	            <?php } ?>

	            <div class="row spacer">
	            	<div class="span12">
	            		<h3>Add New Task</h3>
	            	</div>
	            </div>

	            <div class="row">
	            	<div class="span12">
						<div class="new-task">
							<form action="home.php?user=<?php echo $_SESSION['user']; ?>" method="POST" class="form-horizontal">
								<div class="control-group first">
									<label class="control-label" for="week">Week</label>
									<div class="controls">
										<select name="week" id="week">
											<?php 
												foreach($weeks as $week) {
													if ($week === end($weeks)) {
														echo "<option value='" . $week['id'] . "' selected>" . $week['name'] . " - " . $week['date'] . "</option>";
													} else {
														echo "<option value='" . $week['id'] . "'>" . $week['name'] . " - " . $week['date'] . "</option>";
													}
												}
											?>
										</select>
									</div>
								</div>
								<div class="control-group first">
									<label class="control-label" for="description">Description</label>
									<div class="controls">
								    	<input type="text" id="description" name="description" class="input-xlarge" placeholder="Description" autocomplete="off" />
									</div>
								</div>
								<div class="control-group">
									<label class="control-label" for="hours">Hours</label>
									<div class="controls">
								    	<input type="text" id="hours" name="hours" class="input-small" placeholder="Hours" autocomplete="off" />
									</div>
								</div>
								<div class="control-group">
									<label class="control-label" for="hours">Complete?</label>
									<div class="controls">
								    	<label class="radio">
											<input type="radio" name="completed" id="completed" value="1" />
											Yes
										</label>
										<label class="radio">
											<input type="radio" name="completed" id="completed" value="0" checked />
											No
										</label>
									</div>
								</div>

								<?php if ($user['manager'] === 'progress') { ?>
								<div class="control-group right">
									<h4>Progress Manager Use Only</h4>
									<label class="control-label" for="ovr_usr">Override User</label>
									<div class="controls">
								    	<input type="text" id="ovr_usr" name="ovr_usr" class="input-small" placeholder="User" autocomplete="off" />
									</div>
									<br />
									<label class="control-label" for="ovr_wk">Override Week</label>
									<div class="controls">
								    	<input type="text" id="ovr_wk" name="ovr_wk" class="input-small" placeholder="Week" autocomplete="off" />
									</div>
								</div>
								<?php } ?>

								<div class="form-actions">
									<button type="submit" class="btn btn-primary">Add Task</button>
								</div>
							</form>
						</div>
	            	</div>
	            </div>
	        </div>
		</section>

		<section id="footer">
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