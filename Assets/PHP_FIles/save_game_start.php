<?php
// Database credentials
$host = "localhost";
$username = "albertcf5";
$password = "48103884m";
$dbname = "albertcf5";

// Create connection
$conn = new mysqli($host, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// Debugging: Print all received POST data
print_r($_POST);

// Get POST data
$player_id = isset($_POST['player_id']) ? $_POST['player_id'] : null;
$start_time = isset($_POST['start_time']) ? $_POST['start_time'] : null;

// Check for missing data
if (!$player_id || !$start_time) {
    die("Missing player_id or start_time");
}

// Insert data into the database
$sql = "INSERT INTO game_sessions (player_id, start_time) VALUES ('$player_id', '$start_time')";
if ($conn->query($sql) === TRUE) {
    echo "Data saved successfully!";
} else {
    // Log SQL error
    echo "Error: " . $sql . "<br>" . $conn->error;
}

// Close connection
$conn->close();
?>
