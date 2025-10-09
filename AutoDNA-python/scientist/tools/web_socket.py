import asyncio
import websocket
from typing import Callable, Coroutine, Dict, Any, Optional
from config import scheduler_address 
import threading

CallbackType = Callable[[str], Coroutine[Any, Any, None]]

class WebSocketClient:
    _instance: Optional['WebSocketClient'] = None

    @staticmethod
    def get_instance() -> 'WebSocketClient':
        if WebSocketClient._instance is None:
            WebSocketClient._instance = WebSocketClient("ws://" + scheduler_address)
            WebSocketClient._instance.connect()
        return WebSocketClient._instance

    def __init__(self, uri: str):
        self.uri = uri
        self.websocket = None
        self._receive_task: Optional[asyncio.Task] = None
        self._callbacks: Dict[str, CallbackType] = {}

    def connect(self):
        try:
            self.websocket = websocket.create_connection(self.uri)
            print("Connected to server.")
            self._receive_task = asyncio.create_task(self._receive_messages())
        except Exception as e:
            print(f"Connection failed: {e}")
            self.websocket = None
            raise

    def disconnect(self):
        if self.websocket:
            print("Closing connection...")
            # 关闭 WebSocket 连接，这将导致 _receive_thread 中的 recv() 方法抛出异常并结束
            self.websocket.close()
            # 如果接收线程存在且正在运行，等待它结束
            if self._receive_thread and self._receive_thread.is_alive():
                self._receive_thread.join()
            print("Connection closed.")
            self.websocket = None

    def register_callback(self, callback_id: str, callback) -> bool:
        if callback_id in self._callbacks:
            print(f"Callback ID '{callback_id}' already exists. Registration failed.")
            return False
        
        self._callbacks[callback_id] = callback
        print(f"Successfully registered callback ID '{callback_id}'.")
        return True
    
    def unregister_callback(self, callback_id: str) -> bool:
        if callback_id in self._callbacks:
            del self._callbacks[callback_id]
            print(f"Successfully unregistered callback ID '{callback_id}'.")
            return True
        print(f"Callback ID '{callback_id}' not found, cannot unregister.")
        return False

    def _receive_messages(self):
        try:
            while self.websocket and self.websocket.connected:
                # 阻塞式接收消息
                message = self.websocket.recv()
                if not message: # 连接关闭时 recv() 可能返回空
                    break
                print(f"Received message: {message}")
                
                # 遍历并调用所有注册的回调
                if self._callbacks:
                    for cb in self._callbacks.values():
                        # 在同步模式下，直接调用回调函数
                        cb(message)
        except websocket.WebSocketConnectionClosedException:
            print("Connection closed by server.")
        except Exception as e:
            print(f"An error occurred during reception: {e}")
        finally:
            self.websocket = None # 确保在连接断开后将 websocket 设为 None

    def connect(self):
        try:
            self.websocket = websocket.create_connection(self.uri)
            print("Connected to server.")
            self._receive_thread = threading.Thread(target=self._receive_messages)
            self._receive_thread.start()
        except Exception as e:
            print(f"Connection failed: {e}")
            self.websocket = None
            raise

    def send_message(self, message: str):
        if self.websocket and self.websocket.connected:
            self.websocket.send(message)
            print(f"Sent message: {message}")
        else:
            print("WebSocket disconnected, cannot send message.")


async def handle_log_message(message: str):
    """一个处理日志的回调函数。"""
    print(f"[日志处理器] 消息内容: {message}")
    
async def handle_uppercase_message(message: str):
    """一个将消息转换为大写的处理函数。"""
    print(f"[大写转换器] 转换后的消息: {message.upper()}")

async def main():
    server_uri = "ws://localhost:8765"
    client = WebSocketClient(server_uri)
    
    # 注册回调函数
    client.register_callback("logger", handle_log_message)
    client.register_callback("upper_case", handle_uppercase_message)
    
    # 尝试重复注册，会看到失败的提示
    client.register_callback("logger", handle_log_message)

    try:
        await client.connect()
        
        # 客户端发送消息，所有注册的回调都会被调用
        await client.send_message("Hello, world!")
        
        # 保持运行一段时间以便接收更多消息
        print("\n等待5秒钟以接收更多消息...")
        await asyncio.sleep(5)
        
    except Exception as e:
        print(f"主程序发生错误：{e}")
    finally:
        await client.disconnect()

if __name__ == "__main__":
    asyncio.run(main())