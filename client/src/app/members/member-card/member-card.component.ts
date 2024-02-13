import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PrecenceService } from 'src/app/_services/precence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit{
  @Input() member: Member | undefined


  constructor(private memberService: MembersService, private toastr: ToastrService, public presenceService: PrecenceService){}

  ngOnInit(): void {
  }

  addLike(member: Member){
    console.log(member.userName)
     this.memberService.addLike(member.userName).subscribe({
       next: () => {
         console.log('in card component like function')
         this.toastr.success(`You Liked ${member.knownAs} :)`)},
       error: error => console.log(error),
     })
  }
}
