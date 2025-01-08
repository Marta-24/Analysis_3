<?php
require_once 'db_connect.php';

$sql = "SELECT session_id, player_name, timestamp, position_x, position_y, position_z FROM player_paths";
$result = mysqli_query($conn, $sql);

$paths = array();
while ($row = mysqli_fetch_assoc($result)) {
    $paths[] = $row;
}

header('Content-Type: application/json');
echo json_encode($paths);

mysqli_close($conn);
?>
