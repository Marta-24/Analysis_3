<?php
// Include database connection
require_once 'db_connect.php';  // Connects to the database

// Check if all required POST parameters exist
if (isset($_POST['session_id'], $_POST['player_name'], $_POST['position_x'], $_POST['position_y'], $_POST['position_z'])) {
    $session_id = $_POST['session_id'];
    $player_name = $_POST['player_name'];
    $timestamp = date("Y-m-d H:i:s");  // Current time
    $position_x = floatval($_POST['position_x']);
    $position_y = floatval($_POST['position_y']);
    $position_z = floatval($_POST['position_z']);


    // Insert data into the player_paths table
    $sql = "INSERT INTO player_paths (session_id, player_name, timestamp, position_x, position_y, position_z) 
            VALUES ('$session_id', '$player_name', '$timestamp', '$position_x', '$position_y', '$position_z')";

    if (mysqli_query($conn, $sql)) {
        echo "Player path saved successfully!";
    } else {
        echo "Error: " . mysqli_error($conn);
    }
} else {
    echo "Missing parameters!";
}

mysqli_close($conn);
?>
