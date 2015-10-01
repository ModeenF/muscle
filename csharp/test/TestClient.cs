
using System;
using com.meyer.muscle.client.MessageTransceiver;
using com.meyer.muscle.client.StorageReflectConstants;
using com.meyer.muscle.iogateway.AbstractMessageIOGateway;
using com.meyer.muscle.message.Message;
using com.meyer.muscle.support.UnflattenFormatException;
using com.meyer.muscle.thread.MessageListener;
using com.meyer.muscle.thread.MessageQueue;
using System.Collections.Generic;
using System.Collections;
using System.IO;

/** A quick test class, similar to the C++ muscle/test/testreflectclient program */
namespace com.meyer.muscle.test {
public class TestClient : MessageListener, StorageReflectConstants
{
   public static  void main(string args[])
   {
      if (args.length != 1)
      {
         Console.WriteLine("Usage java com.meyer.muscle.test.TestClient 12.18.240.15");
         Environment.Exit(5);
      }
      
      new TestClient(args[0], (args.length >= 2) ? (new int?(args[1])).intValue() : 2960);
   }
   
   public TestClient(string hostName, int port)
   {      
      // Connect here....
      MessageTransceiver mc = new MessageTransceiver(new MessageQueue(this), AbstractMessageIOGateway.MUSCLE_MESSAGE_ENCODING_ZLIB_6, 6*1024*1024);
      Console.WriteLine("Connecting to " + hostName + ":" + port);
      mc.connect(hostName, port, "Session Connected", "Session Disconnected");
       
      // Listen for user input
      try {
         BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
         string nextLine;
         while((nextLine = br.readLine()) != null)
         {
            Console.WriteLine("You typed: ["+nextLine+"]");  
            StringTokenizer st = new StringTokenizer(nextLine);
            try {
               string command = st.nextToken();
               Message msg = new Message(PR_COMMAND_PING);
               if (command.equalsIgnoreCase("q")) break;
               else if (command.equalsIgnoreCase("t"))
               {
                  int [] ints = {1,3};
                  msg.setInts("ints", ints);
                  float [] floats = {2, 4};
                  msg.setFloats("floats", floats);
                  double [] doubles = {-5, -10};
                  msg.setDoubles("doubles", doubles);
                  byte [] bytes = {0x50};
                  msg.setBytes("bytes", bytes);
                  short [] shorts = {3,15};
                  msg.setShorts("shorts", shorts);
                  long [] longs = {50000};
                  msg.setLongs("longs", longs);
                  boolean [] booleans = {true, false};
                  msg.setBooleans("booleans", booleans);
                  string [] strings = {"Hey", "What's going on?"};
                  msg.setStrings("strings", strings);
                  
                  Message q = (Message) msg.cloneFlat();
                  Message r = new Message(1234);
                  r.setString("I'm way", "down here!");
                  q.setMessage("submessage", r);
                  msg.setMessage("message", q);
               }
               else if (command.equals("s")) {
                  msg.what = PR_COMMAND_SETDATA;
                  msg.setMessage(st.nextToken(), new Message(1234));
               }   
               else if (command.equals("k")) {
                  msg.what = PR_COMMAND_KICK;
                  msg.setString(PR_NAME_KEYS, st.nextToken());
               }
               else if (command.equals("b")) {
                  msg.what = PR_COMMAND_ADDBANS;
                  msg.setString(PR_NAME_KEYS, st.nextToken());
               }
               else if (command.equals("B")) {
                  msg.what = PR_COMMAND_REMOVEBANS;
                  msg.setString(PR_NAME_KEYS, st.nextToken());
               }
               else if (command.equals("g")) {
                  msg.what = PR_COMMAND_GETDATA;
                  msg.setString(PR_NAME_KEYS, st.nextToken());
               }   
               else if (command.equals("p")) {
                  msg.what = PR_COMMAND_SETPARAMETERS;
                  msg.setString(st.nextToken(), "");
               }
               else if (command.equals("d")) {
                  msg.what = PR_COMMAND_REMOVEDATA;
                  msg.setString(PR_NAME_KEYS, st.nextToken());
               }   
               else if (command.equals("D")) {
                  msg.what = PR_COMMAND_REMOVEPARAMETERS;
                  msg.setString(PR_NAME_KEYS, st.nextToken());
               }              
               else if (command.equals("big")) {
                  msg.what = PR_COMMAND_PING;
                  int numFloats = 1*1024*1024;
                  float lotsafloats[] = new float[numFloats];  // 4MB worth of floats!
                  for (int i=0; i<numFloats; i++) lotsafloats[i] = (float)i;
                  msg.setFloats("four_megabytes_of_floats", lotsafloats);                  
               }
               else if (command.equals("toobig")) {
                  msg.what = PR_COMMAND_PING;
                  int numFloats = 2*1024*1024;
                  float lotsafloats[] = new float[numFloats];  // 8MB worth of floats!
                  for (int i=0; i<numFloats; i++) lotsafloats[i] = (float)i;
                  msg.setFloats("eight_megabytes_of_floats", lotsafloats);                  
               }
               else if (command.equals("e")) {
                   msg.what = PR_COMMAND_SETPARAMETERS;
                   msg.setInt(PR_NAME_REPLY_ENCODING, AbstractMessageIOGateway.MUSCLE_MESSAGE_ENCODING_ZLIB_6);
               }
               if (msg != null) mc.sendOutgoingMessage(msg);
            }
            catch(NoSuchElementException ex) {
               Console.WriteLine("Missing argument!");
            }   
         }
      }
      catch(UnflattenFormatException ex) {
         ex.printStackTrace();
      }
      catch(IOException ex) {
         ex.printStackTrace();
      }
           
      Console.WriteLine("Bye!");
      mc.disconnect();
   }
   
   /** Note that this method will be called from another thread! */
   public synchronized void messageReceived(object message, int numLeft)
   {
      if (message instanceof Message) Console.WriteLine("Got message: " + message);
      else
      {
         Console.WriteLine("Got non-Message: " + message);
         if (message.equals("Session Disconnected")) Environment.Exit(5);
      }      
   }
}
    