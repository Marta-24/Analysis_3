<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

include 'db_connect.php';

$session_id = isset($_POST['session_id']) ? (int)$_POST['session_id'] : 0;
$player_id = $_POST['player_id'] ?? 'UnknownPlayer';
$interaction_type = $_POST['interaction_type'] ?? 'Unknown Interaction';
$interaction_time = $_POST['interaction_time'] ?? date("Y-m-d H:i:s");
$position_x = isset($_POST['position_x']) ? (float)$_POST['position_x'] : 0.0;
$position_y = isset($_POST['position_y']) ? (float)$_POST['position_y'] : 0.0;
$position_z = isset($_POST['position_z']) ? (float)$_POST['position_z'] : 0.0;

$sql = "INSERT INTO player_interactions (session_id, player_id, interaction_type, interaction_time, position_x, position_y, position_z)
        VALUES ('$session_id', '$player_id', '$interaction_type', '$interaction_time', '$position_x', '$position_y', '$position_z')";

if ($conn->query($sql) === TRUE) {
    echo "Player interaction saved successfully.";
} else {
    echo "Error: " . $conn->error;
}

$conn->close();
?>
