<?php
require_once 'db_connect.php';

$sql = "SELECT session_id, player_id, pause_time, position_x, position_y, position_z FROM pause_events";
$result = mysqli_query($conn, $sql);

// Verificar si la consulta falló
if (!$result) {
    echo json_encode(["error" => "SQL Error: " . mysqli_error($conn)]);
    mysqli_close($conn);
    exit();
}

$pauses = array();
while ($row = mysqli_fetch_assoc($result)) {
    $pauses[] = $row;
}

header('Content-Type: application/json');
echo json_encode($pauses);

mysqli_close($conn);
?>
