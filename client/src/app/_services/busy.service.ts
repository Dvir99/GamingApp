import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestsCount = 0

  constructor(private spinner: NgxSpinnerService) { }

  busy(){
    this.busyRequestsCount++
    this.spinner.show(undefined, {
      type: 'timer',
      color: 'black'
    })
  }

  idle(){
    this.busyRequestsCount--
    if(this.busyRequestsCount <= 0){
      this.busyRequestsCount = 0
      this.spinner.hide()
    }
  }
}
