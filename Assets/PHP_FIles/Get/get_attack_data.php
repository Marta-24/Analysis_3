<?php
require_once 'db_connect.php';

$sql = "SELECT session_id, player_id, attack_time, damage_amount, position_x, position_y, position_z FROM player_attacks";
$result = mysqli_query($conn, $sql);

$attacks = array();
while ($row = mysqli_fetch_assoc($result)) {
    $attacks[] = $row;
}

header('Content-Type: application/json');
echo json_encode($attacks);

mysqli_close($conn);
?>
