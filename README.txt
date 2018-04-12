PART OF THE CRYTEK SERVER PROGRAMMER TEST
--------------------------------------------------------------------------------------------------
the application can run in server and in client mode
my design intention was to have  multiple  decoupled and cohesive modules enabling testability
through a common interface IEchoApp the program would execute Start Stop and SendMessage methods
listening for clients and for server message is done asycnhronously on the thread pool for a better scalability  and to enable muliple cleints

due to lack of time is not fully completed 
some things left to be done
-> decouple all the tcp related logic through one or classes resposible for 'low level' communing enabling behaiviour to be unit testabele
