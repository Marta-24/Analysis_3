<?php
// Enable error reporting for debugging
ini_set('display_errors', 1);
error_reporting(E_ALL);

// Include database connection credentials
include 'db_connect.php';

$conn = new mysqli($host, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// Output received data for debugging
echo "Received POST data: ";
print_r($_POST);

// Check if required fields are present
if (isset($_POST['session_id'], $_POST['player_id'], $_POST['attack_time'], $_POST['damage_amount'], $_POST['position_x'], $_POST['position_y'], $_POST['position_z'])) {
    $session_id = $conn->real_escape_string($_POST['session_id']);
    $player_id = $conn->real_escape_string($_POST['player_id']);
    $attack_time = $conn->real_escape_string($_POST['attack_time']);
    $damage_amount = $conn->real_escape_string($_POST['damage_amount']);
    $position_x = $conn->real_escape_string($_POST['position_x']);
    $position_y = $conn->real_escape_string($_POST['position_y']);
    $position_z = $conn->real_escape_string($_POST['position_z']);

    // SQL insert statement
    $sql = "INSERT INTO player_attacks (session_id, player_id, attack_time, damage_amount, position_x, position_y, position_z) 
            VALUES ('$session_id', '$player_id', '$attack_time', '$damage_amount', '$position_x', '$position_y', '$position_z')";

    if ($conn->query($sql) === TRUE) {
        echo "Attack logged successfully!";
    } else {
        echo "SQL Error: " . $conn->error;
    }
} else {
    echo "Invalid request: Missing required fields.";
}

// Close connection
$conn->close();
?>
