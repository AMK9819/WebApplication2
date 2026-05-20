import os
import time
from azure.storage.queue import QueueClient, BinaryBase64DecodePolicy, BinaryBase64EncodePolicy

# 1. Setup your connection string and queue name
# Best Practice: Store your connection string in Azure App Settings as an Environment Variable
CONNECTION_STRING = os.getenv("AzureWebJobsStorage") 
QUEUE_NAME = "test-webjob-queue"

def process_queue_messages():
    if not CONNECTION_STRING:
        print("Error: AzureWebJobsStorage connection string environment variable is not set.")
        return

    print(f"Starting WebJob. Listening to queue: '{QUEUE_NAME}'...")
    
    # Initialize the QueueClient
    # Note: We use Base64 decoding policies because many Azure services (like Functions or .NET apps)
    # encode queue messages in Base64 by default.
    queue_client = QueueClient.from_connection_string(
        conn_str=CONNECTION_STRING, 
        queue_name=QUEUE_NAME,
        message_decode_policy=BinaryBase64DecodePolicy(),
        message_encode_policy=BinaryBase64EncodePolicy()
    )

    while True:
        try:
            # Look for messages (visibility_timeout gives the job 30 seconds to finish before returning to queue)
            messages = queue_client.receive_messages(messages_per_page=1, visibility_timeout=30)
            
            message_found = False
            for message in messages:
                message_found = True
                print(f"\n--- New Message Received ---")
                print(f"ID: {message.id}")
                
                # Try to decode the content to text
                try:
                    content = message.content.decode('utf-8')
                    print(f"Content: {content}")
                except Exception:
                    print(f"Raw Content (Bytes): {message.content}")

                print("Processing message task...")
                # Simulating work
                time.sleep(2) 
                
                # CRITICAL: Delete the message from the queue so it isn't re-processed
                queue_client.delete_message(message)
                print("Message successfully processed and deleted from queue.")

            # If the queue was empty, sleep for a few seconds to avoid burning CPU cycles/API calls
            if not message_found:
                time.sleep(5)

        except Exception as e:
            print(f"An error occurred while polling the queue: {e}")
            time.sleep(10) # Sleep longer if there's a connection issue

if __name__ == "__main__":
    process_queue_messages()