import { Component } from '@angular/core';
import { LoginService } from '../Services/login.service';
import { Router } from '@angular/router';
import { TokenService } from '../Services/token.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  email: string = ""
  password: string = ""
  InvalidAuth : boolean = false
  errorMessage : string = ""

  constructor(private loginService: LoginService, private router: Router, private token : TokenService){

  }
  ngOnInit(){
    this.token.removeToken()
    console.log("Werkt da fni")
  }

  resetErrorMessage(){
    this.errorMessage == ""
    this.InvalidAuth = false
  }

  onSubmit() {
    this.errorMessage == ""
    this.InvalidAuth = false

    if (this.email == "" || this.password == ""){
      this.InvalidAuth = true;
      this.errorMessage = "Please fill in both inputs"
    }else{
      this.loginService.login(this.email, this.password)
      .subscribe({
        next: (response) => {
          // Handle a successful login
          console.log('Login successful', response)
          this.token.saveToken(response.token)
          this.router.navigate(['/home'])


          // Redirect to another component or perform other actions
        },
        error: (error) => {
          // Handle a failed login
          this.InvalidAuth = true;
          console.error('Login failed', error);
          this.errorMessage = error.error
          // Display an error message to the user
        }
      });
    }

  }

}
