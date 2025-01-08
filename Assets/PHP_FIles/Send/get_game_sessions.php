<?php
// Database credentials
include 'db_connect.php';

// Create connection
$conn = new mysqli($host, $username, $password, $dbname);

// Query to get all game sessions
$sql = "SELECT * FROM game_sessions";
$result = $conn->query($sql);

$sessions = array(); // Array to store all the retrieved rows

if ($result->num_rows > 0) {
    // Fetch data and add to the array
    while ($row = $result->fetch_assoc()) {
        $sessions[] = $row;
    }
}

// Return JSON response
echo json_encode($sessions);

// Close connection
$conn->close();
?>
