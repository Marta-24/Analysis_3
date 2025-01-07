<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

include 'db_connect.php';

// Collect POST data
$session_id = isset($_POST['session_id']) ? (int)$_POST['session_id'] : 0;
$player_id = $_POST['player_id'] ?? 'UnknownPlayer';
$pause_time = $_POST['pause_time'] ?? date("Y-m-d H:i:s");
$position_x = isset($_POST['position_x']) ? (float)$_POST['position_x'] : 0.0;
$position_y = isset($_POST['position_y']) ? (float)$_POST['position_y'] : 0.0;
$position_z = isset($_POST['position_z']) ? (float)$_POST['position_z'] : 0.0;

// Debug: print received data
echo "Received data: session_id=$session_id, player_id=$player_id, pause_time=$pause_time, position=($position_x, $position_y, $position_z)<br>";

// Insert pause menu event data into the database
$sql = "INSERT INTO pause_events (session_id, player_id, pause_time, position_x, position_y, position_z)
        VALUES ('$session_id', '$player_id', '$pause_time', '$position_x', '$position_y', '$position_z')";

if ($conn->query($sql) === TRUE) {
    echo "Pause menu event saved successfully.";
} else {
    echo "Error: " . $conn->error;
}

$conn->close();
?>
