using System;
using com.meyer.muscle.client.DatagramPacketTransceiver;
using com.meyer.muscle.iogateway.AbstractMessageIOGateway;
using com.meyer.muscle.iogateway.MessageIOGatewayFactory;
using com.meyer.muscle.message.Message;
using com.meyer.muscle.thread.MessageListener;
using com.meyer.muscle.thread.MessageQueue;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace com.meyer.muscle.test {
public class UDPClient extends DatagramPacketTransceiver {
    MessageQueue sendQueue;
	MessageQueue receiveQueue;
	
	public UDPClient() throws Exception {
// Using a slave gateway
//		super(new DatagramSocket(1234, InetAddress.getLocalHost()), null, MessageIOGatewayFactory.getMessageIOGateway(AbstractMessageIOGateway.MUSCLE_MESSAGE_ENCODING_ZLIB_9));
// Raw Message I/O
		super(new DatagramSocket(1234, InetAddress.getLocalHost()));
		sendQueue = new MessageQueue(this);
		
		receiveQueue = new MessageQueue(new MessageListener() {
			public synchronized void messageReceived(object message, int numLeftInQueue) throws Exception {
				Console.WriteLine("Received: " + message);
			}
		});
		setListenQueue(receiveQueue);
	}
	
	public DatagramPacket prepPacket(DatagramPacket packet) {
		try {
			packet.setAddress(InetAddress.getLocalHost());
		} catch (Exception ex) {
		}
		packet.setPort(1234);
		return packet;
	}
	
	public static void main(string[] args) {
		try {
			UDPClient foo = new UDPClient();
			foo.start();
			foo.sendQueue.postMessage(new Message(1097756239));
			//foo.shutdown();
		} catch (Exception ex) {
		}
	}
}
}