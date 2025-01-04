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

        // Validate data
        if (!$session_id || !$player_name || !$cause) {
            die("Missing required fields: session_id, player_name, or cause.");
        }

        // Check if the session ID exists and matches the player_name
        $session_query = "SELECT * FROM game_sessions WHERE session_id = ? AND player_id = ?";
        $stmt = $conn->prepare($session_query);
        if (!$stmt) {
            die("Error preparing session query: " . $conn->error);
        }
        $stmt->bind_param('is', $session_id, $player_name);  // Expect session_id as INT and player_name as STRING
        $stmt->execute();
        $result = $stmt->get_result();
        if ($result->num_rows === 0) {
            die("Session ID and player name not found or mismatched.");
        }

        $death_time = date('Y-m-d H:i:s');  // Current time for death_time

        // Insert the death data into the deaths table
        $sql = "INSERT INTO deaths (session_id, player_id, cause, death_time) VALUES (?, ?, ?, ?)";
        $stmt = $conn->prepare($sql);
        if (!$stmt) {
            die("Error preparing insert statement: " . $conn->error);
        }
        $stmt->bind_param('isss', $session_id, $player_name, $cause, $death_time);
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
        echo "No form data received.";
    }
    $conn->close();
} catch (Exception $e) {
    echo "Error: " . $e->getMessage();
}
?>
