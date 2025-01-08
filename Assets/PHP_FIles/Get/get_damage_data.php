<?php
require_once 'db_connect.php';

$sql = "SELECT session_id, player_id, damage_time, damage_amount, position_x, position_y, position_z FROM player_damage";
$result = mysqli_query($conn, $sql);

// Verificar si la consulta falló
if (!$result) {
    echo json_encode(["error" => "SQL Error: " . mysqli_error($conn)]);
    mysqli_close($conn);
    exit();
}

$damages = array();
while ($row = mysqli_fetch_assoc($result)) {
    $damages[] = $row;
}

header('Content-Type: application/json');
echo json_encode($damages);

mysqli_close($conn);
?>
