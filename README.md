# WebSocketTest

Connects to a Suprema BioStar 2 server via WebSocket to stream real-time event logs with optional filtering.

## 🚀 Features

- Connects to BioStar 2 WebSocket endpoint to get BioStar2 device's endpoints in real-time.
- Parses incoming `Event` JSON payloads for event log data.
- Supports client-side filters:
  - **Device ID** filter – only return events from specified devices.
  - **Event ID** filter – only return events of specific types.

## 🔧 Setup & Usage

1. Clone this repo and install dependencies:
    ```bash
    git clone https://github.com/terryjohndado/WebSocketTest.git
    cd WebSocketTest
    npm install
    ```

2. Update the WebSocket endpoint URL and BioStar2 API endpoint URL.
<img width="744" height="138" alt="image" src="https://github.com/user-attachments/assets/b584f971-92fa-430d-a05a-ea52fa99b527" />

3. Update BioStar2 login credentials.
<img width="322" height="175" alt="image" src="https://github.com/user-attachments/assets/d4b39008-2639-4f9b-acd5-799cc0a9ae49" />

4. Configure filters
Here, you can use bsEvent.Event.DeviceId.Id to filter the events by Device ID or
bsEvent.Event.EventTypeId.Code to filter the events by Event Code.

※ Explore Event class to check for other filters.
<img width="982" height="292" alt="image" src="https://github.com/user-attachments/assets/b68b36ac-a411-4390-9952-6845bdd44f2b" />

5. Run it:
    ```bash
    node index.js
    ```

6. The app will:
    - Connect over WSS to BioStar 2.
    - Authenticate as needed.
    - Listen for and parse incoming event logs
    - Filter based on `deviceFilter` and/or `eventFilter`.
    - Emit or log only the matching events.

