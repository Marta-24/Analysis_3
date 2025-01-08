<?php
require_once 'db_connect.php';

// Consulta SQL para recuperar los datos de muertes
$sql = "SELECT session_id, player_id, death_time, x, y, z FROM deaths";
$result = mysqli_query($conn, $sql);

// Verificar si la consulta falló
if (!$result) {
    echo json_encode(["error" => "SQL Error: " . mysqli_error($conn)]);
    mysqli_close($conn);
    exit();
}

$deaths = array();
while ($row = mysqli_fetch_assoc($result)) {
    $deaths[] = $row;  // Añadir cada fila al array
}

header('Content-Type: application/json');
echo json_encode($deaths);  // Devolver los datos en formato JSON

mysqli_close($conn);
?>
