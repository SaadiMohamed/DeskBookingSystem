import { CanActivateFn } from '@angular/router';
import { TokenService } from '../Services/token.service';
import { Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = new TokenService(); // Instantiate your TokenService
  const token = tokenService.getToken(); // Use the getToken method from TokenService
  const router = new Router();
  
  // Implement your authentication logic here
  if (token != null) {
    return true; // User is authenticated, allow access to the route
  } else {
    // Handle unauthorized access, e.g., redirect to the login page
    router.navigate(['/login'])
    console.log("Eerst inloggen vriend")
    return false;
  }
};