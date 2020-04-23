import { EventEmitter, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from 'src/app/models/user';
import { UserGroup, CreateNewGroup } from 'src/app/models/userGroup';
import { Message } from 'src/app/models/message';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private currentUserSubject: BehaviorSubject<User>;
  public currentUser: Observable<User>;
  messageReceived = new EventEmitter<Message>();
  connectionEstablished = new EventEmitter<Boolean>();
  private connectionIsEstablished = false;
  private _hubConnection: HubConnection;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
    this.createConnection();
    this.registerOnServerEvents();
    this.startConnection();
  }

  saveMessage(newMessage: Message) {
    return this.http.post<Message>('https://groupchatservice.azurewebsites.net/api/groupchat/CreateMessage', newMessage);
  }

  sendMessage(newMessage: Message) {
    this._hubConnection.invoke('SendMessageToGroup', newMessage);
  }

  addToGroup(groupId: number) {
    this._hubConnection.invoke('AddToGroup', groupId);
  }

  removeFromGroup(newMessage: Message) {
    this._hubConnection.invoke('RemoveFromGroup', newMessage);
  }

  getGroupMessages(groupID: number) {
    return this.http.get<Message[]>('https://groupchatservice.azurewebsites.net/api/groupchat/GetGroupMessages/' + groupID);
  }

  private createConnection() {
    this._hubConnection = new HubConnectionBuilder()
      .withUrl('https://groupchatservice.azurewebsites.net/notify')
      .build();
  }

  private startConnection(): void {
    this._hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        console.log('Hub connection started');
        this.connectionEstablished.emit(true);
      })
      .catch(err => {
        console.log('Error while establishing connection, retrying...');
        setTimeout(function () { this.startConnection(); }, 5000);
      });
  }

  private registerOnServerEvents(): void {
    this._hubConnection.on('MessageReceived', (data: any) => {
      this.messageReceived.emit(data);
    });
  }

}
