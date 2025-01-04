<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

// Database configuration
include 'db_connect.php';

try {
    if ($_SERVER['REQUEST_METHOD'] === 'POST') {
        // Get the data from the form POST request
        $session_id = $_POST['session_id'] ?? null;
        $player_name = $_POST['player_name'] ?? null;
        $cause = $_POST['cause'] ?? null;
        $x = $_POST['x'] ?? null;
        $y = $_POST['y'] ?? null;
        $z = $_POST['z'] ?? null;

        // Validate data
        if (!$session_id || !$player_name || !$cause || !$x || !$y || !$z) {
            die("Missing required fields: session_id, player_name, cause, or position.");
        }

        // Check if the session ID and player name exist in the `game_sessions` table
        $session_query = "SELECT * FROM game_sessions WHERE session_id = ? AND player_id = ?";
        $stmt = $conn->prepare($session_query);
        $stmt->bind_param('is', $session_id, $player_name);
        $stmt->execute();
        $result = $stmt->get_result();
        if ($result->num_rows === 0) {
            die("Session ID and player name not found or mismatched.");
        }

        $death_time = date('Y-m-d H:i:s');  // Current time for death_time

        // Insert the death data into the database
        $sql = "INSERT INTO deaths (session_id, player_id, cause, death_time, x, y, z) VALUES (?, ?, ?, ?, ?, ?, ?)";
        if ($stmt = $conn->prepare($sql)) {
            $stmt->bind_param('isssddd', $session_id, $player_name, $cause, $death_time, $x, $y, $z);
            if ($stmt->execute()) {
                $response = array(
                    'id' => $stmt->insert_id,
                    'message' => 'Death recorded successfully!'
                );
                echo json_encode($response);
            } else {
                echo "Error: " . $conn->error;
            }
            $stmt->close();
        } else {
            echo "Error preparing statement: " . $conn->error;
        }
    } else {
        echo "No form data received.";
    }
    $conn->close();
} catch (Exception $e) {
    echo "Error: " . $e->getMessage();
}
?>
