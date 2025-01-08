<?php
require_once 'db_connect.php';

$sql = "SELECT session_id, player_id, interaction_type, interaction_time, position_x, position_y, position_z FROM player_interactions";
$result = mysqli_query($conn, $sql);

$interactions = array();
while ($row = mysqli_fetch_assoc($result)) {
    $interactions[] = $row;
}

header('Content-Type: application/json');
echo json_encode($interactions);

mysqli_close($conn);
?>
