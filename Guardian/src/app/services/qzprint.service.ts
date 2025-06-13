import { Injectable } from '@angular/core';
import { NavigationService } from './navigation.service';
import { AlertLevel } from '../Enums/enums';

@Injectable({
  providedIn: 'root'
})
export class QzprintService {

  constructor(
    private navigationService: NavigationService,
  ) 
  { }

  async findPrinters(): Promise<string[]> {
    let response = [];
    try{
      await this.connectToQZTray();
      let data = await qz.printers.find();
      var list = '';
      for(var i = 0; i < data.length; i++) {
          response.push(data[i]);
      }
     
      await this.disconnectQZTray();
    }
    catch(displayError)
    {
      console.log('>' + displayError);
      this.navigationService.showUIMessage(JSON.stringify(displayError), AlertLevel.Error);
    }
    return response;
}

  async connectToQZTray() {
    try 
    {
      if (!qz.websocket.isActive()) 
      {
        let config = { host: 'localhost', usingSecure: true };
        await qz.websocket.connect(config)
        console.log('Connected to QZ Tray successfully');  
      }
    } 
    catch (error) {
      this.navigationService.showUIMessage('Error al conectarse a QZ Tray ' + error, AlertLevel.Error);
    }
  }


  async disconnectQZTray() {
    try 
    {
      if (!qz.websocket.isActive()) 
      {
        await qz.websocket.disconnect()
        console.log('Disconnected to QZ Tray successfully');  
      }
    } 
    catch (error) {
      this.navigationService.showUIMessage('Error al desconectar QZ Tray ' + error, AlertLevel.Error);
    }
  }

  retryConnect(attempts: number) {
    if (attempts > 3) {
      this.navigationService.showUIMessage('Error al conectarse a QZ Tray después de múltiples intentos', AlertLevel.Error);
      return;
    }
  
    if (!window.qzReady)   {
      setTimeout(() => this.retryConnect(attempts + 1), 1000); // Retry after 1 second
    } 
    else {
      this.connectToQZTray();
    }
  }
}
