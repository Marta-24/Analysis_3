<?php
require_once 'db_connect.php';

$sql = "SELECT session_id FROM game_sessions ORDER BY session_id DESC LIMIT 1";  // Get the last session_id
$result = mysqli_query($conn, $sql);

if ($result && mysqli_num_rows($result) > 0) {
    $row = mysqli_fetch_assoc($result);
    echo json_encode($row);  // Send session_id as JSON
} else {
    echo json_encode(["error" => "No session found"]);
}

mysqli_close($conn);
?>
