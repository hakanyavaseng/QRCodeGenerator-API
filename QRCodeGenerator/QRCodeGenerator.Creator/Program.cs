using QRCodeGenerator.Creator.Concretes;

MessageService messageService = new();

messageService.ListenQueue();

Console.ReadLine();

