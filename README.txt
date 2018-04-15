PART OF THE CRYTEK SERVER PROGRAMMER TEST
--------------------------------------------------------------------------------------------------
For testing there is a TEST.BAT file that starts a server with two clients (You need to fill the server ip's which you can see at the first run)
Also two other bat files start only a client or only a server
The binaries are located at the %solution%\binaries\Development path


The application can run in server or in client mode
My design intention was to have a solution with multiple decoupled and cohesive modules enabling testability
Therefore I have used a DI container, namely Ninject
Through a common interface IEchoApp the program would execute Start Stop and SendMessage methods meaning a client or server is able to send message
Listening for clients and for server message is done asynchronously on the thread pool for a better scalability  and to enable multiple client
A multilayered architecture would enable unit testing and easy changes done in the logic or presentation

Some things left to be done (due to lack of spare time)
-> severs would also communicate with each other so the messages would go to clients connected to different servers
-> server class needs to be decoupled like the client so it can be testable (no tcp client or listener dependency)
