<?php
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
echo "Connected successfully!";
?>
