import { Component, OnInit, NgZone, ViewChild, AfterViewChecked } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { first } from 'rxjs/operators';
import * as $ from "jquery";
import { AlertService } from 'src/app/services/alert.service';
import { RegisterService } from 'src/app/services/register.service';
import { LoginService } from 'src/app/services/login.service';
import { ChatService } from 'src/app/services/chat.service';
import { CreateNewGroup, Group, UserGroup } from '../models/userGroup';
import { Message } from '../models/message';

@Component({
  selector: 'chat-room',
  templateUrl: './chat-room.component.html'
})
export class ChatRoomComponent implements OnInit, AfterViewChecked {
  loading = false;
  Users = [];
  UserGroups = [];
  currentGroupId;
  Messages = [];
  loggedInUserName;
  container;
  ErrorMessage;
  IsError = false;
  AllGroups = [];
  IsUserInGroup = false;
  selectedGroupId;
  IsButtonVisible = false;

  constructor(
    private router: Router,
    private registerService: RegisterService,
    private loginService: LoginService,
    private alertService: AlertService,
    private chatService: ChatService,
    private _ngZone: NgZone
  ) {
    this.subscribeToEvents();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  ngOnInit() {
    // redirect to home if already logged in
    if (localStorage["currentUser"] != undefined) {
      this.router.navigate(['/chat']);
    }
    this.loggedInUserName = JSON.parse(localStorage.getItem('currentUser')) != undefined ? JSON.parse(localStorage.getItem('currentUser')).userName : undefined;
    if (this.loggedInUserName == undefined) {
      this.router.navigate(['/']);
    }
    //this.LoadUsers();
    //this.getUserGroups();
    this.IsButtonVisible = false;
    this.loadGroups();
  }

  LoadUsers() {
    this.registerService.getAllUsers()
      .pipe(first())
      .subscribe(users => this.Users = users);
  }

  getUserGroups() {
    this.registerService.getUserGroups(this.loggedInUserName)
      .pipe(first())
      .subscribe(userGroups => this.UserGroups = userGroups);
  }

  loadGroups() {
    this.registerService.loadGroups()
      .pipe(first())
      .subscribe(groups => this.AllGroups = groups);
  }

  CreateGroup() {
    if ($("#GroupName").val().toString() == '') {
      this.IsError = true;
      this.ErrorMessage = "Enter Group Name";
      return;
    }
    this.IsError = false;
    let data = {
      groupName: $("#GroupName").val().toString(),
    };

    var newGroup = new Group();
    newGroup.groupName = data.groupName;
    this.registerService.createGroup(newGroup)
      .pipe(first())
      .subscribe(
        data => {
          this.loading = false;
          this.loadGroups();
          this.ClearForm();
          var modal: any = $('#CreateNewGroup');
          modal.hide();
          $(".modal-backdrop").remove();
        },
        error => {
          alert(error.error);
          this.loading = false;
        });
  }

  CreateNewGroup() {
    if ($("#GroupName").val().toString() == '') {
      this.IsError = true;
      this.ErrorMessage = "Enter Group Name";
      return;
    }
    this.IsError = false;
    let UserNames = $("input[name='UserName[]']:checked")
      .map(function () {
        return $(this).val().toString();
      }).get();

    let data = {
      groupName: $("#GroupName").val().toString(),
      UserNames: UserNames
    };

    var newGroup = new CreateNewGroup();
    newGroup.groupName = data.groupName;
    newGroup.userNames = data.UserNames;

    this.registerService.createNewGroup(newGroup)
      .pipe(first())
      .subscribe(
        data => {
          this.loading = false;
          this.getUserGroups();
          this.ClearForm();
          var modal: any = $('#CreateNewGroup');
          modal.hide();
          $(".modal-backdrop").remove();
        },
        error => {
          this.ErrorMessage = this.alertService.error(error.error);
          this.loading = false;
        });
  }

  ClearForm() {
    $("#GroupName").val("");
  }

  SendMessage() {
    var newMsg = new Message();
    newMsg.addedBy = this.loggedInUserName;
    newMsg.GroupID = this.currentGroupId;
    newMsg.textMessage = $("#Message").val().toString();
    this.chatService.saveMessage(newMsg)
      .pipe(first())
      .subscribe(
        data => {
          this.chatService.sendMessage(newMsg);
        },
        error => {
          this.alertService.error(error.error);
        });
  }

  WriteMessageToGroup(message: Message) {
    this.Messages.push(message);
    this.scrollToBottom();
    $("#Message").val('');
  }

  selectGroup(groupObj) {
    this.IsButtonVisible = true;
    this.selectedGroupId = groupObj.value.id;
    this.LoadMessages();
  }

  LoadMessages() {
    this.IsUserInGroup = false;
    let group_id = this.selectedGroupId;
    let userName = this.loggedInUserName;
    let msg = "";
    this.selectedGroupId = group_id;

    $('.group').css({ "border-style": "none", cursor: "pointer" });
    $(this).css({ "border-style": "inset", cursor: "default" });

    $("#currentGroup").val(group_id);
    this.currentGroupId = group_id;
    this.registerService.checkUserSubscribedToGroup(group_id, userName).pipe(first())
      .subscribe(data => {
        this.IsUserInGroup = data.isUserInGroup;
        if (data.isUserInGroup) {
          this.chatService.getGroupMessages(this.currentGroupId)
            .subscribe(messages => {
              this.Messages = messages;
              this.scrollToBottom();
            },
              error => {
                this.Messages = [];
              });
        }
        else {
          this.Messages = [];
        }
      },
        error => {
          this.Messages = [];
        });

  }

  private subscribeToEvents(): void {

    this.chatService.messageReceived.subscribe((message: Message) => {
      this._ngZone.run(() => {
        this.WriteMessageToGroup(message);
      });
    });
  }

  Logout() {
    this.loginService.logout();
    this.router.navigate(['/']);
  }

  scrollToBottom(): void {
    this.container = $("#chat_container");
    this.container[0].scrollTop = this.container[0].scrollHeight;
  }

  JoinGroup() {
    let groupId = this.selectedGroupId;
    let userName = this.loggedInUserName;

    var uGrp = new UserGroup()
    uGrp.groupId = groupId;
    uGrp.userName = userName;

    this.registerService.joinGroup(uGrp).pipe(first())
      .subscribe(data => {
        if (data.isGroupFull) {
          this.chatService.addToGroup(groupId);
          //this.alertService.error(data.message);
          alert(data.message);
        }
        else {
          this.chatService.addToGroup(groupId);
          alert(data.message);
          this.LoadMessages();
        }
      },
        error => {
          this.Messages = [];
        });
  }

  LeaveGroup() {
    let groupId = this.selectedGroupId;
    let userName = this.loggedInUserName;

    var uGrp = new UserGroup()
    uGrp.groupId = groupId;
    uGrp.userName = userName;

    this.registerService.leaveGroup(uGrp).pipe(first())
      .subscribe(data => {
        if (data.IsDeleted) {
          this.chatService.removeFromGroup(groupId);
          alert(data.errorMessage);
          this.Messages = [];
        }
        else {
          this.chatService.removeFromGroup(groupId);
          alert(data.errorMessage);
          this.LoadMessages();
        }
      },
        error => {
          this.Messages = [];
        });
  }


}
