PART OF THE CRYTEK SERVER PROGRAMMER TEST
--------------------------------------------------------------------------------------------------
For testing there is a TEST.BAT file that starts a server with two clients (You need to fill the server ip's which you can see at the first run)
The binaries are located at the \binaries\Development


The application can run in server or in client mode
My design intention was to have a solution with multiple decoupled and cohesive modules enabling testability
Therefore I have used a DI container namely Ninject
Through a common interface IEchoApp the program would execute Start Stop and SendMessage methods meaning a client or server is able to send message
Listening for clients and for server message is done asynchronously on the thread pool for a better scalability  and to enable multiple client
A multilayred architecture would enable unit testing and easy changes done in the logic or presentation

Due to lack of time is not fully completed 
Some things left to be done (due to lack of spare time)
-> decouple all the TCP related logic through one or classes responsible for 'low level' communing enabling behavior to be unit testable
-> severs would also communicate with each other so the messages would go to clients connected to different servers
-> there are some bugs when disconnecting clients which I did not have time to solve
-> server class needs to be decoupled like the client so it can be testable (no TCPclient or listner dependency)
