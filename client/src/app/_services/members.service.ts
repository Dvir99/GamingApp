import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResults } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl
  members: Member[] = []
  memberCache = new Map()
  userParams: UserParams | undefined
  user: User | undefined

  constructor(private client: HttpClient, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user){
          this.userParams = new UserParams(user)
          this.user = user
        }
      }
    })
  }

  getUserParams(){
    return this.userParams
  }
  SetUserParams(params: UserParams){
    this.userParams = params
  }

  resetUserParams(){
    if(this.user){
      this.userParams =  new UserParams(this.user)
      return this.userParams
    }
    return
  }

  getMembers(UserParams: UserParams){
    const response = this.memberCache.get(Object.values(UserParams).join('-'))
    if(response) return of(response)
    let params = getPaginationHeaders(UserParams.pageNumber, UserParams.pageSize);
    params = params.append('minAge',UserParams.minAge)
    params = params.append('maxAge',UserParams.maxAge)
    params = params.append('gender',UserParams.gender)
    params = params.append('orderBy',UserParams.orderBy)

    return getPaginatedResult<Member[]>(`${this.baseUrl}/users`,params, this.client).pipe(
      map(response => {
        this.memberCache.set(Object.values(UserParams).join('-'), response)
        return response
      })
    )
  }

  

  getMember(username: string){
    const member = [...this.memberCache.values()]
    .reduce((arr, element) => arr.concat(element.result), [])
    .find((member: Member) => member.userName === username)

    if(member) return of(member)
    return this.client.get<Member>(`${this.baseUrl}/users/${username}`)
  }

  updateMember(member: Member){
    return this.client.put(`${this.baseUrl}/users`, member).pipe(
      map(() =>{
        const index = this.members.indexOf(member)
        this.members[index] = {...this.members[index], ...member}
      })
    )
  }

  setMainPhoto(photoId: number){
    return this.client.put(`${this.baseUrl}/users/set-main-photo/${photoId}`, {})
  }

  deletePhoto(photoId: number){
    return this.client.delete(`${this.baseUrl}/users/delete-photo/${photoId}`)
  }

  addLike(username: string){
    return this.client.post(`${this.baseUrl}/likes/${username}`, {})
  }

  getlikes(predicate: string, pageNumber: number, pageSize: number){
    let params = getPaginationHeaders(pageNumber, pageSize)

    params = params.append('predicate', predicate)
    return getPaginatedResult<Member[]>(`${this.baseUrl}/likes`, params, this.client);
  }

  
}