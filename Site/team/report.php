<?php
	session_start();
	if (!isset($_SESSION['user'])) {
		header('Location: index.php');
		exit;
	}

	$db = mysql_connect('localhost', '319', 'foobar');
	mysql_select_db('319', $db);
	$us_query = "SELECT * FROM `user` WHERE `id` = '{$_SESSION['user']}';";
	$us_result = mysql_query($us_query, $db);
	$user = mysql_fetch_assoc($us_result);

	if ($user['manager'] !== "progress") {
		header('Location: home.php');
		exit;
	}

	// Max Week
	$mw_query = "SELECT MAX(`week`) AS max FROM `task`;";
	$mw_result = mysql_query($mw_query, $db);
	$max_week = mysql_fetch_assoc($mw_result);

	if (isset($_GET['week'])) {
		$week = $_GET['week'];
	} else {
		$week = $max_week['max'];
	}

   	// All Users
   	$uss_query = "SELECT * FROM `user`;";
	$uss_result = mysql_query($uss_query);
	$users = array();
	while ($users[] = mysql_fetch_assoc($uss_result)) {}
	array_pop($users);

	echo "<html><head></head>";
	echo "<body style=\"font-family:'Segoe Pro', 'Segoe UI', Helvetica, Arial, sans-serif; \">";

	echo "Hey Team, <br /><br />";
	echo "It's now the end of the week! That means that all tasks need to be finalized for the website. So please:
	<ul>
	<li>Complete all your existing tasks</li>
	<li>Fill out the hours for your existing tasks</li>
	<li>Add any tasks which you did this week</li>
	</ul>
	For reference, here's the tasks for this week:<br /><br />";

	echo "<table>";
	echo "<tr>";
	echo "<th style='border-bottom: 1px solid black;' width='20%'>Team Member</th><th style='border-bottom: 1px solid black;' width='60%'>Task Description</th><th style='border-bottom: 1px solid black;' width='10%'>Hours</th><th style='border-bottom: 1px solid black;' width='10%'>Complete?</th>";
	echo "</tr>";
	foreach ($users as $user) {
		// All Tasks for Week
		$tk_query = "SELECT * FROM `task` WHERE `week`='$week' AND `user`='{$user['id']}';";
		$tk_result = mysql_query($tk_query, $db);
		$tasks = array();
		while ($tasks[] = mysql_fetch_assoc($tk_result)) {}
		array_pop($tasks);

		if (count($tasks) === 0) {
			echo "<tr>";
			echo "<td style='border-right: 1px solid black;'><strong>{$user[fname]} {$user[lname]}</strong></td>";
			echo "<td colspan='3' style='color:#900000;'><strong>No Tasks - Did you do anything this week?</strong></td>";
			echo "</tr>";
		}

		foreach($tasks as $task) {
			$number = count($tasks);
	   		echo "<tr>";
	   		if ($task['id'] === reset($tasks)['id']) {
	   			echo "<td style='border-right: 1px solid black; vertical-align:top;' rowspan='{$number}'><strong>{$user[fname]} {$user[lname]}</strong></td></strong>";
	   		}
	   		echo "<td>{$task['description']}</td>";
	   		echo "<td style='text-align: center;'>{$task['hours']}</td>";
	   		if ($task['completed'] === '1') {
	   			echo "<td style='text-align: center;'>Yes</td>";
	   		} else {
	   			echo "<td style='color:#900000; text-align: center;'><strong>No</strong></td>";
	   		}
	   		echo "</tr>";
	  	}
	}

	// All Tasks for Week
	$tk_query = "SELECT * FROM `task` WHERE `week`='$week' AND `user`='0';";
	$tk_result = mysql_query($tk_query, $db);
	$tasks = array();
    while ($tasks[] = mysql_fetch_assoc($tk_result)) {}
   	array_pop($tasks);

	foreach($tasks as $task) {
   		$number = count($tasks);
   		echo "<tr>";
   		if ($task['id'] === reset($tasks)['id']) {
   			echo "<td style='border-right: 1px solid black; vertical-align:top;' rowspan='{$number}'><strong>Team Tasks</strong></td></strong>";
   		}
   		echo "<td>{$task['description']}</td>";
   		echo "<td style='text-align: center;'>{$task['hours']}</td>";
   		if ($task['completed'] === '1') {
   			echo "<td style='text-align: center;'>Yes</td>";
   		} else {
   			echo "<td style='color:#700000; text-align: center;'><strong>No</strong></td>";
   		}
   		echo "</tr>";
   }
   echo "</table>";

   echo "<br />Cheers,<br />";
   echo "Vlad Zaharia";

?>