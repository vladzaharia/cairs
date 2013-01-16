<?php
	// CS319 Project - Team Task Entry
	if (!isset($_REQUEST['user'])) {
		header('Location: index.html');
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
	} elseif ($_POST['user']) {
		$insert_query = sprintf("INSERT INTO `task` (user, week, description, completed, hours) VALUES ('%s', '%s', '%s', '%s', '%s');",
			$_REQUEST['user'], 
			$_REQUEST['week'], 
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
	$tk_query = "SELECT * FROM `task` WHERE `user` = '" . $_REQUEST['user'] . "' ORDER BY `week` DESC;";
	$tk_result = mysql_query($tk_query, $db);
	$tasks = array();
    while ($tasks[] = mysql_fetch_assoc($tk_result)) {}
   	array_pop($tasks);
?>

<html lang="en">
<head>
	<title>CS319 Project - Team Area</title>
	<meta name="robots" content="noindex"> <!-- Prevent Google from crawling -->

	<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,300,600' rel='stylesheet' type='text/css'>
	<link rel="stylesheet" type="text/css" href="../css/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="../css/site.css" />
	<script type="text/javascript" src="../js/jquery-1.8.3.js"></script>
	<script type="text/javascript" src="../js/bootstrap.min.js"></script>
	<script type="text/javascript" src="../js/bootstrap-anim.min.js"></script>

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
	<div id="container">
		<section id="home-team">
			<div class="logo"></div>
			<div class="text">Team Area</div>
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
		            				echo "<tr>";
		            				echo "<td class='center'>" . $task['week'] . "</td>";
		            				echo "<td>" . $task['description'] . "</td>";
		            				echo "<td class='center'>" . ($task['hours'] ? $task['hours'] : '0') . "</td>";
		            				echo "<td class='center'>" . ($task['completed'] ? "Yes" : "No") . "</td>";
		            				echo "<td class='center'><a href='home.php?user={$_REQUEST['user']}&task={$task['id']}'><i class='icon-pencil icon-white'></i></a><a href='home.php?user={$_REQUEST['user']}&delete=1&task={$task['id']}'>&nbsp;&nbsp;<i class='icon-remove icon-white'></i></a></td>";
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
								    	<input type="text" id="description" name="description" class="input-xlarge" placeholder="Description" value="<?php echo $edit_task['description']; ?>" />
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
											<input type="radio" name="completed" id="completed" value="1" <?php if ($edit_task['completed']) { echo "checked"; } ?> />
											Yes
										</label>
										<label class="radio">
											<input type="radio" name="completed" id="completed" value="0" <?php if (!$edit_task['completed']) { echo "checked"; } ?> />
											No
										</label>
									</div>
								</div>

								<div class="form-actions">
									<input type="hidden" name="user" value="<?php echo $_REQUEST['user']; ?>" />
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
							<form action="home.php?user=<?php echo $_REQUEST['user']; ?>" method="POST" class="form-horizontal">
								<div class="control-group first">
									<label class="control-label" for="week">Week</label>
									<div class="controls">
										<select name="week" id="week">
											<?php 
												foreach($weeks as $week) {
													echo "<option value='" . $week['id'] . "'>" . $week['name'] . " - " . $week['date'] . "</option>";
												}
											?>
										</select>
									</div>
								</div>
								<div class="control-group first">
									<label class="control-label" for="description">Description</label>
									<div class="controls">
								    	<input type="text" id="description" name="description" class="input-xlarge" placeholder="Description" />
									</div>
								</div>
								<div class="control-group">
									<label class="control-label" for="hours">Hours</label>
									<div class="controls">
								    	<input type="text" id="hours" name="hours" class="input-small" placeholder="Hours" />
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

								<div class="form-actions">
									<input type="hidden" name="user" value="<?php echo $_REQUEST['user']; ?>" />
									<button type="submit" class="btn btn-primary">Add Task</button>
								</div>
							</form>
						</div>
	            	</div>
	            </div>
	        </div>
		</section>

		<section id="footer" class="center">
			Site made by Vlad Zaharia for CS319.
		</section>
	</div>
</body>
</html>