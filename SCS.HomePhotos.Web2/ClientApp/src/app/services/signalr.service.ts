import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
    private subject = new Subject<any>();
    private hubConnection?: signalR.HubConnection;

    public startConnection = () => {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('/message-hub')
            .build();

        this.hubConnection
            .start()
            .then(() => console.log('SignalR connection started'))
            .catch(err => console.error('SignalR connection failed: ' + err));
    }

    public listenForAdminMessages = () => {
        this.hubConnection?.on('SendAdminsMessage', (type, message) => {
            console.log(message);
            this.subject.next({ type: type, message: message });
        });
    }

    public listenForEveryoneMessages = () => {
        this.hubConnection?.on('SendEveryoneMessage', (type, message) => {
            console.log(message);
            this.subject.next({ type: type, message: message });
        });
    }

    getMessages(): Observable<any> {
        return this.subject.asObservable();
    }
}
