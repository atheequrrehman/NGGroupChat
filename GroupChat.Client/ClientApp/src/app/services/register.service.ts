import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from 'src/app/models/user';
import { UserGroup, CreateNewGroup, Group } from 'src/app/models/userGroup';

@Injectable({ providedIn: 'root' })
export class RegisterService {
  private currentUserSubject: BehaviorSubject<User>;
  public currentUser: Observable<User>;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  register(user: User) {
    return this.http.post(`https://groupchatservice.azurewebsites.net/api/user/RegisterUser`, user, { responseType: 'text' });
  }

  getAllUsers() {
    return this.http.get<User[]>(`https://groupchatservice.azurewebsites.net/api/user/GetAllUsers`);
  }

  getUserGroups(userName) {
    return this.http.get<UserGroup[]>('https://groupchatservice.azurewebsites.net/api/user/GetUserGroups/' + userName);
  }

  createNewGroup(newGroup: CreateNewGroup) {
    return this.http.post('https://groupchatservice.azurewebsites.net/api/user/CreateNewGroup', newGroup, { responseType: 'text' });
  }

  createGroup(newGroup: Group) {
    return this.http.post('https://groupchatservice.azurewebsites.net/api/user/CreateGroup', newGroup, { responseType: 'text' });
  }

  loadGroups() {
    return this.http.get<Group[]>('https://groupchatservice.azurewebsites.net/api/user/GetAllGroups/');
  }

  checkUserSubscribedToGroup(GroupId: number, UserName: string) {
    return this.http.get<any>('https://groupchatservice.azurewebsites.net/api/user/CheckUserSubscribedToGroup/' + GroupId + '/' + UserName);
  }

  joinGroup(userGroup: UserGroup) {
    return this.http.post<any>('https://groupchatservice.azurewebsites.net/api/user/JoinGroup/', userGroup);
  }

  leaveGroup(userGroup: UserGroup) {
    return this.http.post<any>('https://groupchatservice.azurewebsites.net/api/user/LeaveGroup/', userGroup);
  }
}
