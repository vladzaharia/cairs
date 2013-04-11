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
	$mw_query = "SELECT * FROM `week`;";
	$mw_result = mysql_query($mw_query, $db);
	$weeks = array();
	while ($weeks[] = mysql_fetch_assoc($mw_result)) {}
	array_pop($weeks);

   	// All Users
   	$uss_query = "SELECT * FROM `user`;";
	$uss_result = mysql_query($uss_query);
	$users = array();
	while ($users[] = mysql_fetch_assoc($uss_result)) {}
	array_pop($users);

	echo "<table>";
	echo "<tr>";
	echo "<th>Name</th>";
	foreach ($weeks as $week) {
		echo "<th>{$week['id']}</th>";
	}
	echo "</tr>";
	foreach ($users as $user) {
		echo "<tr>";
		echo "<td><strong>{$user[fname]} {$user[lname]}</strong></td>";

		foreach ($weeks as $week) {
			if ($_GET['type'] === "num") {
				$tk_query = "SELECT COUNT(`hours`) AS wkhr FROM `task` WHERE `week`='{$week['id']}' AND `user`='{$user['id']}';";
			} else {
				$tk_query = "SELECT SUM(`hours`) AS wkhr FROM `task` WHERE `week`='{$week['id']}' AND `user`='{$user['id']}';";
			}
			$tk_result = mysql_query($tk_query, $db);
			$task = mysql_fetch_assoc($tk_result);

			if ($task['wkhr'] == null || $task['wkhr'] == 0 || $task['wkhr'] == "0.0") {
				echo "<td>0.0</td>";
			} else {
				echo "<td>{$task['wkhr']}</td>";
			}
		}
		echo "</tr>";
	}
   echo "</table>";

?>