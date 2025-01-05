<?php
// Enable error reporting
error_reporting(E_ALL);
ini_set('display_errors', 1);

include 'db_connect.php';

// Collect POST data
$session_id = isset($_POST['session_id']) ? (int)$_POST['session_id'] : 0;
$player_id = $_POST['player_id'] ?? 'UnknownPlayer';
$damage_time = $_POST['damage_time'] ?? date("Y-m-d H:i:s");
$damage_amount = isset($_POST['damage_amount']) ? (int)$_POST['damage_amount'] : 0;
$position_x = isset($_POST['position_x']) ? (float)$_POST['position_x'] : 0.0;
$position_y = isset($_POST['position_y']) ? (float)$_POST['position_y'] : 0.0;
$position_z = isset($_POST['position_z']) ? (float)$_POST['position_z'] : 0.0;

// Debug received data
echo "Received data: session_id=$session_id, player_id=$player_id, damage_time=$damage_time, damage_amount=$damage_amount, position=($position_x, $position_y, $position_z)<br>";

// Check if the session exists
$session_check_query = "SELECT * FROM game_sessions WHERE session_id = '$session_id'";
$session_check_result = $conn->query($session_check_query);

if ($session_check_result->num_rows === 0) {
    echo "Error: Session ID $session_id does not exist in game_sessions.";
    exit();
}

// Insert damage data
$sql = "INSERT INTO player_damage (session_id, player_id, damage_time, damage_amount, position_x, position_y, position_z)
        VALUES ('$session_id', '$player_id', '$damage_time', '$damage_amount', '$position_x', '$position_y', '$position_z')";

if ($conn->query($sql) === TRUE) {
    echo "Player damage data saved successfully.";
} else {
    echo "Error: " . $conn->error;
}

$conn->close();
?>
